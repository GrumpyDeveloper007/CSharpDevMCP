// Ignore Spelling: SQL CONNECTIONSTRING Api Uri Groq

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpDevMCP
{
    internal class SettingValues
    {
        public string PathToSolution { get; set; } = "";
        public string LessonsMdFilePath { get; set; } = "";
        public string ApiKey { get; set; } = "";
        public int MaxTokens { get; set; } = 10000;
        public string EndPoint { get; set; } = "";
        public string ClientName { get; set; } = "";
    }
}
