using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BerAuto.DTO
{
    public class CategoryViewDTO
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public int DailyRate { get; set; }
    }
}
