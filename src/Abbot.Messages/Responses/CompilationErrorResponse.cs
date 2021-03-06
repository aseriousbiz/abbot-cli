using System.Collections.Generic;

namespace Serious.IO.Messages
{
    /// <summary>
    /// A response when running a skill fails.
    /// </summary>
    public class RuntimeErrorResponse : ErrorResponse
    {
        /// <summary>
        /// The set of errors returned by the skill runner.
        /// </summary>
        public IList<RuntimeError> Errors { get; set; } = null!;
    }
}