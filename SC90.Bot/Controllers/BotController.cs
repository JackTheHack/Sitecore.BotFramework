using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using SC90.Bot.Dialogs;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Services.Infrastructure.Web.Http;

namespace Sitecore.ChatBot
{
    [BotAuthentication]
    public class BotController : ServicesApiController
    {
        //Based on 

        private Item _botItem;
        private string _startDialogId;
        private string _welcomeMessage;

        public BotController(Item botItem)
        {            
            _botItem = botItem;
            _startDialogId = _botItem.Fields["StartDialog"].Value;
            _welcomeMessage = _botItem.Fields["WelcomeMessage"].Value;
        }

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        [HttpPost]
        [AcceptVerbs("POST")]
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {            
            var response = Request.CreateResponse(HttpStatusCode.OK);
            
            if (activity.Type == ActivityTypes.Message)
            {
                await Conversation.SendAsync(activity,
                    () => new RootDialog());

                return response;
            }

            if (activity.Type == ActivityTypes.ConversationUpdate)
            {
                if (activity.MembersAdded != null && !string.IsNullOrEmpty(_welcomeMessage))
                {
                    // Iterate over all new members added to the conversation
                    foreach (var member in activity.MembersAdded)
                    {
                        // Greet anyone that was not the target (recipient) of this message
                        // the 'bot' is the recipient for events from the channel,
                        // turnContext.Activity.MembersAdded == turnContext.Activity.Recipient.Id indicates the
                        // bot was added to the conversation.
                        if (member.Id != activity.Recipient.Id)
                        {                            
                            ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));                                                                
                            Activity reply = activity.CreateReply(_welcomeMessage);
                            await connector.Conversations.ReplyToActivityAsync(reply);
                            Log.Info("Welcome message to user", this);

                        }
                    }
                }
            }

            HandleSystemMessage(activity);

            return response;
        }

        [HttpGet]
        public IHttpActionResult Status()
        {
            return Json(new {Text = "Hello"});
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
                if (message.MembersAdded.Count > 0)
                {
                    //new user in the channel
                }
                
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}