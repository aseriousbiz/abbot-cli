using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using Serious.IO.CommandLine.Commands;
using UnitTests.Fakes;
using Xunit;

public class AuthCommandTests
{
    [Fact]
    public async Task WithTokenArgStoresTokenInSecretStoreDefaultLocation()
    {
        var context = new FakeCommandContext();
        var workspace = (FakeWorkspace)context.GetWorkspace("./my-skills");
        var command = new AuthCommand(context);
        var parseResult = command.Parse("auth --token \"I know what you did last summer\" -d ./my-skills");

        var result = await command.Handler!.InvokeAsync(new InvocationContext(parseResult, context.FakeConsole));

        var stored = await workspace.FakeTokenStore.RetrieveTokenAsync();
        Assert.Equal("I know what you did last summer", stored);
        Assert.Equal(0, result);
        var output = context.FakeConsole.Out.ToString();
        Assert.Equal(string.Empty, output);
        var secretsId = await workspace.FakeTokenStore.SecretsIdFile.ReadAllTextAsync();
        var secretsFile = context.FakeFileSystem.GetFile($".abbot/secrets/{secretsId}");
        Assert.True(secretsFile.Exists);
    }
    
    [Fact]
    public async Task WithTokenAndSecretsDirectoryArgStoresTokenInSpecifiedLocation()
    {
        var context = new FakeCommandContext();
        var workspace = (FakeWorkspace)context.GetWorkspace("./my-skills");
        var command = new AuthCommand(context);
        var parseResult = command.Parse("auth --token \"I know what you did last summer\" --secrets-directory .secrets/.abbot/ -d ./my-skills");

        var result = await command.Handler!.InvokeAsync(new InvocationContext(parseResult, context.FakeConsole));

        var stored = await workspace.FakeTokenStore.RetrieveTokenAsync();
        Assert.Equal("I know what you did last summer", stored);
        Assert.Equal(0, result);
        var output = context.FakeConsole.Out.ToString();
        Assert.Equal(string.Empty, output);
        var secretsId = await workspace.FakeTokenStore.SecretsIdFile.ReadAllTextAsync();
        var secretsFile = context.FakeFileSystem.GetFile($".secrets/.abbot/{secretsId}");
        Assert.True(secretsFile.Exists);
    }
}
