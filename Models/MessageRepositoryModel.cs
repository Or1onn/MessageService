using Npgsql;
using Task.Interfaces;

namespace Task.Models;

public class MessageRepositoryModel : IMessageRepository
{
    private readonly string _connectionString;

    public MessageRepositoryModel(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async System.Threading.Tasks.Task AddMessageAsync(MessageModel message)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        
        var command =
            new NpgsqlCommand(
                "INSERT INTO Messages (Text, Timestamp, SequenceNumber) VALUES (@text, @timestamp, @sequencenumber)",
                connection);
        command.Parameters.AddWithValue("text", message.Text);
        command.Parameters.AddWithValue("timestamp", message.Timestamp);
        command.Parameters.AddWithValue("sequencenumber", message.SequenceNumber);
        await command.ExecuteNonQueryAsync();
    }
    
    public async Task<IEnumerable<MessageModel>> GetMessagesAsync(DateTime start, DateTime end)
    {
        var messages = new List<MessageModel>();
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        var command = new NpgsqlCommand("SELECT * FROM Messages WHERE Timestamp BETWEEN @start AND @end", connection);
        command.Parameters.AddWithValue("start", start);
        command.Parameters.AddWithValue("end", end);
        var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            messages.Add(new MessageModel
            {
                Id = reader.GetInt32(0),
                Text = reader.GetString(1),
                Timestamp = reader.GetDateTime(2),
                SequenceNumber = reader.GetInt32(3)
            });
        }
        return messages;
    }
}