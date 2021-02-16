using System;
using Elsa.Activities.Email.Extensions;
using Elsa.Activities.Http.Extensions;
using Elsa.Activities.Timers.Extensions;
using Elsa.Dashboard.Extensions;
using Elsa.Persistence.EntityFrameworkCore.DbContexts;
using Elsa.Persistence.EntityFrameworkCore.Extensions;
using Elsa.Scripting.Liquid.Extensions;
using Elsa.Services;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WorkflowExtension.Activities;
using WorkflowExtension.Elsa;
using WorkflowExtension.Elsa.Liquid;
using WorkflowExtension.Queries;

namespace WebApplication1
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var elsaSection = Configuration.GetSection("Elsa");


            services.AddMediatR(typeof(GetMyCoolProductsQueryHandler));

            services
              // Add workflow services.
              .AddElsa(x => x.AddEntityFrameworkCoreProvider<SqliteContext>(x => x.UseSqlite(Configuration.GetConnectionString("Sqlite")), usePooling: false, autoRunMigrations: true))

              // Add activities we'd like to use.
              .AddHttpActivities(options => options.Bind(elsaSection.GetSection("Http")))
              .AddEmailActivities(options => options.Bind(elsaSection.GetSection("Smtp")))
              .AddTimerActivities(options => options.Bind(elsaSection.GetSection("BackgroundRunner")))
              .AddActivity<EvalProductsDiscountsActivity>()
              .AddActivity<EvalProductsPricesActivity>()
              // Add Dashboard services.
              .AddElsaDashboard();
            
            services.AddSingleton<Func<IWorkflowInvoker>>(provider => () => provider.CreateScope().ServiceProvider.GetService<IWorkflowInvoker>());
            //This workaround is required because the liquid filter uses the default encoding for the resulting JSON, and produces an invalid body for the HTTP request.
            services.AddLiquidFilter<JsonFilter2>("json2");
            //This workaround is required to clean up all executed workflows from the in-memory store, to avoid memory leaks.
            //Anyway, we don't need the long-running workflows for this type of extension.
            services.AddScoped<IWorkflowEventHandler, PersistenceWorkflowEventHandler2>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            //Redirect to sample API method instead of index page
            app.Use(async (context, next) =>
            {
                if (context.Request.Path.Value.Equals("/"))
                {
                    context.Request.Path = "/api/products";
                }

                //app is ready, invoke next component in the pipeline (MVC)
                await next();
            });

            app
                // This is only necessary if we want to be able to run workflows containing HTTP activities from this application. 
                .UseHttpActivities()

                .UseStaticFiles()
                .UseRouting()
                .UseEndpoints(endpoints => endpoints.MapControllers());

        }
    }
}
