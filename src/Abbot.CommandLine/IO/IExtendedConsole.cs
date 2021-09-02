using System;
using System.CommandLine;

namespace Serious.Abbot.CommandLine.IO
{
    /// <summary>
    /// Adds Read methods to <see cref="IConsole"/>.
    /// </summary>
    public interface IExtendedConsole : IConsole
    {
        /// <summary>
        /// Clears the console buffer and corresponding console window of display information.
        /// </summary>
        void Clear();
        
        /// <summary>
        /// Reads the next line of characters from the standard input stream.
        /// </summary>
        string? ReadLine();

        /// <summary>
        /// Reads the next key from the input stream.
        /// </summary>
        int Read();

        /// <summary>
        /// Obtains the next character or function key pressed by the user.
        /// The pressed key is displayed in the console window.
        /// </summary>
        ConsoleKeyInfo ReadKey();
        
        /// <summary>
        /// Obtains the next character or function key pressed by the user.
        /// The pressed key is optionally displayed in the console window.
        /// </summary>
        ConsoleKeyInfo ReadKey(bool intercept);
    }
}