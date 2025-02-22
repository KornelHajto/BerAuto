using System;


namespace BerAuto.Models
{
	[Table("Cars")]
	public class Car
	{
		[Key, Required]
		public int ID { get; set; }
		[Required]
		public string PlateNumber { get; set; } //rendszám
		public string Type { get; set; } //márka + modell
		public int Odometer { get; set; } = 0; //kilóméteróra
		public bool Available { get; set; } = true; //kölcsönözhetőség
		public List<_periods> BookedDates { get; set; }
		public int? CategoryId { get; set; } //kategória
		public Category? Category { get; set; }
	}

	public class _periods
	{
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
	}
}
