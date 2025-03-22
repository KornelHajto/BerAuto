using System;

namespace BerAuto.Models
{
    [Table("CarRents")]
    public class CarRent
    {
        [Key, Required]
        public required string ID { get; set; }
		[Required]
		public required string RentID { get; set; }
        public Rent? Rent { get; set; }
		[Required]
		public required string CarID { get; set; }
        public Car? Car { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

		public override string ToString()
		{
            return $"ID:{ID} ({StartDate} - {EndDate}): Renting: {Rent.ToString}, Car: {Car.ToString}";
		}
	}
}
