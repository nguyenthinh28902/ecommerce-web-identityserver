using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Ecom.IdentityServer.Models.Enums;
using Ecom.IdentityServer.Models.Settings;

namespace Ecom.IdentityServer.Common.Helpers.Identity.Config
{
    public static class Clients
    {
        public static IEnumerable<Client> Get(ConfigServiceUrl configServiceUrl)
        {
            return new[]
             {
                new Client
                {
                    ClientId = "IdentityServer",
                    ClientSecrets = { new Secret("IdentityServer-secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientClaimsPrefix = "",
                    AllowedScopes =
                    {
                        // Chỉ add các quyền "Internal" để Gateway có quyền quản trị cao nhất khi gọi Service
                        "customer.internal",
                    },
                    AccessTokenLifetime = 5 * 60 // ⏱️ 5 phút là quá đủ
                },
                new Client
                {
                    ClientId = "APIGatewayWeb.internal",
                    ClientSecrets = { new Secret("gateway-web-secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientClaimsPrefix = "",
                    AllowedScopes =
                    {
                        // Chỉ add các quyền "Internal" để Gateway có quyền quản trị cao nhất khi gọi Service
                        "customer.internal",
                        "product.read.web",
                        "order.internal.web",
                        "payment.internal.web"
                    },
                    AccessTokenLifetime = 5 * 60 // ⏱️ 5 phút là quá đủ
                },

                new Client
                {
                    ClientId = ServiceAuth.ecom_web_client.ToString(),
                    ClientName = "DotNet MVC Client",

                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true, // Chuẩn bảo mật cao nhất hiện nay
                    RequireClientSecret = true,

                    ClientSecrets = { new Secret("ecom_web_client_secret_key_123".Sha256()) },

                    RedirectUris = { $"{configServiceUrl.EcomWebUrl}/signin-oidc" },
                    PostLogoutRedirectUris = { configServiceUrl.EcomWebUrl },
    
                    // Cho phép Nuxt nhận thông báo đăng xuất qua front-channel
                    FrontChannelLogoutUri = $"{configServiceUrl.EcomWebUrl}/signout-callback-oidc",
                    // Quan trọng cho UX
                    RequireConsent = false,

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "customer.read",
                        "customer.write",
                        "product.read.web",
                        "order.write.web",
                        "order.read.web",
                        "payment.read.web",
                        "payment.write.web"
                    },
                    AlwaysSendClientClaims = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    AllowOfflineAccess = true, // Cho phép dùng Refresh Token
                    RefreshTokenUsage = TokenUsage.ReUse,
                    RefreshTokenExpiration = TokenExpiration.Sliding, // Gia hạn thời gian logout khi user còn hoạt động
    
                    AccessTokenLifetime = 7200, // 1 giờ
                    IdentityTokenLifetime = 7200,
                    UpdateAccessTokenClaimsOnRefresh = true
                }

            };
        }
    }

}
