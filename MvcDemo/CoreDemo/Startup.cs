using CoreDemo.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CoreDemo
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //注册service
            services.AddSingleton<ICinemaService, CinemaMemoryService>();
            services.AddSingleton<IMovieService, MovieMemoryService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //之前为run不会使用下一个中间件，使用use则可以
            app.Use(async (context,next) =>
            {
                logger.LogInformation("M1 Start");
                await context.Response.WriteAsync("Hello World!");
                await next();
                logger.LogInformation("M1 End");
            });

            app.Run(async (context) =>
            {
                logger.LogInformation("M2 Start");
                await context.Response.WriteAsync("Another Hello!");
                logger.LogInformation("M2 End");
            });
        }
    }
}
