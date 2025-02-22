using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BerAuto.Models
{
    public class ApiResponse
    {
		public int StatusCode { get; set; } = 0;
		public string? Message { get; set; }
		public DateTime Date { get; set; } = DateTime.Now;
		public object? Data { get; set; }
	}
}
