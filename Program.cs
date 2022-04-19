using SignalRChat.Hubs;
using Backend;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();
builder.Services.AddSignalR();
builder.Services.AddSingleton<IDictionary<string, UserConnection>>(opts => new Dictionary<string, UserConnection>());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
if (!app.Environment.IsDevelopment())
{
    // app.UseHttpsRedirection();
    app.UseExceptionHandler("/error");
}
// app.UseAuthorization();
app.UseCors(builder =>
        {
            builder.WithOrigins("http://127.0.0.1:3000") //Source
                .AllowAnyHeader()
                .WithMethods("GET", "POST", "OPTIONS")
                .AllowCredentials();
        });
app.UseRouting();
app.UseForwardedHeaders();
app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatHub>("/chat");
            });
app.MapControllers();

app.Run();
