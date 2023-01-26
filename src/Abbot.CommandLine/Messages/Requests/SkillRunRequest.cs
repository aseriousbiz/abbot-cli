namespace Serious.Abbot.Messages;

/// <summary>
/// Body of the request by a client to run a skill. This is used by the the skill editor or abbot cli
/// to run skill code. It's also used by the signal handler. It allows testing current
/// changes to code without having to save the code.
/// </summary>
public class SkillRunRequest
{
    /// <summary>
    /// The current skill name.
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// The skill arguments to pass.
    /// </summary>
    public string Arguments { get; set; } = null!;

    /// <summary>
    /// The code to run
    /// </summary>
    public string Code { get; set; } = null!;

    /// <summary>
    /// A reference that allows the skill runner to reply to an attached chat room. This is a serialized
    /// Microsoft.Bot.Schema.ConversationReference.
    /// </summary>
    public string? ConversationReference { get; set; }
}
