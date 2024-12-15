using meetings_server.Models.Domain;

namespace meetings_server.Repositories;

public interface ITokenRepository
{
    string CreateJWTToken(ApplicationUser user);
}
