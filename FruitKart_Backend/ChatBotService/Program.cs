using ChatBotService.Services;
using JWTAuth;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// ─── JWT Authentication ───────────────────────────────────────────────────────
builder.Services.AddJwtAuthentication();

// ─── HTTP Clients ─────────────────────────────────────────────────────────────
builder.Services.AddHttpClient("Groq");     
builder.Services.AddHttpClient("MenuService"); 
builder.Services.AddHttpClient("OrderService"); 

// ─── Chatbot Service ──────────────────────────────────────────────────────────
builder.Services.AddScoped<IChatBotService, GroqChatBotService>();

// ─── Controllers + OpenAPI ───────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// ─── CORS ────────────────────────────────────────────────────────────────────
builder.Services.AddCors(opt =>
    opt.AddPolicy("AllowAll", p =>
        p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddLogging();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();