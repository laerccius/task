using api.domain.Entities;

namespace api.application.Interfaces;

public interface ITokenService
{
    string CreateToken(User user);
}
