using Bot_Application1.DataTypes;
using Bot_Application1.NLP;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bot_Application1.Dialogs
{
    [LuisModel("932c2326-c205-4c00-b494-248462fff2d9", "6ba5d24908f24b84b2f57c5cc27e9a9d")]
    [Serializable]
    public class CensusDialog : LuisDialog<object>
    {
        protected CensusDataTypes _dataTypes = new CensusDataTypes();

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = "Sorry, I don't understand";

            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("People")]
        public async Task People(IDialogContext context, LuisResult result)
        {
            string message = "";
            var conversation = context.Activity as Activity;
            Activity replyToConversation = conversation.CreateReply(message);
            replyToConversation.Recipient = conversation.From;
            replyToConversation.Type = "message";
            var entities = new List<EntityRecommendation>(result.Entities);

            await context.PostAsync(replyToConversation);
            context.Wait(MessageReceived);
        }

        [LuisIntent("Location")]
        public async Task Location(IDialogContext context, LuisResult result)
        {
            string message = ""; string levelOfGeography = "";
            Tuple<string, string> subjectTuple;
            var conversation = context.Activity as Activity;
            Activity replyToConversation = conversation.CreateReply(message);
            replyToConversation.Type = "message";

            var entities = new List<EntityRecommendation>(result.Entities);
            int resultCount = EntityHandler.GetResultCount(entities);
            subjectTuple =  EntityHandler.DetermineSubject(entities);
            string fullSubjectStr = subjectTuple.Item2;
            if (fullSubjectStr == "B")
            {
                replyToConversation.Text = "Sorry, unable to determine what subject you're asking about";
                await context.PostAsync(replyToConversation);
                context.Wait(MessageReceived); return;
            }
            string queryTopic = DataSubjects.Topics.First(t => t.Item2 == fullSubjectStr.Substring(1, 2)).Item1;
            QueryObject _queryObj = new QueryObject(fullSubjectStr);

            if (entities.Any((entity) => entity.Type == "level_of_geography"))
            {
                levelOfGeography = entities.Where(e => e.Type == "level_of_geography")
                    .First().Entity;
                Parser.ParseLevelOfGeography(levelOfGeography, _queryObj, _dataTypes);
                if (!_dataTypes.ResolveSuccess)
                {
                    replyToConversation.Text = $"{_dataTypes.Message}: '{levelOfGeography}'";
                    await context.PostAsync(replyToConversation);
                    context.Wait(MessageReceived);
                    return;
                }
            }
            if (entities.Any((entity) => entity.Type == "location"))
            {
                string locationEntity = entities.Where(e => e.Type == "location")
                    .First().Entity.ToLower();
                LevelsOfGeography lg = Parser.ParseLocation(locationEntity, _queryObj);
                if (lg.ResolveSuccess) _queryObj.Location = $"{_queryObj.LocationLevel}:{lg.LocationCode}";
                else
                {
                    replyToConversation.Text = $"{lg.Message}: '{locationEntity}'";
                    await context.PostAsync(replyToConversation);
                    context.Wait(MessageReceived);
                    return;
                }
            }

            await context.PostAsync(replyToConversation);
            string apiResponse = await _queryObj.QueryWeb();
            var resultObjects = Parser.ParseToObjects(apiResponse, _queryObj);
            List<JObject> newObjects = new List<JObject>();
            try
            {
                if (entities.Any(e => e.Type == "ascending"))
                {
                    newObjects = resultObjects
                        .OrderBy(x => Convert.ToInt32(x[fullSubjectStr])).Take(resultCount).ToList();
                    for (var i = 0; i < newObjects.Count(); i++)
                    {
                        if (i == 0) {
                            replyToConversation.Text = $"{newObjects[i]["NAME"].ToString().Replace("  ", "%").Split('%')[0]} has the least {subjectTuple.Item1} with {newObjects[i][fullSubjectStr]}.  \r\n";
                        } else if (i == newObjects.Count() - 1) {
                            replyToConversation.Text += $"Finally, {newObjects[i]["NAME"].ToString().Replace("  ","%").Split('%')[0]} has {subjectTuple.Item1} with {newObjects[i][fullSubjectStr]}. ";
                        } else
                        {
                            //randomize conjunctions to make more natural syntax
                            replyToConversation.Text += $"{PartsOfSpeech.GetConjunctiveAdverb()} {newObjects[i]["NAME"].ToString().Replace("  ", "%").Split('%')[0]} with {newObjects[i][fullSubjectStr]}.  \r\n";
                        }
                    }
                }
                else if (entities.Any(e => e.Type == "descending"))
                {
                    newObjects = resultObjects
                        .OrderByDescending(x => Convert.ToInt32(x[fullSubjectStr])).Take(resultCount).ToList();
                    for (var i = 0; i < newObjects.Count(); i++)
                    {
                        if (i == 0)
                        {
                            replyToConversation.Text = $"{newObjects[i]["NAME"].ToString().Replace("  ", "%").Split('%')[0]} has the most {subjectTuple.Item1} with {newObjects[i][fullSubjectStr]}.  \r\n";
                        }
                        else if (i == newObjects.Count() - 1)
                        {
                            replyToConversation.Text += $"Finally, {newObjects[i]["NAME"].ToString().Replace("  ", "%").Split('%')[0]} has {newObjects[i][fullSubjectStr]}.  ";
                        }
                        else
                        {
                            //randomize conjunctions to make more natural syntax
                            replyToConversation.Text += $"{PartsOfSpeech.GetConjunctiveAdverb()} {newObjects[i]["NAME"].ToString().Replace("  ", "%").Split('%')[0]} with {newObjects[i][fullSubjectStr]}.  \r\n";
                        }
                    }
                }
                var json = JsonConvert.SerializeObject(newObjects);
                //replyToConversation.Text = json;
            }
            catch (Exception e)
            {
                replyToConversation.Text = "Sorry, I ran into an error trying to serialize my response";
                Console.WriteLine(e.Message);
            }
            
            await context.PostAsync(replyToConversation);
            context.Wait(MessageReceived);
        }
    }
}