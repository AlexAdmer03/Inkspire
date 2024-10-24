var builder = WebApplication.CreateBuilder(args);

// VIKTIGT - Lägg till denna rad för att registrera HttpClientFactory
builder.Services.AddHttpClient();

// Lägg till controllers
builder.Services.AddControllers();

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorApp", policy =>
        policy.WithOrigins("https://localhost:7227")
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// Swagger configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowBlazorApp");
app.UseAuthorization();
app.MapControllers();

// Debug-utskrift för att verifiera API-nyckel
var apiKey = app.Configuration["HuggingFace:ApiKey"] ??
             app.Configuration["HuggingFace:apiKey"];
Console.WriteLine($"API Key configured: {!string.IsNullOrEmpty(apiKey)}");

app.Run();