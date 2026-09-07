using AiChatApi.Data;
using AiChatApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Add Entity Framework Core with MSSQL
builder.Services.AddDbContext<TeachWallDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register the AI service with a typed HttpClient
builder.Services.AddHttpClient<IAiService, GroqAiService>();

// Register TeachWall services
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IAnswerService, AnswerService>();
builder.Services.AddScoped<ITeachBackService, TeachBackService>();
builder.Services.AddScoped<IUpvoteService, UpvoteService>();

var app = builder.Build();

// Apply migrations automatically
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TeachWallDbContext>();
    dbContext.Database.Migrate();
}

// Enable Swagger in all environments
app.UseCors("AllowAll");
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();