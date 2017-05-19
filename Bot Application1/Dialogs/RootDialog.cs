using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Text.RegularExpressions;
using Microsoft.Cognitive.LUIS;

namespace Bot_Application1.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            //determine intent of message
            
            // calculate something for us to return
            int length = (activity.Text ?? string.Empty).Length;
            Match m = Regex.Match(activity.Text, @"pizza", RegexOptions.IgnoreCase);
            bool pizza = m.Success;
            //build reply
            ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
            var replyMessage = activity.CreateReply();
            if (pizza)
            {
                replyMessage.Text = $"So {activity.From.Name}, you like pizza?";
                replyMessage.Attachments.Add(new Attachment() {
                    ContentUrl = @"https://sabrecombot.azurewebsites.net/Images/pizza.png",
                    ContentType = "image/jpg"
                    //Name = "Yummy_Pizza.jpg"
                });
            } else if (activity.Entities.Count != 0) {
                var mentions = activity.GetMentions();
                var something = mentions[0].Text ?? "no mentions"; //This just gives the text of the mention i.e. @sabrecombot
                
                replyMessage.Text = $"Mentioned: {mentions[0].Text} Mentions: {mentions.Length} Is Group: {activity.Conversation.IsGroup}";
            }
            else
            {
                replyMessage.Text = $"Hello {activity.From.Name}, you sent {activity.Text} which was {length} characters";
            }
            // return our reply to the user
            await context.PostAsync(replyMessage);

            context.Wait(MessageReceivedAsync);
        }
    }
}