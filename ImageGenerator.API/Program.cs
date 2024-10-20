var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorApp", policy =>
        policy.WithOrigins("https://localhost:7227/") // Your Blazor app's URL
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// Add HTTP client for Together AI
builder.Services.AddHttpClient("TogetherAI", client =>
{
    client.BaseAddress = new Uri("https://api.together.xyz/");
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {builder.Configuration["TogetherAI:ApiKey"]}");
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Use CORS before Authorization
app.UseCors("AllowBlazorApp");

app.UseAuthorization();

app.MapControllers();

app.Run();