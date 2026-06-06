using CSharpDevMCP.Services;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text;

namespace CSharpDevMCP.MCP;

/// <summary>
/// Provides MCP tools for interacting with Git, such as getting pending changes.
/// </summary>
[McpServerToolType]
public class GitToolMCP
{
    [McpServerTool, Description("GetPendingChanges")]
    public string GetPendingChanges(string subPath)
    {
        try
        {
            var subPaths = subPath.Split(',', StringSplitOptions.RemoveEmptyEntries);
            var result = new StringBuilder();

            foreach (var path in subPaths)
            {
                string startDir = StaticSettings.SettingValues.PathToSolution + path;
                var dirInfo = new DirectoryInfo(startDir);
                string workingDir = dirInfo?.FullName ?? Environment.CurrentDirectory;

                var stdout = GitCommands.GetChangedFiles(workingDir);
                var sb = new StringBuilder();
                GitCommands.GetNewFiles(workingDir, sb);

                result.AppendLine(stdout + "\r\n" + sb.ToString());
            }
            return string.IsNullOrEmpty(result.ToString()) ? "No changes\r\n" : result.ToString();
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
            var subPaths = subPath.Split(',', StringSplitOptions.RemoveEmptyEntries);
            var result = new StringBuilder();

            foreach (var path in subPaths)
            {
                string startDir = StaticSettings.SettingValues.PathToSolution + path;
                var dirInfo = new DirectoryInfo(startDir);
                string workingDir = dirInfo?.FullName ?? Environment.CurrentDirectory;

                var stdout = GitCommands.GetBranchChanges(workingDir);
                var sb = new StringBuilder();
                GitCommands.GetNewFiles(workingDir, sb);

                result.AppendLine(stdout + "\r\n" + sb.ToString());
            }

            return string.IsNullOrEmpty(result.ToString()) ? "No changes\r\n" : result.ToString();
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
