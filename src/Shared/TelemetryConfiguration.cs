using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace AgenticWorkflowConsole.Shared;

/// <summary>
/// Configures OpenTelemetry distributed tracing using standard OpenTelemetry Protocol (OTLP).
/// Strictly resolves official environment variables (OTEL_EXPORTER_OTLP_ENDPOINT, OTEL_EXPORTER_OTLP_HEADERS, OTEL_SERVICE_NAME).
/// Compatible with any OTLP collector including Langfuse, SigNoz, Jaeger, Honeycomb, and .NET Aspire.
/// </summary>
public static class TelemetryConfiguration
{
    public const string DefaultOtlpEndpoint = "https://dev-monitoring.pronative.ai/api/public/otel";
    public const string DefaultServiceName = "loop-vs-graph";

    private static TracerProvider? s_tracerProvider;

    /// <summary>
    /// Custom ActivitySource for recording top-level agent workflows, graph nodes, and loop cycles.
    /// </summary>
    public static readonly ActivitySource ActivitySource = new("AgenticWorkflowConsole", "1.0.0");

    /// <summary>
    /// Helper to strip leading and trailing quotation marks and whitespace.
    /// </summary>
    public static string? CleanEnvValue(string? rawValue)
    {
        if (string.IsNullOrWhiteSpace(rawValue))
        {
            return null;
        }

        var trimmed = rawValue.Trim();
        if ((trimmed.StartsWith('"') && trimmed.EndsWith('"')) || 
            (trimmed.StartsWith('\'') && trimmed.EndsWith('\'')))
        {
            trimmed = trimmed[1..^1].Trim();
        }

        return string.IsNullOrWhiteSpace(trimmed) ? null : trimmed;
    }

    /// <summary>
    /// Loads the configured OpenTelemetry service name.
    /// </summary>
    public static string LoadServiceName()
    {
        var rawName = Environment.GetEnvironmentVariable("OTEL_SERVICE_NAME");
        var name = CleanEnvValue(rawName);
        return string.IsNullOrWhiteSpace(name) ? DefaultServiceName : name;
    }

    /// <summary>
    /// Constructs and normalizes the base OTLP exporter endpoint.
    /// </summary>
    public static string LoadOtlpEndpoint()
    {
        var rawEndpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");
        var endpoint = CleanEnvValue(rawEndpoint);
        var url = string.IsNullOrWhiteSpace(endpoint) ? DefaultOtlpEndpoint : endpoint;
        return url.TrimEnd('/');
    }

    /// <summary>
    /// Constructs the explicit OTLP HTTP trace endpoint (with /v1/traces path).
    /// Ensures that .NET OTLP exporter does not truncate path prefixes like /api/public/otel.
    /// </summary>
    public static string LoadOtlpTraceEndpoint()
    {
        var baseEndpoint = LoadOtlpEndpoint();
        return baseEndpoint.EndsWith("/v1/traces", StringComparison.OrdinalIgnoreCase)
            ? baseEndpoint
            : $"{baseEndpoint}/v1/traces";
    }

    /// <summary>
    /// Resolves headers for the OTLP exporter from OTEL_EXPORTER_OTLP_HEADERS.
    /// Automatically attaches x-langfuse-ingestion-version=4 when Basic authentication is detected.
    /// </summary>
    public static string? LoadAuthHeaders()
    {
        var rawHeaders = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS");
        var headers = CleanEnvValue(rawHeaders);

        if (string.IsNullOrWhiteSpace(headers))
        {
            return null;
        }

        // If explicit headers are provided for Langfuse Basic auth, ensure ingestion version is attached
        if (headers.Contains("Basic", StringComparison.OrdinalIgnoreCase) && 
            !headers.Contains("x-langfuse-ingestion-version", StringComparison.OrdinalIgnoreCase))
        {
            return $"{headers},x-langfuse-ingestion-version=4";
        }

        return headers;
    }

    /// <summary>
    /// Initializes and builds the OpenTelemetry TracerProvider.
    /// </summary>
    public static TracerProvider? InitializeTracerProvider()
    {
        var traceEndpoint = LoadOtlpTraceEndpoint();
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
                .AddSource("*")
                .AddSource("AgenticWorkflowConsole")
                .AddSource("AgenticWorkflowConsole.*")
                .AddSource("Microsoft.Agents.AI")
                .AddSource("Microsoft.Agents.AI.*")
                .AddSource("Microsoft.Extensions.AI")
                .AddSource("Microsoft.Extensions.AI.*");

            if (!string.IsNullOrWhiteSpace(traceEndpoint))
            {
                builder.AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri(traceEndpoint);
                    options.Protocol = OtlpExportProtocol.HttpProtobuf;

                    if (!string.IsNullOrWhiteSpace(headers))
                    {
                        options.Headers = headers;
                    }
                });
            }

            s_tracerProvider = builder.Build();
            return s_tracerProvider;
        }
        catch (Exception ex)
        {
            ConsoleLogger.SecurityWarning($"OpenTelemetry initialization notice: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Forces an immediate export of all queued OpenTelemetry trace spans.
    /// </summary>
    public static void Flush(int timeoutMilliseconds = 5000)
    {
        try
        {
            s_tracerProvider?.ForceFlush(timeoutMilliseconds);
        }
        catch
        {
            // Best-effort flush; ignore transient network flush exceptions
        }
    }
}
