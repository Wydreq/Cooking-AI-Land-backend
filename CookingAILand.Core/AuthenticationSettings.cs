namespace CookingAILand;

public class AuthenticationSettings
{
    public string JwtKey { get; set; }
    public double JwtExpireDays { get; set; }
    public string JwtIssuer { get; set; }
}