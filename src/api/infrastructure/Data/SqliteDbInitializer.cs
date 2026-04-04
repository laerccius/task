using Microsoft.Data.Sqlite;
using api.application.Interfaces;
using api.domain.Entities;
using api.domain.Enums;

namespace api.infrastructure.Data;

public class SqliteDbInitializer
{
    private readonly SqliteConnectionFactory _connectionFactory;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IDateTimeProvider _dateTimeProvider;

    public SqliteDbInitializer(
        SqliteConnectionFactory connectionFactory,
        IPasswordHasher passwordHasher,
        IDateTimeProvider dateTimeProvider)
    {
        _connectionFactory = connectionFactory;
        _passwordHasher = passwordHasher;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        var createUsers = connection.CreateCommand();
        createUsers.CommandText =
            """
            CREATE TABLE IF NOT EXISTS Users (
                Id TEXT PRIMARY KEY,
                Email TEXT NOT NULL UNIQUE,
                FullName TEXT NOT NULL,
                PasswordHash TEXT NOT NULL,
                PasswordSalt TEXT NOT NULL,
                CreatedAtUtc TEXT NOT NULL
            );
            """;
        await createUsers.ExecuteNonQueryAsync(cancellationToken);

        var createTasks = connection.CreateCommand();
        createTasks.CommandText =
            """
            CREATE TABLE IF NOT EXISTS Tasks (
                Id TEXT PRIMARY KEY,
                UserId TEXT NOT NULL,
                Title TEXT NOT NULL,
                Description TEXT NOT NULL,
                Status INTEGER NOT NULL,
                DueDate TEXT NOT NULL,
                CreatedAtUtc TEXT NOT NULL,
                UpdatedAtUtc TEXT NOT NULL,
                FOREIGN KEY(UserId) REFERENCES Users(Id)
            );
            """;
        await createTasks.ExecuteNonQueryAsync(cancellationToken);

        var countCommand = connection.CreateCommand();
        countCommand.CommandText = "SELECT COUNT(1) FROM Users;";
        var count = Convert.ToInt32(await countCommand.ExecuteScalarAsync(cancellationToken));
        if (count > 0)
        {
            return;
        }

        var demoUser = CreateDemoUser();
        var insertUser = connection.CreateCommand();
        insertUser.CommandText =
            """
            INSERT INTO Users (Id, Email, FullName, PasswordHash, PasswordSalt, CreatedAtUtc)
            VALUES (@Id, @Email, @FullName, @PasswordHash, @PasswordSalt, @CreatedAtUtc);
            """;
        insertUser.Parameters.AddWithValue("@Id", demoUser.Id.ToString());
        insertUser.Parameters.AddWithValue("@Email", demoUser.Email);
        insertUser.Parameters.AddWithValue("@FullName", demoUser.FullName);
        insertUser.Parameters.AddWithValue("@PasswordHash", demoUser.PasswordHash);
        insertUser.Parameters.AddWithValue("@PasswordSalt", demoUser.PasswordSalt);
        insertUser.Parameters.AddWithValue("@CreatedAtUtc", demoUser.CreatedAtUtc.ToString("O"));
        await insertUser.ExecuteNonQueryAsync(cancellationToken);

        foreach (var task in CreateSeedTasks(demoUser.Id))
        {
            var insertTask = connection.CreateCommand();
            insertTask.CommandText =
                """
                INSERT INTO Tasks (Id, UserId, Title, Description, Status, DueDate, CreatedAtUtc, UpdatedAtUtc)
                VALUES (@Id, @UserId, @Title, @Description, @Status, @DueDate, @CreatedAtUtc, @UpdatedAtUtc);
                """;
            insertTask.Parameters.AddWithValue("@Id", task.Id.ToString());
            insertTask.Parameters.AddWithValue("@UserId", task.UserId.ToString());
            insertTask.Parameters.AddWithValue("@Title", task.Title);
            insertTask.Parameters.AddWithValue("@Description", task.Description);
            insertTask.Parameters.AddWithValue("@Status", (int)task.Status);
            insertTask.Parameters.AddWithValue("@DueDate", task.DueDate.ToString("O"));
            insertTask.Parameters.AddWithValue("@CreatedAtUtc", task.CreatedAtUtc.ToString("O"));
            insertTask.Parameters.AddWithValue("@UpdatedAtUtc", task.UpdatedAtUtc.ToString("O"));
            await insertTask.ExecuteNonQueryAsync(cancellationToken);
        }
    }

    private User CreateDemoUser()
    {
        var (hash, salt) = _passwordHasher.HashPassword("Demo123!");
        return new User
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Email = "demo@tasktrack.local",
            FullName = "Demo User",
            PasswordHash = hash,
            PasswordSalt = salt,
            CreatedAtUtc = _dateTimeProvider.UtcNow
        };
    }

    private IEnumerable<TaskItem> CreateSeedTasks(Guid userId)
    {
        var now = _dateTimeProvider.UtcNow;
        return
        [
            new TaskItem
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222221"),
                UserId = userId,
                Title = "Prepare interview presentation",
                Description = "Summarize architecture, testing, and demo flow.",
                Status = TaskItemStatus.InProgress,
                DueDate = now.AddDays(2),
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            },
            new TaskItem
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                UserId = userId,
                Title = "Review API validations",
                Description = "Double-check edge cases around login and ownership.",
                Status = TaskItemStatus.Pending,
                DueDate = now.AddDays(3),
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            }
        ];
    }
}
