using Ecom.IdentityServer.Models;
using Ecom.IdentityServer.Models.DTOs.SignIn;
using Ecom.IdentityServer.Models.ViewModels.Accounts;

namespace Ecom.IdentityServer.Services.Interfaces
{
    public interface IAuthService
    {
        /// <summary>
        /// Xác thực thông tin đăng nhập với IdentityCMSService thông qua System Token.
        /// </summary>
        /// <param name="signInViewModel">Thông tin UserID và Password từ nhân sự.</param>
        /// <returns>Thông tin User đã được xác thực dưới dạng SignInResponseDto.</returns>
        Task<Result<SignInResponseDto>> AuthenticateWithUserServiceAsync(SignInViewModel signInViewModel);

        /// <summary>
        /// Thực hiện lưu trữ phiên đăng nhập vào IdentityServer Cookie.
        /// Thiết lập các Claims quan trọng như sub, email, wid và roles.
        /// </summary>
        /// <param name="user">Dữ liệu người dùng từ hệ thống nội bộ.</param>
        Task EstablishUserSessionAsync(SignInResponseDto user);

        /// <summary>
        /// Hàm xác thực thông tin user với IdentityCMSService và thực hiện lưu trữ phiên đăng nhập 
        /// kết kết hợp 2 hàm AuthenticateProvider + SignInIdentityUserAsync
        /// </summary>
        /// <param name="signInViewModel">Thông tin UserID và Password từ nhân sự</param>
        /// <returns></returns>
        public Task<Result<SignInResponseDto>> ProcessSignInAsync(SignInViewModel signInViewModel);

        /// <summary>
        /// hàm sign-in với google
        /// </summary>
        /// <param name="userInfoSinginDto">thông tin user của goole trả về</param>
        /// <param name="providerName">google</param>
        /// <returns>sigin-in với idenity server true</returns>
        public Task<bool> AuthenticateProvider(UserInfoSinginDto userInfoSinginDto, string providerName);

    }
}
