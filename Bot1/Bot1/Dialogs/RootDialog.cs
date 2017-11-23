using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace Bot1.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private string category;
        private string severity;
        private string description;
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            await context.PostAsync("Hi i m the bot with all the option and I can help you create a ticket");
            PromptDialog.Text(context, DescriptionMessageReceivedAsync, "First please briefly describe your problem to me");

        }

        public async Task DescriptionMessageReceivedAsync(IDialogContext context, IAwaitable<string> argument )
        {
            description = await argument;
            var severities = new string[] { "high", "normal", "low" };
            await context.PostAsync($"Got it. Your problem is \"{description}\"");
            PromptDialog.Choice(context, SeverityMessageReceivedAsync, severities, "What is the severity of this problem?");

            
        }

        public async Task SeverityMessageReceivedAsync(IDialogContext context, IAwaitable<string> argument)
        {
            severity = await argument;
            PromptDialog.Text(context, CategoryMessageReceivedAsync, "Which would be the category for this ticket (software, hardware, networking, security or other)?");
        }

        public async Task CategoryMessageReceivedAsync(IDialogContext context, IAwaitable<string> argument)
        {
            category = await argument;
            var text = $"Great! I'm going to create a \"{severity}\" severity ticket in the \"{category}\" category. " +
                        $"The description I will use is \"{description}\". Can you please confirm that this information is correct?";

            PromptDialog.Confirm(context, IssueConfirmedMessageReceivedAsync, text);
        }

        public async Task IssueConfirmedMessageReceivedAsync(IDialogContext context, IAwaitable<bool> argument)
        {
            var confirmed = await argument;

            if (confirmed)
            {
                await context.PostAsync("Awesome! Your ticked has been created.");
            }
            else
            {
                await context.PostAsync("Ok. The ticket was not created. You can start again if you want.");
            }

            context.Done<object>(null);
        }
    }
}