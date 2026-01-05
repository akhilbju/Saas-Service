public interface IRefreshTokenRepository
{
    void AddRefreshToken(RefreshToken refreshToken);
    RefreshToken GetRefreshToken(string token);
    void Update(RefreshToken refreshToken);
}