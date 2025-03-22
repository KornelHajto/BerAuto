using System;

namespace BerAuto.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        public required string ID { get; set; }
        public required string Name { get; set; }
		[EmailAddress]
		public required string Email { get; set; }
		public required string Address { get; set; }
		public required string PhoneNumber { get; set; }
        public EUserType AccesLevel { get; set; } = EUserType.User;
		public string? Password { get; set; }
		public bool Enabled { get; set; }
		public string? Description { get; set; }
		public ICollection<Rent>? Rents { get; set; }

		public override string ToString()
		{
            return $"UserID:{ID}, Name: {Name} ({AccesLevel}) - Email: {Email}, Address: {Address}, PhoneNumber: {PhoneNumber}, Description: {Description}";
		}
	}
}
