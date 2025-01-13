using FluentValidation;
using FluentValidation.AspNetCore;
using GamaEdtech.Back.DataSource.Cities;
using GamaEdtech.Back.DataSource.Contries;
using GamaEdtech.Back.DataSource.Schools;
using GamaEdtech.Back.DataSource.States;
using GamaEdtech.Back.DataSource.Utils;
using GamaEdtech.Back.Domain.Cities;
using GamaEdtech.Back.Domain.Countries;
using GamaEdtech.Back.Domain.Schools;
using GamaEdtech.Back.Domain.States;
using GamaEdtech.Back.Gateway.Rest.Utils;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

ConnectionString connectionString;

if(builder.Environment.IsDevelopment())
{
	connectionString = new ConnectionString(builder.Configuration.GetConnectionString("Default")!);
}
else
{
	connectionString = new ConnectionString(builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING")!);
}

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
builder.Services.AddTransient<ICountryRepository, SqlServerCountryRepository>();
builder.Services.AddTransient<IStateRepository, SqlServerStateRepository>();
builder.Services.AddTransient<ICityRepository, SqlServerCityRepository>();
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
