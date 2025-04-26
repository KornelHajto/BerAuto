using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BerAuto_Lib.DTO
{
    public class UserDataDTO
    {
        public string UserID { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string Address { get; set; }
		public string PhoneNumber { get; set; }
		public string? Description { get; set; }
		public bool Override { get; set; } = false; //if true, the user can override the existing/added data, if false, the user cannot override the data
		//password is left out, it is required to add at registration
	}
}
