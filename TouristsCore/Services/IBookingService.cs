using TouristsCore.DTOS.Booking;

namespace TouristsCore.Services;

public interface IBookingService
{
    Task<BookingResponseDto> CreateBookingAsync(CreateBookingDto dto, Guid userId);
}