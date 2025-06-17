using ChatAgent.Api.Services.OpenAI;
using ChatAgent.Api.Services.Privy;
using ChatAgent.Api.Services.Solana;

var builder = WebApplication.CreateBuilder(args);

// CORS for react
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
        policy.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
    );
});

builder.Services.AddControllers();
builder.Services.AddSingleton<IPrivyService, PrivyService>();
builder.Services.AddHttpClient<IOpenAiService, OpenAiService>(); 

builder.Services
    .AddHttpClient<ISolanaAgentService, SolanaAgentService>(client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["SolanaAgent:BaseUrl"]!);
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "ChatAgent API", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ChatAgent API V1");
        c.RoutePrefix = string.Empty;  // Swagger UI http://localhost:5043/
    });
}

app.UseCors("AllowReactApp");

app.MapControllers();

app.Run();