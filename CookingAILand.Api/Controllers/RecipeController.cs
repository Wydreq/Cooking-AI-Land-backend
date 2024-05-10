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
    public ActionResult CreateRecipe([FromBody] CreateRecipeDto dto, [FromRoute] Guid cookbookId)
    {
        var id = _recipeService.Create(cookbookId, dto);
        return Created($"/api/recipe/{id}", null);
    }
    
    [HttpDelete("{id}")]
    public ActionResult DeleteRecipe([FromRoute] Guid id)
    {
        _recipeService.Delete(id);
        return Ok(id);
    }
    
    [HttpPut("{id}")]
    public ActionResult Update([FromBody] CreateRecipeDto dto, [FromRoute] Guid id)
    {
        _recipeService.Update(id, dto);
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
    public ActionResult<GetRecipeDto> GetRecipeById([FromRoute] Guid id)
    {
        var recipeDto = _recipeService.GetRecipeById(id);
        return Ok(recipeDto);
    }
}