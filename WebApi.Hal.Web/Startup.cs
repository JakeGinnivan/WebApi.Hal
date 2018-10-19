using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using WebApi.Hal.Web.Data;

namespace WebApi.Hal.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment hostingEnvironment)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(hostingEnvironment.ContentRootPath)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{hostingEnvironment.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables().Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvcCore()
                .AddJsonFormatters(
                    options =>
                    {
                        options.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                        options.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    })
                .AddApiExplorer()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.TryAddEnumerable(
                ServiceDescriptor.Transient<IConfigureOptions<MvcOptions>, FormattersMvcOptionsSetup>());

            services.AddDbContext<BeerDbContext>((oa) => oa.UseSqlite(Configuration.GetConnectionString("BeersDb")));
            services.AddScoped<IBeerDbContext, BeerDbContext>();
            services.AddScoped<IRepository, BeerRepository>();

            services.AddSwaggerGen(
                options =>
                {
                    options.DescribeAllEnumsAsStrings();
                    options.SwaggerDoc("v1", new Info {Title = "WebApi.Hal Demo API", Version = "v1"});
                    options.CustomSchemaIds(type => type.FullName);
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, BeerDbContext beerDbContext)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            beerDbContext.Database.Migrate();

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApi.Hal Demo API V1"); });

            app.UseMvc();
        }
    }
}
