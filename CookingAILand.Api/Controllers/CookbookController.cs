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
    public ActionResult<IEnumerable<GetCookbookDto>> GetAllPublishedCookbooks([FromQuery] CookingQuery query)
    {
        var cookbooksDtos = _cookbookService.GetAllPublishedCookbooks(query);
        return Ok(cookbooksDtos);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<GetCookbookDto>> GetCookbookById([FromRoute] Guid id)
    {
        var cookbookDto = await _cookbookService.GetCookbookByIdAsync(id);
        return Ok(cookbookDto);
    }
    
    [HttpGet("mine")]
    public ActionResult<GetCookbookDto> GetAllUsersCookbooks([FromQuery] CookingQuery query)
    {
        var cookbookDtos = _cookbookService.GetAllUsersCookbooks(query);
        return Ok(cookbookDtos);
    }

    [HttpPost]
    public async Task<ActionResult> CreateCookbook([FromForm] CookbookDto dto)
    {
        var id =  await _cookbookService.CreateAsync(dto);
        return Created($"/api/cookbook/{id}", null);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCookbook([FromRoute] Guid id)
    {
        await _cookbookService.DeleteAsync(id);
        return Ok(id);
    }
    
    [HttpPut("{id}")]
    public async Task<ActionResult> Update([FromForm] CookbookDto dto, [FromRoute] Guid id)
    {
        await _cookbookService.UpdateAsync(id, dto);
        return Ok();
    }
}