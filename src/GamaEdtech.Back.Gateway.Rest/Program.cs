using GamaEdtech.Back.DataSource.Schools;
using GamaEdtech.Back.DataSource.Utils;
using GamaEdtech.Back.Domain.Schools;

var builder = WebApplication.CreateBuilder(args);

var conectionString =
	new ConnectionString(builder.Configuration.GetConnectionString("Default")!);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSingleton(conectionString);
builder.Services.AddScoped<GamaEdtechDbContext>();
builder.Services.AddTransient<ISchoolRepository, SqlServerSchoolRepository>();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

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
