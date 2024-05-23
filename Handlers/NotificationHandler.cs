using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Task.Models;

namespace Task.Handlers;

public class NotificationHandler
{
    private static readonly List<WebSocket> _sockets = new List<WebSocket>();

    public static async System.Threading.Tasks.Task SendMessageToAllAsync(MessageModel message)
    {
        var messageJson = JsonSerializer.Serialize(message);
        var messageBytes = Encoding.UTF8.GetBytes(messageJson);
        var buffer = new ArraySegment<byte>(messageBytes);

        foreach (var socket in _sockets)
        {
            if (socket.State == WebSocketState.Open)
            {
                await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }

    public static async System.Threading.Tasks.Task HandleWebSocketAsync(HttpContext context)
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            var socket = await context.WebSockets.AcceptWebSocketAsync();
            _sockets.Add(socket);

            while (socket.State == WebSocketState.Open)
            {
                await System.Threading.Tasks.Task.Delay(1000);
            }

            _sockets.Remove(socket);
        }
        else
        {
            context.Response.StatusCode = 400;
        }
    }
    
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MessageService API v1"));

        app.UseRouting();
        app.UseWebSockets();
        app.Use(async (context, next) =>
        {
            if (context.Request.Path == "/ws")
            {
                await NotificationHandler.HandleWebSocketAsync(context);
            }
            else
            {
                await next();
            }
        });

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}