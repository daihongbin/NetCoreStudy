using CookieSample.Data;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CookieSample
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //配置Cookie验证
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options => 
            {
                // 在这里可以根据需要添加一些Cookie认证相关的配置，在本次示例中使用默认值就可以了。
                options.SessionStore = new MemoryCacheTicketStore();

                //Cookie有效期，用户更改信息后重新获取Token
                //options.Events = new CookieAuthenticationEvents
                //{
                //    OnValidatePrincipal = LastChangedValidator.ValidateAsync
                //};
            });

            services.AddSingleton<UserStore>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseAuthentication();

            //登录（允许匿名访问，故放在UseAuthorize前面）
            app.Map("/Account/Login",builder => builder.Run(async context => 
            {
                if (context.Request.Method == "GET")
                {
                    await context.Response.WriteHtmlAsync(async res => 
                    {
                        await res.WriteAsync($"<form method=\"post\">");
                        await res.WriteAsync($"<input type=\"hidden\" name=\"returnUrl\" value=\"{HttpResponseExtensions.HtmlEncode(context.Request.Query["ReturnUrl"])}\"/>");
                        await res.WriteAsync($"<div class=\"form-group\"><label>用户名：<input type=\"text\" name=\"userName\" class=\"form-control\"></label></div>");
                        await res.WriteAsync($"<div class=\"form-group\"><label>密码：<input type=\"password\" name=\"password\" class=\"form-control\"></label></div>");
                        await res.WriteAsync($"<button type=\"submit\" class=\"btn btn-default\">登录</button>");
                        await res.WriteAsync($"</form>");
                    });
                }
                else
                {
                    var userStore = context.RequestServices.GetService<UserStore>();
                    var user = userStore.FindUser(context.Request.Form["userName"],context.Request.Form["password"]);
                    if (user == null)
                    {
                        await context.Response.WriteHtmlAsync(async res => 
                        {
                            await res.WriteAsync($"<h1>用户名或密码错误。</h1>");
                            await res.WriteAsync("<a class=\"btn btn-default\" href=\"/Account/Login\">返回</a>");
                        });
                    }
                    else
                    {
                        //1.0版本
                        //var claimIdentity = new ClaimsIdentity("Cookie");
                        //claimIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()));
                        //claimIdentity.AddClaim(new Claim(ClaimTypes.Name,user.Name));
                        //claimIdentity.AddClaim(new Claim(ClaimTypes.Email,user.Email));
                        //claimIdentity.AddClaim(new Claim(ClaimTypes.MobilePhone,user.PhoneNumber));
                        //claimIdentity.AddClaim(new Claim(ClaimTypes.DateOfBirth,user.Birthday.ToString()));

                        //2.0版本
                        var claimIdentity = new ClaimsIdentity("Cookie",JwtClaimTypes.Name,JwtClaimTypes.Role);
                        claimIdentity.AddClaim(new Claim(JwtClaimTypes.Id,user.Id.ToString()));
                        claimIdentity.AddClaim(new Claim(JwtClaimTypes.Name,user.Name));
                        claimIdentity.AddClaim(new Claim(JwtClaimTypes.Email,user.Email));
                        claimIdentity.AddClaim(new Claim(JwtClaimTypes.PhoneNumber,user.PhoneNumber));
                        claimIdentity.AddClaim(new Claim(JwtClaimTypes.BirthDate,user.Birthday.ToString()));

                        var claimsPrincipal = new ClaimsPrincipal(claimIdentity);

                        // 在上面注册AddAuthentication时
                        await context.SignInAsync(claimsPrincipal);

                        // 指定Cookie是否持久以及过期时间
                        //await context.SignInAsync("MyCookieAuthenticationScheme",claimsPrincipal,new AuthenticationProperties
                        //{
                        //    //持久保存
                        //    IsPersistent = true,

                        //    //指定过期时间
                        //    ExpiresUtc = DateTime.UtcNow.AddMinutes(20)
                        //});

                        if (string.IsNullOrEmpty(context.Request.Form["ReturnUrl"]))
                        {
                            context.Response.Redirect("/");
                        }
                        else
                        {
                            context.Response.Redirect(context.Request.Form["ReturnUrl"]);
                        }
                    }
                }
            }));

            app.UseAuthorize();

            //个人信息
            app.Map("/profile",builder => builder.Run(async context => 
            {
                await context.Response.WriteHtmlAsync(async res => 
                {
                    await res.WriteAsync($"<h1>你好，当前登录用户： {HttpResponseExtensions.HtmlEncode(context.User.Identity.Name)}</h1>");
                    await res.WriteAsync("<a class=\"btn btn-default\" href=\"/Account/Logout\">退出</a>");
                    await res.WriteAsync($"<h2>AuthenticationType：{context.User.Identity.AuthenticationType}</h2>");

                    await res.WriteAsync("<h2>Claims:</h2>");
                    await res.WriteTableHeader(new string[] { "Claim Type", "Value" }, context.User.Claims.Select(c => new string[] { c.Type, c.Value }));
                });
            }));

            //退出
            app.Map("/Account/Logout",builder => builder.Run(async context => 
            {
                await context.SignOutAsync();
                context.Response.Redirect("/");
            }));

            //首页
            app.Run(async (context) =>
            {
                await context.Response.WriteHtmlAsync(async res => 
                {
                    await res.WriteAsync($"<h2>Hello Cookie Authentication</h2>");
                    await res.WriteAsync("<a class=\"btn btn-default\" href=\"/profile\">我的信息</a>");
                });
            });
        }
    }

    public static class MyAppBuilderExtensions
    {
        public static IApplicationBuilder UseAuthorize(this IApplicationBuilder app)
        {
            return app.Use(async (context,next) => 
            {
                if (context.Request.Path == "/")
                {
                    await next();
                }
                else
                {
                    if (context.User?.Identity?.IsAuthenticated ?? false) //检测是否有授权
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

    //public static class LastChangedValidator
    //{
    //    public static async Task ValidateAsync(CookieValidatePrincipalContext context)
    //    {
    //        var userRepository = context.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
    //        var userPrincipal = context.Principal;
    //        var lastChanged = (from c in userPrincipal.Claims
    //                           where c.Type == "LastUpdated"
    //                           select c.Value).FirstOrDefault();

    //        if (string.IsNullOrEmpty(lastChanged) || !userRepository.ValidateLastChanged(userPrincipal,lastChanged))
    //        {
    //            // 1.验证失败 等同于 Principal = principal
    //            context.RejectPrincipal();

    //            // 2.验证通过，并会重新生成Cookie
    //            context.ShouldRenew = true;
    //        }

    //    }
    //}
}
