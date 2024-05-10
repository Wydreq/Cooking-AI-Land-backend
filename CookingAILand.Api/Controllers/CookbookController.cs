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

    [HttpGet]
    [AllowAnonymous]
    public ActionResult<IEnumerable<CookbookDto>> GetAllPublishedCookbooks([FromQuery] CookingQuery query)
    {
        var cookbooksDtos = _cookbookService.GetAllPublishedCookbooks(query);
        return Ok(cookbooksDtos);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public ActionResult<GetCookbookDto> GetCookbookById([FromRoute] Guid id)
    {
        var cookbookDto = _cookbookService.GetCookbookById(id);
        return Ok(cookbookDto);
    }
    
    [HttpGet("mine")]
    public ActionResult<GetCookbookDto> GetAllUsersCookbooks([FromQuery] CookingQuery query)
    {
        var cookbookDtos = _cookbookService.GetAllUsersCookbooks(query);
        return Ok(cookbookDtos);
    }

    [HttpPost]
    public ActionResult CreateCookbook([FromBody] CookbookDto dto)
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
    
    [HttpPut("{id}")]
    public ActionResult Update([FromBody] CookbookDto dto, [FromRoute] Guid id)
    {
        _cookbookService.Update(id, dto);
        return Ok();
    }
}