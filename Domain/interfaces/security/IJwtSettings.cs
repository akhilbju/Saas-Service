using System.Security.Claims;

public interface IJwtSettings
{
    string GenerateToken(User user);
}