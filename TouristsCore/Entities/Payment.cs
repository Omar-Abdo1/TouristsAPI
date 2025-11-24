using TouristsCore.Enums.Payment;

namespace TouristsCore.Entities;

public class Payment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid BookingId { get; set; }
    public Booking Booking { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; } // Pending / Succeeded / Failed
    public string Provider { get; set; } // Stripe / PayPal / Mock
    public string TransactionId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}