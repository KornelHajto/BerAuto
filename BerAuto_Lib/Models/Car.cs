using System;


namespace BerAuto.Models
{
	[Table("Cars")]
	public class Car
	{
		[Key]
		public required string ID { get; set; }
		public required string PlateNumber { get; set; } //rendszám
		public required string Type { get; set; } //márka + modell
		public required int Odometer { get; set; } = 0; //kilóméteróra
		public required bool Available { get; set; } = true; //kölcsönözhetőség
		public string? CategoryId { get; set; } //kategória
		public Category? Category { get; set; }
		public string? Description { get; set; }
		public ICollection<CarRent>? CarRents { get; set; }

		public override string ToString()
		{
			return $"CarID:{ID}, PlateNumber: {PlateNumber}, Type: {Type}, Odometer: {Odometer} Km, Available: {Available}, Category: {Category.ToString()}, Description: {Description}";
		}
	}

}
