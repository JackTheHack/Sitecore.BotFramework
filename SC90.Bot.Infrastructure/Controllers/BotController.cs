using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Autofac;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using SC90.Bot.Helpers;
using SC90.Bot.Infrastructure.Dialogs;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Resources.Media;
using Sitecore.Services.Infrastructure.Web.Http;

namespace SC90.Bot.Controllers
{
    [BotAuthentication]
    public class BotController : ServicesApiController
    {
        private static RootDialog Dialog;

        //Based on 

        private readonly string _welcomeMessage;
        private string[] _optionsStrings;
        private readonly string _options;
        private string _image;
        private ImageField _imageField;
        private string _optionsTitle;
        private Item _botItem;

        public BotController(Item botItem)
        {
            _welcomeMessage = botItem.Fields["WelcomeMessage"].Value;
            _options = botItem.Fields["Options"].Value;
            _optionsTitle = botItem.Fields["OptionsTitle"].Value;
            if (!string.IsNullOrEmpty(botItem["Image"]))
            {
                _imageField = (ImageField) botItem.Fields["Image"];
                _image = MediaManager.GetMediaUrl(_imageField.MediaItem, new MediaUrlOptions()
                {
                    AlwaysIncludeServerUrl = true
                });
            }

            _botItem = botItem;
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

            //if (Dialog == null)
            //{
            //    Dialog = new RootDialog();
            //}

            var rootDialog = new RootDialog();

            if (activity.Type == ActivityTypes.Message)
            {
                await Conversation.SendAsync(activity,
                    () =>  rootDialog);                
            }

            if (activity.Type == ActivityTypes.ConversationUpdate)
            {
                if (activity.MembersAdded != null && !string.IsNullOrEmpty(_welcomeMessage))
                {
                    using (var scope = DialogModule.BeginLifetimeScope(Conversation.Container, activity))
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
                                var connector = scope.Resolve<IConnectorClient>();                                
                                
                                //ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                                Activity reply =
                                    activity.CreateReply(_welcomeMessage);

                                if (!string.IsNullOrEmpty(_options) || !string.IsNullOrEmpty(_image))
                                {
                                    var basicCard = DialogueHelper.CreateOptionsCard(_botItem);

                                    if (!string.IsNullOrEmpty(_image))
                                    {
                                        var cardImage = new CardImage(_image);
                                        basicCard.Images = new List<CardImage> {cardImage};
                                    }                                    

                                    reply.Attachments.Add(basicCard.ToAttachment());
                                }

                                

                                await connector.Conversations.ReplyToActivityAsync(reply);

                                return response;

                            }
                        }
                    }
                }
            }

            HandleSystemMessage(activity);

            return response;
        }

        

        private async Task Resume(IDialogContext context, IAwaitable<string> result)
        {
            var text = await result;

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