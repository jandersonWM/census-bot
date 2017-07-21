using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//TODO: move to a data layer project
namespace Bot_Application1.DataTypes
{
    [Serializable]
    public class CensusDataTypes
    {
        public List<string> LevelsOfGeography { get; set; }
        public List<string> States { get; set; }
        public string LevelOfGeography { get; set; }
        public bool ResolveSuccess { get; set; }
        public string Message { get; set; }

        public CensusDataTypes()
        {
            LevelsOfGeography = new List<string>()
            {
                "nation", "country", "division", "region", "state", "county", "city", "census tract",
                "tract", "block", "congressional district", "cities", "counties", "bushes"
            };
            States = new List<string>()
            {
                "alabama", "alaska", "arizona", "arkansas", "california", "colorado", "connecticut",
                "delaware","florida", "georgia", "district of columbia", "washington dc", "hawaii",
                "idaho","illinois","indiana", "iowa", "kansas", "kentucky", "louisiana", "maine",
                "maryland", "massachusetts", "michigan", "minnesota", "mississippi", "missouri",
                "montana", "nebraska", "nevada", "new hampshire", "new jersey", "new mexico",
                "new york", "north carolina", "north dakota", "ohio", "oklahoma", "oregon",
                "pennsylvania", "rhode island", "south carolina", "south dakota", "tennessee",
                "texas", "utah", "vermont", "virginia", "washington", "west virginia",
                "wisconsin", "wyoming"
            };
        }
    }
}