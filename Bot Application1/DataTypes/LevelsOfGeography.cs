using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bot_Application1.DataTypes
{
    [Serializable]
    public class LevelsOfGeography
    {
        public List<KeyValuePair<string, string>> States { get; set; }
        public bool ResolveSuccess { get; set; }
        public string Message { get; set; }

        public string LocationCode { get; set; }

        public LevelsOfGeography()
        {
            States = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string,string>("alabama", "01" ), new KeyValuePair<string,string>("alaska", "02" ),
                new KeyValuePair<string,string>("arizona", "04" ), new KeyValuePair<string,string>("arkansas", "05" ),
                new KeyValuePair<string,string>("california", "06" ), new KeyValuePair<string,string>("colorado", "08" ),
                new KeyValuePair<string,string>("connecticut", "09" ), new KeyValuePair<string,string>("delaware", "10" ),
                new KeyValuePair<string,string>("district of columbia", "11" ), new KeyValuePair<string,string>("louisiana", "22" ),
                new KeyValuePair<string,string>("georgia", "13" ), new KeyValuePair<string,string>("georgia", "13" ),
                new KeyValuePair<string,string>("hawaii", "15" ), new KeyValuePair<string,string>("idaho", "16" ),
                new KeyValuePair<string,string>("illinois", "17" ), new KeyValuePair<string,string>("indiana", "18" ),
                new KeyValuePair<string,string>("iowa", "19" ), new KeyValuePair<string,string>("kansas", "20" ),
                new KeyValuePair<string,string>("kentucky", "21" ), new KeyValuePair<string,string>("maine", "23" ),
                new KeyValuePair<string,string>("maryland", "24" ), new KeyValuePair<string,string>("michigan", "26" ),
                new KeyValuePair<string,string>("massachusetts", "25" ), new KeyValuePair<string,string>("mississippi", "28" ),
                new KeyValuePair<string,string>("minnesota", "27" ), new KeyValuePair<string,string>("montana", "30" ),
                new KeyValuePair<string,string>("missouri", "29" ), new KeyValuePair<string,string>("nevada", "32" ),
                new KeyValuePair<string,string>("nebraska", "31" ), new KeyValuePair<string,string>("new hampshire", "33"),
                new KeyValuePair<string,string>("new jersey", "34" ), 
                new KeyValuePair<string,string>("new mexico", "35" ), new KeyValuePair<string,string>("new york", "36" ),
                new KeyValuePair<string,string>("north carolina", "37" ), new KeyValuePair<string,string>("north dakota", "38" ),
                new KeyValuePair<string,string>("ohio", "39" ), new KeyValuePair<string,string>("oklahoma", "40" ),
                new KeyValuePair<string,string>("oregon", "41" ), new KeyValuePair<string,string>("pennsylvania", "42" ),
                new KeyValuePair<string,string>("rhode island", "44" ),
                new KeyValuePair<string,string>("south carolina", "45" ), new KeyValuePair<string,string>("south dakota", "46" ),
                new KeyValuePair<string,string>("tennessee", "47" ), new KeyValuePair<string,string>("texas", "48" ),
                new KeyValuePair<string,string>("texas", "49" ), new KeyValuePair<string,string>("utah", "50" ),
                new KeyValuePair<string,string>("vermont", "50" ), new KeyValuePair<string,string>("virginia", "51" ),
                new KeyValuePair<string,string>("washington", "53" ), new KeyValuePair<string,string>("west virginia", "54" ),
                new KeyValuePair<string,string>("wisconsin", "55" ), new KeyValuePair<string,string>("wyoming", "56" )
            };
        }
    }
}