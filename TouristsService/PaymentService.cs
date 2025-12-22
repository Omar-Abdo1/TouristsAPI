using Hangfire;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stripe;
using Stripe.Checkout;
using TouristsCore;
using TouristsCore.Entities;
using TouristsCore.Enums;
using TouristsCore.Enums.Payment;
using TouristsCore.Services;

namespace TouristsService;

public class PaymentService
{
   private readonly IUnitOfWork _unitOfWork;
   private readonly IConfiguration _config;
   private readonly IBackgroundJobClient _jobClient;
   private readonly IEmailService _emailService;
   private readonly ILogger<PaymentService> _logger;

   public PaymentService(IUnitOfWork  unitOfWork,IConfiguration  configuration,IBackgroundJobClient jobClient,
      IEmailService  emailService,ILogger<PaymentService>  logger)
   {
      _unitOfWork = unitOfWork;
      _config = configuration;
      _jobClient = jobClient;
      _emailService = emailService;
      _logger = logger;
   }
   public async Task<Stripe.Checkout.Session> CreateCheckoutSessionAsync(int bookingId)
   {

      var booking = await _unitOfWork.Repository<Booking>().GetByIdAsync(bookingId, true,
         b => b.Tour , b=>b.Tourist.User
      );
      
      if(booking == null)
         throw new Exception("Booking not found");
      if(booking.Status==BookingStatus.Confirmed)
         throw new Exception("Already paid!");

      var options = new SessionCreateOptions() // Configure the Product for Stripe
      {
         PaymentMethodTypes = new List<string>() { "card" },
         Mode = "payment",
         SuccessUrl =
            _config["FrontBaseUrl"] + "/payment-success?session_id={CHECKOUT_SESSION_ID}", // Redirect back to Frontend
           CancelUrl = _config["FrontBaseUrl"] + "/payment-cancel",
           CustomerEmail = booking.Tourist.User.Email,
           Metadata = new Dictionary<string, string>()
           {
              {"booking_id", booking.Id.ToString()},
           },
           PaymentIntentData = new SessionPaymentIntentDataOptions
           {
              Metadata = new Dictionary<string, string> { {"booking_id", booking.Id.ToString()} } // save the metaData if Failed
           },
           LineItems = new List<SessionLineItemOptions>()
           {
             new SessionLineItemOptions()
             {
                PriceData = new SessionLineItemPriceDataOptions
                {
                   UnitAmount = (long)(booking.PriceAtBooking * 100), // Stripe uses Cents ($10.00 = 1000)  TotalPrice
                   Currency = "usd",
                   ProductData = new SessionLineItemPriceDataProductDataOptions
                   {
                      Name = booking.Tour.Title,
                      Description = $"Booking for {booking.TicketCount} people"
                   }
                },
                Quantity = 1,  
             }
           },
      };

      var service = new SessionService();
      var session = await service.CreateAsync(options);
      
      var existingPayment = await _unitOfWork.Repository<Payment>()
         .GetEntityByConditionAsync(p => p.BookingId == bookingId);
      if (existingPayment != null)
      {
         existingPayment.TransactionId = session.Id; 
         existingPayment.Status = PaymentStatus.Pending;
         existingPayment.Amount = booking.PriceAtBooking;
         existingPayment.Provider = "Stripe";
         _unitOfWork.Repository<Payment>().Update(existingPayment);
      }
      else
      {
         var payment = new Payment
         {
            BookingId = booking.Id,
            Amount = booking.PriceAtBooking,
            Status = PaymentStatus.Pending,
            Provider = "Stripe",
            TransactionId = session.Id // save it to DB (its temp we will swap it with PaymentIntentId later) 
         };
         _unitOfWork.Repository<Payment>().Add(payment);
      }

      await _unitOfWork.CompleteAsync();
      
      return session;
   }

