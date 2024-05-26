using Common.Extensions;
using Common.Middlewares;
using Dictionary.BL.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDictionaryDB(builder.Configuration);
builder.Services.ConfigureDictionaryServices();
builder.Services.AddAuthenticationCustom(builder.Configuration);
builder.Services.AddEasyNetQ();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerAuth();

var app = builder.Build();

app.UseMiddleware<ErrorsHandling>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

await app.Migrate();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
