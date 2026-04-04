using api.application.Interfaces;

namespace api.application.Tests;


public sealed class FakePasswordHasher : IPasswordHasher
{
    private readonly bool _verifyResult;

    public FakePasswordHasher(bool verifyResult = true)
    {
        _verifyResult = verifyResult;
    }

    public (string Hash, string Salt) HashPassword(string password) => ("hash", "salt");

    public bool Verify(string password, string passwordHash, string passwordSalt) => _verifyResult;
}


