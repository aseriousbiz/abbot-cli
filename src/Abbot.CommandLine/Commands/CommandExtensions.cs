using System.CommandLine;

namespace Serious.Abbot.CommandLine.Commands
{
    /// <summary>
    /// Extensions on <see cref="Command" /> that encapsulate common command line options etc.
    /// </summary>
    public static class CommandExtensions
    {
        /// <summary>
        /// Adds the directory option. This is used by several skills.
        /// </summary>
        /// <param name="command">The command to add this option to.</param>
        public static Option<string?> AddDirectoryOption(this Command command)
        {
            return command.AddOption<string?>(
                "--directory",
                "-d",
                "The path to the Abbot Workspace directory. If omitted, assumes the current directory.");
        }

        public static Option<T> AddOption<T>(this Command command, string alias, string shortForm, string description)
        {
            var option = new Option<T>(alias, description);
            option.AddAlias(shortForm);
            command.AddOption(option);
            return option;
        }
    }
}