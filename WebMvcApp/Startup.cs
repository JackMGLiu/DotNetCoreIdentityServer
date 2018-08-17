using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebMvcApp
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

            //添加IdentityServer配置
            //JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); 这句话是指, 我们关闭了JWT的Claim 类型映射, 以便允许well-known claims.

            //这样做, 就保证它不会修改任何从Authorization Server返回的Claims.

            //AddAuthentication()方法是像DI注册了该服务.

            //这里我们使用Cookie作为验证用户的首选方式: DefaultScheme = "Cookies".

            //而把DefaultChanllangeScheme设为"oidc"是因为, 当用户需要登陆的时候, 将使用的是OpenId Connect Scheme.

            //然后的AddCookie, 是表示添加了可以处理Cookie的处理器(handler).

            //最后AddOpenIdConnect是让上面的handler来执行OpenId Connect 协议.

            //其中的Authority是指信任的Identity Server(Authorization Server).

            //ClientId是Client的识别标志.目前Authorization Server还没有配置这个Client, 一会我们再弄.

            //Client名字也暗示了我们要使用的是implicit flow, 这个flow主要应用于客户端应用程序, 这里的客户端应用程序主要是指javascript应用程序. implicit flow是很简单的重定向flow, 它允许我们重定向到authorization server, 然后带着id token重定向回来, 这个 id token就是openid connect 用来识别用户是否已经登陆了.同时也可以获得access token. 很明显, 我们不希望access token出现在那个重定向中. 这个一会再说.

            //一旦OpenId Connect协议完成, SignInScheme使用Cookie Handler来发布Cookie (中间件告诉我们已经重定向回到MvcClient了, 这时候有token了, 使用Cookie handler来处理).

            //SaveTokens为true表示要把从Authorization Server的Reponse中返回的token们持久化在cookie中.

            //注意正式生产环境要使用https, 这里就不用了
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            })
            .AddCookie("Cookies")
            .AddOpenIdConnect("oidc", options =>
            {
                options.SignInScheme = "Cookies";

                options.Authority = "http://localhost:5000";
                options.RequireHttpsMetadata = false;

                options.ClientId = "mvc_implicit";
                options.SaveTokens = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            //添加验证中间件
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
