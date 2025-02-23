using System;

namespace BerAuto.Models
{
    [Table("Logs")]
    public class Log
    {
		[Key, Required]
		public int ID { get; set; }
		[Required]
		public DateTime Date { get; set; } = DateTime.Now;
		[Required]
		public string UserId { get; set; } = "System";
		[Required]
		public ESeverity Severity { get; set; } = ESeverity.Information;
		[Required]
		public string Message { get; set; }

		public override string ToString()
		{
			return $"Log:{ID} - {UserId} logged at {Date}, Severity: {Severity}, Message: {Message}";
		}
	}
}
