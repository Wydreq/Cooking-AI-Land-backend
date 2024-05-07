using System.Security.Claims;
using CookingAILand.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CookingAILand.Api.Controllers;

[Route("/api/cookbook")]
[ApiController]
[Authorize]
public class CookbookController : ControllerBase
{
    private readonly ICookbookService _cookbookService;

    public CookbookController(ICookbookService cookbookService)
    {
        _cookbookService = cookbookService;
    }

    [HttpPost]
    public ActionResult CreateCookbook([FromBody] CreateCookbookDto dto)
    {
        var id = _cookbookService.Create(dto);
        return Created($"/api/cookbook/{id}", null);
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteCookbook([FromRoute] Guid id)
    {
        _cookbookService.Delete(id);
        return Ok(id);
    }

}