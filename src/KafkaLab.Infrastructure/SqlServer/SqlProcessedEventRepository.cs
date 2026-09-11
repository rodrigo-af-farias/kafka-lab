using KafkaLab.Application.Interfaces;
using Microsoft.Data.SqlClient;

namespace KafkaLab.Infrastructure.SqlServer;

public sealed class SqlProcessedEventRepository : IProcessedEventRepository
{
    private readonly string _connectionString;

    public SqlProcessedEventRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<bool> TryMarkAsProcessedAsync(
        Guid eventId,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
                           INSERT INTO ProcessedEvents (EventId, ProcessedAt)
                           VALUES (@EventId, SYSUTCDATETIME());
                           """;

        try
        {
            await using var connection = new SqlConnection(_connectionString);

            await connection.OpenAsync(cancellationToken);

            await using var command = new SqlCommand(sql, connection);

            command.Parameters.AddWithValue("@EventId", eventId);

            await command.ExecuteNonQueryAsync(cancellationToken);

            return true;
        }
        catch (SqlException ex) when (ex.Number is 2601 or 2627)
        {
            return false;
        }
    }
}