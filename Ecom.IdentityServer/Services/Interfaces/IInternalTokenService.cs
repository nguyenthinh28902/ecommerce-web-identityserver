using Ecom.IdentityServer.Models.DTOs.SignIn;

namespace Ecom.IdentityServer.Services.Interfaces
{
    public interface IInternalTokenService
    {
        /// <summary>
        /// Lấy token hệ thống (Client Credentials) cho một service cụ thể.
        /// Thường dùng cho các tác vụ background hoặc server-to-server không có user.
        /// </summary>
        Task<TokenResponseDto?> GetSystemTokenAsync();
    }
}
