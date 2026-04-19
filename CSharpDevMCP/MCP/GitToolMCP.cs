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
    public class GitToolMCP
    {
        [McpServerTool, Description("A test endpoint")]
        public string RunTest()
        {
            return "It works!";
        }

        [McpServerTool, Description("GetPendingChanges")]
        public string GetPendingChanges(string subPath)
        {
            try
            {
                string startDir = StaticSettings.SettingValues.PathToSolution + subPath;
                var dirInfo = new DirectoryInfo(startDir);

                string workingDir = dirInfo?.FullName ?? Environment.CurrentDirectory;

                var stdout = GitCommands.GetChangedFiles(workingDir);

                var sb = new StringBuilder();
                GitCommands.GetNewFiles(workingDir, sb);

                return string.IsNullOrEmpty(stdout + sb.ToString()) ? "No changes\r\n" : $"{stdout}\r\n{sb}";
            }
            catch (Exception ex)
            {
                return $"Exception running git: {ex.Message}";
            }
        }

        [McpServerTool, Description("GetBranchChanges")]
        public string GetBranchChanges(string subPath)
        {
            try
            {
                string startDir = StaticSettings.SettingValues.PathToSolution + subPath;
                var dirInfo = new DirectoryInfo(startDir);

                string workingDir = dirInfo?.FullName ?? Environment.CurrentDirectory;

                var stdout = GitCommands.GetBranchChanges(workingDir);

                var sb = new StringBuilder();
                GitCommands.GetNewFiles(workingDir, sb);

                return string.IsNullOrEmpty(stdout + sb.ToString()) ? "No changes\r\n" : $"{stdout}\r\n{sb}";
            }
            catch (Exception ex)
            {
                return $"Exception running git: {ex.Message}";
            }
        }

        [McpServerTool, Description("GetPastLessons")]
        public string GetPastLessons(string subPath)
        {
            try
            {
                var lessons = MDExtractor.ExtractHeadingBlocks(File.ReadAllText(StaticSettings.SettingValues.LessonsMdFilePath), [subPath, "General"]);

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



        private static void WriteLog(string message)
        {
            string logPath = StaticSettings.SettingValues.PathToSolution + @"\log.txt";
            try
            {
                System.IO.File.AppendAllText(logPath, $"{DateTime.Now}: {message}\r\n");
            }
            catch
            {
                // Ignore logging errors
            }
        }

    }
}