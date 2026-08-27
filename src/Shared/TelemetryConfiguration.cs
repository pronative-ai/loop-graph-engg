using System.Diagnostics;
using System.Text;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace AgenticWorkflowConsole.Shared;

/// <summary>
/// Configures OpenTelemetry distributed tracing using standard OpenTelemetry Protocol (OTLP).
/// Compatible with any OTLP collector including Langfuse, SigNoz, Jaeger, Honeycomb, and .NET Aspire.
/// </summary>
public static class TelemetryConfiguration
{
    public const string DefaultOtlpEndpoint = "https://dev-monitoring.pronative.ai/api/public/otel";
    public const string DefaultServiceName = "loop-vs-graph";

    /// <summary>
    /// Custom ActivitySource for recording top-level agent workflows and loop cycles.
    /// </summary>
    public static readonly ActivitySource ActivitySource = new("AgenticWorkflowConsole", "1.0.0");

    /// <summary>
    /// Loads the configured OpenTelemetry service name.
    /// </summary>
    public static string LoadServiceName()
    {
        var name = Environment.GetEnvironmentVariable("OTEL_SERVICE_NAME");
        return string.IsNullOrWhiteSpace(name) ? DefaultServiceName : name.Trim();
    }

    /// <summary>
    /// Constructs the OTLP exporter endpoint from standard environment variables.
    /// </summary>
    public static string LoadOtlpEndpoint()
    {
        var endpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");
        if (!string.IsNullOrWhiteSpace(endpoint))
        {
            return endpoint.Trim();
        }

        var host = Environment.GetEnvironmentVariable("LANGFUSE_HOST");
        if (!string.IsNullOrWhiteSpace(host))
        {
            var cleanHost = host.Trim().TrimEnd('/');
            return cleanHost.EndsWith("/api/public/otel", StringComparison.OrdinalIgnoreCase)
                ? cleanHost
                : $"{cleanHost}/api/public/otel";
        }

        return DefaultOtlpEndpoint;
    }

    /// <summary>
    /// Resolves headers for the OTLP exporter.
    /// Supports standard OTEL_EXPORTER_OTLP_HEADERS or automatic Langfuse Basic Auth derivation.
    /// </summary>
    public static string? LoadAuthHeaders()
    {
        var explicitHeaders = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS");
        if (!string.IsNullOrWhiteSpace(explicitHeaders))
        {
            return explicitHeaders.Trim();
        }

        var publicKey = Environment.GetEnvironmentVariable("LANGFUSE_PUBLIC_KEY")?.Trim();
        var secretKey = Environment.GetEnvironmentVariable("LANGFUSE_SECRET_KEY")?.Trim();

        if (!string.IsNullOrWhiteSpace(publicKey) && !string.IsNullOrWhiteSpace(secretKey))
        {
            var credentials = $"{publicKey}:{secretKey}";
            var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));
            return $"Authorization=Basic {base64}";
        }

        return null;
    }

    /// <summary>
    /// Initializes and builds the OpenTelemetry TracerProvider.
    /// </summary>
    public static TracerProvider? InitializeTracerProvider()
    {
        var endpoint = LoadOtlpEndpoint();
        var serviceName = LoadServiceName();
        var headers = LoadAuthHeaders();

        try
        {
            var builder = Sdk.CreateTracerProviderBuilder()
                .SetResourceBuilder(ResourceBuilder.CreateDefault()
                    .AddService(
                        serviceName: serviceName,
                        serviceVersion: "1.0.0")
                    .AddAttributes(new Dictionary<string, object>
                    {
                        ["deployment.environment"] = "development",
                        ["framework"] = "Microsoft.Agents.AI"
                    }))
                .AddSource("AgenticWorkflowConsole")
                .AddSource("AgenticWorkflowConsole.*")
                .AddSource("Microsoft.Agents.AI")
                .AddSource("Microsoft.Agents.AI.*")
                .AddSource("Microsoft.Extensions.AI")
                .AddSource("Microsoft.Extensions.AI.*");

            if (!string.IsNullOrWhiteSpace(endpoint))
            {
                builder.AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri(endpoint);
                    options.Protocol = OtlpExportProtocol.HttpProtobuf;

                    if (!string.IsNullOrWhiteSpace(headers))
                    {
                        options.Headers = headers;
                    }
                });
            }

            return builder.Build();
        }
        catch (Exception ex)
        {
            ConsoleLogger.SecurityWarning($"OpenTelemetry initialization notice: {ex.Message}");
            return null;
        }
    }
}
