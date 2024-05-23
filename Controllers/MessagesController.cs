using Microsoft.AspNetCore.Mvc;
using Task.Handlers;
using Task.Interfaces;
using Task.Models;

namespace Task.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    private readonly IMessageRepository _repository;

    public MessagesController(IMessageRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Отправляет сообщение.
    /// </summary>
    /// <param name="model">Модель сообщения.</param>
    /// <returns>Результат выполнения операции.</returns>
    /// <response code="200">Сообщение успешно принято.</response>
    /// <response code="400">Ошибка валидации или другие ошибки.</response>
    [HttpPost]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> PostMessage([FromBody] MessageDto messageDto)
    {
        var message = new MessageModel
        {
            Text = messageDto.Text,
            Timestamp = DateTime.UtcNow,
            SequenceNumber = messageDto.SequenceNumber
        };
        
        await _repository.AddMessageAsync(message);
        await NotificationHandler.SendMessageToAllAsync(message);
        
        return Ok();
    }
    
    /// <summary>
    /// Получает список сообщений за указанный диапазон дат.
    /// </summary>
    /// <param name="startDate">Начальная дата диапазона.</param>
    /// <param name="endDate">Конечная дата диапазона.</param>
    /// <returns>Список сообщений.</returns>
    [HttpGet]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IEnumerable<MessageModel>> GetMessages([FromQuery] DateTime start, [FromQuery] DateTime end)
    {
        return await _repository.GetMessagesAsync(start, end);
    }
}

public class MessageDto
{
    public string Text { get; set; }
    public int SequenceNumber { get; set; }
}