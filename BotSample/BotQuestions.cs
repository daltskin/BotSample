using Microsoft.ApplicationInsights;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace BotSample
{
    [Serializable]
    public class BotQuestions
    {
        public static IForm<JObject> BuildForm()
         {
            OnCompletionAsyncDelegate<JObject> processResult = async (context, state) =>
            {
                bool showSource = false;
                var responses = new Dictionary<string, string>();

                // Iterate the responses and do what you like here
                foreach (JProperty item in (JToken)state)
                {
                    responses.Add(item.Name, item.Value.ToString());

                    // Here we're only interested in the final answer to determine response
                    if (item.Name.Equals("Question10"))
                    {
                        // Display the repo url only if requested
                        if ((bool)item.Value == true)
                        {
                            showSource = true;
                        }
                    }
                }

                var msg = context.MakeMessage();
                if (showSource)
                {
                    string url = ConfigurationManager.AppSettings["repo_url"];
                    msg.Text = "Great, hope you find this sample code useful: " + url;
                }
                else
                {
                    msg.Text = "Enjoy building your bot!";
                }

                await context.PostAsync(msg);

                // Wang in your own telemetry here
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackEvent("BotSample", responses);
            };

            // The Questions.json file is an embedded resource that we can simply read with a stream
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("BotSample.Questions.json"))
            {
                var schema = JObject.Parse(new StreamReader(stream).ReadToEnd());

                // The FormBuilder will manage where we are in the form flow and ask each subsequent question as they get answered
                var form = new FormBuilderJson(schema)
                    // This is a bot, so let's be friendly    
                    .Message(new PromptAttribute("Hey party people, I'm a (MS Bot Framework) Sample Bot - here to help you build your own me."))

                     //Questions not yet answered
                    .AddRemainingFields()
                    .Message("Thanks for sticking with me, processing responses..")

                     //Callback once user has finished all the questions so we can process the result
                    .OnCompletion(processResult)
                    .Build();

                return form;
            }
        }
    }
}
