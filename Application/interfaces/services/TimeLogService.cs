public interface TimeLogService
{
    Response Register(CreateUser request);
    LoginResponse Login(LoginUser request);
    Task<LoginResponse> GenerateRefreshToken(string RefreshToken);
}