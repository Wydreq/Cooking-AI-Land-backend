using CookingAILand.Core.Models;
using CookingAILand.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CookingAILand.Api.Controllers;

[Route("/api/recipe")]
[ApiController]
[Authorize]
public class RecipeController : ControllerBase
{
    private readonly IRecipeService _recipeService;

    public RecipeController(IRecipeService recipeService)
    {
        _recipeService = recipeService;
    }

    [HttpPost("{cookbookId}")]
    public async Task<ActionResult> CreateRecipe([FromForm] CreateRecipeDto dto, [FromRoute] Guid cookbookId)
    {
        var id = await _recipeService.CreateAsync(cookbookId, dto);
        return Created($"/api/recipe/{id}", null);
    }
    
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteRecipe([FromRoute] Guid id)
    {
        await _recipeService.DeleteAsync(id);
        return Ok(id);
    }
    
    [HttpPut("{id}")]
    public async Task<ActionResult> Update([FromForm] CreateRecipeDto dto, [FromRoute] Guid id)
    {
        await _recipeService.UpdateAsync(id, dto);
        return Ok();
    }
    
    [HttpGet]
    [AllowAnonymous]
    public ActionResult<IEnumerable<GetRecipeDto>> GetAllPublishedRecipes([FromQuery] CookingQuery query)
    {
        var recipesDtos = _recipeService.GetAllPublishedRecipes(query);
        return Ok(recipesDtos);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<GetRecipeDto>> GetRecipeById([FromRoute] Guid id)
    {
        var recipeDto = await _recipeService.GetRecipeByIdAsync(id);
        return Ok(recipeDto);
    }
}