using Common.Extensions;
using User.BL.Extensions;
using User.BL.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddUserDB(builder.Configuration);
builder.Services.ConfigureIdentityDependencies();
builder.Services.AddAuthenticationCustom(builder.Configuration);
builder.Services.ConfigureUserServices();
builder.Services.AddEasyNetQ();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerAuth();

var app = builder.Build();

await app.ConfigureRoles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
