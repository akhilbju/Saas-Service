public interface IUserRepository
{
    void AddUser(User user);
    User GetUserByEmail(string email);
    User GetUserById(int userId);
    void UpdateUser(User user);
}