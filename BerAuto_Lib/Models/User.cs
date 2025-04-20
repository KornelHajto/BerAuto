using System;

namespace BerAuto.Models
{
    [Table("Users")]
    public class User
    {
        [Key, Required]
        public  Guid ID { get; set; } = Guid.NewGuid();
		[Required]
		public  string Name { get; set; }
		[Required, EmailAddress]
		public  string Email { get; set; }
		[Required]
		public  string Address { get; set; }
		[Required]
		public  string PhoneNumber { get; set; }
        public EUserType AccesLevel { get; set; } = EUserType.User;
		public string? Password { get; set; }
		public bool Enabled { get; set; }
		public string? Description { get; set; }
		public ICollection<Rent>? Rents { get; set; }

		public string? RefreshToken { get; set; }
		
		public DateTime? RefreshTokenExpires { get; set; }
		public override string ToString()
		{
            return $"UserID:{ID}, Name: {Name} ({AccesLevel}) - Email: {Email}, Address: {Address}, PhoneNumber: {PhoneNumber}, Description: {Description}";
		}
	}
}
