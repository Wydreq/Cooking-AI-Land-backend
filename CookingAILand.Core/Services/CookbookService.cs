using System.Linq.Expressions;
using AutoMapper;
using CookingAILand.Core.Authorization;
using CookingAILand.Core.DAL.Entities;
using CookingAILand.Core.DAL.Enums;
using CookingAILand.Core.DAL.Persistence;
using CookingAILand.Core.Models;
using CookingAILand.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace CookingAILand;

public interface ICookbookService
{
    Guid Create(CookbookDto dto);
    void Delete(Guid id);
    void Update(Guid id, CookbookDto dto);
    PagedResult<CookbookDto> GetAllPublishedCookbooks(CookingQuery query);
    GetCookbookDto GetCookbookById(Guid id);
    PagedResult<CookbookDto> GetAllUsersCookbooks(CookingQuery query);
}

public class CookbookService : ICookbookService
{
    private readonly CookingDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IUserContextService _userContextService;
    private readonly IAuthorizationService _authorizationService;

    public CookbookService(CookingDbContext dbContext, IMapper mapper, IUserContextService userContextService, IAuthorizationService authorizationService)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _userContextService = userContextService;
        _authorizationService = authorizationService;
    }
    
    public Guid Create(CookbookDto dto)
    {
        var cookbook = _mapper.Map<Cookbook>(dto);
        cookbook.CreatedById = (Guid)_userContextService.GetUserId!;
        _dbContext.Cookbooks.Add(cookbook);
        _dbContext.SaveChanges();
        return cookbook.Id;
    }

    public void Delete(Guid id)
    {
        var cookbook = _dbContext.Cookbooks.FirstOrDefault(r => r.Id == id);

        if (cookbook is null)
            throw new NotFoundException("Cookbook not found");

        var authorizationResult = _authorizationService.AuthorizeAsync(_userContextService.User, cookbook,
            new ResourceOperationRequirement(ResourceOperation.Delete)).Result;

        if (!authorizationResult.Succeeded)
            throw new ForbidException("Access denied");

        _dbContext.Cookbooks.Remove(cookbook);
        _dbContext.SaveChanges();
    }

    public void Update(Guid id, CookbookDto dto)
    {
        var cookbook = _dbContext.Cookbooks.FirstOrDefault(r => r.Id == id);

        if (cookbook is null)
            throw new NotFoundException("Cookbook not found");

        var authorizationResult = _authorizationService.AuthorizeAsync(_userContextService.User, cookbook,
            new ResourceOperationRequirement(ResourceOperation.Delete)).Result;

        if (!authorizationResult.Succeeded)
            throw new ForbidException("Access denied");

        cookbook.Name = dto.Name;
        cookbook.Description = dto.Description;
        cookbook.Published = dto.Published;

        _dbContext.SaveChanges();
    }

    public PagedResult<CookbookDto> GetAllPublishedCookbooks(CookingQuery query)
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
                {nameof(Cookbook.Name), c => c.Name},
                {nameof(Cookbook.Description), c => c.Description},
            };
            var selectedColumn = columnsSelectors[query.SortBy];
            baseQuery = query.SortDirection == SortDirection.ASC ? baseQuery.OrderBy(selectedColumn) : baseQuery.OrderByDescending(selectedColumn);
        }
        
        var cookbooks = baseQuery.Skip(query.PageSize * (query.PageNumber - 1))
            .Take(query.PageSize)
            .ToList();
        
        var totalItemsCount = baseQuery.Count();
            
        var cookbooksDto = _mapper.Map<List<CookbookDto>>(cookbooks);

        return new PagedResult<CookbookDto>(cookbooksDto, totalItemsCount, query.PageSize, query.PageNumber);
    }

    public GetCookbookDto GetCookbookById(Guid id)
    {
        var cookbook = _dbContext.Cookbooks.FirstOrDefault(r => r.Id == id);

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
    
    public PagedResult<CookbookDto> GetAllUsersCookbooks(CookingQuery query)
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
                {nameof(Cookbook.Name), c => c.Name},
                {nameof(Cookbook.Description), c => c.Description},
            };
            var selectedColumn = columnsSelectors[query.SortBy];
            baseQuery = query.SortDirection == SortDirection.ASC ? baseQuery.OrderBy(selectedColumn) : baseQuery.OrderByDescending(selectedColumn);
        }
        
        var cookbooks = baseQuery.Skip(query.PageSize * (query.PageNumber - 1))
            .Take(query.PageSize)
            .ToList();
        
        var totalItemsCount = baseQuery.Count();
            
        var cookbooksDto = _mapper.Map<List<CookbookDto>>(cookbooks);

        return new PagedResult<CookbookDto>(cookbooksDto, totalItemsCount, query.PageSize, query.PageNumber);
    }

}