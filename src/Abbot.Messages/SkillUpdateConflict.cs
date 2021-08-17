using System;

namespace Serious.Abbot.Messages
{
    public class SkillUpdateConflict
    {
        /// <summary>
        /// The date this was last modified.
        /// </summary>
        public DateTime LastModified { get; set; }

        /// <summary>
        /// The user that last modified the skill.
        /// </summary>
        public UserGetResponse LastModifiedBy { get; set; } = null!;
    }
}