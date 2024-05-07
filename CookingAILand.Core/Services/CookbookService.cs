using AutoMapper;
using CookingAILand.Core.Authorization;
using CookingAILand.Core.DAL.Entities;
using CookingAILand.Core.DAL.Persistence;
using CookingAILand.Core.Models;
using CookingAILand.Exceptions;
using Microsoft.AspNetCore.Authorization;

namespace CookingAILand;

public interface ICookbookService
{
    Guid Create(CookbookDto dto);
    void Delete(Guid id);
    void Update(Guid id, CookbookDto dto);
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

}