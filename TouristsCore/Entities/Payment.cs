using System.ComponentModel.DataAnnotations;
using System.Data;
using TouristsCore.Enums.Payment;

namespace TouristsCore.Entities;

public class Payment : BaseEntity
{
    public int BookingId { get; set; }
    public Booking Booking { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    
    public PaymentStatus Status { get; set; } // Pending / Succeeded / Failed
    
    public string Provider { get; set; } // Stripe / PayPal / Mock
    public string? TransactionId { get; set; }
    
    // Debugging
    public DateTime? PaymentDate { get; set; } // Null until success
    public string? FailureMessage { get; set; } // "Card declined", etc.

    [Timestamp]
    public byte[] RowVersion { get; set; } // for concurrency when Paying 
     
}