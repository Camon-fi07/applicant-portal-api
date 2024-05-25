using Common.Extensions;
using Dictionary.BL.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDictionaryDB(builder.Configuration);
builder.Services.ConfigureDictionaryServices();
builder.Services.AddEasyNetQ();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerAuth();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Migrate();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
