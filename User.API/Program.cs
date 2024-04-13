using Common.Extensions;
using User.BL.Extensions;
using User.BL.Models;

var builder = WebApplication.CreateBuilder(args);
TokenProperties tokenProperties = new();
builder.Configuration.GetSection(nameof(TokenProperties)).Bind(tokenProperties);

builder.Services.AddAuthenticationCustom(tokenProperties);

builder.Services.AddUserDB(builder.Configuration);
builder.Services.ConfigureIdentityDependencies();
builder.Services.ConfigureUserServices();
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
