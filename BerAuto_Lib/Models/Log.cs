using System;

namespace BerAuto.Models
{
    [Table("Logs")]
    public class Log
    {
		[Key, Required]
		public required int ID { get; set; }
		[Required]
		public required DateTime Date { get; set; } = DateTime.Now;
		[Required]
		public required string UserId { get; set; } = "System";
		[Required]
		public required ESeverity Severity { get; set; } = ESeverity.Information;
		[Required]
		public required string Message { get; set; }

		public override string ToString()
		{
			return $"Log:{ID} - {UserId} logged at {Date}, Severity: {Severity}, Message: {Message}";
		}
	}
}
