using ModelContextProtocol.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace CSharpDevMCP.MCP
{
    [McpServerToolType]
    public class GitTool
    {
        [McpServerTool, Description("")]
        public string RunTest()
        {
            return "test";
        }

        [McpServerTool, Description("")]
        public string GetPendingChanges()
        {
            try
            {
                string startDir = StaticSettings.SettingValues.PathToSolution;
                var dirInfo = new DirectoryInfo(startDir);

                string workingDir = dirInfo?.FullName ?? Environment.CurrentDirectory;

                var psi = new ProcessStartInfo("git", "--no-pager diff .")
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = workingDir
                };

                using var proc = Process.Start(psi);
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
            catch (Exception ex)
            {
                return $"Exception running git: {ex.Message}";
            }
        }
    }
}