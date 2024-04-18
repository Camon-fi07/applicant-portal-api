using Applicant.BL.Extensions;
using Common.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApplicantDB(builder.Configuration);
builder.Services.ConfigureApplicantServices();

builder.Services.AddSwaggerAuth();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
