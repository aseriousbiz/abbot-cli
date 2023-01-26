using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using Serious.Abbot.Messages;
using Serious.IO.CommandLine.Commands;
using UnitTests.Fakes;
using Xunit;

public class RunCommandTests
{
    [Fact]
    public async Task ReportsErrorWhenNotWorkspace()
    {
        var context = new FakeCommandContext();
        var command = new RunCommand(context);
        var parseResult = command.Parse("run test \"some args\" -d ./my-skills");

        var result = await command.Handler!.InvokeAsync(new InvocationContext(parseResult, context.FakeConsole));

        Assert.Equal(1, result);
        var error = context.FakeConsole.Error.ToString();
        Assert.Equal("The specified directory is not an Abbot Workspace. Either specify the path to an Abbot Workspace, or initialize a new one using `abbot init`\n", error);
    }

    [Fact]
    public async Task ReportsWhenSkillDirectoryDoesNotExist()
    {
        var context = new FakeCommandContext();
        var command = new RunCommand(context);
        var workspace = context.GetWorkspace("./my-skills");
        await workspace.EnsureAsync(null);
        var parseResult = command.Parse("run test \"some args\" -d ./my-skills");

        var result = await command.Handler!.InvokeAsync(new InvocationContext(parseResult, context.FakeConsole));

        Assert.Equal(1, result);
        var error = context.FakeConsole.Error.ToString();
        Assert.Equal("The skill directory ./my-skills/test does not exist. Have you run `abbot get test` yet? Or use the `--deployed` flag to run the deployed version of this skill on the server.\n", error);
    }

    [Fact]
    public async Task RunsLocalSkill()
    {
        var context = new FakeCommandContext();
        var command = new RunCommand(context);
        var workspace = context.GetWorkspace("./my-skills");
        await workspace.EnsureAsync(null);
        var apiClient = context.CreateApiClient(workspace) as FakeApiClient;
        apiClient!.AddResponse("test", new SkillRunResponse { Replies = new List<string> {"Hello, World!"}});
        var skillWorkspace = workspace.GetSkillWorkspace("test");
        await skillWorkspace.CreateAsync(new SkillGetResponse { Name = "test", Code = "// Code" });
        var parseResult = command.Parse("run test \"some args\" -d ./my-skills");

        var result = await command.Handler!.InvokeAsync(new InvocationContext(parseResult, context.FakeConsole));

        Assert.Equal(0, result);
        var output = context.FakeConsole.Out.ToString();
        Assert.Equal("Hello, World!\n", output);
    }

    [Fact]
    public async Task RunsDeployedSkillEvenWhenLocalDoesNotExist()
    {
        var context = new FakeCommandContext();
        var command = new RunCommand(context);
        var workspace = context.GetWorkspace("./my-skills");
        await workspace.EnsureAsync(null);
        var apiClient = context.CreateApiClient(workspace) as FakeApiClient;
        apiClient!.AddResponse("test", new SkillRunResponse { Replies = new List<string> {"Hello, World!"}});
        var parseResult = command.Parse("run test \"some args\" -d ./my-skills -s");

        var result = await command.Handler!.InvokeAsync(new InvocationContext(parseResult, context.FakeConsole));

        Assert.Equal(0, result);
        var output = context.FakeConsole.Out.ToString();
        Assert.Equal("Hello, World!\n", output);
    }
}
