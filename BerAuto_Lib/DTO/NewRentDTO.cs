using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BerAuto_Lib.DTO
{
    public class NewRentDTO
    {
		public string RenterID { get; set; }
		public int Owed { get; set; } = 0;
	}
}
