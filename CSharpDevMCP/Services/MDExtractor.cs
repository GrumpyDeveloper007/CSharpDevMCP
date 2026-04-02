using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CSharpDevMCP.Services
{
    internal class MDExtractor
    {
        /// <summary>
        /// Extracts text blocks for specified markdown headings.
        /// Each block includes content from the heading until the next heading of equal or higher level.
        /// </summary>
        /// <param name="markdownContent">The markdown content to extract from</param>
        /// <param name="headingNames">List of heading names to extract</param>
        /// <returns>Dictionary with heading names as keys and their content blocks as values</returns>
        public static Dictionary<string, string> ExtractHeadingBlocks(string markdownContent, List<string> headingNames)
        {
            if (string.IsNullOrEmpty(markdownContent) || headingNames == null || headingNames.Count == 0)
            {
                return new Dictionary<string, string>();
            }

            var result = new Dictionary<string, string>();
            var lines = markdownContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            foreach (var headingName in headingNames)
            {
                var block = ExtractSingleHeadingBlock(lines, headingName);
                if (!string.IsNullOrEmpty(block))
                {
                    result[headingName] = block;
                }
            }

            return result;
        }

        /// <summary>
        /// Extracts a single heading block and its content.
        /// </summary>
        private static string ExtractSingleHeadingBlock(string[] lines, string headingName)
        {
            // Find the line with the matching heading
            int startIndex = -1;
            int headingLevel = 0;

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                
                // Check if this line is a heading that matches our heading name
                if (IsHeadingLine(line, headingName, out int level))
                {
                    startIndex = i;
                    headingLevel = level;
                    break;
                }
            }

            // If heading not found, return empty string
            if (startIndex == -1)
            {
                return string.Empty;
            }

            // Find the end of the block (next heading of equal or higher level)
            int endIndex = lines.Length;

            for (int i = startIndex + 1; i < lines.Length; i++)
            {
                var line = lines[i];
                
                if (GetHeadingLevel(line) > 0)
                {
                    int currentLevel = GetHeadingLevel(line);
                    
                    // Stop if we find a heading at the same or higher level (lower number = higher level)
                    if (currentLevel <= headingLevel)
                    {
                        endIndex = i;
                        break;
                    }
                }
            }

            // Extract and join the lines
            var contentLines = lines.Skip(startIndex).Take(endIndex - startIndex).ToArray();
            return string.Join(Environment.NewLine, contentLines);
        }

        /// <summary>
        /// Checks if a line is a heading that matches the given heading name.
        /// </summary>
        private static bool IsHeadingLine(string line, string headingName, out int level)
        {
            level = GetHeadingLevel(line);
            
            if (level == 0)
            {
                return false;
            }

            // Extract heading text without the markdown symbols
            var headingText = line.Substring(level).Trim();
            
            // Compare case-insensitively
            return headingText.Equals(headingName, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets the heading level (number of # symbols) from a line.
        /// Returns 0 if the line is not a heading.
        /// </summary>
        private static int GetHeadingLevel(string line)
        {
            if (string.IsNullOrEmpty(line) || !line.StartsWith("#"))
            {
                return 0;
            }

            int level = 0;
            foreach (var character in line)
            {
                if (character == '#')
                {
                    level++;
                }
                else if (character == ' ')
                {
                    // Heading level symbols must be followed by a space
                    break;
                }
                else
                {
                    // Invalid heading format
                    return 0;
                }
            }

            return level;
        }
    }
}
