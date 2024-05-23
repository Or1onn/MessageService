using Microsoft.OpenApi.Models;
using Task.Handlers;
using Task.Interfaces;
using Task.Middleware;
using Task.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddControllers();
builder.Services.AddSingleton<IMessageRepository, MessageRepositoryModel>();


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "MessageService API", 
        Version = "v1",
        Description = "Тестовое задание для Кошелек.Ру",
    });
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "MessageService API v1");
    c.RoutePrefix = "Swagger";
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseMiddleware<LoggingMiddleware>();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Clients}/{action=FirstClient}/{id?}");

app.Run();
