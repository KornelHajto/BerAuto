using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BerAuto.DTO
{
    public class CarViewDTO
    {
		public Guid ID { get; set; } 
		public string PlateNumber { get; set; } //rendszám
		public string Type { get; set; } //márka + modell
		public int Odometer { get; set; } = 0; //kilóméteróra
		public bool Available { get; set; } = true; //kölcsönözhetőség
		public string? Description { get; set; }
		public string CategoryName { get; set; }
		public int DailyRate { get; set; }
	}
}
