public interface IAuthServices
{
    Response Register(CreateUser request);
    Response Login(string email, string password);
    Response ChangePassword(int userId, string currentPassword, string newPassword);
}