using TaskTrack.Application.DTOs;
using TaskTrack.Application.Interfaces;
using TaskTrack.Application.Services;
using TaskTrack.Domain.Entities;
using Xunit;

namespace TaskTrack.Application.Tests;

public class UserServiceTests
{
    [Fact]
    public async Task RegisterAsync_Should_Create_User_When_Email_Is_New()
    {
        var repository = new InMemoryUserRepository();
        var service = new UserService(
            repository,
            new FakePasswordHasher(),
            new FakeTokenService(),
            new FakeDateTimeProvider(new DateTime(2026, 4, 3, 12, 0, 0, DateTimeKind.Utc)));

        var response = await service.RegisterAsync(new RegisterUserRequest
        {
            FullName = "Demo User",
            Email = "demo@example.com",
            Password = "Password1!"
        }, CancellationToken.None);

        Assert.Equal("demo@example.com", response.Email);
        Assert.Equal("fake-token", response.Token);
        Assert.Single(repository.Users);
    }

    [Fact]
    public async Task LoginAsync_Should_Throw_When_Password_Is_Invalid()
    {
        var repository = new InMemoryUserRepository();
        repository.Users.Add(new User
        {
            Id = Guid.NewGuid(),
            Email = "demo@example.com",
            FullName = "Demo User",
            PasswordHash = "hash",
            PasswordSalt = "salt"
        });

        var service = new UserService(
            repository,
            new FakePasswordHasher(verifyResult: false),
            new FakeTokenService(),
            new FakeDateTimeProvider(DateTime.UtcNow));

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.LoginAsync(
            new LoginRequest
            {
                Email = "demo@example.com",
                Password = "wrong"
            },
            CancellationToken.None));
    }

    private sealed class InMemoryUserRepository : IUserRepository
    {
        public List<User> Users { get; } = [];

        public Task<User> CreateAsync(User user, CancellationToken cancellationToken)
        {
            Users.Add(user);
            return Task.FromResult(user);
        }

        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return Task.FromResult(Users.FirstOrDefault(x => x.Email.Equals(email, StringComparison.OrdinalIgnoreCase)));
        }

        public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return Task.FromResult(Users.FirstOrDefault(x => x.Id == id));
        }
    }

    private sealed class FakePasswordHasher : IPasswordHasher
    {
        private readonly bool _verifyResult;

        public FakePasswordHasher(bool verifyResult = true)
        {
            _verifyResult = verifyResult;
        }

        public (string Hash, string Salt) HashPassword(string password) => ("hash", "salt");

        public bool Verify(string password, string passwordHash, string passwordSalt) => _verifyResult;
    }

    private sealed class FakeTokenService : ITokenService
    {
        public string CreateToken(User user) => "fake-token";
    }

    private sealed class FakeDateTimeProvider : IDateTimeProvider
    {
        public FakeDateTimeProvider(DateTime utcNow)
        {
            UtcNow = utcNow;
        }

        public DateTime UtcNow { get; }
    }
}
