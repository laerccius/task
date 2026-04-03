using TaskTrack.Domain.Entities;

namespace TaskTrack.Application.Interfaces;

public interface ITokenService
{
    string CreateToken(User user);
}
