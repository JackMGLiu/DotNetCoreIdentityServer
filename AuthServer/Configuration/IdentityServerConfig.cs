using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthServer.Configuration
{
    /// <summary>
    /// IdentityServer配置类
    /// </summary>
    public class IdentityServerConfig
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email()
            };
        }

        //基本说明
        //ApiResources: 这里指定了name和display name, 以后api使用authorization server的时候, 这个name一定要一致, 否则就不好用的.
        //Clients: Client的属性太多了, 这里就指定几个.其中ClientSecrets是Client用来获取token用的.AllowedGrantType: 这里使用的是通过用户名密码和ClientCredentials来换取token的方式.ClientCredentials允许Client只使用ClientSecrets来获取token.这比较适合那种没有用户参与的api动作.AllowedScopes: 这里只用demonetwork1
        //Users: 这里的内存用户的类型是TestUser, 只适合学习和测试使用, 实际生产环境中还是需要使用数据库来存储用户信息的, 例如接下来会使用asp.net core identity.TestUser的SubjectId是唯一标识.
        //然后回到StartUp的ConfigureServices 去注册
        //需要对token进行签名, 这意味着identity server需要一对public和private key.我们可以告诉identity server在程序的运行时候对这项工作进行设定: AddDeveloperSigningCredential(), 它默认会存到硬盘上的, 所以每次重启服务不会破坏开发时的数据同步.这个方法只适合用于identity server4在单个机器运行, 如果是production farm你得使用

        public static IEnumerable<ApiResource> ApiResources()
        {
            return new[]
            {
                new ApiResource("demonetwork1", "测试网站1")
            };
        }

        public static IEnumerable<Client> Clients()
        {
            return new[]
            {
                new Client
                {
                    ClientId = "democlient1",
                    ClientSecrets = new [] { new Secret("mysecret123qweASD".Sha256()) },
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    AllowedScopes = new [] { "demonetwork1" }
                },
                new Client
                { 
                    //OAuth是使用Scopes来划分Api的, 而OpenId Connect则使用Scopes来限制信息, 例如使用offline access时的Profile信息, 还有用户的其他细节信息.
                    //这里GrantType要改为Implicit. 使用Implicit flow时, 首先会重定向到Authorization Server, 然后登陆, 然后Identity Server需要知道是否可以重定向回到网站, 如果不指定重定向返回的地址的话, 我们的Session有可能就会被劫持.
                    //RedirectUris就是登陆成功之后重定向的网址, 这个网址(http://localhost:5002/signin-oidc)在MvcClient里, openid connect中间件使用这个地址就会知道如何处理从authorization server返回的response. 这个地址将会在openid connect 中间件设置合适的cookies, 以确保配置的正确性.
                    //而PostLogoutRedirectUris是登出之后重定向的网址. 有可能发生的情况是, 你登出网站的时候, 会重定向到Authorization Server, 并允许从Authorization Server也进行登出动作.
                    //最后还需要指定OpenId Connect使用的Scopes, 之前我们指定的socialnetwork是一个ApiResource. 而这里我们需要添加的是让我们能使用OpenId Connect的SCopes, 这里就要使用Identity Resources. Identity Server带了几个常量可以用来指定OpenId Connect预包装的Scopes. 上面的AllowedScopes设定的就是我们要用的scopes, 他们包括 openid Connect和用户的profile, 同时也包括我们之前写的api resource: "demonetwork1". 要注意区分, 这里有Api resources, 还有openId connect scopes(用来限定client可以访问哪些信息), 而为了使用这些openid connect scopes, 我们需要设置这些identity resoruces, 这和设置ApiResources差不多:
                    ClientId = "mvc_implicit",
                    ClientName = "MVC_Client",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    RedirectUris = { "http://localhost:5002/signin-oidc" },
                    PostLogoutRedirectUris = { "http://localhost:5002/signout-callback-oidc" },
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "demonetwork1"
                    },
                    AllowAccessTokensViaBrowser = true
                },
                new Client
                {
                    ClientId = "mvc_code",
                    ClientName = "MVC Code Client",
                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    RedirectUris = { "http://localhost:5003/signin-oidc" },
                    PostLogoutRedirectUris = { "http://localhost:5003/signout-callback-oidc" },
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,//因为想改变api来允许我基于email来创建用户的数据, 因为authorization server 和 web api是分开的, 所以用户的数据库也是分开的. Api使用用户名(email)来查询数据库中的数据
                        "demonetwork1"
                    },
                    AllowOfflineAccess = true,//需要获取Refresh Token, 这就要求我们的网站必须可以"离线"工作, 这里离线是指用户和网站之间断开了, 并不是指网站离线了
                    AllowAccessTokensViaBrowser = true
                }
            };
        }

        public static IEnumerable<TestUser> Users()
        {
            return new[]
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "testuser1@qq.com",
                    Password = "123qweA",
                    Claims = new [] { new Claim("email", "test1@qq.com") }
                }
            };
        }
    }
}
