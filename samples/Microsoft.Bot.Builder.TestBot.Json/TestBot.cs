﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Input;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Recognizers;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Rules;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Steps;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Builder.Dialogs.Debugging;
using Microsoft.Bot.Builder.Dialogs.Declarative;
using Microsoft.Bot.Builder.Dialogs.Declarative.Resources;
using Microsoft.Bot.Builder.Expressions.Parser;
using Microsoft.Bot.Schema;
using static Microsoft.Bot.Builder.Dialogs.Debugging.Source;

namespace Microsoft.Bot.Builder.TestBot.Json
{
    public class TestBot : ActivityHandler
    {
        private IStatePropertyAccessor<DialogState> dialogStateAccessor;
        private AdaptiveDialog rootDialog;
        private readonly ResourceExplorer resourceExplorer;

        public TestBot(ConversationState conversationState, ResourceExplorer resourceExplorer)
        {
            this.dialogStateAccessor = conversationState.CreateProperty<DialogState>("RootDialogState");
            this.resourceExplorer = resourceExplorer;

            // auto reload dialogs when file changes
            this.resourceExplorer.Changed += (paths) =>
            {
                if (paths.Any(p => Path.GetExtension(p) == ".dialog"))
                {
                    Task.Run(() => this.LoadDialogs());
                }
            };

            LoadDialogs();
        }


        private void LoadDialogs()
        {
            System.Diagnostics.Trace.TraceInformation("Loading resources...");

            //this.rootDialog = new AdaptiveDialog()
            //{
            //    AutoEndDialog = false,
            //    Steps = new List<IDialog>()
            //};
            //var textInput = new TextInput();
            //textInput.Prompt = new ActivityTemplate("");
            //textInput.Property = "dialog.word";

            //var remoteCall = new RemoteCall();
            //remoteCall.Url = "https://bfcalendarskill.azurewebsites.net/api/skill/messages";

            //this.rootDialog.Steps.Add(remoteCall);
            //this.rootDialog.Steps.Add(textInput);
            //this.rootDialog.Steps.Add(new RepeatDialog());
            var rootDialogFile = "RemoteCall.main.dialog";
            var rootFile = resourceExplorer.GetResource(rootDialogFile);

            rootDialog = DeclarativeTypeLoader.Load<AdaptiveDialog>(rootFile, resourceExplorer, DebugSupport.SourceRegistry);

            System.Diagnostics.Trace.TraceInformation("Done loading resources.");
        }

        protected override Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            return rootDialog.OnTurnAsync((ITurnContext)turnContext, null, cancellationToken);
        }

        protected async override Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await rootDialog.OnTurnAsync(turnContext, null, cancellationToken).ConfigureAwait(false);
        }
    }
}
