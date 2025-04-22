namespace BerAuto.DTO
{

    public class LoginDTO
    {
        [Required, EmailAddress] public string Email { get; set; }

        [Required] public string Password { get; set; }

    }

    public class RegisterDTO
    {
        [Required]
        public string Name { get; set; }
        
        [Required, EmailAddress]
        public string Email { get; set; }
        
        [Required]
        public string Password { get; set; }
        
        [Required]
        public string Address { get; set; }
        
        [Required]
        public string PhoneNumber { get; set; }
        
        public string? Description { get; set; }
    }

    public class AuthResponseDTO
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime Expiration { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string AccesLevel { get; set; }
    }
    
    public class RefreshTokenDTO
    {
        [Required]
        public string RefreshToken { get; set; }
    }
    public class LoginResponseDTO
    {
        public Guid ID { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
    }
}