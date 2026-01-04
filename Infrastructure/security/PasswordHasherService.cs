public class PasswordHasherService : IPasswordHasherService
{
    public string Hash(string password)
    {
        // workFactor = cost (10–12 is typical; higher = slower but stronger)
        return BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12);
    }

    public bool Verify(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}