   public async Task HandleCheckoutCompletedAsync(Stripe.Checkout.Session session)
   {
      if(!session.Metadata.TryGetValue("booking_id",out var bookingIdStr))
         return;
      
      int bookingId = int.Parse(bookingIdStr);

      var payment = await _unitOfWork.Repository<Payment>().GetEntityByConditionAsync(
         p => p.TransactionId == session.Id, false, p => p.Booking,
         p=>p.Booking.Tourist.User);
      
      if (payment == null || payment.Status == PaymentStatus.Succeeded)
      return;
      
      long expectedAmount = (long)(payment.Booking.PriceAtBooking * 100);
      long receivedAmount = session.AmountTotal ?? 0;
      if (expectedAmount != receivedAmount)
      {
         _logger.LogCritical("[SECURITY] Amount mismatch for booking {BookingId}", bookingId);
         return;
      }

      payment.TransactionId = session.PaymentIntentId;
      payment.Status = PaymentStatus.Succeeded;
      payment.PaymentDate = DateTime.UtcNow;
      payment.Booking.Status = BookingStatus.Confirmed;
      
      _unitOfWork.Repository<Payment>().Update(payment);
      await _unitOfWork.CompleteAsync();
      
      _jobClient.Enqueue(() =>
         _emailService.SendEmailAsync(
            payment.Booking.Tourist.User.Email,
            "Ticket Confirmed",
            "Your booking has been successfully confirmed."
         )
      );
   }

   public async Task HandleCheckoutExpiredAsync(Stripe.Checkout.Session session)
   {
      if(!session.Metadata.TryGetValue("booking_id",out var bookingIdStr))
         return;
      int bookingId = int.Parse(bookingIdStr);
      
      var payment = await _unitOfWork.Repository<Payment>().GetEntityByConditionAsync(
         p => p.TransactionId == session.Id);
      
      if (payment == null || payment.Status != PaymentStatus.Pending)
         return;

      payment.TransactionId = session.PaymentIntentId;
      payment.Status = PaymentStatus.Cancelled;
      payment.FailureMessage = "Checkout session expired or canceled by user";

      _unitOfWork.Repository<Payment>().Update(payment);
      await _unitOfWork.CompleteAsync();

      _logger.LogWarning("[Webhook] Checkout expired for booking {BookingId}", bookingId);
   }

   public async Task HandlePaymentFailedAsync(PaymentIntent intent)
   {
      if (!intent.Metadata.TryGetValue("booking_id", out var bookingIdStr))
         return;

      int bookingId = int.Parse(bookingIdStr);
      
      var payment = await _unitOfWork.Repository<Payment>().GetEntityByConditionAsync(
         p => p.BookingId ==bookingId && p.Status == PaymentStatus.Pending);
      
      if (payment == null || payment.Status != PaymentStatus.Pending)
         return;
      
      payment.TransactionId = intent.Id;
      payment.Status = PaymentStatus.Failed;
      payment.FailureMessage = intent.LastPaymentError?.Message;
      
      _unitOfWork.Repository<Payment>().Update(payment);
      await _unitOfWork.CompleteAsync();

      _logger.LogWarning("[Webhook] Payment failed for PaymentIntent {IntentId}", intent.Id);
   }

   public async Task HandleChargeRefundedAsync(string paymentIntentId)
   {
      var payment = await _unitOfWork.Repository<Payment>()
         .GetEntityByConditionAsync(p => p.TransactionId == paymentIntentId,false,
            p=>p.Booking);
      
      if (payment == null) 
         return;
      
      if (payment.Status == PaymentStatus.Refunded) 
      {
         _logger.LogInformation("Payment already marked as refunded. Skipping.");
         return; 
      }
      payment.Status = PaymentStatus.Refunded;
      payment.FailureMessage = "Refunded via External Webhook";
   
      if (payment.Booking != null) 
         payment.Booking.Status = BookingStatus.Cancelled;

      _unitOfWork.Repository<Payment>().Update(payment);
      await _unitOfWork.CompleteAsync();

      _logger.LogInformation($"[Webhook] Synced refund status for {paymentIntentId}");
      
     
   }
   public async Task RefundPaymentAsync(int bookingId)
   {
      var payment = await _unitOfWork.Repository<Payment>()
         .GetEntityByConditionAsync(p => p.BookingId == bookingId, false, 
            p => p.Booking);

      if (payment == null || payment.Status != PaymentStatus.Succeeded)
         throw new Exception("Cannot refund this payment.");

      try
      {
         var refundOptions = new RefundCreateOptions
         {
            PaymentIntent = payment.TransactionId,
            Reason = RefundReasons.RequestedByCustomer
         };
         var refundService = new RefundService();
         await refundService.CreateAsync(refundOptions); 

         payment.Status = PaymentStatus.Refunded;
         payment.FailureMessage = "Refunded by Admin API";
         if (payment.Booking != null) payment.Booking.Status = BookingStatus.Cancelled;

         _unitOfWork.Repository<Payment>().Update(payment);
         await _unitOfWork.CompleteAsync();

      }
      catch (StripeException ex)
      {
         throw new Exception(ex.Message);
      }
   }

}
