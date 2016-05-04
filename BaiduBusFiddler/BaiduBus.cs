
using System;
using System.Collections.Generic;

namespace BaiduBusFiddler
{

    public class PairLine
    {
        public string direction { get; set; }
        public string endTime { get; set; }
        public int kindtype { get; set; }
        public string name { get; set; }
        public string startTime { get; set; }
        public string uid { get; set; }
    }

    public class Subway
    {
        public string background_color { get; set; }
        public string name { get; set; }
    }

    public class Station
    {
        public string geo { get; set; }
        public string name { get; set; }
        public object rt_info { get; set; }
        public object tri_rt_info { get; set; }
        public string uid { get; set; }
        public IList<Subway> subways { get; set; }
    }

    public class WorkTime
    {
        public string end { get; set; }
        public string start { get; set; }
    }

    public class Content
    {
        public string alias { get; set; }
        public string company { get; set; }
        public string endTime { get; set; }
        public string geo { get; set; }
        public int isMonTicket { get; set; }
        public int is_display { get; set; }
        public int kind { get; set; }
        public int kindtype { get; set; }
        public string line_direction { get; set; }
        public int maxPrice { get; set; }
        public string name { get; set; }
        public int nearest_station_idx { get; set; }
        public PairLine pair_line { get; set; }
        public string startTime { get; set; }
        public IList<Station> stations { get; set; }
        public int ticketPrice { get; set; }
        public string ticket_price_ext { get; set; }
        public string timetable { get; set; }
        public string timetable_ext { get; set; }
        public string uid { get; set; }
        public IList<WorkTime> workTime { get; set; }
        public IList<object> workingTimeDesc { get; set; }
    }

    public class CurrentCity
    {
        public int code { get; set; }
        public string geo { get; set; }
        public int level { get; set; }
        public string name { get; set; }
        public IList<int> sgeo { get; set; }
        public int sup { get; set; }
        public int sup_bus { get; set; }
        public int sup_business_area { get; set; }
        public int sup_lukuang { get; set; }
        public int sup_subway { get; set; }
        public int type { get; set; }
        public string up_province_name { get; set; }
    }

    public class Result
    {
        public int count { get; set; }
        public int error { get; set; }
        public int linetype { get; set; }
        public string qid { get; set; }
        public int time { get; set; }
        public int total { get; set; }
        public int type { get; set; }
        public string uii_type { get; set; }
        public string region { get; set; }
        public string uii_qt { get; set; }
        public int login_debug { get; set; }
    }

    public class BaiduBus
    {
        public IList<Content> content { get; set; }
        public CurrentCity current_city { get; set; }
        public IList<string> hot_city { get; set; }
        public Result result { get; set; }
    }

}
