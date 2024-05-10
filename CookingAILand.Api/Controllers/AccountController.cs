using CookingAILand.Core.Models;
using CookingAILand.Exceptions;
using CookingAILand.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace CookingAILand.Controllers;

[Route("/api/account")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly AuthenticationSettings _authenticationSettings;

    public AccountController(IAccountService accountService, AuthenticationSettings authenticationSettings)
    {
        _accountService = accountService;
        _authenticationSettings = authenticationSettings;
    }

    [HttpPost("register")]
    public ActionResult CreateUser([FromBody] RegisterUserDto dto)
    {
        _accountService.RegisterUser(dto);
        return Ok();
    }

    [HttpPost("login")]
    public ActionResult<TokenDto> Login([FromBody] LoginDto dto)
    {
        var token = _accountService.GenerateJwt(dto);
        Response.Cookies.Append("X-Access-Token", token,
            new CookieOptions()
            {
                HttpOnly = true, IsEssential = true, Secure = false, SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(_authenticationSettings.JwtExpireDays)
            });
        return Ok(new TokenDto() { Token = token });
    }

    [HttpPost("resetPassword")]
    public ActionResult ResetPassword([FromBody] ForgotPasswordDto dto)
    {
        if (!ModelState.IsValid)
            throw new BadRequestException("Insert correct email address");
        _accountService.ResetPassword(dto);
        return Ok();
    }

    [HttpGet("me")]
    [Authorize]
    public ActionResult<GetUserDto> GetMe()
    {
        var user = _accountService.GetMe();
        return Ok(user);
    }
}