using TouristsCore.DTOS.Accounts;
using TouristsCore.Entities;

namespace TouristsCore.Services;

public interface IProfileService
{
    public Task<UserProfileDto> GetUserProfileAsync(string userId);
    Task<bool> ChangeAvatarAsync(Guid userId, int FileId);
    
    Task<TouristProfile> UpdateTouristProfileAsync(string userId, TouristProfileUpdateDto dto);
    Task<GuideProfile> UpdateGuideProfileAsync(string userId, GuideProfileUpdateDto dto);
    Task<(bool Success, string Message)> BecomeGuideAsync(string userId);
    Task<object> GetGuidePublicProfileAsync(string userId);
}