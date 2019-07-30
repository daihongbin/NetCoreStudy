using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace MVCClient
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            //关闭Jwt的claim类型映射，以便允许well-know claims。
            //
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            })
            .AddCookie("Cookies",options => 
            {
                options.AccessDeniedPath = "/Authroization/AccessDenied";
            })
            .AddOpenIdConnect("oidc", options =>
             {
                 options.SignInScheme = "Cookies";
                 options.Authority = "https://localhost:5001";
                 options.RequireHttpsMetadata = true;
                 options.ClientId = "mvcclient";
                 options.ResponseType = "code id_token";
                 options.Scope.Clear();
                 options.Scope.Add("openid");
                 options.Scope.Add("profile");
                 options.Scope.Add("email");
                 options.Scope.Add("roles");
                 options.Scope.Add("restapi");

                 options.SaveTokens = true;
                 options.ClientSecret = "secret";
                 options.GetClaimsFromUserInfoEndpoint = true;

                 //不想让中间件过滤一些cliams，就是显示一些claim
                 options.ClaimActions.Remove("nbf");
                 options.ClaimActions.Remove("amr");

                 //删除某些claims
                 options.ClaimActions.DeleteClaim("sid");
                 options.ClaimActions.DeleteClaim("idp");
                 options.ClaimActions.DeleteClaim("email");

                 options.ClaimActions.MapUniqueJsonKey("role","role");

                 //控制访问权限
                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     NameClaimType = JwtClaimTypes.GivenName,
                     RoleClaimType = JwtClaimTypes.Role
                 };
             });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}

