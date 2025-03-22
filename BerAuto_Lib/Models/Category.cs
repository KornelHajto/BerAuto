using System;

namespace BerAuto.Models
{
	[Table("Categories")]
    public class Category
    {
		[Key, Required]
		public required string ID { get; set; }
		[Required]
		public required string Name { get; set; }
		[Required]
		public required int DailyRate { get; set; }
		public ICollection<Car>? Cars { get; set; }

		public override string ToString()
		{
			return $"CategoryID:{ID}, Name: {Name}, DailyRate: {DailyRate} Ft";
		}
	}
}
