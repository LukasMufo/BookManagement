using BookManagement.Binders;
using BookManagement.Controllers;
using BookManagement.DbContexts;
using BookManagement.Models;
using BookManagement.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

//enable Swagger to read the XML comments and annotations
builder.Services.AddSwaggerGen(a =>
{
    a.SwaggerDoc("v1", new OpenApiInfo { Title = "BookLibrary API", Version = "v1" });
    // Specify XML comments file
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    a.IncludeXmlComments(xmlPath);
});

// Configure the database context with SQLite
builder.Services.AddDbContext<BookLibraryContext>(options =>
{
    var configuration = builder.Configuration.GetSection("ConnectionStrings");
    var connectionString = configuration["DefaultConnection"];
    options.UseSqlite(connectionString);
});

//Inject dependency for service that checks for borrowed books that are to be returned
builder.Services.AddScoped<IEmailService, SmtpEmailService>();
//Add custom service to check for borrowed books that are to be returned
builder.Services.AddHostedService<ReminderService>();
/// use custom model binder for DateOnly as ASP.NET don't support binding for DateOnly
builder.Services.AddControllers(options => { options.ModelBinderProviders.Insert(0, new DateOnlyModelBinderProvider()); });


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    //enable Swagger middleware and swagger UI
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllerRoute(
    name: "default",
    pattern: "default",
    defaults: new { controller = "Home", action = "Index" }); // Default controller and action

/*app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "default",
        defaults: new { controller = "Home", action = "Index" }); // Default controller and action
});*/



app.Run();

