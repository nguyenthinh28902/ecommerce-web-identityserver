using Duende.IdentityServer.Models;
using System.IdentityModel.Tokens.Jwt;

namespace Ecom.IdentityServer.Common.Helpers.Identity.Config
{
    public static class ApiResources
    {
        public static IEnumerable<ApiResource> Get()
        {
            return new[]
            {
                // 1. Customer Service API Resource
                new ApiResource("customer.api", "Identity CMS Service API")
                {
                    Scopes = { "customer.internal", "customer.read", "customer.write" },
                    // Thêm Email và PhoneNumber vào đây để IdentityServer biết đường mà nhồi vào Access Token
                    UserClaims =
                    {
                        JwtRegisteredClaimNames.Sub,
                        JwtRegisteredClaimNames.Email,
                        JwtRegisteredClaimNames.PhoneNumber
                    },
                },

                // 2. Product Service API Resource
                new ApiResource("product.api", "Product Service API")
                {
                    Scopes = { "product.read.web" },
                    // Tương tự, nếu Product Service cần thông tin liên lạc thì add vào đây
                    UserClaims =
                    {
                        JwtRegisteredClaimNames.Sub,
                        JwtRegisteredClaimNames.Email
                    },
                },
                // 3. order Service API Resource
                new ApiResource("order.api", "order Service API")
                {
                    Scopes = {"order.internal.web" ,"order.read.web", "order.write.web"},
                    // Tương tự, nếu Order Service cần thông tin liên lạc thì add vào đây
                    UserClaims =
                    {
                        JwtRegisteredClaimNames.Sub,
                        JwtRegisteredClaimNames.Email
                    },
                },
                new ApiResource("payment.api", "Payment Service API")
                {
                    Scopes = { "payment.internal.web", "payment.read.web", "payment.write.web" },
                    // UserClaims giúp đính kèm thêm thông tin vào Access Token khi gọi API này
                    UserClaims =
                    {
                        JwtRegisteredClaimNames.Sub,
                        JwtRegisteredClaimNames.Email
                    },
                }
            };
        }
    }
}
