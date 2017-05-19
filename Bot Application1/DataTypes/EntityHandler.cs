using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bot_Application1.DataTypes
{
    public static class EntityHandler
    {
        public static int GetResultCount(List<EntityRecommendation> entities)
        {
            string resultCountStr; int resultCount = 1;
            try { resultCountStr = entities.First(x => x.Type == "count").Entity; }
            catch { resultCountStr = "1"; }
            try
            {
                resultCount = Convert.ToInt32(resultCountStr);
            }
            catch
            {
                resultCount = Parser.ToLong(resultCountStr);
            }
            return resultCount;
        }

        public static Tuple<string, string> DetermineSubject(List<EntityRecommendation> entities)
        {
            string messageSubject = "";
            string fullSubjectStr = "B";
            foreach (var entity in entities)
            {
                if (DataSubjects.Topics.Any(x => x.Item1.Contains(entity.Type.ToLower())))
                {
                    messageSubject = entity.Entity.ToString();
                    //if our first subject entity is race, build the race parameter for query
                    if (entity.Type.ToLower() == "race" && fullSubjectStr == "B")
                    {
                        //Determine which race the entity is referring to from list of races
                        var matches = DataSubjects.Races.Where(x => x.Item1.Contains(entity.Entity.ToLower()))
                            .Select(e => e.Item1).ToList();
                        string bestMatchStr = "";
                        if (matches.Count > 1) bestMatchStr = StringMatcher.StringMatcher.FindBestMatch(entity.Entity.ToLower(), matches);
                        else { bestMatchStr = entity.Entity.ToLower(); }
                        //using the best matched race, grab that races corresponding lookup key
                        fullSubjectStr += DataSubjects.Races.First(x => x.Item1.Contains(bestMatchStr)).Item2;
                        Tuple<string, string> goodReturnTuple = new Tuple<string, string>(messageSubject, fullSubjectStr);
                        return goodReturnTuple;
                    }
                    else if (entity.Type.ToLower() == "income")
                    {
                        if (entities.Any(e => e.Type == "descending" || e.Entity.Contains(entity.Entity)))
                        {
                            fullSubjectStr += TableNumberLookup.Incomes.First(i => i.Item1 == "200000").Item2;
                        }
                    }
                    else //if main subject isn't race but race was asked about, use the supplemental letter for that race
                    {
                        fullSubjectStr += DataSubjects.Topics
                            .First(x => x.Item1.Contains(entity.Type)).Item2;
                        fullSubjectStr += "001";
                        if (entities.Any(e => e.Type == "race"))
                        {
                            fullSubjectStr += DataSubjects.RaceSupplements
                                .First(x => x.Item1.Contains(entities.First(e => e.Type == "race").Entity.ToLower()))
                                .Item2;
                        }
                        fullSubjectStr += "_001E";
                        Tuple<string, string> goodReturnTuple = new Tuple<string, string>(messageSubject, fullSubjectStr);
                        return goodReturnTuple;
                    }
                } else
                {
                    messageSubject = "Unable to determine subject of message";
                }
            }
            Tuple<string, string> returnTuple = new Tuple<string, string>(messageSubject, fullSubjectStr);
            return returnTuple;
        }
    }
}