using TouristsAPI.Helpers;
using TouristsCore.DTOS.Admin;

namespace TouristsCore.Services;

public interface IAdminService
{
    Task<AdminStatsDto> GetSystemStatsAsync();
    Task<(IReadOnlyList<AdminUserDto>, int)> GetAllUsersAsync(PaginationArg arg);
    Task ToggleUserBanAsync(Guid userId);
}