namespace TouristsCore.Services;

public interface IProfileService
{
    Task<bool> ChangeAvatarAsync(Guid userId, int FileId);
}