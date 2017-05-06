using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fahrtenbuch.Library
{
    public class SearchFilter
    {
        public DateTime StartDate { get; set; } = DateTime.MinValue;
        public DateTime EndDate { get; set; } = DateTime.MaxValue;
        public int? SeatCount { get; set; } = null;
        public string Brand { get; set; } = null;
        public string Model { get; set; } = null;
    }
}
