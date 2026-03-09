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
                new ApiScope("order.internal.web", "Full access to order Service in web"),
                new ApiScope("order.read.web", "Read order information"),
                new ApiScope("order.write.web", "Write order information"),
                new ApiScope("payment.internal.web", "Full access to payment Service in web"),
                new ApiScope("payment.read.web", "Read payment information"),
                new ApiScope("payment.write.web", "Write payment information"),
            };
        }
    }
}
