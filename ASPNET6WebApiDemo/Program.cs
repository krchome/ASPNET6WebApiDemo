using ASPNET6WebApiDemo.Contexts;
var builder = WebApplication.CreateBuilder(args);

//Add connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ;

//Add a context to the services collection
builder.Services.AddSqlServer<ApplicationDbContext>(connectionString);

// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
