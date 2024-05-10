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

namespace CookingAILand.Core.Services;

public interface IRecipeService
{
    Guid Create(Guid cookbookId, CreateRecipeDto dto);
    void Delete(Guid id);
    void Update(Guid id, CreateRecipeDto dto);
    PagedResult<GetRecipeDto> GetAllPublishedRecipes(CookingQuery query);
    GetRecipeDto GetRecipeById(Guid id);
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
    
    public void Delete(Guid id)
    {
        var userId = _userContextService.GetUserId;
        var recipe = _dbContext.Recipes.FirstOrDefault(r => r.Id == id);

        if (recipe is null)
            throw new NotFoundException("Recipe not found");

        if (userId != recipe.CreatedById)
            throw new ForbidException("Access denied");

        _dbContext.Recipes.Remove(recipe);
        _dbContext.SaveChanges();
    }
    
    public void Update(Guid id, CreateRecipeDto dto)
    {
        var userId = _userContextService.GetUserId;
        var recipe = _dbContext.Recipes.FirstOrDefault(r => r.Id == id);

        if (recipe is null)
            throw new NotFoundException("Cookbook not found");

        if (userId != recipe.CreatedById)
            throw new ForbidException("Access denied");

        

        var updatedRecipe = _mapper.Map(dto, recipe);

        _dbContext.Recipes.Update(updatedRecipe);
        _dbContext.SaveChanges();
    }

    public PagedResult<GetRecipeDto> GetAllPublishedRecipes(CookingQuery query)
    {
        var baseQuery = _dbContext
            .Recipes
            .Include(r => r.Cookbook)
            .Where(r => r.Cookbook.Published == true)
            .Where(c => query.SearchPhrase == null || c.Name.ToLower()
                .Contains(query.SearchPhrase.ToLower()) || c.Description.ToLower()
                .Contains(query.SearchPhrase.ToLower()));
        
        if (!string.IsNullOrEmpty(query.SortBy))
        {
            var columnsSelectors = new Dictionary<string, Expression<Func<Recipe, object>>>
            {
                {nameof(Recipe.Name), c => c.Name},
                {nameof(Recipe.Description), c => c.Description},
            };
            var selectedColumn = columnsSelectors[query.SortBy];
            baseQuery = query.SortDirection == SortDirection.ASC ? baseQuery.OrderBy(selectedColumn) : baseQuery.OrderByDescending(selectedColumn);
        }
        
        var recipes = baseQuery.Skip(query.PageSize * (query.PageNumber - 1))
            .Take(query.PageSize)
            .ToList();
        
        var totalItemsCount = baseQuery.Count();
            
        var recipesDto = _mapper.Map<List<GetRecipeDto>>(recipes);

        return new PagedResult<GetRecipeDto>(recipesDto, totalItemsCount, query.PageSize, query.PageNumber);
    }

    public GetRecipeDto GetRecipeById(Guid id)
    {
        var recipe = _dbContext.Recipes.FirstOrDefault(r => r.Id == id);
        var cookbook = _dbContext.Cookbooks.FirstOrDefault(c => c.Id == recipe.CookbookId);
        
        if(recipe is null) 
            throw new NotFoundException("Cookbook not found");

        if (cookbook != null && cookbook.Published== false && cookbook.CreatedById == _userContextService.GetUserId)
        {
            var userId = _userContextService.GetUserId;
            if (userId != recipe.CreatedById)
                throw new ForbidException("Access denied");
            
        }

        var recipeDto = _mapper.Map<GetRecipeDto>(recipe);
        return recipeDto;
    }

    // public PagedResult<GetRecipeDto> GetCookbookRecipes(CookingQuery query, Guid cookbookId)
    // {
    //     var cookbook = _dbContext.Cookbooks.FirstOrDefault(r => r.Id == cookbookId);
    //     
    // }

}