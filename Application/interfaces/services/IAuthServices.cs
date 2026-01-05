public interface IAuthServices
{
    Response Register(CreateUser request);
    LoginResponse Login(LoginUser request);
    Task<LoginResponse> GenerateRefreshToken(string RefreshToken);
}