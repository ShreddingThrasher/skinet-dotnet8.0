using API.Middleware;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddDbContext<StoreContext>(opt => 
{
	opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

builder.Services.AddCors();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	
}


// Configure HTTP pipeline

app.UseMiddleware<ExceptionMiddleware>();

app.UseCors(config => 
{
	config.AllowAnyHeader().AllowAnyMethod()
		.WithOrigins("http://localhost:4200", "https://localhost:4200");
});

app.MapControllers();


try
{
	using var scope = app.Services.CreateScope();
	var services = scope.ServiceProvider;
	var context = services.GetRequiredService<StoreContext>();
	
	await context.Database.MigrateAsync();
	await StoreContextSeed.SeedAsync(context);
}
catch (Exception e)
{
	Console.WriteLine(e);
	throw;
}

app.Run();
