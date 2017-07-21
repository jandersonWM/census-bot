using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//TODO: move to a data layer project
namespace Bot_Application1.DataTypes
{
    [Serializable]
    public static class DataSubjects
    {
        public static List<Tuple<string, string>> Topics { get; set; }
        public static List<Tuple<string,string>> Races { get; set; }
        public static List<Tuple<string, string>> RaceSupplements { get; set; }

        static DataSubjects()
        {
            Topics = new List<Tuple<string, string>>()
            {
                new Tuple<string, string>("age; sex", "01"), new Tuple<string, string>("race", "02"),
                new Tuple<string, string>("hispanic or latino origin", "03"), new Tuple<string, string>("ancestry", "04"),
                new Tuple<string, string>("citizenship status; year of entry; foregin born place of birth", "05"),
                new Tuple<string, string>("place of birth", "06"),
                new Tuple<string, string>("migration/residence 1 year ago", "07"), new Tuple<string, string>("commuting (journey to work); place of work", "08"),
                new Tuple<string, string>("relationship to householder", "09"), new Tuple<string, string>("grandparents and grandchildren characteristics", "10"),
                new Tuple<string, string>("school enrollment", "14"), new Tuple<string, string>("household type; family type", "11"),
                new Tuple<string, string>("language spoken at home", "16"), new Tuple<string, string>("marital status; marital history", "12"),
                new Tuple<string, string>("education attainment; undergraduate field of degree", "15"), new Tuple<string, string>("fertility", "13"),
                new Tuple<string, string>("poverty status", "17"), new Tuple<string, string>("disability status", "18"),
                new Tuple<string, string>("income", "19"), new Tuple<string, string>("earnings", "20"),
                new Tuple<string, string>("employment status", "23"), new Tuple<string, string>("veteran status", "21"),
                new Tuple<string, string>("industry, occupation and class of worker", "24"), new Tuple<string, string>("food stamps/supplemental nutrition assistance program (SNAP)", "22"),
                new Tuple<string, string>("housing characteristics", "25"), new Tuple<string, string>("group quarters", "26"),
                new Tuple<string, string>("health insurance coverage", "27"), new Tuple<string, string>("computer and internet use", "28"),
                new Tuple<string, string>("quality measures", "98")
            };
            Races = new List<Tuple<string, string>>()
            {
                new Tuple<string,string>("hispanics or latinos", "03001_003E"), 
                new Tuple<string,string>("biracial or multiracial", "02001_008E"), new Tuple<string,string>("islanders", "02001_006E"),
                new Tuple<string,string>("asians", "02001_005E"), new Tuple<string,string>("native americans or american indians", "02001_004E"),
                new Tuple<string,string>("blacks or african americans", "02001_003E"), new Tuple<string,string>("whites", "02001_002E"),
            };
            RaceSupplements = new List<Tuple<string, string>>()
            {
                new Tuple<string,string>("hispanic or latino", "I"), new Tuple<string,string>("caucasian", "H"),
                new Tuple<string,string>("biracial or multiracial", "G"), new Tuple<string,string>("islander", "E"),
                new Tuple<string,string>("asian", "D"), new Tuple<string,string>("native american or american indian", "C"),
                new Tuple<string,string>("black or african american", "B"), new Tuple<string,string>("white", "A"),
            };
        }

        //Source of logic: https://people.cs.pitt.edu/~kirk/cs1501/Pruhs/Spring2006/assignments/editdistance/Levenshtein%20Distance.htm
        public static int ComputeLevDistance(string source, string target)
        {
            int n = source.Length;
            int m = target.Length;
            int[,] d = new int[n + 1, m + 1];

            //shortcut to save time - if one is empty the difference is obviously the length of the other
            if (n == 0) { return m; }
            if (m == 0) { return n; }

            //fill array with 2 vectors - one of source length and one of target length
            for (int i = 0; i <= n; d[i, 0] = i++) { }
            for (int j = 0; j <= m; d[0, j] = j++) { }

            //loop over the source
            for (int i = 1; i <= n; i++)
            {
                //loop through each letter in target to compare with source
                for (int j = 1; j <= m; j++)
                {
                    //calculate cost, either 0 (letters match) or 1 (they don't match)
                    int cost = (target[j - 1] == source[i - 1]) ? 0 : 1;

                    //
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }

        public static string FindBestMatch(string source, List<string> targets)
        {
            int bestMatch = source.Length; string bestMatchStr = "";
            for (var i = 0; i < targets.Count(); i++)
            {
                //Compute how similar the entity is to the matched race
                int thisMatch = ComputeLevDistance(source, targets[i]);
                //if thismatch is a better match, set the bestmatch str and int values
                if (bestMatch > thisMatch) { bestMatch = thisMatch; bestMatchStr = targets[i]; }
                //If the distance is 0, the strings are identical and we've found our best match
                if (bestMatch == 0) break;
            }
            return bestMatchStr;
        }

        public static List<string> FindAllMatches(string location, List<string> targets, int? difference = 2)
        {
            List<string> returnMatches = new List<string>();
            foreach (var target in targets)
            {
                IEnumerable<string> substrings;
                string longestSub = string.Empty;
                if (target.Length <= location.Length)
                {
                    substrings = getSubstrings(target);
                    foreach (string sub in substrings)
                    {
                        if (location.Contains(sub)) longestSub = sub; break;
                    }
                }
                else
                {
                    substrings = getSubstrings(location);
                    foreach (string sub in substrings)
                    {
                        if (target.Contains(sub)) { longestSub = sub; break; }
                    }
                }
                if (ComputeLevDistance(location, longestSub) < difference) returnMatches.Add(target);
            }
            return returnMatches;
        }

        public static IEnumerable<string> getSubstrings(string source)
        {
            return from firstChar in Enumerable.Range(0, source.Length)
                   from secondChar in Enumerable.Range(0, source.Length - firstChar + 1)
                   where secondChar > 0
                   let myString = source.Substring(firstChar, secondChar)
                   orderby myString.Length descending
                   select source.Substring(firstChar, secondChar);
        }
    }
}