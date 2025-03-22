using System;

namespace BerAuto.Models
{
    [Table("Rents")]
    public class Rent
    {
        [Key, Required]
        public  Guid ID { get; set; } = Guid.NewGuid();
		[Required]
		public  Guid RenterID { get; set; }
        public User? Renter { get; set; }
		[Required]
		public  ERentStatus Status { get; set; }
        public DateTime ApplicationTime { get; set; } = DateTime.Now;
        public int Owed { get; set; } // fizetendő

		public override string ToString()
		{
            return $"RentID:{ID} ({Status}), ApplicationTime: {ApplicationTime}, Owed: {Owed} Ft, Renter: {Renter.ToString}";
		}
	}
}
