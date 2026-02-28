using Duende.IdentityServer.Models;

namespace Ecom.IdentityServer.Common.Helpers.Identity.Config
{
    public static class ApiScopes
    {
        public static IEnumerable<ApiScope> Get()
        {
            return new[]
            {
                new ApiScope("customer.internal", "Full access to customer Service"),
                new ApiScope("customer.read", "view infor"),
                new ApiScope("customer.write", "Update infor"),
                new ApiScope("product.read.web", "Read product information"),
            };
        }
    }
}
