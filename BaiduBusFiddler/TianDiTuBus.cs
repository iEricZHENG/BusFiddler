using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TianDiTuBusFiddler
{
    public class Station
    {
        public string lonlat { get; set; }
        public string name { get; set; }
        public string uuid { get; set; }
    }

    public class TianDiTuBus
    {
        public string endtime { get; set; }
        public int linetype { get; set; }
        public string starttime { get; set; }
        public int interval { get; set; }
        public int totalprice { get; set; }
        public IList<Station> station { get; set; }
        public int increasedprice { get; set; }
        public string linepoint { get; set; }
        public int ismonticket { get; set; }
        public int stationnum { get; set; }
        public int ticketcal { get; set; }
        public string byuuid { get; set; }
        public string linename { get; set; }
        public int ismanual { get; set; }
        public int increasedstep { get; set; }
        public string company { get; set; }
        public int isbidirectional { get; set; }
        public int length { get; set; }
        public int totaltime { get; set; }
        public int startprice { get; set; }
    }
}
