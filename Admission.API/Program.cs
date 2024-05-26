using Admission.BL.Extensions;
using Common.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAdmissionDB(builder.Configuration);
builder.Services.ConfigureAdmissionServices();
builder.Services.AddAuthenticationCustom(builder.Configuration);
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

await app.Migrate();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
