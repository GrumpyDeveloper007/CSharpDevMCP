using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpDevMCP.Services;

internal static class GitCommands
{
    public static List<(string HeaderLine, List<string> Lines)> GetChangedFilesByFile(string workingDir)
    {
        List<(string HeaderLine, List<string> Lines)> result = [];
        var textBlock = GetChangedFiles(workingDir);
        var lines = textBlock.Split('\n');
        var headerLine = "";

        List<string> currentFile = [];
        foreach (var line in lines)
        {
            if (line.StartsWith("index")) continue;

            if (line.StartsWith("diff "))
            {
                if (currentFile.Count > 0)
                {
                    result.Add((headerLine, currentFile));
                }
                // new file
                headerLine = line;
                currentFile = [];
                continue;
            }

            currentFile.Add(line);
        }

        return result;
    }

    public static List<(string HeaderLine, List<string> Lines)> GetSplitByFile(string textBlock)
    {
        List<(string HeaderLine, List<string> Lines)> result = [];
        var lines = textBlock.Split('\n');
        var headerLine = "";

        List<string> currentFile = [];
        foreach (var line in lines)
        {
            if (line.StartsWith("index")) continue;

            if (line.StartsWith("diff "))
            {
                if (currentFile.Count > 0)
                {
                    result.Add((headerLine, currentFile));
                }
                // new file
                headerLine = line;
                currentFile = [];
                continue;
            }

            currentFile.Add(line);
        }

        return result;
    }

    public static string GetChangedFiles(string workingDir)
    {
        var psi = new ProcessStartInfo("git", "--no-pager diff .")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = workingDir,
            RedirectStandardInput = true,
        };

        var proc = Process.Start(psi);
        if (proc == null)
        {
            return $"Failed to start git process in '{workingDir}'";
        }

        string stdout = proc.StandardOutput.ReadToEnd();
        string stderr = proc.StandardError.ReadToEnd();

        proc.WaitForExit();

        if (!string.IsNullOrEmpty(stderr))
        {
            return $"GIT ERROR:\n{stderr}\n{stdout}";
        }

        return string.IsNullOrEmpty(stdout) ? "No changes" : stdout;
    }

    public static string GetBranchChanges(string workingDir)
    {
        string filterUnitTests = "\":(exclude)**/*Test.cs\" \":(exclude)**/*Tests.cs\" \":(exclude)**/*Tests/*.cs\" \":(exclude)**/*.UnitTests.cs\"  \":(exclude)**/*.IntegrationTests.cs\"";
        var psi = new ProcessStartInfo("git", "--no-pager diff main...HEAD " + filterUnitTests)
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = workingDir,
            RedirectStandardInput = true,
        };

        var proc = Process.Start(psi);
        if (proc == null)
        {
            return $"Failed to start git process in '{workingDir}'";
        }

        string stdout = proc.StandardOutput.ReadToEnd();
        string stderr = proc.StandardError.ReadToEnd();

        proc.WaitForExit();

        if (!string.IsNullOrEmpty(stderr))
        {
            return $"GIT ERROR:\n{stderr}\n{stdout}";
        }

        return string.IsNullOrEmpty(stdout) ? "No changes" : stdout;
    }

    public static string GetGitFiles(string workingDir)
    {
        // Run git status --porcelain to get changed files
        var statusPsi = new ProcessStartInfo("git", "status -s")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardInput = true,
            WorkingDirectory = workingDir
        };

        using var statusProc = Process.Start(statusPsi);
        if (statusProc == null)
        {
            return "Failed to start git status process in '{workingDir}'";
        }

        string statusOut = statusProc.StandardOutput.ReadToEnd();
        string statusErr = statusProc.StandardError.ReadToEnd();
        statusProc.WaitForExit();

        if (!string.IsNullOrEmpty(statusErr))
        {
            return $"GIT STATUS ERROR: {statusErr}";
        }

        if (string.IsNullOrWhiteSpace(statusOut))
        {
            return "";
        }
        return statusOut;
    }


    private static (bool, string?) GetGitStatus(string workingDir)
    {
        StringBuilder sb = new();
        // Run git status --porcelain to get changed files
        var statusPsi = new ProcessStartInfo("git", "status --porcelain -uall")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardInput = true,
            WorkingDirectory = workingDir
        };

        using var statusProc = Process.Start(statusPsi);
        if (statusProc == null)
        {
            sb.AppendLine($"Failed to start git status process in '{workingDir}'");
            return (false, null);
        }

        string statusOut = statusProc.StandardOutput.ReadToEnd();
        string statusErr = statusProc.StandardError.ReadToEnd();
        statusProc.WaitForExit();

        if (!string.IsNullOrEmpty(statusErr))
        {
            sb.AppendLine("GIT STATUS ERROR:");
            sb.AppendLine(statusErr);
        }

        if (string.IsNullOrWhiteSpace(statusOut))
        {
            sb.AppendLine("No changed files");
            return (false, null);
        }

        return (true, statusOut);
    }

    public static List<string> GetAddedFileNames(string workingDir)
    {
        var (result, statusOut) = GetGitStatus(workingDir);
        if (!result || statusOut == null) return [];

        var files = new List<string>();
        var lines = statusOut.Split(['\n', '\r'], StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            if (line.StartsWith(" M")) continue;
            // porcelain format: XY <path> or XY <from> -> <to>
            var pathPart = line.Length > 3 ? line.Substring(3).Trim() : line.Trim();
            // if rename, take destination
            if (pathPart.Contains("->"))
            {
                var parts = pathPart.Split(["->"], StringSplitOptions.None);
                pathPart = parts.Last().Trim();
            }

            var filePath = Path.Combine(workingDir, pathPart.Replace('/', Path.DirectorySeparatorChar).Replace("\"", ""));
            if (File.Exists(filePath))
            {
                files.Add(filePath);
            }
        }
        return files;
    }

    public static void GetNewFiles(string workingDir, StringBuilder sb)
    {
        var (result, statusOut) = GetGitStatus(workingDir);
        if (!result || statusOut == null)
        {
            return;
        }

        var lines = statusOut.Split(['\n', '\r'], StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            if (line.StartsWith(" M")) continue;
            // porcelain format: XY <path> or XY <from> -> <to>
            var pathPart = line.Length > 3 ? line.Substring(3).Trim() : line.Trim();
            // if rename, take destination
            if (pathPart.Contains("->"))
            {
                var parts = pathPart.Split(["->"], StringSplitOptions.None);
                pathPart = parts.Last().Trim();
            }

            if (line.StartsWith(" D"))
            {
                sb.AppendLine($"file deleted {pathPart}");
                continue;
            }

            sb.AppendLine($"New file added {pathPart}");

            try
            {
                var filePath = Path.Combine(workingDir, pathPart.Replace('/', Path.DirectorySeparatorChar).Replace("\"", ""));
                if (File.Exists(filePath))
                {
                    string content = File.ReadAllText(filePath);
                    sb.AppendLine(content);
                }
                else
                {
                    sb.AppendLine($"File not found: {pathPart}");
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine($"Could not read file {pathPart}: {ex.Message}");
            }
        }

    }
}
