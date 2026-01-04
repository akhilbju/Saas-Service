public class AuthService : IAuthServices
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasherService _passwordHasherService;
    private readonly ICacheService _cacheService;

    public AuthService(IUserRepository userRepository, IPasswordHasherService passwordHasherService, ICacheService cacheService)
    {
        _userRepository = userRepository;
        _passwordHasherService = passwordHasherService;
        _cacheService = cacheService;
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

    public Response Login(string email, string password)
    {
        var user = _userRepository.GetUserByEmail(email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            return new Response { IsSuccess = false, Message = "Invalid email or password." };
        }

        return new Response { IsSuccess = true, Message = "Login successful." };
    }

    public Response ChangePassword(int userId, string currentPassword, string newPassword)
    {
        var user = _userRepository.GetUserById(userId);
        if (user == null || !BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
        {
            return new Response { IsSuccess = false, Message = "Current password is incorrect." };
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        _userRepository.UpdateUser(user);

        return new Response { IsSuccess = true, Message = "Password changed successfully." };
    }
}