using Azure;
using Azure.AI.OpenAI;
using Azure.Identity;

namespace AksAgenticWorkflowConsole;

public static class LlmConfiguration
{
    public static string LoadGatewayUrl()
    {
        var url = Environment.GetEnvironmentVariable("GATEWAY_URL");
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new InvalidOperationException(
                "GATEWAY_URL environment variable is not set. " +
                "Please set this variable to your gateway URL.");
        }
        return url;
    }

    public static string LoadGatewayKey()
    {
        var key = Environment.GetEnvironmentVariable("GATEWAY_KEY");
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new InvalidOperationException(
                "GATEWAY_KEY environment variable is not set. " +
                "Please set this variable to your gateway API key.");
        }
        return key;
    }

    public static string LoadModelName()
    {
        return Environment.GetEnvironmentVariable("MODEL_NAME") ?? "gpt-4o";
    }

    public static AzureOpenAIClient CreateClient()
    {
        var gatewayUrl = LoadGatewayUrl();
        var gatewayKey = LoadGatewayKey();

        var endpoint = new Uri(gatewayUrl);
        var credential = new AzureKeyCredential(gatewayKey);

        return new AzureOpenAIClient(endpoint, credential);
    }

    public static OpenAI.Chat.ChatClient CreateChatClient(string? modelName = null)
    {
        var client = CreateClient();
        var model = modelName ?? LoadModelName();
        return client.GetChatClient(model);
    }
}
