namespace AksAgenticWorkflowConsole.Tests;

public class LlmConfigurationTests
{
    [Fact]
    public void LoadModelName_DefaultsToGpt4oWhenUnset()
    {
        var model = LlmConfiguration.LoadModelName();
        Assert.False(string.IsNullOrWhiteSpace(model));
    }

    [Fact]
    public void CreateChatClient_WithConfiguredEnv_InstantiatesClient()
    {
        Environment.SetEnvironmentVariable("GATEWAY_URL", "https://gateway.pronative.ai");
        Environment.SetEnvironmentVariable("GATEWAY_KEY", "test-key");
        Environment.SetEnvironmentVariable("MODEL_NAME", "DeepSeek-V4-Pro");

        var client = LlmConfiguration.CreateChatClient();
        Assert.NotNull(client);
    }
}
