using Google.Apis.Auth.OAuth2.Web;
using Microsoft.EntityFrameworkCore;
using TouristsAPI.Helpers;
using TouristsCore;
using TouristsCore.DTOS.Tours;
using TouristsCore.Entities;
using TouristsCore.Services;

namespace TouristsService;

public class TourService : ITourService
{
    private readonly IUnitOfWork _unitOfWork;

    public TourService(IUnitOfWork  unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<int> CreateTourAsync(CreateTourDto model, Guid userId)
    {
        var guid = await _unitOfWork.Repository<GuideProfile>().GetEntityByConditionAsync(
            g=>g.UserId==userId,true);
        if (guid == null)
            throw new Exception("User is not a Guid or Profile is Missing");
        var tour = new Tour
        {
            GuideProfileId = guid.Id,
            Title = model.Title,
            Description = model.Description,
            Price = model.Price,
            DurationMinutes = model.DurationMinutes,
            City = model.City,
            Country = model.Country,
            IsPublished = true,
            Media = new List<TourMedia>()
        };
        int currentIndex = 0;
        if (model.MediaIds != null && model.MediaIds.Any())
        {
            foreach (var mediaId in model.MediaIds)
            {
                var file = await _unitOfWork.Repository<FileRecord>().GetByIdAsync(mediaId, true);
                if (file != null)
                {
                    tour.Media.Add(
                        new TourMedia()
                        {
                            FileId = file.Id,
                            IsVideo = file.ContentType.StartsWith("video"),
                            OrderIndex = currentIndex++
                        });
                }
            }
        }
        _unitOfWork.Repository<Tour>().Add(tour);
        await _unitOfWork.CompleteAsync();
        return tour.Id;
    }

    public async Task<TourDto> GetTourByIdAsync(int id)
    {
        return  await _unitOfWork.Context.Set<Tour>()
            .Where(t => t.Id == id)
            .Select(t => new TourDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Price = t.Price,
                DurationMinutes = t.DurationMinutes,
                City = t.City,
                IsPublished = t.IsPublished,
                CreatedAt = t.CreatedAt,
                UpdateAt = t.UpdatedAt,

                GuideName = t.GuideProfile.User != null ? t.GuideProfile.User.UserName : null,
                GuideAvatarUrl = t.GuideProfile.AvatarFile != null ? t.GuideProfile.AvatarFile.FilePath : null,
                
                ImageUrls = t.Media
                    .OrderBy(m => m.OrderIndex)
                    .Select(m => m.File.FilePath)
                    .ToList()
            })
            .AsNoTracking()
            .FirstOrDefaultAsync();
    }

    public async Task<(IReadOnlyList<TourDto>,int)> GetToursAsync(TourRequestDto request)
    {
        var qry = _unitOfWork.Context.Set<Tour>().AsQueryable().
            Where(t=>t.IsPublished==true);
        if (!string.IsNullOrEmpty(request.City))
            qry = qry.Where(t => t.City.Contains(request.City));
        if(request.MinPrice.HasValue)
            qry=qry.Where(t=>t.Price>=request.MinPrice.Value);
        if(request.MaxPrice.HasValue)
            qry=qry.Where(t=>t.Price<=request.MaxPrice.Value);
        if (request.GuideId.HasValue)
            qry = qry.Where(t => t.GuideProfileId ==request.GuideId.Value);
        
        qry = request.SortBy?.ToLower() switch
        {
            "priceasc" => qry.OrderBy(t => t.Price),
            "pricedesc" => qry.OrderByDescending(t => t.Price),
            "duration" => qry.OrderBy(t => t.DurationMinutes),
            _ => qry.OrderByDescending(t => t.CreatedAt) // Default: Newest First
        };

        int totalCount = await qry.CountAsync() ; 

        var Tours =
            await qry.Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(t => new TourDto()
                {
                    Id = t.Id,
                    Title = t.Title,
                    Price = t.Price,
                    City = t.City,
                    
                    GuideName = t.GuideProfile.User != null ? t.GuideProfile.User.UserName : null,

                    ImageUrls = t.Media
                        .Where(m => m.OrderIndex == 0) 
                        .Select(m => m.File.FilePath)
                        .ToList()
                })
                .AsNoTracking()
                .ToListAsync();
        
        return (Tours, totalCount);
    }

