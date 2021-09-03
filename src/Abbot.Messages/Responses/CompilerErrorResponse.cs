using System.Collections.Generic;
using Serious.IO.Messages.Models;

namespace Serious.IO.Messages
{
    /// <summary>
    /// A response when compiling a skill fails.
    /// </summary>
    public class CompilerErrorResponse : ErrorResponse
    {
        /// <summary>
        /// The set of errors returned by the skill runner.
        /// </summary>
        public IList<CompilationError> Errors { get; set; } = null!;
    }
}