﻿using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
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
                    new Claim("family_name","Carter"),
                    new Claim("email","12345@qq.com"),
                    new Claim("role","管理员"),
                    new Claim("nationality","China"),
                    new Claim("gender","female")
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
                    new Claim("family_name","Mustaine"),
                    new Claim("email","67890@qq.com"),
                    new Claim("role","注册用户"),
                    new Claim("nationality","USA"),
                    new Claim("gender","male")
                }
            },
            new TestUser
            {
                SubjectId = "3",
                Username = "Kevin",
                Password = "password",
                Claims = new List<Claim>
                {
                    new Claim("given_name","Kevin"),
                    new Claim("family_name","Chang"),
                    new Claim("email","67890111@qq.com"),
                    new Claim("role","注册用户"),
                    new Claim("nationality","China"),
                    new Claim("gender","male")
                }
            }
        };

        public static IEnumerable<IdentityResource> GetIdentityResources() => new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email(),
            new IdentityResource("roles","角色",new List<string>{"role" }),
            new IdentityResource("nationality","国籍",new List<string>{"nationality" })
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

                PostLogoutRedirectUris = {"https://localhost:5002/singout-callback-oidc"},

                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "roles",
                    "restapi",
                    "nationality"
                },
                ClientSecrets =
                {
                    new Secret("secret".Sha256())
                }
            }
        };

        public static IEnumerable<ApiResource> GetApiResources() => new List<ApiResource>
        {
            new ApiResource("restapi","RESTful API",new List<string>
            {
                "role",
                "given_name",
                "gender",
                "nationality"
            })
        };

    }
}