    public async Task<bool> DeleteTourAsync(int tourId, Guid userId)
    {
        var tour = await _unitOfWork.Repository<Tour>().GetByIdAsync(tourId, false,
            t => t.GuideProfile);
        if (tour == null)
            return false;
        if(tour.GuideProfile.UserId != userId)
            throw new UnauthorizedAccessException("You do not own this tour.");
        
        _unitOfWork.Repository<Tour>().Delete(tour);
        await _unitOfWork.CompleteAsync();
        return true;
    }

    public async Task<TourDto> UpdateTourAsync(int tourId, Guid userId, UpdateTourDto model)
    {
        var tour = await _unitOfWork.Repository<Tour>().GetByIdAsync(tourId, false,
            t => t.GuideProfile,t=>t.Media);
        if (tour == null) return null;

        if (tour.GuideProfile.UserId != userId)
            throw new UnauthorizedAccessException("You do not own this tour.");
        
        tour.Title = model.Title;
        tour.Description = model.Description;
        tour.Price = model.Price;
        tour.DurationMinutes = model.DurationMinutes;
        tour.City = model.City;
        tour.Country = model.Country;

        if (model.MediaIds != null)
        {
            if (tour.Media.Any())
            {
                _unitOfWork.Context.Set<TourMedia>().RemoveRange(tour.Media); // delete sql commands 
            }

            tour.Media = new List<TourMedia>();
            int currentIndex = 0;
            foreach (var fileId in model.MediaIds)
            {
                var file = await _unitOfWork.Repository<FileRecord>().GetByIdAsync(fileId,true);
                if (file != null)
                {
                    tour.Media.Add(new TourMedia
                    {
                        FileId = fileId,
                        IsVideo = file.ContentType.StartsWith("video"),
                        OrderIndex = currentIndex++
                    });
                }
            }
        }

        tour.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<Tour>().Update(tour);
        await _unitOfWork.CompleteAsync();
        
        return await GetTourByIdAsync(tour.Id);
    }
    
    public async Task<bool> TogglePublishStatusAsync(int tourId, string guideUserId)
    {
        if(!Guid.TryParse(guideUserId, out var userid))
            throw new UnauthorizedAccessException("You do not own this tour.");
        
        var tour = await _unitOfWork.Repository<Tour>()
            .GetByIdAsync(tourId);

        if (tour == null) 
            throw new KeyNotFoundException($"Tour with ID {tourId} not found.");
        
        var guideProfile = await _unitOfWork.Repository<GuideProfile>()
            .GetEntityByConditionAsync(g => g.UserId == userid);

        if (guideProfile == null || tour.GuideProfileId != guideProfile.Id) 
            throw new UnauthorizedAccessException("You do not own this tour.");

        tour.IsPublished = !tour.IsPublished;

        await _unitOfWork.CompleteAsync();
    
        return tour.IsPublished; 
    }

    public async Task<(IReadOnlyList<TourDto>,int)> GetMyToursAsync(string guideUserId,PaginationArg arg)
    {
        if(!Guid.TryParse(guideUserId, out var userid))
            throw new UnauthorizedAccessException("You do not own this tour.");
        
        var guideProfile = await _unitOfWork.Repository<GuideProfile>()
            .GetEntityByConditionAsync(g => g.UserId == userid);

        if (guideProfile == null)
            return (new List<TourDto>(),0);


        int count = await _unitOfWork.Context.Set<Tour>()
            .Where(t => t.GuideProfileId == guideProfile.Id).
            CountAsync();
        
        var tours =   await _unitOfWork.Context.Set<Tour>()
            .Where(t => t.GuideProfileId == guideProfile.Id)
            .OrderByDescending(t => t.CreatedAt)
            .Skip((arg.PageIndex-1)*arg.PageSize)
            .Take(arg.PageSize)
            .Select(t => new TourDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Price = t.Price,
                DurationMinutes = t.DurationMinutes,
                City = t.City,
                IsPublished = t.IsPublished,
                CreatedAt = t.CreatedAt,
                UpdateAt = t.UpdatedAt,
    
                GuideName = t.GuideProfile.User != null ? t.GuideProfile.User.UserName : null,
                GuideAvatarUrl = t.GuideProfile.AvatarFile != null ? t.GuideProfile.AvatarFile.FilePath : null,
                
                ImageUrls = t.Media
                    .OrderBy(m => m.OrderIndex)
                    .Select(m => m.File.FilePath)
                    .ToList()
            })
            .AsNoTracking()
            .ToListAsync();
        return (tours, count);
    }
}