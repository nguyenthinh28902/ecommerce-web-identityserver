using Duende.IdentityServer.Models;

namespace EcommerceIdentityServerCMS.Common.Helpers.Identity.Config
{
    public static class ApiScopes
    {
        public static IEnumerable<ApiScope> Get()
        {
            return new[]
            {
               

                // Quyền cũ của bạn
                new ApiScope("user.internal", "User full access"),
                new ApiScope("user.read", "User read information"),
                new ApiScope("user.write", "User write information"),
            };
        }
    }
}
