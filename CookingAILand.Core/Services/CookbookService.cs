using System.Linq.Expressions;
using AutoMapper;
using CookingAILand.Core.Authorization;
using CookingAILand.Core.DAL.Entities;
using CookingAILand.Core.DAL.Enums;
using CookingAILand.Core.DAL.Persistence;
using CookingAILand.Core.Models;
using CookingAILand.Core.Services;
using CookingAILand.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace CookingAILand;

public interface ICookbookService
{
    Task<Guid> CreateAsync(CookbookDto dto);
    Task DeleteAsync(Guid id);
    Task UpdateAsync(Guid id, CookbookDto dto);
    PagedResult<GetCookbookDto> GetAllPublishedCookbooks(CookingQuery query);
    Task<GetCookbookDto> GetCookbookByIdAsync(Guid id);
    PagedResult<GetCookbookDto> GetAllUsersCookbooks(CookingQuery query);
}

public class CookbookService : ICookbookService
{
    private readonly CookingDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IUserContextService _userContextService;
    private readonly IAuthorizationService _authorizationService;
    private readonly IPhotoUploadService _photoUploadService;

    public CookbookService(CookingDbContext dbContext, IMapper mapper, IUserContextService userContextService,
        IAuthorizationService authorizationService, IPhotoUploadService photoUploadService)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _userContextService = userContextService;
        _authorizationService = authorizationService;
        _photoUploadService = photoUploadService;
    }

    public async Task<Guid> CreateAsync(CookbookDto dto)
    {
        var cookbook = _mapper.Map<Cookbook>(dto);
        cookbook.CreatedById = (Guid)_userContextService.GetUserId!;
        if (dto.Photo is not null)
        {
            var uploadResult = await _photoUploadService.upload(dto.Photo);
            cookbook.PhotoUrl = uploadResult.Url;
        }

        await _dbContext.Cookbooks.AddAsync(cookbook);
        await _dbContext.SaveChangesAsync();
        return cookbook.Id;
    }

    public async Task DeleteAsync(Guid id)
    {
        var cookbook = await _dbContext.Cookbooks.FirstOrDefaultAsync(r => r.Id == id);

        if (cookbook is null)
            throw new NotFoundException("Cookbook not found");

        var authorizationResult =  _authorizationService.AuthorizeAsync(_userContextService.User, cookbook,
            new ResourceOperationRequirement(ResourceOperation.Delete)).Result;

        if (!authorizationResult.Succeeded)
            throw new ForbidException("Access denied");

        _dbContext.Cookbooks.Remove(cookbook);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Guid id, CookbookDto dto)
    {
        var cookbook = await _dbContext.Cookbooks.FirstOrDefaultAsync(r => r.Id == id);

        if (cookbook is null)
            throw new NotFoundException("Cookbook not found");

        var authorizationResult = _authorizationService.AuthorizeAsync(_userContextService.User, cookbook,
            new ResourceOperationRequirement(ResourceOperation.Delete)).Result;

        if (!authorizationResult.Succeeded)
            throw new ForbidException("Access denied");

        if (dto.Photo is not null)
        {
            var uploadResult = await _photoUploadService.upload(dto.Photo);
            cookbook.PhotoUrl = uploadResult.Url;
        }
        
        cookbook.Name = dto.Name;
        cookbook.Description = dto.Description;
        cookbook.Published = dto.Published;

       await _dbContext.SaveChangesAsync();
    }

    public PagedResult<GetCookbookDto> GetAllPublishedCookbooks(CookingQuery query)
    {
        var baseQuery = _dbContext
            .Cookbooks
            .Where(c => c.Published == true)
            .Where(c => query.SearchPhrase == null || c.Name.ToLower()
                .Contains(query.SearchPhrase.ToLower()) || c.Description.ToLower()
                .Contains(query.SearchPhrase.ToLower()));

        if (!string.IsNullOrEmpty(query.SortBy))
        {
            var columnsSelectors = new Dictionary<string, Expression<Func<Cookbook, object>>>
            {
                { nameof(Cookbook.Name), c => c.Name },
                { nameof(Cookbook.Description), c => c.Description },
            };
            var selectedColumn = columnsSelectors[query.SortBy];
            baseQuery = query.SortDirection == SortDirection.ASC
                ? baseQuery.OrderBy(selectedColumn)
                : baseQuery.OrderByDescending(selectedColumn);
        }

        var cookbooks = baseQuery.Skip(query.PageSize * (query.PageNumber - 1))
            .Take(query.PageSize)
            .ToList();

        var totalItemsCount = baseQuery.Count();

        var cookbooksDto = _mapper.Map<List<GetCookbookDto>>(cookbooks);

        return new PagedResult<GetCookbookDto>(cookbooksDto, totalItemsCount, query.PageSize, query.PageNumber);
    }

    public async Task<GetCookbookDto> GetCookbookByIdAsync(Guid id)
    {
        var cookbook = await _dbContext.Cookbooks.FirstOrDefaultAsync(r => r.Id == id);

        if (cookbook is null)
            throw new NotFoundException("Cookbook not found");

        if (!cookbook.Published)
        {
            var authorizationResult = _authorizationService.AuthorizeAsync(_userContextService.User, cookbook,
                new ResourceOperationRequirement(ResourceOperation.Delete)).Result;

            if (!authorizationResult.Succeeded)
                throw new ForbidException("Access denied");
        }

        return _mapper.Map<GetCookbookDto>(cookbook);
    }

    public PagedResult<GetCookbookDto> GetAllUsersCookbooks(CookingQuery query)
    {
        var baseQuery = _dbContext
            .Cookbooks
            .Where(c => c.CreatedById == _userContextService.GetUserId)
            .Where(c => query.SearchPhrase == null || c.Name.ToLower()
                .Contains(query.SearchPhrase.ToLower()) || c.Description.ToLower()
                .Contains(query.SearchPhrase.ToLower()));

        if (!string.IsNullOrEmpty(query.SortBy))
        {
            var columnsSelectors = new Dictionary<string, Expression<Func<Cookbook, object>>>
            {
                { nameof(Cookbook.Name), c => c.Name },
                { nameof(Cookbook.Description), c => c.Description },
            };
            var selectedColumn = columnsSelectors[query.SortBy];
            baseQuery = query.SortDirection == SortDirection.ASC
                ? baseQuery.OrderBy(selectedColumn)
                : baseQuery.OrderByDescending(selectedColumn);
        }

        var cookbooks = baseQuery.Skip(query.PageSize * (query.PageNumber - 1))
            .Take(query.PageSize)
            .ToList();

        var totalItemsCount = baseQuery.Count();

        var cookbooksDto = _mapper.Map<List<GetCookbookDto>>(cookbooks);

        return new PagedResult<GetCookbookDto>(cookbooksDto, totalItemsCount, query.PageSize, query.PageNumber);
    }
}