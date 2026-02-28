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
        new ApiResource("Product.api", "Product Service API")
        {
            Scopes = { "Product.read.web" },
            // Tương tự, nếu Product Service cần thông tin liên lạc thì add vào đây
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
