using System.Runtime.InteropServices.JavaScript;
using CookingAILand.Core.Models;
using CookingAILand.Exceptions;
using CookingAILand.Models;
using Microsoft.AspNetCore.Authentication;
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
    public async Task<ActionResult> CreateUser([FromBody] RegisterUserDto dto)
    {
        await _accountService.RegisterUserAsync(dto);
        return Ok();
    }

    [HttpPost("login")]
    public async Task<ActionResult<TokenDto>> Login([FromBody] LoginDto dto)
    {
        var token = await _accountService.GenerateJwtAsync(dto);
        Response.Cookies.Append("X-Access-Token", token,
            new CookieOptions()
            {
                HttpOnly = true, IsEssential = true, Secure = false, SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(_authenticationSettings.JwtExpireDays)
            });
        return Ok(new TokenDto()
        {
            Token = token,
            ExpirationDate = DateTime.Now.AddDays(_authenticationSettings.JwtExpireDays)
                .ToString("yyyy-M-d HH:mm:ss")
        });
    }

    [HttpPost("logout")]
    public ActionResult Logout()
    {
        Response.Cookies.Delete("X-Access-Token");
        return Ok();
    }

    [HttpPost("photoUpload")]
    [Authorize]
    public async Task<ActionResult<string>> UploadPhoto([FromForm] IFormFile file)
    {
        var photoUrl =  await _accountService.UploadProfilePhoto(file);
        return Ok(photoUrl);
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
    public async Task<ActionResult<GetUserDto>> GetMe()
    {
        var user = await _accountService.GetMeAsync();
        return Ok(user);
    }
}