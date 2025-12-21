using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

        var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
        
        var logins = await _userManager.GetLoginsAsync(user);
        var providers = logins.Select(l => l.LoginProvider).ToList();
        var hasPassword = await _userManager.HasPasswordAsync(user);
        
        var dto =  new UserProfileDto
        {
            Id = user.Id.ToString(),
            Username = user.UserName,
            Email = user.Email,
            Role = role,
            LinkedProviders = providers,
            HasPassword = hasPassword
        };
        
        
        if (role.ToUpper() == "GUIDE")
        {
            var guide = await _unitOfWork.Repository<GuideProfile>().GetEntityByConditionAsync(
                g => g.UserId == user.Id, true, g => g.AvatarFile);
            
            if (guide != null)
            {
                dto.Bio = guide.Bio;
                dto.ExperienceYears = guide.ExperienceYears;
                dto.RatePerHour = guide.RatePerHour;
                if (guide.AvatarFile != null) dto.AvatarUrl = guide.AvatarFile.FilePath;
            }
        }
        else 
        {
            var tourist = await _unitOfWork.Repository<TouristProfile>().GetEntityByConditionAsync(
                t => t.UserId == user.Id, true, t => t.AvatarFile);

            if (tourist != null)
            {
                dto.Country = tourist.Country;
                dto.Phone = tourist.Phone;
                if (tourist.AvatarFile != null) dto.AvatarUrl = tourist.AvatarFile.FilePath;
            }
        }

        return dto;
    }
    
    public async Task<bool> ChangeAvatarAsync(Guid userId, int fileId)
    {
        var file = await _unitOfWork.Repository<FileRecord>().GetByIdAsync(fileId);
        if (file is null) 
            return false;
        var guid = await _unitOfWork.Repository<GuideProfile>().GetEntityByConditionAsync(g => g.UserId == userId);
        if (guid != null)
        {
            await HandleAvatarUpdate(guid, fileId);
            var me = await _unitOfWork.Context.Set<User>().FindAsync(guid.UserId);
            me.PhotoUrl = file.FilePath;
            _unitOfWork.Context.Set<User>().Update(me);
            await _unitOfWork.Context.SaveChangesAsync();
            
            return true;
        }
        var tourist = await _unitOfWork.Repository<TouristProfile>().GetEntityByConditionAsync(t => t.UserId == userId);
        if (tourist != null)
        {
            await HandleAvatarUpdate(tourist, fileId);
            var me = await _unitOfWork.Context.Set<User>().FindAsync(tourist.UserId);
            me.PhotoUrl = file.FilePath;
            _unitOfWork.Context.Set<User>().Update(me);
            await _unitOfWork.Context.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task<TouristProfile> UpdateTouristProfileAsync(string userId, TouristProfileUpdateDto dto)
    {
        
        if (!Guid.TryParse(userId, out var userGuid)) 
            throw new ArgumentException("Invalid User ID");
        
        var profile = await _unitOfWork.Repository<TouristProfile>()
            .GetEntityByConditionAsync(t => t.UserId == userGuid);
        if (profile == null)
            throw new Exception("Critical Error: User is a Tourist but has no profile!"); // this should not happen

        profile.Country = dto.Country;
        profile.Phone = dto.Phone;
        profile.FullName = dto.FullName;
        
        await _unitOfWork.CompleteAsync();
        return profile;
    }

    public async Task<GuideProfile> UpdateGuideProfileAsync(string userId, GuideProfileUpdateDto dto)
    {
        if (!Guid.TryParse(userId, out var userGuid)) 
            throw new ArgumentException("Invalid User ID");
        
        var profile =await _unitOfWork.Repository<GuideProfile>()
            .GetEntityByConditionAsync(g => g.UserId == userGuid, false, g => g.GuideLanguages);
        
        if (profile == null) 
            throw new Exception("Critical Error: User is a Guide but has no profile!");
        
        profile.Bio = dto.Bio;
        profile.ExperienceYears = dto.ExperienceYears;
        profile.RatePerHour = dto.RatePerHour;
        
        if (dto.Languages != null && dto.Languages.Any())
        {
            _unitOfWork.Context.Set<GuideLanguage>().RemoveRange(profile.GuideLanguages);
            var languagesEntities = await _unitOfWork.Repository<Language>().GetAllByConditionAsync(
                criteria: l => dto.Languages.Contains(l.Name), asNoTracking: true);

            foreach (var langEntity in languagesEntities)
            {
                profile.GuideLanguages.Add(new GuideLanguage()
                {
                    GuideProfileId = profile.Id,
                    LanguageId = langEntity.Id
                });
            }
        }

        await _unitOfWork.CompleteAsync();
            return profile;
    }
    
    public async Task<(bool Success, string Message)> BecomeGuideAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return (false, "User not found");

        if (await _userManager.IsInRoleAsync(user, "Guide"))
            return (false, "You are already a Guide.");

        await _userManager.AddToRoleAsync(user, "Guide");

        var guideProfile = new GuideProfile
        {
            UserId = user.Id,
            Bio = "",
            FullName = user.UserName
        };

        _unitOfWork.Repository<GuideProfile>().Add(guideProfile);
        await _unitOfWork.CompleteAsync();

        return (true, "You are now a Guide!");
    }

    public async Task<object> GetGuidePublicProfileAsync(string userId)
    {
        if (!Guid.TryParse(userId, out var userGuid)) 
            throw new ArgumentException("Invalid User ID");
        
        return await _unitOfWork.Context.Set<GuideProfile>().AsQueryable().
            AsNoTracking()
            .Where(g => g.UserId == userGuid)
            .Select(g => new
            {
                FullName=g.FullName,
                Bio=g.Bio,
                ExperienceYears=g.ExperienceYears,
                RatePerHour=g.RatePerHour,
                PictureUrl = g.AvatarFile != null ? g.AvatarFile.FilePath : null,
                Languages = g.GuideLanguages.Select(gl => gl.Language.Name)
            }).FirstOrDefaultAsync();
    }


    private async Task HandleAvatarUpdate<T>(T profile, int newFileId) where T : BaseEntity,IHasAvatar
    {

        if (profile.AvatarFileId.HasValue)
        {
            var oldFileId = profile.AvatarFileId.Value;
            var oldFile = await _unitOfWork.Repository<FileRecord>().GetByIdAsync(oldFileId);
            if (oldFile != null)
            {
                _unitOfWork.Repository<FileRecord>().SoftDelete(oldFile); // softDelete 
            }
        }
        profile.AvatarFileId = newFileId;
        _unitOfWork.Repository<T>().Update(profile);
        await _unitOfWork.CompleteAsync();
    }
    
}
