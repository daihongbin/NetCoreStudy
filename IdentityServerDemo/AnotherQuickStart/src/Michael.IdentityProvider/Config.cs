using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;

namespace Michael.IdentityProvider
{
    public static class Config
    {
        public static List<TestUser> GetUsers() => new List<TestUser>
        {
            new TestUser
            {
                SubjectId = "1",
                Username = "Nick",
                Password = "password",
                Claims = new List<Claim>
                {
                    new Claim("given_name","Nick"),
                    new Claim("family_name","Carter")
                }
            },
            new TestUser
            {
                SubjectId = "2",
                Username = "Dave",
                Password = "password",
                Claims = new List<Claim>
                {
                    new Claim("given_name","Dave"),
                    new Claim("family_name","Mustaine")
                }
            }
        };

        public static IEnumerable<IdentityResource> GetIdentityResources() => new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile()
        };

        public static IEnumerable<Client> GetClients() => new List<Client>
        {
            new Client
            {
                ClientId = "mvcclient",
                ClientName = "MVC 客户端",
                AllowedGrantTypes = GrantTypes.Hybrid,

                //登陆后跳转地址
                RedirectUris = {"https://localhost:5002/signin-oidc"},

                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId
                },
                ClientSecrets =
                {
                    new Secret("secret".Sha256())
                }
            }
        };

    }
}
