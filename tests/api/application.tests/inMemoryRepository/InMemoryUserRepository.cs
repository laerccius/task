using api.application.Interfaces;
using api.domain.Entities;

namespace api.application.Tests;

public sealed class UserRepository : IUserRepository
{
    public UserRepository()
    {
    }

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


