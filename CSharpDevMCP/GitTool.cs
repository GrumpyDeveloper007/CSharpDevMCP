using Microsoft.Extensions.AI;
using ModelContextProtocol.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpDevMCP
{
    [McpServerToolType]
    public class TimeTool
    {
        [McpServerTool, Description("Gets the current time.")]
        public string GetCurrentTime()
        {
            return DateTime.Now.ToString("HH:mm:ss");
        }
    }

    //[McpServerTool(Name = "SummarizeContentFromUrl"), Description("Summarizes content downloaded from a specific URI")]
    //    public static async Task<string> SummarizeDownloadedContent(
    //McpServer thisServer,
    //HttpClient httpClient,
    //[Description("The url from which to download the content to summarize")] string url,
    //CancellationToken cancellationToken)
    //    {
    //        string content = await httpClient.GetStringAsync(url);

    //        ChatMessage[] messages =
    //        [
    //            new(ChatRole.User, "Briefly summarize the following downloaded content:"),
    //    new(ChatRole.User, content),
    //];

    //        ChatOptions options = new()
    //        {
    //            MaxOutputTokens = 256,
    //            Temperature = 0.3f,
    //        };

    //        return $"Summary: {await thisServer.AsSamplingChatClient().GetResponseAsync(messages, options, cancellationToken)}";
    //    }
    //}


    [McpServerPromptType]
    public static class MyPrompts
    {
        [McpServerPrompt, Description("Creates a prompt to summarize the provided message.")]
        public static ChatMessage Summarize([Description("The content to summarize")] string content) =>
            new(ChatRole.User, $"Please summarize this content into a single sentence: {content}");
    }
}



