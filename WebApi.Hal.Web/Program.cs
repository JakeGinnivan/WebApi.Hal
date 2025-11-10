using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using WebApi.Hal.Web;
using WebApi.Hal.Web.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<MvcOptions>, FormattersMvcOptionsSetup>());

builder.Services.AddDbContext<BeerDbContext>((oa) => oa.UseSqlite(builder.Configuration.GetConnectionString("BeersDb")));

builder.Services.AddScoped<IBeerDbContext, BeerDbContext>();
builder.Services.AddScoped<IRepository, BeerRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
