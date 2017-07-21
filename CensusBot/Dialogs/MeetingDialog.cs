using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bot_Application1.Dialogs
{
    [LuisModel("3da7f526-2f81-4f9e-9321-efa5dd7a4eb1", "6ba5d24908f24b84b2f57c5cc27e9a9d")]
    [Serializable]
    public class MeetingDialog : LuisDialog<object>
    {
        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            bool existsMeeting = false;
            bool scheduleInProcess = context.PrivateConversationData.TryGetValue<bool>("schedulingMeeting", out existsMeeting);
            string message = "Sorry, I don't understand";
            var conversation = context.Activity as Activity;
            Activity replyToConversation = conversation.CreateReply(message);
            replyToConversation.Recipient = conversation.From;
            replyToConversation.Type = "message";
            if (scheduleInProcess)
            {
                if (result.Query.ToLower() == "stop")
                {
                    ClearMeetingData(context);
                    message = "Meeting scheduler canceled";
                }
                else
                {
                    var fillOutAttempt = FillOutMeeting(context, result);
                    message = fillOutAttempt.Item1;
                    if (fillOutAttempt.Item2)
                    {
                        AttachSignInCard(replyToConversation);
                    }
                }
            }
            await context.PostAsync(replyToConversation);
            context.Wait(MessageReceived);
        }

        [LuisIntent("check calendar")]
        public async Task CheckCalendar(IDialogContext context, LuisResult result)
        {
            string message = $"Calendar checker coming";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        //Schedule the meeting with some generic account, add user to the meeting
        [LuisIntent("schedule meeting")]
        public async Task ScheduleMeeting(IDialogContext context, LuisResult result)
        {
            bool existsMeeting = false;
            string message = "";
            bool scheduleInProcess = context.PrivateConversationData.TryGetValue<bool>("schedulingMeeting", out existsMeeting);
            if (!scheduleInProcess)
            {
                message = $"You want to **schedule a meeting.**\n\n";
            }
            context.PrivateConversationData.SetValue<bool>("schedulingMeeting", true);
            var fillOutAttempt = FillOutMeeting(context, result);
            message += fillOutAttempt.Item1;

            var conversation = context.Activity as Activity;
            Activity replyToConversation = conversation.CreateReply(message);
            replyToConversation.Recipient = conversation.From;
            replyToConversation.Type = "message";

            if (fillOutAttempt.Item2)
            {
                AttachSignInCard(replyToConversation);
            }
            
            await context.PostAsync(replyToConversation);
            context.Wait(MessageReceived);

        }
        [LuisIntent("extend meeting")]
        public async Task AlterMeeting(IDialogContext context, LuisResult result)
        {
            string message = $"Meeting changer coming";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }
        [LuisIntent("delete meeting")]
        public async Task CancelMeeting(IDialogContext context, LuisResult result)
        {
            string message = $"Meeting deleter coming";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        //This method is used to pull relevant scheduling information from the message
        //And set those values to the conversation data to be sent to the relevant API
        //It also builds the prompt for any missing necessary data
        public Tuple<string, bool> FillOutMeeting(IDialogContext context, LuisResult result)
        {
            var message = "";
            var isFilledOut = false;
            TimeSpan resolvedTime;
            DateTime resolvedDate;
            List<string> contacts = new List<string>();
            string locationEntity;
            var entities = new List<EntityRecommendation>(result.Entities);
            //Check to see if user mentioned any dates
            if (entities.Any((entity) => entity.Type == "builtin.datetime.date"))
            {
                var dateEntity = entities.Where((entity) => entity.Type == "builtin.datetime.date").First();
                var resStr = dateEntity.Resolution.First().Value ?? null;
                if (!string.IsNullOrEmpty(resStr))
                {
                    resolvedDate = DateTime.MinValue;
                    DateTime.TryParse(resStr, out resolvedDate);
                    if (resolvedDate != DateTime.MinValue)
                    {
                        context.PrivateConversationData.SetValue("meetingDate", resolvedDate.ToShortDateString());
                    }
                }
            }
            //Check to see if user mentioned any times
            if (entities.Any((entity) => entity.Type == "builtin.datetime.time"))
            {
                var timeEntity = entities.Where((entity) => entity.Type == "builtin.datetime.time").First();
                var resStr = timeEntity.Resolution["time"] ?? null;
                if (!string.IsNullOrEmpty(resStr))
                {
                    resolvedTime = TimeSpan.FromHours(Convert.ToDouble(resStr.Remove(0, 1)));
                    if (resolvedTime != TimeSpan.MinValue)
                    {
                        context.PrivateConversationData.SetValue("meetingTime", resolvedTime.ToString(@"hh\:mm"));
                    }
                }
            }
            if (entities.Any((entity) => entity.Type == "Contact"))
            {
                var contactEntities = entities.Where((entity) => entity.Type == "Contact");
                foreach (var contact in contactEntities)
                {
                    if (!string.IsNullOrEmpty(contact.Entity))
                    {
                        contacts.Add(contact.Entity);
                    }
                }
                context.PrivateConversationData.SetValue<List<string>>("meetingContacts", contacts);
            }
            if (entities.Any((entity) => entity.Type == "location"))
            {
                locationEntity = entities.Where((entity) => entity.Type == "location").First().Entity;

                context.PrivateConversationData.SetValue<string>("meetingLocation", locationEntity);
            }
            if (!context.PrivateConversationData.TryGetValue("meetingTime", out resolvedTime)
                && !context.PrivateConversationData.TryGetValue("meetingDate", out resolvedDate))
            {
                message += $"When do you want the meeting to take place?";
            } else if (!context.PrivateConversationData.TryGetValue("meetingDate", out resolvedDate))
            {
                message += $"What day do you want the meeting at {context.PrivateConversationData.Get<string>("meetingTime")}?";
            } else if (!context.PrivateConversationData.TryGetValue("meetingTime", out resolvedTime))
            {
                message += $"What time do you want the meeting on {context.PrivateConversationData.Get<string>("meetingDate")}?";
            } else if (!context.PrivateConversationData.TryGetValue("meetingLocation", out locationEntity))
            {
                message += $"Where do you want the meeting to take place?";
            }
            else
            {
                //TODO: call API and actually create meeting
                
                message += $"The meeting is on " +
                    $"**{context.PrivateConversationData.Get<string>("meetingDate")}** at " + 
                    $"**{context.PrivateConversationData.Get<string>("meetingTime")}** in " +
                    $"**{context.PrivateConversationData.Get<string>("meetingLocation")}**";

                //Clear out the conversation data values related to the meeting
                ClearMeetingData(context);
                isFilledOut = true;
            }
            return Tuple.Create(message, isFilledOut);
        }

        public void ClearMeetingData(IDialogContext context)
        {
            context.PrivateConversationData.RemoveValue("schedulingMeeting");
            context.PrivateConversationData.RemoveValue("meetingTime");
            context.PrivateConversationData.RemoveValue("meetingDate");
            context.PrivateConversationData.RemoveValue("meetingContacts");
            context.PrivateConversationData.RemoveValue("meetingLocation");
        }

        public void AttachSignInCard(Activity replyToConversation)
        {
            replyToConversation.Attachments = new List<Attachment>();

            List<CardAction> cards = new List<CardAction>();
            CardAction signIn = new CardAction()
            {
                Value = "https://sms.mysabre.com",
                Type = "signin",
                Title = "Connect"
            };
            cards.Add(signIn);
            SigninCard signInCard = new SigninCard(text: "Authorization required", buttons: cards);
            Attachment signInAttachment = signInCard.ToAttachment();
            replyToConversation.Attachments.Add(signInAttachment);
        }
    }
}