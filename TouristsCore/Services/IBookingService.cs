using TouristsAPI.Helpers;
using TouristsCore.DTOS.Booking;

namespace TouristsCore.Services;

public interface IBookingService
{
    Task<BookingResponseDto> CreateBookingAsync(CreateBookingDto dto, Guid userId);
    Task CancelBookingAsync(int bookingId, Guid userId);
    Task<(IReadOnlyList<BookingTouristDto>, int)> GetBookingsForUserAsync(Guid userId, PaginationArg arg);
    Task<(IReadOnlyList<GuideSalesDto>, int)> GetSalesForTourAsync(int tourId, Guid userId, PaginationArg arg);
    
    Task<BookingDetailDto> GetBookingByIdAsync(int bookingId, Guid userId);
}