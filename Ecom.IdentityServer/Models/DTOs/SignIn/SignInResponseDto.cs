namespace Ecom.IdentityServer.Models.DTOs.SignIn
{
    public class SignInResponseDto
    {
        // Sử dụng kiểu string cho Id nếu bạn có ý định dùng GUID sau này, 
        // hoặc giữ int nhưng nên nhất quán với DB.
        public SignInResponseDto() { }
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
