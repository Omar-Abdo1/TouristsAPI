using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using TouristsAPI.ErrorResponses;
using TouristsService;

namespace TouristsAPI.Controllers;
[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly PaymentService _paymentService;
    private readonly ILogger<PaymentController> _logger;
    private readonly IConfiguration _config;

    public PaymentController(PaymentService paymentService,ILogger<PaymentController> logger,IConfiguration 
        config)
    {
        _paymentService = paymentService;
        _logger = logger;
        _config = config;
    }
    
    [HttpPost("Pay/{bookingId:int}")]
    [Authorize]
    public async Task<IActionResult> CreateCheckout(int bookingId)
    {
        try
        {
            var session = await _paymentService.CreateCheckoutSessionAsync(bookingId);
            
            return Ok(new { url = session.Url }); 
            //Stripe-generated Checkout page URL 
            //ex : https://checkout.stripe.com/c/pay/cs_test_a1B2C3D4E5
            
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiErrorResponse(400, ex.Message));
        }
    }

    // here we listen on stripe when the event end we know on this end point stipe will hit this end point
    [HttpPost("webhook")]
    public async Task<IActionResult> StripeWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                _config["StripeSettings:WebhookSecret"],
                throwOnApiVersionMismatch:false
            ); // compare with  Stripe-Signature as JWT Signature  using -> HMAC 

            switch (stripeEvent.Type)
            {
                case "checkout.session.completed": // click pay and success
                    var completedSession = stripeEvent.Data.Object as Stripe.Checkout.Session;
                    if (completedSession != null)
                    {
                        await _paymentService.HandleCheckoutCompletedAsync(completedSession);
                    }
                    break;

                case "checkout.session.expired": // f the user opened the Checkout page but never paid
                    var expiredSession = stripeEvent.Data.Object as Stripe.Checkout.Session;
                    if (expiredSession != null)
                    {
                        await _paymentService.HandleCheckoutExpiredAsync(expiredSession);
                    }
                    break;

                case "payment_intent.payment_failed": // enter a card and it gets Declined
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    if (paymentIntent != null)
                    {
                        await _paymentService.HandlePaymentFailedAsync(paymentIntent);
                    }
                    break;
                  case "charge.refunded": // want to refund
                    var charge = stripeEvent.Data.Object as Stripe.Charge;
                    if (charge?.PaymentIntentId != null)
                    {
                        await _paymentService.HandleChargeRefundedAsync(charge.PaymentIntentId);
                    }
                    break;
            }

            return Ok();
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe webhook error");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine("STRIPE ERROR: " + ex.Message);
            Console.WriteLine("--------------------------------------------------");
            return BadRequest(new ApiErrorResponse(400,ex.Message));
        }
    }
    
    [HttpPost("refund/{bookingId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Refund(int bookingId)
    {
        try
        {
            await _paymentService.RefundPaymentAsync(bookingId);
            return Ok(new { Message = "Money returned to customer successfully." });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiErrorResponse(400, ex.Message));
        }
    }
}