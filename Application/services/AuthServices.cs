public class AuthService : IAuthServices
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasherService _passwordHasherService;
    private readonly ICacheService _cacheService;
    private readonly IJwtSettings _jwtSettings;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public AuthService(IUserRepository userRepository, IPasswordHasherService passwordHasherService, ICacheService cacheService, IJwtSettings jwtSettings, IRefreshTokenRepository refreshTokenRepository)
    {
        _userRepository = userRepository;
        _passwordHasherService = passwordHasherService;
        _cacheService = cacheService;
        _jwtSettings = jwtSettings;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public Response Register(CreateUser request)
    {
        Response res = new();
        User newUser = new()
        {
            Username = request.Username,
            PasswordHash = _passwordHasherService.Hash(request.Password),
            Email = request.Email,
            UserType = request.UserType
        };
        _userRepository.AddUser(newUser);
        _cacheService.Remove("GetUsers");
        res.IsSuccess = true;
        res.Message = "User created successfully";
        return res;
    }

    public LoginResponse Login(LoginUser request)
    {
        LoginResponse res = new();
        var user = _userRepository.GetUserByEmail(request.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return new LoginResponse { IsSuccess = false, Message = ErrorMessages.InvalidCredentials };
        }
        var token = _jwtSettings.GenerateToken(user);
        res.IsSuccess = true;
        res.AccessToken = token;
        res.RefreshToken = Guid.NewGuid().ToString();
        createRefreshTokenForUser(user.Id, res.RefreshToken);
        res.UserType = user.UserType;
        res.Message = SuccessMessages.LoginSuccess;
        return res;
    }

    public async Task<LoginResponse> GenerateRefreshToken(string RefreshToken)
    {
        LoginResponse res = new();
        var refreshToken = _refreshTokenRepository.GetRefreshToken(RefreshToken);
        if (refreshToken == null)
        {
            res.IsSuccess = false;
            res.Message = ErrorMessages.TokenExpired;
            return res;
        }
        var user = _userRepository.GetUserById(refreshToken.UserId);
        if (user == null)
        {
            res.IsSuccess = false;
            res.Message = ErrorMessages.UserNotFound;
            return res;
        }
        refreshToken.IsRevoked = true;
        _refreshTokenRepository.Update(refreshToken);
        var newAccessToken = _jwtSettings.GenerateToken(user);
        var newRefreshToken = Guid.NewGuid().ToString();
        createRefreshTokenForUser(user.Id, newRefreshToken);
        res.IsSuccess = true;
        res.AccessToken = newAccessToken;
        res.RefreshToken = newRefreshToken;
        return res;
    }

    private void createRefreshTokenForUser(int userId, string refreshToken)
    {
        RefreshToken newRefreshToken = new RefreshToken()
        {
            UserId = userId,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        };
        _refreshTokenRepository.AddRefreshToken(newRefreshToken);
    }
}