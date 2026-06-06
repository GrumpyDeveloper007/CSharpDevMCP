using CSharpDevMCP.Services;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text;

namespace CSharpDevMCP.MCP;

/// <summary>
/// 
/// </summary>
[McpServerToolType]
public class LessonMCP
{
    [McpServerTool, Description(".")]
    public string GetPastLessons(string subPath)
    {
        try
        {
            var defaultTimeout = new TimeSpan(0, 60, 0);
            var ai = new AiService(StaticSettings.SettingValues.ApiKey,
              StaticSettings.SettingValues.EndPoint,
              StaticSettings.SettingValues.ClientName,
              StaticSettings.SettingValues.MaxTokens, defaultTimeout);

            var subPaths = subPath.Split(',', StringSplitOptions.RemoveEmptyEntries);

            var allFeedback = "";
            foreach (var path in subPaths)
            {
                string startDir = StaticSettings.SettingValues.PathToSolution + path;
                var dirInfo = new DirectoryInfo(startDir);
                string workingDir = dirInfo?.FullName ?? Environment.CurrentDirectory;
                var file = GitCommands.GetChangedFilesByFile(workingDir);
                foreach (var (HeaderLine, Lines) in file)
                {
                    var lessons = MDExtractor.ExtractHeadingBlocks(File.ReadAllText(StaticSettings.SettingValues.LessonsMdFilePath), [path, "General"]);
                    var codeBlock = String.Join('\n', Lines);

                    var response = ai.SendMessage($"You are a helpful concise assistant for software developers. If no relevant items are found return Nothing. Here is some information about past lessons:\n\n{string.Join("\n\n", lessons.Select(l => $"{l.Value}\n"))}\n\nProvide feedback for the following code block based only on any relevent lessions (skip positive/change summary): {codeBlock}");
                    if (response != null)
                    {
                        allFeedback += response.Content.Last().Text ?? "";
                    }
                }

                var newFile = GitCommands.GetAddedFileNames(workingDir);
                foreach (var f in newFile)
                {
                    var lessons = MDExtractor.ExtractHeadingBlocks(File.ReadAllText(StaticSettings.SettingValues.LessonsMdFilePath), [path, "General"]);
                    var codeBlock = File.ReadAllText(f);

                    var response = ai.SendMessage($"You are a helpful concise assistant for software developers. If no relevant items are found return Nothing. Here is some information about past lessons:\n\n{string.Join("\n\n", lessons.Select(l => $"{l.Value}\n"))}\n\nProvide feedback for the following code block based only on any relevant lessons (skip positive/change summary): {codeBlock}");
                    if (response != null)
                    {
                        allFeedback += response.Content.Last().Text ?? "";
                    }
                }

            }

            return allFeedback;
        }
        catch (Exception ex)
        {
            return $"Exception running git: {ex.Message}";
        }
    }

    [McpServerTool, Description(".")]
    public string GetPastLessonsBranch(string subPath)
    {
        try
        {
            var defaultTimeout = new TimeSpan(0, 60, 0);
            var ai = new AiService(StaticSettings.SettingValues.ApiKey,
              StaticSettings.SettingValues.EndPoint,
              StaticSettings.SettingValues.ClientName,
              StaticSettings.SettingValues.MaxTokens, defaultTimeout);

            var subPaths = subPath.Split(',', StringSplitOptions.RemoveEmptyEntries);

            var allFeedback = "";
            foreach (var path in subPaths)
            {
                string startDir = StaticSettings.SettingValues.PathToSolution + path;
                var dirInfo = new DirectoryInfo(startDir);
                string workingDir = dirInfo?.FullName ?? Environment.CurrentDirectory;
                var textBlock = GitCommands.GetBranchChanges(workingDir);
                var file = GitCommands.GetSplitByFile(textBlock);
                foreach (var (HeaderLine, Lines) in file)
                {
                    var lessons = MDExtractor.ExtractHeadingBlocks(File.ReadAllText(StaticSettings.SettingValues.LessonsMdFilePath), [path, "General"]);
                    var codeBlock = String.Join('\n', Lines);

                    var response = ai.SendMessage($"You are a helpful concise assistant for software developers. If no relevant items are found return Nothing. Here is some information about past lessons:\n\n{string.Join("\n\n", lessons.Select(l => $"{l.Value}\n"))}\n\nProvide feedback for the following code block based only on any relevent lessions (skip positive/change summary): {codeBlock}");
                    if (response != null)
                    {
                        allFeedback += response.Content.Last().Text ?? "";
                    }
                }
            }

            return allFeedback;
        }
        catch (Exception ex)
        {
            return $"Exception running git: {ex.Message}";
        }
    }

}