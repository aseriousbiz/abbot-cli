using System;
using System.ComponentModel.DataAnnotations;

[assembly: CLSCompliant(false)]
namespace Serious.IO.Entities
{
    /// <summary>
    /// The programming language for a skill.
    /// </summary>
    public enum CodeLanguage
    {
        [Display(Name = "C#")]
        CSharp = 0,
        Python = 1,
        JavaScript = 2,
        Ink = 3,
        None = int.MaxValue,
    }
}