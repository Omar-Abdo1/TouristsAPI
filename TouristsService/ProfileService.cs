using System.Net;
using Microsoft.EntityFrameworkCore.Metadata;
using TouristsCore;
using TouristsCore.Entities;
using TouristsCore.Services;

namespace TouristsService;

public class ProfileService : IProfileService
{
    private readonly IUnitOfWork _unitOfWork;

    public ProfileService(IUnitOfWork 
        unitOfWork)
    {
        _unitOfWork = unitOfWork;
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
