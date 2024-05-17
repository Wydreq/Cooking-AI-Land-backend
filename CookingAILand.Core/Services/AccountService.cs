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
using CookingAILand.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.IdentityModel.Tokens;

namespace CookingAILand;

public interface IAccountService
{
    Task RegisterUserAsync(RegisterUserDto dto);
    Task<string> GenerateJwtAsync(LoginDto dto);

    Task<string> UploadProfilePhoto(IFormFile file);

    Task ResetPassword(ForgotPasswordDto dto);

    Task<GetUserDto> GetMeAsync();
}

public class AccountService : IAccountService
{
    private readonly IPasswordHasher<User?> _passwordHasher;
    private readonly CookingDbContext _context;
    private readonly IEmailSender _emailSender;
    private readonly IUserContextService _userContextService;
    private readonly IMapper _mapper;
    private readonly IPhotoUploadService _photoUploadService;
    private readonly AuthenticationSettings _authenticationSettings;
    private readonly IUserRepository _userRepository;

    public AccountService(IPasswordHasher<User?> passwordHasher, AuthenticationSettings authenticationSettings,
        IUserRepository userRepository, CookingDbContext context, IEmailSender emailSender,
        IUserContextService userContextService, IMapper mapper, IPhotoUploadService photoUploadService)
    {
        _passwordHasher = passwordHasher;
        _authenticationSettings = authenticationSettings;
        _userRepository = userRepository;
        _context = context;
        _emailSender = emailSender;
        _userContextService = userContextService;
        _mapper = mapper;
        _photoUploadService = photoUploadService;
    }

    public async Task RegisterUserAsync(RegisterUserDto dto)
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
        await _context.Users.AddAsync(newUser);
        await _context.SaveChangesAsync();
    }

    public async Task<string> UploadProfilePhoto(IFormFile file)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == _userContextService.GetUserId);
        if (user is null)
        {
            throw new BadRequestException("User not found");
        }

        var uploadResult = await _photoUploadService.upload(file);
        user.PhotoUrl = uploadResult.Url;
        await _context.SaveChangesAsync();
        return uploadResult.Url;
    }

    public async  Task<string> GenerateJwtAsync(LoginDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
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

    public async Task ResetPassword(ForgotPasswordDto dto)
    {
        await _emailSender.SendEmailAsync(dto.Email, "test", "test");
    }

    public async Task<GetUserDto> GetMeAsync()
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == _userContextService.GetUserId);
        return _mapper.Map<GetUserDto>(user);
    }
}