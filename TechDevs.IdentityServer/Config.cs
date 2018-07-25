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
                // SPA client using implicit flow
                new Client
                {
                    ClientId = "spa",
                    ClientName = "SPA Client",
                    ClientUri = "http://identityserver.io",

                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    RedirectUris =
                    {
                        "http://localhost:5003/index.html",
                        "http://localhost:5003/callback.html",
                        "http://localhost:5003/silent.html",
                        "http://localhost:5003/popup.html",
                    },

                    PostLogoutRedirectUris = { "http://localhost:5003/index.html" },
                    AllowedCorsOrigins = { "http://localhost:5003", "http://localhost:5001" },

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
                    RequireConsent = true
                }
            };
        }
    }
}