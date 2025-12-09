using TouristsCore.DTOS.Accounts;

namespace TouristsCore.Services;

public interface IProfileService
{
    public Task<UserProfileDto> GetUserProfileAsync(string userId);
    Task<bool> ChangeAvatarAsync(Guid userId, int FileId);
}