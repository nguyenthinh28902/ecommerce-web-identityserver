using Ecom.IdentityServer.Models.DTOs.SignIn;
using Ecom.IdentityServer.Models.Enums;
using Ecom.IdentityServer.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ecom.IdentityServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoogleController : ControllerBase
    {
        private readonly ILogger<GoogleController> _logger;
        private readonly IAuthService _authService;
        public GoogleController(ILogger<GoogleController> logger,
            IAuthService authService) { 
            _logger = logger;
            _authService = authService;
        }

        /// <summary>
        ///chuyển hướng đăng nhập với google
        /// </summary>
        /// <param name="returnUrl">đường dẫn sao khi đăng nhập</param>
        /// <returns>chuyển trang đăng nhập với google</returns>
        [HttpPost("dang-nhap-google")]
        public IActionResult GoogleSignIn([FromForm] string returnUrl = "/")
        {
            
            // RedirectUri ở đây là nơi Middleware trả về SAU KHI đã xác thực xong tại /signin-google
            var redirectUri = Url.Action(nameof(GoogleCallback), CookieName.Google.ToString(), new { returnUrl });
            var props = new AuthenticationProperties
            {
                RedirectUri = redirectUri,
                Items = {
                    { "scheme", CookieName.Google.ToString() },
                    { "returnUrl", returnUrl }
                }
            };
            _logger.LogInformation($"redirectUri googole: {redirectUri}");
            // Khi bạn gọi Challenge, Middleware Google sẽ tự lấy Request.Host (lúc này là 7288 nhờ ForwardedHeaders)
            // để tạo ra tham số redirect_uri=https://localhost:7109/signin-google gửi cho Google.
            return Challenge(props, CookieName.Google.ToString());
        }

        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback(string returnUrl = "/")
        {
            _logger.LogInformation($"Login thành công google call api callback");
            var resultHttpContext = await HttpContext.AuthenticateAsync(CookieName.External.ToString());
            _logger.LogInformation("External authentication result: {Succeeded}", resultHttpContext.Succeeded);
            var targetUrl = Url.Action("Index", "Login", new { returnUrl = returnUrl });
            if (!resultHttpContext.Succeeded)
            {

                return Redirect(targetUrl);
            }
            var googleUser = new UserInfoSinginDto();
            googleUser.ProviderUserId = resultHttpContext.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            googleUser.Email = resultHttpContext.Principal.FindFirst(ClaimTypes.Email)?.Value;
            googleUser.Name = resultHttpContext.Principal.FindFirst(ClaimTypes.Name)?.Value;
            googleUser.Picture = resultHttpContext.Principal.FindFirst("image_url")?.Value; // Google avatar
            var result = await _authService.AuthenticateProvider(googleUser, "Google");
            if (!result)
            {
                return Redirect(targetUrl);
            }
            return Redirect(returnUrl);
        }
    }
}
