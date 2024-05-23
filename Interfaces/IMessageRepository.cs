using Task.Models;

namespace Task.Interfaces;

public interface IMessageRepository
{
    System.Threading.Tasks.Task AddMessageAsync(MessageModel message);
    Task<IEnumerable<MessageModel>> GetMessagesAsync(DateTime start, DateTime end);
}