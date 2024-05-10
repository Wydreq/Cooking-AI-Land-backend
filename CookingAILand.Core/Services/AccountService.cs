using System.IdentityModel.Tokens.Jwt;
using CookingAILand.Exceptions;
using CookingAILand.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using CookingAILand.Core.DAL.Entities;
using CookingAILand.Core.DAL.Persistence;
using CookingAILand.Core.DAL.Repositories;
using CookingAILand.Core.Entities;
using CookingAILand.Core.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.IdentityModel.Tokens;

namespace CookingAILand;

public interface IAccountService
{
    void RegisterUser(RegisterUserDto dto);
    string GenerateJwt(LoginDto dto);

    void ResetPassword(ForgotPasswordDto dto);

    GetUserDto GetMe();
}

public class AccountService : IAccountService
{
    private readonly IPasswordHasher<User?> _passwordHasher;
    private readonly CookingDbContext _context;
    private readonly IEmailSender _emailSender;
    private readonly IUserContextService _userContextService;
    private readonly IMapper _mapper;
    private readonly AuthenticationSettings _authenticationSettings;
    private readonly IUserRepository _userRepository;

    public AccountService(IPasswordHasher<User?> passwordHasher, AuthenticationSettings authenticationSettings,
        IUserRepository userRepository, CookingDbContext context, IEmailSender emailSender,
        IUserContextService userContextService, IMapper mapper)
    {
        _passwordHasher = passwordHasher;
        _authenticationSettings = authenticationSettings;
        _userRepository = userRepository;
        _context = context;
        _emailSender = emailSender;
        _userContextService = userContextService;
        _mapper = mapper;
    }

    public async void RegisterUser(RegisterUserDto dto)
    {
        var newUser = new User()
        {
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            DateOfBirth = dto.DateOfBirth,
            RoleId = dto.RoleId
        };

        newUser.PasswordHash = _passwordHasher.HashPassword(newUser, dto.Password);
        _context.Users.Add(newUser);
        _context.SaveChanges();
    }

    public string GenerateJwt(LoginDto dto)
    {
        var user = _context.Users.FirstOrDefault(u => u.Email == dto.Email);
        if (user is null)
        {
            throw new BadRequestException("Invalid username or password");
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            throw new BadRequestException("Invalid username or password");
        }

        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, $"{user.FirstName}{user.LastName}"),
            new Claim(ClaimTypes.Role, $"{user.Role}"),
        };


        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.JwtKey));
        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddDays(_authenticationSettings.JwtExpireDays);
        var token = new JwtSecurityToken(_authenticationSettings.JwtIssuer, _authenticationSettings.JwtIssuer, claims,
            expires: expires, signingCredentials: cred);
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }

    public async void ResetPassword(ForgotPasswordDto dto)
    {
        await _emailSender.SendEmailAsync(dto.Email, "test", "test");
    }

    public GetUserDto GetMe()
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == _userContextService.GetUserId);
        return _mapper.Map<GetUserDto>(user);
    }
}