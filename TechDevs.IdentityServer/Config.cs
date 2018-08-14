// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;
using System.Collections.Generic;

namespace TechDevs.IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        public static IEnumerable<ApiResource> GetApis()
        {
            var api1 = new ApiResource("techdevs-accounts-api", "TechDevs Accounts API");
            api1.ApiSecrets.Add(new Secret("TECHDEVS"));
            api1.UserClaims.Add("email");

            return new ApiResource[]
            {
                new ApiResource("api1", "My API #1"),
               api1
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new[]
             {
                 // resource owner password grant client
                new Client
                {
                    ClientId = "ro.client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = {
                        "emailaddress",
                        "name",
                        "given_name",
                        "family_name",
                        "openid",
                        "profile",
                        "api1",
                        "techdevs-accounts-api"
                    }
                },

                // SPA client using implicit flow
                new Client
                {
                    ClientId = "dev-customer-portal",
                    ClientName = "DEV Customer Portal",
                    ClientUri = "http://localhost:4200",

                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    RedirectUris =
                    {
                        "http://localhost:4200",
                        "http://localhost:4200/assets/callback.html"
                    },


                    PostLogoutRedirectUris = { "http://localhost:4200/index.html", "http://localhost:4200" },
                    AllowedCorsOrigins = { "http://localhost:4200", "http://localhost:4200", "http://localhost:4200" },

                    AllowedScopes =
                    {
                        "emailaddress",
                        "name",
                        "given_name",
                        "family_name",
                        "openid",
                        "profile",
                        "api1",
                        "techdevs-accounts-api"
                    },
                    RequireConsent = false
                },    
            
                // SPA client using implicit flow
                new Client
                {
                    ClientId = "dev-spa",
                    ClientName = "DEV SPA Client",
                    ClientUri = "http://localhost:8100",

                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    RedirectUris =
                    {
                        "http://localhost:5003/index.html",
                        "http://localhost:5003/callback.html",
                        "http://localhost:5003/silent.html",
                        "http://localhost:5003/popup.html",
                        "http://localhost:8100/auth-callback",
                        "http://localhost:8100/auth-callback.html",
                        "http://localhost:8100/auth.html",
                        "http://localhost:8100"
                    },


                    PostLogoutRedirectUris = { "http://localhost:5003/index.html", "http://localhost:8100" },
                    AllowedCorsOrigins = { "http://localhost:5003", "http://localhost:5001", "http://localhost:8100" },

                    AllowedScopes =
                    {
                        "emailaddress",
                        "name",
                        "given_name",
                        "family_name",
                        "openid",
                        "profile",
                        "api1",
                        "techdevs-accounts-api"
                    },
                    RequireConsent = false
                }, 
                  // SPA client using implicit flow
                new Client
                {
                    ClientId = "spa",
                    ClientName = "SPA Client",
                    ClientUri = "https://techdevs-dpmobile.netlify.com",

                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    RedirectUris =
                    {
                        "https://techdevs-dpmobile.netlify.com",
                        "https://techdevs-dpmobile.netlify.com/auth-callback"
                    },

                    PostLogoutRedirectUris = { "https://techdevs-dpmobile.netlify.com" },
                    AllowedCorsOrigins = { "https://techdevs-dpmobile.netlify.com" },

                    AllowedScopes =
                    {
                        "emailaddress",
                        "name",
                        "given_name",
                        "family_name",
                        "openid",
                        "profile",
                        "api1",
                        "techdevs-accounts-api"
                    },
                    RequireConsent = false
                }
            };
        }
    }
}