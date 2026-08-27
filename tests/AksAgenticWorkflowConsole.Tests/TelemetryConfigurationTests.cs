using System.Text;
using AgenticWorkflowConsole.Shared;
using Xunit;

namespace AksAgenticWorkflowConsole.Tests;

public class TelemetryConfigurationTests
{
    [Fact]
    public void LoadServiceName_Default_ReturnsLoopVsGraph()
    {
        var original = Environment.GetEnvironmentVariable("OTEL_SERVICE_NAME");
        try
        {
            Environment.SetEnvironmentVariable("OTEL_SERVICE_NAME", null);
            Assert.Equal("loop-vs-graph", TelemetryConfiguration.LoadServiceName());
        }
        finally
        {
            Environment.SetEnvironmentVariable("OTEL_SERVICE_NAME", original);
        }
    }

    [Fact]
    public void LoadServiceName_Custom_ReturnsConfiguredName()
    {
        var original = Environment.GetEnvironmentVariable("OTEL_SERVICE_NAME");
        try
        {
            Environment.SetEnvironmentVariable("OTEL_SERVICE_NAME", "custom-agent-service");
            Assert.Equal("custom-agent-service", TelemetryConfiguration.LoadServiceName());
        }
        finally
        {
            Environment.SetEnvironmentVariable("OTEL_SERVICE_NAME", original);
        }
    }

    [Fact]
    public void LoadOtlpEndpoint_ExplicitOtelEndpoint_ReturnsConfiguredUrl()
    {
        var original = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");
        try
        {
            Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", "https://ingest.signoz.io:443");
            Assert.Equal("https://ingest.signoz.io:443", TelemetryConfiguration.LoadOtlpEndpoint());
        }
        finally
        {
            Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", original);
        }
    }

    [Fact]
    public void LoadOtlpEndpoint_FromLangfuseHost_AppendsOtelRoute()
    {
        var originalOtel = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");
        var originalHost = Environment.GetEnvironmentVariable("LANGFUSE_HOST");
        try
        {
            Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", null);
            Environment.SetEnvironmentVariable("LANGFUSE_HOST", "https://dev-monitoring.pronative.ai/");

            Assert.Equal("https://dev-monitoring.pronative.ai/api/public/otel", TelemetryConfiguration.LoadOtlpEndpoint());
        }
        finally
        {
            Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", originalOtel);
            Environment.SetEnvironmentVariable("LANGFUSE_HOST", originalHost);
        }
    }

    [Fact]
    public void LoadOtlpEndpoint_Default_ReturnsPronativeEndpoint()
    {
        var originalOtel = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");
        var originalHost = Environment.GetEnvironmentVariable("LANGFUSE_HOST");
        try
        {
            Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", null);
            Environment.SetEnvironmentVariable("LANGFUSE_HOST", null);

            Assert.Equal("https://dev-monitoring.pronative.ai/api/public/otel", TelemetryConfiguration.LoadOtlpEndpoint());
        }
        finally
        {
            Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", originalOtel);
            Environment.SetEnvironmentVariable("LANGFUSE_HOST", originalHost);
        }
    }

    [Fact]
    public void LoadAuthHeaders_ExplicitHeaders_ReturnsConfiguredHeaders()
    {
        var original = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS");
        try
        {
            Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS", "signoz-access-token=token123");
            Assert.Equal("signoz-access-token=token123", TelemetryConfiguration.LoadAuthHeaders());
        }
        finally
        {
            Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS", original);
        }
    }

    [Fact]
    public void LoadAuthHeaders_FromLangfuseKeys_BuildsBasicAuth()
    {
        var originalHeaders = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS");
        var originalPk = Environment.GetEnvironmentVariable("LANGFUSE_PUBLIC_KEY");
        var originalSk = Environment.GetEnvironmentVariable("LANGFUSE_SECRET_KEY");
        try
        {
            Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS", null);
            Environment.SetEnvironmentVariable("LANGFUSE_PUBLIC_KEY", "pk-test");
            Environment.SetEnvironmentVariable("LANGFUSE_SECRET_KEY", "sk-test");

            var expectedBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes("pk-test:sk-test"));
            Assert.Equal($"Authorization=Basic {expectedBase64}", TelemetryConfiguration.LoadAuthHeaders());
        }
        finally
        {
            Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS", originalHeaders);
            Environment.SetEnvironmentVariable("LANGFUSE_PUBLIC_KEY", originalPk);
            Environment.SetEnvironmentVariable("LANGFUSE_SECRET_KEY", originalSk);
        }
    }

    [Fact]
    public void InitializeTracerProvider_BuildsSuccessfully()
    {
        using var provider = TelemetryConfiguration.InitializeTracerProvider();
        Assert.NotNull(provider);
    }
}
