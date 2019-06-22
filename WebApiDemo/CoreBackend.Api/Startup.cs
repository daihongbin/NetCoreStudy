using CoreBackend.Api.Dtos;
using CoreBackend.Api.Entities;
using CoreBackend.Api.Repositories;
using CoreBackend.Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace CoreBackend.Api
{
    public class Startup
    {
        public static IConfiguration Configuration { get; private set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()//注册mvc到Container
                             //添加返回xml的设置
                .AddMvcOptions(options =>
                {
                    options.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
                });

            //.AddJsonOptions(options => //去除json首字母小写的设置，推荐默认小写形式
            //{
            //    if (options.SerializerSettings.ContractResolver is DefaultContractResolver resolver)
            //    {
            //        resolver.NamingStrategy = null;
            //    }
            //});

            //services.AddTransient<LocalMailService>();


#if DEBUG
            services.AddTransient<IMailService, LocalMailService>();
#else
            services.AddTransient<IMailService,CloudMailService>();
#endif

            //注册DbContext，使用OnConfiguring
            //services.AddDbContext<MyContext>(); //默认生命范围Scoped

            var connectionString = @"Server=(localdb)\MSSQLLocalDB;Database=ProductDB;Trusted_Connection=True";
            //var connectionString = Configuration["connectionStrings:productionInfoDbConnectionString"];
            services.AddDbContext<MyContext>(o => o.UseSqlServer(connectionString));

            //注册Repository,Repository，最好的生命周期是Scoped（每个请求生成一个实例）
            services.AddScoped<IProductRepository,ProductRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,MyContext myContext)
        {
            //loggerFactory.AddProvider(new NLogLoggerProvider());
            loggerFactory.AddNLog();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler();
            }

            myContext.EnsureSeedDataForContext();

            app.UseStatusCodePages();

            //创建AutoMapper的映射关系
            AutoMapper.Mapper.Initialize(cfg => 
            {
                cfg.CreateMap<Product, ProductWithoutMaterialDto>();
                cfg.CreateMap<Product, ProductDto>();
                cfg.CreateMap<ProductCreation,Product>();
                cfg.CreateMap<ProductModification, Product>();
                cfg.CreateMap<Product, ProductModification>();
                cfg.CreateMap<Material, MaterialDto>();
            });

            app.UseMvc();//使用mvc

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
