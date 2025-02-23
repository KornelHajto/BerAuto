using System;

namespace BerAuto.Models
{
    [Table("CarRents")]
    public class CarRents
    {
        [Key, Required]
        public string ID { get; set; }
        [Required]
        public string RentID { get; set; }
        public Rent? Rent { get; set; }
        [Required]
        public string CarID { get; set; }
        public Car? Car { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

		public override string ToString()
		{
            return $"ID:{ID} ({StartDate} - {EndDate}): Renting: {Rent.ToString}, Car: {Car.ToString}";
		}
	}
}
