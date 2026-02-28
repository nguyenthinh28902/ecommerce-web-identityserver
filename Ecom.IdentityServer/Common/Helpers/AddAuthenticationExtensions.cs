using Ecom.IdentityServer.Models.Enums;
using Ecom.IdentityServer.Models.Google;
using Ecom.IdentityServer.Models;
using Ecom.IdentityServer.Models.Enums;
using Ecom.IdentityServer.Models.Settings;
using Microsoft.AspNetCore.Authentication;

namespace Ecom.IdentityServer.Common.Helpers
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddAuthenticationExtensions(this IServiceCollection services, IConfiguration configuration)
        {
            var configServiceUrl = configuration.GetSection(nameof(ConfigServiceUrl)).Get<ConfigServiceUrl>();
            var googleConfig = configuration.GetSection(nameof(GoogleAuthentication)).Get<GoogleAuthentication>();
            if (configServiceUrl == null) throw new ArgumentNullException($"Không tìm thấy cấu hình trong appsettings.{nameof(AddAuthenticationExtensions)}");

            services.AddAuthentication().AddCookie(CookieName.Cookies.ToString()) // Cookie chính cho ứng dụng
            .AddCookie(CookieName.External.ToString()) // Cookie tạm cho Google
            .AddGoogle(CookieName.Google.ToString(), options =>
            {
                if (googleConfig != null)
                {
                    options.ClientId = googleConfig.client_id;
                    options.ClientSecret = googleConfig.client_secret;
                    options.SaveTokens = true;
                    // Chỉ định nơi lưu kết quả tạm thời sau khi Google đăng nhập xong
                    options.SignInScheme = CookieName.External.ToString();
                    options.Scope.Add("profile");
                    options.ClaimActions.MapJsonKey("image_url", "picture");
                }
            });


            services.AddCors(options =>
            {
                options.AddPolicy(AuthEnum.AllowWebCMS.ToString(), policy =>
                {
                    var EcommerceMVCCMS = configServiceUrl.EcomWebUrl;
                    if (!string.IsNullOrEmpty(EcommerceMVCCMS))
                    {
                        policy.WithOrigins(EcommerceMVCCMS)
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials();
                    }
                });
            });
            var hours = (int)ExpireTimeSpanSignIn.Medium; // hours = 8
            // 3. Cấu hình Cookie (Để IdentityServer tương tác tốt với trình duyệt hiện đại)
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = "identity_web_auth_session";
                // Đổi về Lax vì đi qua Gateway/Cùng Domain chính sẽ an toàn và dễ chịu hơn
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.ExpireTimeSpan = TimeSpan.FromHours(hours);
                options.SlidingExpiration = true;
            });

            return services;
        }
    }
}
