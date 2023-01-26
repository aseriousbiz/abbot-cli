using Serious.Abbot.Messages;

namespace Serious.IO.CommandLine
{
    public static class CodeLanguageExtensions
    {
        public static string GetFileExtension(this CodeLanguage language)
        {
            return language switch
            {
                CodeLanguage.Python => "py",
                CodeLanguage.CSharp => "csx",
                CodeLanguage.JavaScript => "js",
                _ => "txt"
            };
        }
    }
}
