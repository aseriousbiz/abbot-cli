using System.Collections.Generic;

namespace Serious.Abbot.Messages
{
    /// <summary>
    /// Body of the response when calling a skill via the skill editor or the abbot cli.
    /// </summary>
    public class SkillRunResponse
    {
        /// <summary>
        /// The set of replies returned by calling the skill. Note that this ignores attachments and other media
        /// elements in the reply such as buttons.
        /// </summary>
        public IList<string>? Replies { get; set; } = new List<string>();

        /// <summary>
        /// The set of errors returned from the skill.
        /// </summary>
        public IList<RuntimeError>? Errors { get; set; } = new List<RuntimeError>();
        
        /// <summary>
        /// The content type of the request. If null, we attempt to use content negotiation and default
        /// to application/json in the end.
        /// </summary>
        public string? ContentType { get; set; }
        
        /// <summary>
        /// The raw content of the response to send back to a webhook.
        /// </summary>
        public string? Content { get; set; }

        /// <summary>
        /// Whether or not executing the skill was a success.
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// The set of HTTP response headers to send with the response when responding to an HTTP trigger.
        /// </summary>
        public Dictionary<string, string[]>? Headers { get; set; }
    }
}