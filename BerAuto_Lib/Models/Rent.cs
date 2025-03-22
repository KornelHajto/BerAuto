using System;

namespace BerAuto.Models
{
    [Table("Rents")]
    public class Rent
    {
        [Key]
        public required string ID { get; set; }
        public required string RenterID { get; set; }
        public User? Renter { get; set; }
        public required ERentStatus Status { get; set; }
        public DateTime ApplicationTime { get; set; } = DateTime.Now;
        public int Owed { get; set; } // fizetendő

		public override string ToString()
		{
            return $"RentID:{ID} ({Status}), ApplicationTime: {ApplicationTime}, Owed: {Owed} Ft, Renter: {Renter.ToString}";
		}
	}
}
