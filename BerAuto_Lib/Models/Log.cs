using System;

namespace BerAuto.Models
{
    [Table("Logs")]
    public class Log
    {
		[Key]
		public required int ID { get; set; }
		public required DateTime Date { get; set; } = DateTime.Now;
		public required string UserId { get; set; } = "System";
		public required ESeverity Severity { get; set; } = ESeverity.Information;
		public required string Message { get; set; }

		public override string ToString()
		{
			return $"Log:{ID} - {UserId} logged at {Date}, Severity: {Severity}, Message: {Message}";
		}
	}
}
