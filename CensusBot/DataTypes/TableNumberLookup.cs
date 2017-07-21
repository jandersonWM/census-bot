using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//TODO: move to a data layer project
namespace Bot_Application1.DataTypes
{
    public static class TableNumberLookup
    {
        public static List<Tuple<string, string>> Incomes {get; set; }
        static TableNumberLookup()
        {
            //keys represent the minimum income for the subject number
            //ranges could call all tuples with keys in the range
            Incomes = new List<Tuple<string, string>>
            {
                new Tuple<string, string>("all", "001E"), new Tuple<string, string>("0", "002E"),
                new Tuple<string, string>("10000", "003E"), new Tuple<string, string>("15000", "004E"),
                new Tuple<string, string>("20000", "005E"), new Tuple<string, string>("25000", "006E"),
                new Tuple<string, string>("30000", "007E"), new Tuple<string, string>("35000", "008E"),
                new Tuple<string, string>("40000", "009E"), new Tuple<string, string>("45000", "010E"),
                new Tuple<string, string>("50000", "011E"), new Tuple<string, string>("60000", "012E"),
                new Tuple<string, string>("75000", "013E"), new Tuple<string, string>("100000", "014E"),
                new Tuple<string, string>("125000", "015E"), new Tuple<string, string>("150000", "016E"),
                new Tuple<string, string>("200000", "017E")
            };
        }
    }
}