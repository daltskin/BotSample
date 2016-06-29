using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Http;

namespace BotSample
{
    // Only enable authentication once you've configured your bot in the Bot Framework Portal and 
    // have set the BotId, MicrosoftAppId and MicrosoftAppPassword in the Web.Config for your bot 
    // [BotAuthentication]
    public class MessagesController : ApiController
    {
        internal static IDialog<JObject> MakeRootDialog()
        {
            return Chain.From(() => FormDialog.FromForm(BotQuestions.BuildForm));
        }

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<Activity> Post([FromBody]Activity activity)
        {
            try
            {
                //ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                if (activity.Type == ActivityTypes.Message)
                {
                    await Conversation.SendAsync(activity, MakeRootDialog);
                }
                else
                {
                    HandleSystemMessage(activity);
                }
                return null;
            }
            catch (Exception exp)
            {
                Debug.WriteLine(exp);
                return null;
            }
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