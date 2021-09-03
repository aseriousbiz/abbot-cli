using System;
using System.CommandLine.IO;

namespace Serious.IO
{
    /// <summary>
    /// Extends <see cref="SystemConsole" /> with methods to read from the standard input stream.
    /// </summary>
    public class ExtendedSystemConsole : SystemConsole, IExtendedConsole
    {
        /// <summary>
        /// Clears the console buffer and corresponding console window of display information.
        /// </summary>
        public void Clear() => Console.Clear();

        /// <summary>
        /// Reads the next line of characters from the standard input stream.
        /// </summary>
        public string? ReadLine() => Console.ReadLine();

        /// <summary>
        /// Reads the next key from the input stream.
        /// </summary>
        public int Read() => Console.Read();

        /// <summary>
        /// Obtains the next character or function key pressed by the user.
        /// The pressed key is displayed in the console window.
        /// </summary>
        public ConsoleKeyInfo ReadKey() => Console.ReadKey();
        
        /// <summary>
        /// Obtains the next character or function key pressed by the user.
        /// The pressed key is optionally displayed in the console window.
        /// </summary>
        public ConsoleKeyInfo ReadKey(bool intercept) => Console.ReadKey(intercept);
    }
}