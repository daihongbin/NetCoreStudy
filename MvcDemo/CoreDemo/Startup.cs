using Autofac;
using Autofac.Extensions.DependencyInjection;
using CoreDemo.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace CoreDemo
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public IContainer ApplicationContainer { get; private set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            //注册mvc服务
            services.AddMvc();

            //注册service
            //services.AddSingleton<ICinemaService, CinemaMemoryService>();
            //services.AddSingleton<IMovieService, MovieMemoryService>();

            var builder = new ContainerBuilder();
            builder.Populate(services);

            builder.RegisterType<DataProvider>().As<IDataProvider>();
            builder.RegisterType<CinemaMemoryService>().As<ICinemaService>();
            builder.RegisterType<MovieMemoryService>().As<IMovieService>();

            this.ApplicationContainer = builder.Build();

            return new AutofacServiceProvider(this.ApplicationContainer);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,ILogger<Startup> logger)
        {
            //中间件顺序较为重要
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //返回错误码页面
            app.UseStatusCodePages();

            //使用自定义错误页
            //app.UseStatusCodePagesWithRedirects();

            //配置使用js和css文件
            app.UseStaticFiles();

            //配置使用mvc
            app.UseMvc(routes => 
            {
                routes.MapRoute
                (
                    name:"default",
                    template:"{controller=Home}/{action=Index}/{id?}"
                );
            });

            //之前为run不会使用下一个中间件，使用use则可以
            //app.Use(async (context,next) =>
            //{
            //    logger.LogInformation("M1 Start");
            //    await context.Response.WriteAsync("Hello World!");
            //    await next();
            //    logger.LogInformation("M1 End");
            //});

            //app.Run(async (context) =>
            //{
            //    logger.LogInformation("M2 Start");
            //    await context.Response.WriteAsync("Another Hello!");
            //    logger.LogInformation("M2 End");
            //});
        }
    }
}
