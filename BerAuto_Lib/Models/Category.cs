using System;

namespace BerAuto.Models
{
	[Table("Categories")]
    public class Category
    {
		[Key, Required]
		public  Guid ID { get; set; } = Guid.NewGuid();
		[Required]
		public  string Name { get; set; }
		[Required]
		public  int DailyRate { get; set; }
		public ICollection<Car>? Cars { get; set; }

		public override string ToString()
		{
			return $"CategoryID:{ID}, Name: {Name}, DailyRate: {DailyRate} Ft";
		}
	}
}
