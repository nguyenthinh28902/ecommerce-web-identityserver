using Duende.IdentityServer;
using Ecom.IdentityServer.Common.Exceptions;
using Ecom.IdentityServer.Models;
using Ecom.IdentityServer.Models.DTOs.SignIn;
using Ecom.IdentityServer.Models.Enums;
using Ecom.IdentityServer.Models.Settings;
using Ecom.IdentityServer.Models.ViewModels.Accounts;
using Ecom.IdentityServer.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace Ecom.IdentityServer.Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuthService> _logger;
        private readonly IInternalCacheService _internalCacheService;
        private readonly IInternalTokenService _tokenService;
        private readonly ConfigServiceUrl _configServiceUrl;

        public AuthService(HttpClient httpClient,
            ILogger<AuthService> logger,
            IHttpContextAccessor httpContextAccessor,
            IInternalCacheService internalCacheService,
            IInternalTokenService internalTokenService,
            IOptions<ConfigServiceUrl> options)
        {
            _httpClient = httpClient;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _internalCacheService = internalCacheService;
            _tokenService = internalTokenService;
            _configServiceUrl = options.Value;
        }

        /// <summary>
        /// Xác thực thông tin đăng nhập với IdentityCMSService.
        /// </summary>
        /// <param name="signInViewModel">Thông tin UserID và Password từ nhân sự.</param>
        /// <returns>Thông tin User đã được xác thực dưới dạng SignInResponseDto.</returns>
        public async Task<Result<SignInResponseDto>> AuthenticateWithUserServiceAsync(SignInViewModel signInViewModel)
        {
            // 1. Tạo request gọi sang User Service
            var request = new HttpRequestMessage(HttpMethod.Post, ConfigApi.ApiAuthenticateInternal);
            var token = await _tokenService.GetSystemTokenAsync();
            if (token == null) throw new UnauthorizedException("Yêu cầu không được chấp nhận");
            _logger.LogInformation($"token {token.AccessToken}");
            _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token.AccessToken);
            var payload = new
            {
                Id = signInViewModel.UserId,
                Password = signInViewModel.Password
            };

            request.Content = JsonContent.Create(payload);

            try
            {
                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Xác thực thất bại tại User Service. Status: {StatusCode}", response.StatusCode);
                    return Result<SignInResponseDto>.Failure("Tài khoản hoặc mật khẩu không chính xác.");
                }

                var signInResponseDto = await response.Content.ReadFromJsonAsync<SignInResponseDto>();
                return Result<SignInResponseDto>.Success(signInResponseDto!, "Xác thực thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi kết nối khi gọi User Service qua BaseAddress: {BaseUrl}", _httpClient.BaseAddress);
                return Result<SignInResponseDto>.Failure("Hệ thống xác thực nhân sự đang bận, ný thử lại sau nhé.");
            }
        }

        /// <summary>
        /// Hàm xác thực thông tin user với IdentityCMSService và thực hiện lưu trữ phiên đăng nhập 
        /// kết kết hợp 2 hàm AuthenticateProvider + SignInIdentityUserAsync
        /// </summary>
        /// <param name="signInViewModel">Thông tin UserID và Password từ nhân sự</param>
        /// <returns></returns>
        public async Task<Result<SignInResponseDto>> ProcessSignInAsync(SignInViewModel signInViewModel)
        {
            // 1. Thực hiện xác thực thông tin qua User Service
            var userCheck = await AuthenticateWithUserServiceAsync(signInViewModel);

            // Kiểm tra nếu thông tin trả về null hoặc API báo lỗi nghiệp vụ
            if (userCheck.Data == null)
            {
                return Result<SignInResponseDto>.Failure("Thông tin đăng nhập không chính xác hoặc tài khoản bị khóa.");
            }

            try
            {
                // 2. Thiết lập phiên đăng nhập (Ghi Redis và Cookie IdentityServer)
                // Đây là bước quan trọng nhất để trình duyệt nhận được thẻ bài (Cookie)
                await EstablishUserSessionAsync(userCheck.Data);

                return Result<SignInResponseDto>.Success(userCheck.Data, "Xác thực thành công.");
            }
            catch (Exception ex)
            {
                // Log chi tiết lỗi hệ thống để kỹ thuật kiểm tra (ví dụ: mất kết nối Redis)
                _logger.LogError(ex, "Lỗi xảy ra trong quá trình thiết lập phiên đăng nhập cho User: {UserId}", signInViewModel.UserId);

                return Result<SignInResponseDto>.Failure("Hệ thống gặp sự cố khi khởi tạo phiên làm việc.");
            }
        }

        /// <summary>
        /// Thực hiện lưu trữ phiên đăng nhập vào IdentityServer Cookie.
        /// Thiết lập các Claims quan trọng như sub, email, wid và roles.
        /// </summary>
        /// <param name="user">Dữ liệu người dùng từ hệ thống nội bộ.</param>
        public async Task EstablishUserSessionAsync(SignInResponseDto user)
        {
            var claims = new List<Claim>
            {
                 new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                 new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                 new Claim(JwtRegisteredClaimNames.PhoneNumber, user.PhoneNumber ?? "")
             };



            var isUser = new IdentityServerUser(user.Id.ToString())
            {
                DisplayName = user.Email ?? user.PhoneNumber,
                AdditionalClaims = claims
            };

            await _httpContextAccessor.HttpContext.SignInAsync(isUser);
        }

        public async Task<bool> AuthenticateProvider(UserInfoSinginDto userInfoSinginDto, string providerName)
        {

            var token = await _tokenService.GetSystemTokenAsync();
            _logger.LogInformation($"check token AuthenticateInternal  {token?.AccessToken}");
            _httpClient.DefaultRequestHeaders.Authorization =
                  new AuthenticationHeaderValue("Bearer", token?.AccessToken);

            var payload = new
            {
                Request = userInfoSinginDto,
                ProviderName = providerName
            };
            try
            {
                var response = await _httpClient.PostAsJsonAsync(
              $"{_configServiceUrl.EcomGatewayUrl}{ConfigApi.ApiAuthenticateInternal}",
              payload);
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var result = await response.Content.ReadFromJsonAsync<Result<SignInResponseDto>>();
                    if (result == null || result.IsSuccess == false || result.Data == null)
                    {
                        return false;
                    }
                    await EstablishUserSessionAsync(result.Data);
                }
                else return false;
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"call api check thông tin khách hàng {userInfoSinginDto.Name} lỗi: {ex.Message}");
                _logger.LogInformation($"Thông tin google: {JsonSerializer.Serialize(userInfoSinginDto)}");
                return false;
            }

            return true;
        }


    }
}
