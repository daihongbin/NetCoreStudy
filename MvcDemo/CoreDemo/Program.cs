using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace CoreDemo
{
    public class Program
    {
        //安装BuildBundlerMinifier包进行bundleconfig配置文件使用
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
