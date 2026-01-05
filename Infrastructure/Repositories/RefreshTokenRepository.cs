public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _dbContext;

    public RefreshTokenRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void AddRefreshToken(RefreshToken refreshToken)
    {
        _dbContext.RefreshTokens.Add(refreshToken);
        _dbContext.SaveChanges();
    }

    public RefreshToken GetRefreshToken(string token)
    {
        return _dbContext.RefreshTokens.FirstOrDefault(rt => rt.Token == token);
    }

    public void Update(RefreshToken refreshToken)
    {
        _dbContext.RefreshTokens.Update(refreshToken);
        _dbContext.SaveChanges();
    }
}