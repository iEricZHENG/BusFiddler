using System;
using System.Collections.Generic;

namespace QQBusFiddler
{

    public class Info
    {
        public int type { get; set; }
        public int error { get; set; }
        public string request_id { get; set; }
        public int time { get; set; }
    }

    public class Gonggao
    {
    }

    public class Xpinfo
    {
        public int src { get; set; }
        public string svid { get; set; }
        public double x { get; set; }
        public double y { get; set; }
    }

    public class Station
    {
        public int is_updown { get; set; }
        public int isxp { get; set; }
        public string name { get; set; }
        public string pointx { get; set; }
        public string pointy { get; set; }
        public int poitype { get; set; }
        public string trans_lines { get; set; }
        public string uid { get; set; }
        public Xpinfo xpinfo { get; set; }
    }

    public class Poi
    {
        public int ccode { get; set; }
        public string dist { get; set; }
        public string etime { get; set; }
        public string from { get; set; }
        public Gonggao gonggao { get; set; }
        public string name { get; set; }
        public int poilinetype { get; set; }
        public string points { get; set; }
        public int poitype { get; set; }
        public string price { get; set; }
        public string price_info { get; set; }
        public string reverse { get; set; }
        public int snum { get; set; }
        public IList<Station> stations { get; set; }
        public string stime { get; set; }
        public string to { get; set; }
        public string uid { get; set; }
    }

    public class Detail
    {
        public Poi poi { get; set; }
    }

    public class QQBus
    {
        public Info info { get; set; }
        public Detail detail { get; set; }
    }

}
