using System;


namespace BerAuto.Models
{
	[Table("Cars")]
	public class Car
	{
		[Key, Required]
		public Guid ID { get; set; } = Guid.NewGuid();
		[Required]
		public  string PlateNumber { get; set; } //rendszám
		[Required]
		public  string Type { get; set; } //márka + modell
		[Required]
		public  int Odometer { get; set; } = 0; //kilóméteróra
		[Required]
		public  bool Available { get; set; } = true; //kölcsönözhetőség
		public Guid? CategoryId { get; set; } //kategória
		public Category? Category { get; set; }
		public string? Description { get; set; }
		public ICollection<CarRent>? CarRents { get; set; }

		public override string ToString()
		{
			return $"CarID:{ID}, PlateNumber: {PlateNumber}, Type: {Type}, Odometer: {Odometer} Km, Available: {Available}, Category: {Category.ToString()}, Description: {Description}";
		}
	}

}
