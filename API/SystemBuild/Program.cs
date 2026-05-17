using API.SystemBuild;
using Application;
using Persistence.Data;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiRegistrationServices(builder.Configuration);


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi



var app = builder.Build();

app.UseApplicationPipeline();

using(var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DataContext>();
    await DbSeeder.SeedAsync(db);
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
