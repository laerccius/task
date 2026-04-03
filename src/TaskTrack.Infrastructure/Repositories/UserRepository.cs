using Microsoft.Data.Sqlite;
using TaskTrack.Application.Interfaces;
using TaskTrack.Domain.Entities;
using TaskTrack.Infrastructure.Data;

namespace TaskTrack.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly SqliteConnectionFactory _connectionFactory;

    public UserRepository(SqliteConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        const string sql =
            """
            SELECT Id, Email, FullName, PasswordHash, PasswordSalt, CreatedAtUtc
            FROM Users
            WHERE lower(Email) = lower(@Email);
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.AddWithValue("@Email", email);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return Map(reader);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        const string sql =
            """
            SELECT Id, Email, FullName, PasswordHash, PasswordSalt, CreatedAtUtc
            FROM Users
            WHERE Id = @Id;
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.AddWithValue("@Id", id.ToString());

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return Map(reader);
    }

    public async Task<User> CreateAsync(User user, CancellationToken cancellationToken)
    {
        const string sql =
            """
            INSERT INTO Users (Id, Email, FullName, PasswordHash, PasswordSalt, CreatedAtUtc)
            VALUES (@Id, @Email, @FullName, @PasswordHash, @PasswordSalt, @CreatedAtUtc);
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.AddWithValue("@Id", user.Id.ToString());
        command.Parameters.AddWithValue("@Email", user.Email);
        command.Parameters.AddWithValue("@FullName", user.FullName);
        command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
        command.Parameters.AddWithValue("@PasswordSalt", user.PasswordSalt);
        command.Parameters.AddWithValue("@CreatedAtUtc", user.CreatedAtUtc.ToString("O"));
        await command.ExecuteNonQueryAsync(cancellationToken);
        return user;
    }

    private static User Map(SqliteDataReader reader) =>
        new()
        {
            Id = Guid.Parse(reader.GetString(0)),
            Email = reader.GetString(1),
            FullName = reader.GetString(2),
            PasswordHash = reader.GetString(3),
            PasswordSalt = reader.GetString(4),
            CreatedAtUtc = DateTime.Parse(reader.GetString(5)).ToUniversalTime()
        };
}
