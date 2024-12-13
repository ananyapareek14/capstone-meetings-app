//using meetings_app_server.Models.Domain;

//namespace meetings_app_server.Repositories;

//public interface IUserRepository
//{
//    Task<User> Register(User user);
//    Task<User?> Login(string email, string password);
//    Task SaveUserSessionKey(Guid userId, string sessionKey);
//    Task<string?> GetUserSessionKey(Guid userId);

//    Task<IEnumerable<User>> GetAllUsers();
//}

using meetings_app_server.Models.Domain;

namespace meetings_app_server.Repositories
{
    public interface IUserRepository
    {
        Task<User> Register(User user);
        Task<User?> Login(string email, string password);
        Task SaveUserSessionKey(Guid userId, string sessionKey);
        Task<string?> GetUserSessionKey(Guid userId);
        Task<IEnumerable<User>> GetAllUsers();
    }
}
