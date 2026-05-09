using CSharpDevMCP.Services;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text;

namespace CSharpDevMCP.MCP
{
    /// <summary>
    /// Provides MCP tools for interacting with Git, such as getting pending changes.
    /// </summary>
    [McpServerToolType]
    public class MemoryMCP
    {
        [McpServerTool, Description("Use this to get past lessons based on a query")]
        public string GetPastLessons(string subPath, string query)
        {
            try
            {
                var defaultTimeout = new TimeSpan(0, 60, 0);
                var ai = new AiService(StaticSettings.SettingValues.ApiKey,
                    StaticSettings.SettingValues.EndPoint,
                    StaticSettings.SettingValues.ClientName,
                    StaticSettings.SettingValues.MaxTokens, defaultTimeout);

                var lessons = MDExtractor.ExtractHeadingBlocks(File.ReadAllText(StaticSettings.SettingValues.LessonsMdFilePath), [subPath, "General"]);

                var response = ai.SendMessage($"You are a helpful assistant for software developers. Here is some information about past lessons:\n\n{string.Join("\n\n", lessons.Select(l => $"{l.Value}\n"))}\n\nAnswer the following question based on the above lessons: {query}");

                var sb = new StringBuilder();
                foreach (var lesson in lessons)
                {
                    sb.AppendLine($"Lesson: {lesson.Key}");
                    sb.AppendLine(lesson.Value);
                    sb.AppendLine();
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                return $"Exception running git: {ex.Message}";
            }
        }
    }
}