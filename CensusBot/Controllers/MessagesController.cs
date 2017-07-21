using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Text.RegularExpressions;
using System.Web;

namespace Bot_Application1
{
    [BotAuthentication] //Validate Bot Connector credentials over https
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            //Only reply if conversation is 1:1 or bot is mentioned
            if (activity.Type == ActivityTypes.Message)
            {
                var mentions = activity.GetMentions();
                bool botMentioned = false;
                for (var i = 0; i < mentions.Length; i++)
                {
                    if (Regex.Match(mentions[i].Text, "sabrecombot", RegexOptions.IgnoreCase).Success)
                    {
                        botMentioned = true;
                    }
                }
                if (activity.ChannelId != "emulator" && (bool)activity.Conversation.IsGroup && !botMentioned)
                {
                    return null;
                }
                await Conversation.SendAsync(activity, () => new Dialogs.CensusDialog());
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
                if (message.MembersAdded != null)
                {
                    ConnectorClient connector = new ConnectorClient(new Uri(message.ServiceUrl));
                    
                    if (message.MembersAdded[0].Name != "Bot" && message.MembersAdded[0].Name != "sabrecombot")
                    {
                        Activity welcomeMessage = message.CreateReply($"Welcome {message.MembersAdded[0].Name}!");
                        connector.Conversations.SendToConversation(welcomeMessage);
                    }                 
                }
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing that the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }
            return null;
        }
    }
}