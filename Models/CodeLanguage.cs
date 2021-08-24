using System;
using System.ComponentModel.DataAnnotations;

[assembly: CLSCompliant(true)]
namespace Serious.Abbot.Entities
{
    /// <summary>
    /// The programming language for a skill.
    /// </summary>
    public enum CodeLanguage
    {
        [Display(Name = "C#")]
        CSharp,
        Python,
        JavaScript
    }
}