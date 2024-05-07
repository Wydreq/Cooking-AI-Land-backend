using System.IdentityModel.Tokens.Jwt;
using CookingAILand.Exceptions;
using CookingAILand.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;
using CookingAILand.Core.DAL.Entities;
using CookingAILand.Core.DAL.Persistence;
using CookingAILand.Core.DAL.Repositories;
using CookingAILand.Core.Entities;
using CookingAILand.Core.Models;
using Microsoft.IdentityModel.Tokens;

namespace CookingAILand;

public interface IAccountService
{
    void RegisterUser(RegisterUserDto dto);
    string GenerateJwt(LoginDto dto);
}

public class AccountService : IAccountService
{
    private readonly IPasswordHasher<User?> _passwordHasher;
    private readonly CookingDbContext _context;
    private readonly AuthenticationSettings _authenticationSettings;
    private readonly IUserRepository _userRepository;

    public AccountService(IPasswordHasher<User?> passwordHasher, AuthenticationSettings authenticationSettings, IUserRepository userRepository, CookingDbContext context)
    {
        _passwordHasher = passwordHasher;
        _authenticationSettings = authenticationSettings;
        _userRepository = userRepository;
        _context = context;
    }

    public void RegisterUser(RegisterUserDto dto)
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
        _userRepository.AddAsync(newUser);
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
}