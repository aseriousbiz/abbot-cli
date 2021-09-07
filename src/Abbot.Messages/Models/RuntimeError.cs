using Serious.IO.Messages.Models;

namespace Serious.IO.Messages
{
    /// <summary>
    /// Represents a runtime error when running a user skill in a skill runner.
    /// </summary>
    public class RuntimeError : CompilationError
    {
        /// <summary>
        /// The stack trace of the runtime error.
        /// </summary>
        public string? StackTrace { get; set; }
    }
}