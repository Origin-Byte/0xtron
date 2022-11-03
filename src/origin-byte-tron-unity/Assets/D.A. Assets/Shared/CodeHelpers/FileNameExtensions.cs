using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DA_Assets.Shared.CodeHelpers
{
    public static class FileNameExtensions
    {
        private static char[] invalidFileNameChars = new char[] { '“', '”', '"', '^', '<', '>', ';', '|', '/', ',', '\\', ':', '=', '?', '\"', '*', '\'' };
        public static string GetInvalidFileNameChars(this string filename)
        {
            List<char> invalidChars = new List<char>();

            foreach (char c in filename)
            {
                if (invalidFileNameChars.Contains(c))
                {
                    invalidChars.Add(c);
                }
            }

            string result = "";

            if (invalidChars.Count() > 0)
            {
                result = string.Join(" ", invalidChars);
            }

            return result;
        }
        public static string ReplaceInvalidFileNameChars(this string fileName, char newChar = '_')
        {
            string newName = "";

            for (int i = 0; i < fileName.Length; i++)
            {
                if (invalidFileNameChars.Contains(fileName[i]))
                {
                    newName += newChar;
                }
                else
                {
                    newName += fileName[i];
                }
            }

            newName = Regex.Replace(newName, @"\t|\n|\r", newChar.ToString());
            return newName;
        }
    }
}