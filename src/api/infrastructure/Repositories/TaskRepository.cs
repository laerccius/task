using Microsoft.Data.Sqlite;
using api.application.Interfaces;
using api.domain.Entities;
using api.domain.Enums;
using api.infrastructure.Data;

namespace api.infrastructure.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly SqliteConnectionFactory _connectionFactory;

    public TaskRepository(SqliteConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<TaskItem>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        const string sql =
            """
            SELECT Id, UserId, Title, Description, Status, DueDate, CreatedAtUtc, UpdatedAtUtc
            FROM Tasks
            WHERE UserId = @UserId
            ORDER BY DueDate ASC;
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.AddWithValue("@UserId", userId.ToString());

        var results = new List<TaskItem>();
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            results.Add(Map(reader));
        }

        return results;
    }

    public async Task<TaskItem?> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        const string sql =
            """
            SELECT Id, UserId, Title, Description, Status, DueDate, CreatedAtUtc, UpdatedAtUtc
            FROM Tasks
            WHERE Id = @Id AND UserId = @UserId;
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.AddWithValue("@Id", id.ToString());
        command.Parameters.AddWithValue("@UserId", userId.ToString());

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return Map(reader);
    }

    public async Task<TaskItem> CreateAsync(TaskItem taskItem, CancellationToken cancellationToken)
    {
        const string sql =
            """
            INSERT INTO Tasks (Id, UserId, Title, Description, Status, DueDate, CreatedAtUtc, UpdatedAtUtc)
            VALUES (@Id, @UserId, @Title, @Description, @Status, @DueDate, @CreatedAtUtc, @UpdatedAtUtc);
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        var command = connection.CreateCommand();
        command.CommandText = sql;
        AddParameters(command, taskItem);
        await command.ExecuteNonQueryAsync(cancellationToken);
        return taskItem;
    }

    public async Task<bool> UpdateAsync(TaskItem taskItem, CancellationToken cancellationToken)
    {
        const string sql =
            """
            UPDATE Tasks
            SET Title = @Title,
                Description = @Description,
                Status = @Status,
                DueDate = @DueDate,
                UpdatedAtUtc = @UpdatedAtUtc
            WHERE Id = @Id AND UserId = @UserId;
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        var command = connection.CreateCommand();
        command.CommandText = sql;
        AddParameters(command, taskItem);
        var rows = await command.ExecuteNonQueryAsync(cancellationToken);
        return rows > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken)
    {
        const string sql = "DELETE FROM Tasks WHERE Id = @Id AND UserId = @UserId;";

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);

        var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.AddWithValue("@Id", id.ToString());
        command.Parameters.AddWithValue("@UserId", userId.ToString());

        var rows = await command.ExecuteNonQueryAsync(cancellationToken);
        return rows > 0;
    }

    private static void AddParameters(SqliteCommand command, TaskItem taskItem)
    {
        command.Parameters.AddWithValue("@Id", taskItem.Id.ToString());
        command.Parameters.AddWithValue("@UserId", taskItem.UserId.ToString());
        command.Parameters.AddWithValue("@Title", taskItem.Title);
        command.Parameters.AddWithValue("@Description", taskItem.Description);
        command.Parameters.AddWithValue("@Status", (int)taskItem.Status);
        command.Parameters.AddWithValue("@DueDate", taskItem.DueDate.ToString("O"));
        command.Parameters.AddWithValue("@CreatedAtUtc", taskItem.CreatedAtUtc.ToString("O"));
        command.Parameters.AddWithValue("@UpdatedAtUtc", taskItem.UpdatedAtUtc.ToString("O"));
    }

    private static TaskItem Map(SqliteDataReader reader) =>
        new()
        {
            Id = Guid.Parse(reader.GetString(0)),
            UserId = Guid.Parse(reader.GetString(1)),
            Title = reader.GetString(2),
            Description = reader.GetString(3),
            Status = (TaskItemStatus)reader.GetInt32(4),
            DueDate = DateTime.Parse(reader.GetString(5)).ToUniversalTime(),
            CreatedAtUtc = DateTime.Parse(reader.GetString(6)).ToUniversalTime(),
            UpdatedAtUtc = DateTime.Parse(reader.GetString(7)).ToUniversalTime()
        };
}
