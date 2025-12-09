using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata;
using TouristsCore;
using TouristsCore.DTOS.Accounts;
using TouristsCore.Entities;
using TouristsCore.Services;

namespace TouristsService;

public class ProfileService : IProfileService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<User> _userManager;

    public ProfileService(IUnitOfWork 
        unitOfWork,UserManager<User> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }
    
    public async Task<UserProfileDto> GetUserProfileAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return null;
        string avatarUrl = null;
        var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
        if (role.ToUpper() == "GUIDE")
        {
            var guide = await _unitOfWork.Repository<GuideProfile>().GetEntityByConditionAsync(
                g => g.UserId == user.Id, true, g => g.AvatarFile);
            if (guide?.AvatarFile != null)
            {
                avatarUrl = guide.AvatarFile.FilePath; 
            }
        }
        else 
        {
            var tourist =  await _unitOfWork.Repository<TouristProfile>().GetEntityByConditionAsync(
                t => t.UserId == user.Id, true, t => t.AvatarFile);

            if (tourist?.AvatarFile != null)
            {
                avatarUrl = tourist.AvatarFile.FilePath;
            }
        }

        var logins = await _userManager.GetLoginsAsync(user);
        var providers = logins.Select(l => l.LoginProvider).ToList();
        var hasPassword = await _userManager.HasPasswordAsync(user);

        return new UserProfileDto
        {
            Id = user.Id.ToString(),
            Username = user.UserName,
            Email = user.Email,
            Role = role,
            AvatarUrl = avatarUrl, // send to Front
            LinkedProviders = providers,
            HasPassword = hasPassword
        };
    }
    
    public async Task<bool> ChangeAvatarAsync(Guid userId, int fileId)
    {
        var guid = await _unitOfWork.Repository<GuideProfile>().GetEntityByConditionAsync(g => g.UserId == userId);
        if (guid != null)
        {
            await HandleAvatarUpdate(guid, fileId);
            return true;
        }
        var tourist = await _unitOfWork.Repository<TouristProfile>().GetEntityByConditionAsync(t => t.UserId == userId);
        if (tourist != null)
        {
            await HandleAvatarUpdate(tourist, fileId);
            return true;
        }
        return false;
    }

    private async Task HandleAvatarUpdate<T>(T profile, int newFileId) where T : BaseEntity,IHasAvatar
    {

        if (profile.AvatarFileId.HasValue)
        {
            var oldFileId = profile.AvatarFileId.Value;
            var oldFile = await _unitOfWork.Repository<FileRecord>().GetByIdAsync(oldFileId);
            if (oldFile != null)
            {
                _unitOfWork.Repository<FileRecord>().Delete(oldFile); // softDelete 
            }
        }
        profile.AvatarFileId = newFileId;
        _unitOfWork.Repository<T>().Update(profile);
        await _unitOfWork.CompleteAsync();
    }
    
}
