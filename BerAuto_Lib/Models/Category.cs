using System;

namespace BerAuto.Models
{
	[Table("Categories")]
    public class Category
    {
		[Key, Required]
		public string ID { get; set; }
		[Required]
		public string Name { get; set; }
		[Required]
		public int DailyRate { get; set; }

		public override string ToString()
		{
			return $"CategoryID:{ID}, Name: {Name}, DailyRate: {DailyRate} Ft";
		}
	}
}
