using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.IO;
using System.Web;
using AdaptiveCards;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Bot_Demo.Dialogs
{
    [Serializable]
    public class MenuDialog : IDialog<string>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);           
        }
        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            try
            {
                var message = await result;
                if (message != null)
                {
                    switch (message.GetActivityType())
                    {
                        case ActivityTypes.ConversationUpdate:
                            var convActivity = context.Activity.AsConversationUpdateActivity();
                            foreach (var member in convActivity.MembersAdded ?? System.Array.Empty<ChannelAccount>())
                            {
                                // if the bot is added, then 
                                if (member.Id == message.Recipient.Id)
                                {
                                    await this.ReplyWithMenuCard(context, result);
                                    //context.Done("Say menu to see the options again");
                                }
                            }
                           
                            break;

                        case ActivityTypes.Message:                           
                            break;
                    }
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //  context.Wait(this.MessageReceivedAsync);
            }
        }
        private async Task ReplyWithMenuCard(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            var reply = ((Activity)message).CreateReply();

            AdaptiveCard card = JsonConvert.DeserializeObject<AdaptiveCard>(File.ReadAllText(HttpContext.Current.Server.MapPath("../Dialogs/menu.json")));

            reply.Attachments = new List<Attachment>();

            if (card != null)
            {
                // Create the attachment.
                Attachment attachment = new Attachment()
                {
                    ContentType = AdaptiveCard.ContentType,
                    Content = card
                };
                reply.Attachments.Add(attachment);

                // MedicineToRemind = detail.Name;
            }

            await context.PostAsync(reply);
        }
    }
}