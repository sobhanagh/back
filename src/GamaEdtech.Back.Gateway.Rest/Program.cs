using FluentValidation;
using FluentValidation.AspNetCore;
using GamaEdtech.Back.DataSource.Schools;
using GamaEdtech.Back.DataSource.Utils;
using GamaEdtech.Back.Domain.Schools;
using GamaEdtech.Back.Gateway.Rest.Utils;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

var conectionString =
	new ConnectionString(builder.Configuration.GetConnectionString("Default")!);

// Add services to the container.
builder.Services
	.AddControllers()
	.ConfigureApiBehaviorOptions(options =>
		options.InvalidModelStateResponseFactory = ModelStateValidator.Validate); ;

builder.Services
	.AddFluentValidationAutoValidation()
	.AddFluentValidationClientsideAdapters()
	.AddValidatorsFromAssemblyContaining<Program>(); ;

builder.Services.AddSingleton(conectionString);
builder.Services.AddScoped<GamaEdtechDbContext>();
builder.Services.AddTransient<ISchoolRepository, SqlServerSchoolRepository>();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(options =>
{
	var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
	options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
