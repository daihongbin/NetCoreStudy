using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace OAuthSample
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OAuthDefaults.DisplayName;
                })
                .AddCookie()
                .AddOAuth(OAuthDefaults.DisplayName, options =>
                {
                    options.ClientId = "oauth.code";
                    options.ClientSecret = "secret";
                    options.AuthorizationEndpoint = "https://oidc.faasx.com/connect/authorize";
                    options.TokenEndpoint = "https://oidc.faasx.com/connect/token";
                    options.CallbackPath = "/signin-oauth";

                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("email");
                    options.SaveTokens = true;

                    // 事件创建顺序：
                    //1.创建Ticket之前触发
                    options.Events.OnCreatingTicket = context => Task.CompletedTask;

                    //2.创建Ticket时失败触发
                    options.Events.OnRemoteFailure = context => Task.CompletedTask;

                    //3.Ticket接受完成之后触发
                    options.Events.OnTicketReceived = context => Task.CompletedTask;

                    //4.Challenge时触发，默认跳转到OAuth服务器
                    /*options.Events.OnRedirectToAuthorizationEndpoint =
                        context =>
                        {
                            context.Response.Redirect(context.RedirectUri);
                            return Task.CompletedTask;
                        };*/
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseAuthentication();
            
            //检查是否认证
            app.UseAuthorize();

            app.Map("/profile", builder => builder.Run(async context =>
                {
                    await context.Response.WriteHtmlAsync(async res =>
                    {
                        await res.WriteAsync($"<h1>你好，当前登录用户： {HttpResponseExtensions.HtmlEncode(context.User.Identity.Name)}</h1>");
                        await res.WriteAsync("<a class=\"btn btn-default\" href=\"/Account/Logout\">退出</a>");

                        await res.WriteAsync($"<h2>AuthenticationType：{context.User.Identity.AuthenticationType}</h2>");

                        await res.WriteAsync("<h2>Claims:</h2>");
                        await res.WriteTableHeader(new string[] { "Claim Type", "Value" }, context.User.Claims.Select(c => new string[] { c.Type, c.Value }));

                        // 在第一章中介绍过HandleAuthenticateOnceAsync方法，在此调用并不会有多余的性能损耗。
                        var result = await context.AuthenticateAsync();
                        await res.WriteAsync("<h2>Tokens:</h2>");
                        await res.WriteTableHeader(new string[] { "Token Type", "Value" }, result.Properties.GetTokens().Select(token => new string[] { token.Name, token.Value }));
                    });
                }));

            app.Map("/Account/Logout", builder => builder.Run(async context =>
            {
                await context.SignOutAsync();
                context.Response.Redirect("/");
            }));
            
            app.Run(async context =>
            {
                await context.Response.WriteHtmlAsync(async res =>
                {
                    await res.WriteAsync($"<h2>Hello OAuth Authentication</h2>");
                    await res.WriteAsync("<a class=\"btn btn-default\" href=\"/profile\">我的信息</a>");
                });
            });
        }
    }

    public static class MyAppBuilderExtensions
    {
        //模拟授权实现
        public static IApplicationBuilder UseAuthorize(this IApplicationBuilder app)
        {
            return app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/" || context.Request.Path == "/favicon.ico")
                {
                    await next();
                }
                else
                {
                    if (context.User?.Identity?.IsAuthenticated ?? false)
                    {
                        await next();
                    }
                    else
                    {
                        await context.ChallengeAsync();
                    }
                }
            });
        }
    }
}
