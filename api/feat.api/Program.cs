using Microsoft.EntityFrameworkCore;
using feat.api.Data;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("IngestionConnection");
builder.Services.AddDbContext<IngestionDbContext>(options =>
    options.UseSqlServer(connectionString, x => x.UseNetTopologySuite()));

builder.Services.AddOpenApi();

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8080);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    
    ApplyNewDatabaseMigrations(app);
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
return;

void ApplyNewDatabaseMigrations(WebApplication webApp)
{
    using (var scope = webApp.Services.CreateScope())
    {
        var ingestionDbContext = scope.ServiceProvider.GetRequiredService<IngestionDbContext>();

        var pendingMigrations = ingestionDbContext.Database.GetPendingMigrations();
        if (pendingMigrations.Any())
        {
            ingestionDbContext.Database.Migrate();            
        }
    }
}