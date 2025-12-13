using Microsoft.EntityFrameworkCore;
using TouristsAPI.Helpers;
using TouristsCore;
using TouristsCore.DTOS.Admin;
using TouristsCore.Entities;
using TouristsCore.Enums;
using TouristsCore.Services;

namespace TouristsService;

public class AdminService : IAdminService
{
    private readonly IUnitOfWork _unitOfWork;

    public AdminService(IUnitOfWork  unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<AdminStatsDto> GetSystemStatsAsync()
    {
        var revenue = await _unitOfWork.Context.Set<Booking>()
            .Where(b => b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Completed)
            .SumAsync(b => b.PriceAtBooking);

        var bookingsCount = await _unitOfWork.Repository<Booking>().CountAsync();
        
        var toursCount = await _unitOfWork.Repository<Tour>().CountAsync();
        
        var guidesCount = await _unitOfWork.Repository<GuideProfile>().CountAsync();
        
        var touristsCount = await _unitOfWork.Repository<TouristProfile>().CountAsync();
        
        return new AdminStatsDto
        {
            TotalRevenue = revenue,
            TotalBookings = bookingsCount,
            TotalTours = toursCount,
            TotalGuides = guidesCount,
            TotalTourists = touristsCount
        };
    }

    public async Task<(IReadOnlyList<AdminUserDto>, int)> GetAllUsersAsync(PaginationArg arg)
    {
        var query = _unitOfWork.Context.Set<User>()
            .AsNoTracking()
            .AsQueryable();

        var totalCount = await query.CountAsync();
        
        var users = await query.OrderByDescending(u=>u.CreatedAt)
            .Skip((arg.PageIndex - 1) * arg.PageSize)
            .Take(arg.PageSize).Select(u => new AdminUserDto
            {
                Id = u.Id,
                Email = u.Email,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                FullName = u.TouristProfile != null ? u.TouristProfile.FullName :
                    u.GuideProfile != null ? u.GuideProfile.FullName : 
                    u.UserName,
                Role = u.GuideProfile != null ? "Guide" : "Tourist"
            })
            .ToListAsync();
        return (users, totalCount);
    }

    public async Task ToggleUserBanAsync(Guid userId)
    {
        var user = await _unitOfWork.Context.Set<User>().FindAsync(userId);
        if (user == null) 
            throw new Exception("User not found");
        
        user.IsActive = !user.IsActive;
        
        _unitOfWork.Context.Set<User>().Update(user);
        await _unitOfWork.CompleteAsync();
    }
}