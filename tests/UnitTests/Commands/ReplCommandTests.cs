using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using Serious.Abbot.CommandLine.Commands;
using UnitTests.Fakes;
using Xunit;

public class ReplCommandTests
{
    [Fact]
    public async Task ReportsErrorWhenNotWorkspace()
    {
        var context = new FakeCommandContext();
        var command = new ReplCommand(context, new RunCommand(context));
        var parseResult = command.Parse("repl test \"some args\" -d ./my-skills");

        var result = await command.Handler!.InvokeAsync(new InvocationContext(parseResult, context.FakeConsole));
        
        Assert.Equal(1, result);
        var error = context.FakeConsole.Error.ToString();
        Assert.Equal("The specified directory is not an Abbot Workspace. Either specify the path to an Abbot Workspace, or initialize a new one using `abbot init`\n", error);
    }
    
    [Fact]
    public async Task ReportsWhenSkillDirectoryDoesNotExist()
    {
        var context = new FakeCommandContext();
        var command = new ReplCommand(context, new RunCommand(context));
        var workspace = context.GetWorkspace("./my-skills");
        await workspace.EnsureAsync();
        var parseResult = command.Parse("repl test \"some args\" -d ./my-skills");

        var result = await command.Handler!.InvokeAsync(new InvocationContext(parseResult, context.FakeConsole));
        
        Assert.Equal(1, result);
        var error = context.FakeConsole.Error.ToString();
        Assert.Equal("The skill directory ./my-skills/test does not exist. Have you run `abbot get test` yet? Or use the `--deployed` flag to run the deployed version of this skill on the server.\n", error);
    }    
}
