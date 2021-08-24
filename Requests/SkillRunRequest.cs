namespace Serious.Abbot.Messages
{
    /// <summary>
    /// Body of the request by the skill editor or abbot cli to run skill code. Primarily used to test current
    /// changes without having to save the code.
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
    }
}