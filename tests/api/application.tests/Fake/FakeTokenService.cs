using api.application.Interfaces;
using api.domain.Entities;

namespace api.application.Tests;

public sealed class FakeTokenService : ITokenService
{
    public string CreateToken(User user) => "fake-token";
}


