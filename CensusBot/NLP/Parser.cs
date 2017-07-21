using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Bot_Application1.DataTypes
{
    public static class Parser
    {
        private static Dictionary<string, long> numberTable =
            new Dictionary<string, long>
                {{"zero",0},{"one",1},{"two",2},{"three",3},{"four",4},
                {"five",5},{"six",6},{"seven",7},{"eight",8},{"nine",9},
                {"ten",10},{"eleven",11},{"twelve",12},{"thirteen",13},
                {"fourteen",14},{"fifteen",15},{"sixteen",16},
                {"seventeen",17},{"eighteen",18},{"nineteen",19},{"twenty",20},
                {"thirty",30},{"forty",40},{"fifty",50},{"sixty",60},
                {"seventy",70},{"eighty",80},{"ninety",90},{"hundred",100},
                {"thousand",1000},{"million",1000000},{"billion",1000000000},
                {"trillion",1000000000000},{"quadrillion",1000000000000000},
                {"quintillion",1000000000000000000}};
        public static int ToLong(string numberString)
        {
            var numbers = Regex.Matches(numberString, @"\w+").Cast<Match>()
                 .Select(m => m.Value.ToLowerInvariant())
                 .Where(v => numberTable.ContainsKey(v))
                 .Select(v => numberTable[v]);
            long acc = 0, total = 0L;
            foreach (var n in numbers)
            {
                if (n >= 1000)
                {
                    total += (acc * n);
                    acc = 0;
                }
                else if (n >= 100)
                {
                    acc *= n;
                }
                else acc += n;
            }
            return Convert.ToInt32((total + acc) * (numberString.StartsWith("minus",
                  StringComparison.InvariantCultureIgnoreCase) ? -1 : 1));
        }
        public static List<JObject> ParseToObjects(string response, QueryObject queryObj)
        {
            string[] separator = new string[] { "\n" };
            var lines = response.Replace("[[\"", "").Replace("[\"", "")
                .Replace("\\", "").Replace("\"", "").Replace("]", "")
                .Replace("city,", "").Replace("CDP,", "").Replace("town,", "")
                .Replace("village,", "").Replace("borough,", "").Replace("),", ")")
                .Split(separator, StringSplitOptions.None).ToList();
            var props = lines[0].TrimEnd(',').Split(',').ToList();
            try { props.Remove(queryObj.LocationLevel); props.Remove(queryObj.QueryLevel); }
            catch (Exception e) { Console.WriteLine(e.Message); }
            lines.RemoveAt(0);
            List<JObject> myObjects = new List<JObject>();
            for (var n = 0; n < lines.Count; n++)
            {
                var myLineVals = lines[n].Split(',').ToList();
                if (!myLineVals[0].Contains("CDP"))
                {
                    JObject tempObj = new JObject();
                    for (var i = 0; i < props.Count; i++)
                    {
                        tempObj[props[i]] = myLineVals[i];
                    }
                    myObjects.Add(tempObj);
                }
            }
            return myObjects;
        }

        public static void ParseLevelOfGeography(string level, QueryObject queryObj, CensusDataTypes dataTypes)
        {
            if (dataTypes.LevelsOfGeography.Contains(level))
            {
                dataTypes.ResolveSuccess = true;
                if (level == "city" || level == "cities") queryObj.QueryLevel = "place";
            }
            else
            {
                foreach (var place in dataTypes.LevelsOfGeography)
                {
                    dataTypes.Message += $"{place} -- ";
                }
                dataTypes.Message = "Unable to determine requested level of geography";
                dataTypes.ResolveSuccess = false;
            }
        }

        public static LevelsOfGeography ParseLocation(string location, QueryObject queryObj)
        {
            LevelsOfGeography levels = new LevelsOfGeography();
            location = location.Replace("the", "");
            var stateMatches = StringMatcher.StringMatcher.FindAllMatches(location, levels.States.Select(s => s.Key).ToList());
            if (stateMatches != null)
            {
                queryObj.LocationLevel = "state";
                foreach (var state in stateMatches)
                {
                    levels.LocationCode += $"{levels.States.First(s => s.Key == state).Value},";
                }
                levels.LocationCode = levels.LocationCode.TrimEnd(',');
                levels.ResolveSuccess = true;
            } //else ifs for cities, blocks, etc...
            else
            {
                levels.Message = "Unable to determine location";
                levels.ResolveSuccess = false;
            }

            return levels;
        }
    }
}