using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
//TODO: move to a data layer project
namespace Bot_Application1.DataTypes
{
    [Serializable]
    public class QueryObject
    {
        public string BaseUri { get; set; }
        //Level of geography that the user is asking about i.e. "What is the biggest city?"
        public string QueryLevel { get; set; }
        //Level of geography the user wants to constrain the search to i.e. "Population in Iowa"
        public string LocationLevel { get; set; }
        public string Location { get; set; }
        public string Income { get; set; }
        public string Race { get; set; }


        public QueryObject(string subjectVar)
        {
            BaseUri = "http://api.census.gov/data/2015/acs5?get=NAME," + subjectVar;
        }
        public QueryObject(string year, string dataset)
        {
            //BaseUri = $"http://api.census.gov/data/{year}/{dataset}?get=";
        }

        public async Task<string> QueryWeb()
        {
            string queryStr = "";
            if (Location != "") queryStr = $"{BaseUri}&in={Location}&for={QueryLevel}";
            else queryStr = $"{BaseUri}&for={QueryLevel}";
            var client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(queryStr);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}