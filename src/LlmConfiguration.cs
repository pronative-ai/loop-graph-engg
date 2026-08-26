namespace AgenticWorkflowConsole;

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

    public static IChatClient CreateChatClient(string? modelName = null)
    {
        var gatewayUrl = LoadGatewayUrl().TrimEnd('/');
        var gatewayKey = LoadGatewayKey();
        var model = modelName ?? LoadModelName();

        var endpointUrl = gatewayUrl.EndsWith(model, StringComparison.OrdinalIgnoreCase)
            ? gatewayUrl
            : $"{gatewayUrl}/{model}";

        var endpoint = new Uri(endpointUrl);
        var credential = new ApiKeyCredential(gatewayKey);
        var clientOptions = new OpenAIClientOptions { Endpoint = endpoint };

        var openAiClient = new OpenAIClient(credential, clientOptions);
        var chatClient = openAiClient.GetChatClient(model);

        return chatClient.AsIChatClient();
    }

    public static async Task<bool> VerifyConnectivityAsync(IChatClient chatClient)
    {
        try
        {
            var response = await chatClient.GetResponseAsync("Ping");
            return !string.IsNullOrWhiteSpace(response?.Text);
        }
        catch
        {
            return false;
        }
    }
}