using CookingAILand.Core.Models;
using CookingAILand.Exceptions;
using CookingAILand.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace CookingAILand.Controllers;

[Route("/api/account")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }
    
    [HttpPost("register")]
    public ActionResult CreateUser([FromBody] RegisterUserDto dto)
    {
        _accountService.RegisterUser(dto);
        return Ok();
    }

    [HttpPost("login")]
    public ActionResult Login([FromBody] LoginDto dto)
    {
        var token = _accountService.GenerateJwt(dto);
        return Ok(token);
    }
    
    [HttpPost("resetPassword")]
    public ActionResult ResetPassword([FromBody] ForgotPasswordDto dto)
    {
        if (!ModelState.IsValid)
            throw new BadRequestException("Insert correct email address");
        _accountService.ResetPassword(dto);
        return Ok();
    }
}