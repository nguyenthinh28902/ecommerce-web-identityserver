using Ecom.IdentityServer.Models.Settings;

namespace Ecom.IdentityServer.Common.Helpers
{
    public static class ConfigAppSetting
    {
        public static IServiceCollection AddConfigAppSetting(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<InternalAuthOptions>(configuration.GetSection(nameof(InternalAuthOptions)));
            services.Configure<ConfigServiceUrl>(configuration.GetSection(nameof(ConfigServiceUrl)));
            return services;
        }
    }
}
