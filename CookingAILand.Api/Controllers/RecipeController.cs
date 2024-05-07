using CookingAILand.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CookingAILand.Api.Controllers;

[Route("/api/cookbook/{cookbookId}/recipe")]
[ApiController]
[Authorize]
public class RecipeController : ControllerBase
{
    private readonly IRecipeService _recipeService;

    public RecipeController(IRecipeService recipeService)
    {
        _recipeService = recipeService;
    }

    [HttpPost]
    public ActionResult CreateCookbook([FromBody] CreateRecipeDto dto, [FromRoute] Guid cookbookId)
    {
        var id = _recipeService.Create(cookbookId, dto);
        return Created($"/api/{cookbookId}/recipe/{id}", null);
    }
    
}