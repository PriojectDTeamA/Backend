using SignalRChat.Hubs;
using Backend;
// using Microsoft.AspNetCore.Http.Json;
using Newtonsoft.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//JSON Serializer
// builder.Services.AddControllersWithViews().AddNewtonsoftJson(options =>
// options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore)
//     .AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver
//     = new DefaultContractResolver());

// // Set the JSON serializer options
// builder.Services.Configure<JsonOptions>(options =>
// {
//     options.SerializerOptions.PropertyNameCaseInsensitive = false;
//     options.SerializerOptions.PropertyNamingPolicy = null;
//     options.SerializerOptions.WriteIndented = true;
// });
builder.Services.AddControllersWithViews().AddNewtonsoftJson(options =>
options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore)
    .AddNewtonsoftJson(options => options.SerializerSettings.ContractResolver
    = new DefaultContractResolver());
// builder.Services.AddControllers()
//     .AddNewtonsoftJson();

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
            builder.WithOrigins("http://127.0.0.1:8060") //Source
                .AllowAnyHeader()
                .WithMethods("GET", "POST", "OPTIONS")
                .AllowCredentials();
            builder.WithOrigins("http://codojo.made-by-s.id:8060") //Source
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
