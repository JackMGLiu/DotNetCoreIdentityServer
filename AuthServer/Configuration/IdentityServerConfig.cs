using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Configuration
{
    /// <summary>
    /// IdentityServer配置类
    /// </summary>
    public class IdentityServerConfig
    {
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
                    Password = "123qweA"
                }
            };
        }
    }
}
