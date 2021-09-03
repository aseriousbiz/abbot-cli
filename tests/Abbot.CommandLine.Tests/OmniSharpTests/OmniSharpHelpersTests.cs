using Serious.IO.CommandLine.Editors;
using Xunit;

public class OmniSharpHelpersTests
{
    public class TheRemoveGlobalsDirectiveMethod
    {
        [Theory]
        [InlineData("#load \".meta/globals.csx\" // This is required for Intellisense in VS Code, etc. DO NOT TOUCH THIS LINE!\n// The actual code")]
        [InlineData("#load \".meta/globals.csx\" // This is required for Intellisense in VS Code, etc. DO NOT TOUCH THIS LINE!\r\n// The actual code")]
        public void RemovesGlobalsDirective(string code)
        {
            var result = OmniSharpHelpers.RemoveGlobalsDirective(code);
            
            Assert.Equal("// The actual code", result);
        }
    }
}
