using AutoMapper;
using CookingAILand.Core.Authorization;
using CookingAILand.Core.DAL.Entities;
using CookingAILand.Core.DAL.Persistence;
using CookingAILand.Core.Models;
using CookingAILand.Exceptions;
using Microsoft.AspNetCore.Authorization;

namespace CookingAILand;

public interface IRecipeService
{
    Guid Create(Guid cookbookId, CreateRecipeDto dto);
}

public class RecipeService : IRecipeService
{
    private readonly CookingDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IUserContextService _userContextService;
    private readonly IAuthorizationService _authorizationService;

    public RecipeService(CookingDbContext dbContext, IMapper mapper, IUserContextService userContextService, IAuthorizationService authorizationService)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _userContextService = userContextService;
        _authorizationService = authorizationService;
    }

    public Guid Create(Guid cookbookId, CreateRecipeDto dto)
    {
        var cookbook = _dbContext.Cookbooks.FirstOrDefault(r => r.Id == cookbookId);
        
        if (cookbook is null)
            throw new NotFoundException("Cookbook not found");

        var authorizationResult = _authorizationService.AuthorizeAsync(_userContextService.User, cookbook,
            new ResourceOperationRequirement(ResourceOperation.Delete)).Result;

        if (!authorizationResult.Succeeded)
            throw new ForbidException("Access denied");
        
        var recipe = _mapper.Map<Recipe>(dto);
        recipe.CreatedById = (Guid)_userContextService.GetUserId!;
        recipe.CookbookId = cookbookId;
        _dbContext.Recipes.Add(recipe);
        _dbContext.SaveChanges();
        return recipe.Id;
    }
}