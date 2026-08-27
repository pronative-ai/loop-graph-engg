namespace AgenticWorkflowConsole;

// Central factory that maps environment configuration (GATEWAY_URL, GATEWAY_KEY,
// MODEL_NAME) onto the OpenAI-compatible client used by all demos. This is the
// only place the Azure/OpenAI wiring is assembled, so callers just ask for an
// IChatClient and never touch connection details.
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

        // The gateway URL may or may not already include the model segment, so
        // append it only when it is absent to form the completed endpoint.
        var endpointUrl = gatewayUrl.EndsWith(model, StringComparison.OrdinalIgnoreCase)
            ? gatewayUrl
            : $"{gatewayUrl}/{model}";

        // Map the URL + API key onto the MAF OpenAI client: the endpoint pins the
        // gateway, the key authenticates, and GetChatClient selects the model.
        // AsIChatClient adapts the OpenAI SDK client to MAF's IChatClient contract.
        var endpoint = new Uri(endpointUrl);
        var credential = new ApiKeyCredential(gatewayKey);
        var clientOptions = new OpenAIClientOptions { Endpoint = endpoint };

        var openAiClient = new OpenAIClient(credential, clientOptions);
        var chatClient = openAiClient.GetChatClient(model);

        return chatClient.AsIChatClient();
    }

    // Lightweight smoke test that round-trips a "Ping" through the live gateway
    // so startup can log connectivity before any real demo runs.
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