namespace AksAgenticWorkflowConsole.Tests;

public class TelemetryConfigurationTests
{
    [Fact]
    public void CleanEnvValue_StripsQuotesAndWhitespace()
    {
        Assert.Null(TelemetryConfiguration.CleanEnvValue(null));
        Assert.Null(TelemetryConfiguration.CleanEnvValue("   "));
        Assert.Equal("value", TelemetryConfiguration.CleanEnvValue("\"value\""));
        Assert.Equal("value", TelemetryConfiguration.CleanEnvValue("'value'"));
        Assert.Equal("value", TelemetryConfiguration.CleanEnvValue("  \"value\"  "));
    }

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
    public void LoadServiceName_CustomWithQuotes_ReturnsStrippedName()
    {
        var original = Environment.GetEnvironmentVariable("OTEL_SERVICE_NAME");
        try
        {
            Environment.SetEnvironmentVariable("OTEL_SERVICE_NAME", "\"custom-agent-service\"");
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
            Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", "\"https://ingest.signoz.io:443\"");
            Assert.Equal("https://ingest.signoz.io:443", TelemetryConfiguration.LoadOtlpEndpoint());
        }
        finally
        {
            Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", original);
        }
    }

    [Fact]
    public void LoadOtlpTraceEndpoint_NormalizesWithOrWithoutTrailingV1Traces()
    {
        var original = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");
        try
        {
            Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", "https://dev-monitoring.pronative.ai/api/public/otel/v1/traces");
            Assert.Equal("https://dev-monitoring.pronative.ai/api/public/otel/v1/traces", TelemetryConfiguration.LoadOtlpTraceEndpoint());

            Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", "https://dev-monitoring.pronative.ai/api/public/otel");
            Assert.Equal("https://dev-monitoring.pronative.ai/api/public/otel/v1/traces", TelemetryConfiguration.LoadOtlpTraceEndpoint());
        }
        finally
        {
            Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", original);
        }
    }

    [Fact]
    public void LoadOtlpEndpoint_Default_ReturnsPronativeEndpoint()
    {
        var originalOtel = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");
        try
        {
            Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", null);
            Assert.Equal("https://dev-monitoring.pronative.ai/api/public/otel", TelemetryConfiguration.LoadOtlpEndpoint());
        }
        finally
        {
            Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", originalOtel);
        }
    }

    [Fact]
    public void LoadOtlpTraceEndpoint_AppendsV1TracesWhenMissing()
    {
        var originalOtel = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");
        try
        {
            Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", "https://dev-monitoring.pronative.ai/api/public/otel");
            Assert.Equal("https://dev-monitoring.pronative.ai/api/public/otel/v1/traces", TelemetryConfiguration.LoadOtlpTraceEndpoint());
        }
        finally
        {
            Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", originalOtel);
        }
    }

    [Fact]
    public void LoadAuthHeaders_QuotedHeaders_StripsQuotesAndAttachesIngestionVersion()
    {
        var original = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS");
        try
        {
            Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS", "\"Authorization=Basic dGVzdDp0ZXN0\"");
            Assert.Equal("Authorization=Basic dGVzdDp0ZXN0,x-langfuse-ingestion-version=4", TelemetryConfiguration.LoadAuthHeaders());
        }
        finally
        {
            Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS", original);
        }
    }

    [Fact]
    public void LoadAuthHeaders_NonBasicHeaders_PreservedAsIs()
    {
        var original = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS");
        try
        {
            Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS", "'signoz-access-token=token123'");
            Assert.Equal("signoz-access-token=token123", TelemetryConfiguration.LoadAuthHeaders());
        }
        finally
        {
            Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_HEADERS", original);
        }
    }

    [Fact]
    public void InitializeTracerProvider_BuildsSuccessfully()
    {
        using var provider = TelemetryConfiguration.InitializeTracerProvider();
        Assert.NotNull(provider);
    }

    [Fact]
    public void Flush_DoesNotThrow()
    {
        using var provider = TelemetryConfiguration.InitializeTracerProvider();
        var exception = Record.Exception(() => TelemetryConfiguration.Flush(100));
        Assert.Null(exception);
    }

    [Fact]
    public void TestOtlpExporterDirectExport()
    {
        DotNetEnv.Env.TraversePath().Load();
        var traceEndpoint = TelemetryConfiguration.LoadOtlpTraceEndpoint();
        var headers = TelemetryConfiguration.LoadAuthHeaders();

        var options = new OpenTelemetry.Exporter.OtlpExporterOptions
        {
            Endpoint = new Uri(traceEndpoint),
            Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf,
            Headers = headers
        };

        var exporter = new OpenTelemetry.Exporter.OtlpTraceExporter(options);

        var activitySource = new ActivitySource("TestDirectExportSource");
        using var listener = new ActivityListener
        {
            ShouldListenTo = _ => true,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded
        };
        ActivitySource.AddActivityListener(listener);

        using var activity = activitySource.StartActivity("DirectExportVerificationSpan");
        Assert.NotNull(activity);
        activity.SetTag("test.key", "test.value");
        activity.Stop();

        // Export the activity
        var batch = new OpenTelemetry.Batch<Activity>(new[] { activity }, 1);
        var result = exporter.Export(batch);

        Assert.Equal(OpenTelemetry.ExportResult.Success, result);
    }

    [Fact]
    public void ToolActivity_AttachesInputAndOutputTags()
    {
        using var listener = new ActivityListener
        {
            ShouldListenTo = _ => true,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded
        };
        ActivitySource.AddActivityListener(listener);

        using var activity = TelemetryConfiguration.ActivitySource.StartActivity("Tool.InspectCode");
        Assert.NotNull(activity);
        activity.SetTag("gen_ai.tool.name", "InspectCode");
        activity.SetTag("gen_ai.tool.input", "targetFileName=OrderDiscountEngine.cs");
        activity.SetTag("gen_ai.tool.output", "public class OrderDiscountEngine { }");
        activity.SetTag("gen_ai.tool.is_success", true);

        Assert.Equal("InspectCode", activity.GetTagItem("gen_ai.tool.name"));
        Assert.Equal("targetFileName=OrderDiscountEngine.cs", activity.GetTagItem("gen_ai.tool.input"));
        Assert.Equal("public class OrderDiscountEngine { }", activity.GetTagItem("gen_ai.tool.output"));
        Assert.Equal(true, activity.GetTagItem("gen_ai.tool.is_success"));
    }

    [Fact]
    public void WorkflowNodeActivity_AttachesStateTags()
    {
        using var listener = new ActivityListener
        {
            ShouldListenTo = _ => true,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded
        };
        ActivitySource.AddActivityListener(listener);

        using var activity = TelemetryConfiguration.ActivitySource.StartActivity("Workflow.Node.ArchitectNode");
        Assert.NotNull(activity);
        activity.SetTag("workflow.node_name", "ArchitectNode");
        activity.SetTag("workflow.goal", "Build Task Manager Microservice");
        activity.SetTag("workflow.output_spec", "Spec: TaskManager API Contract");

        Assert.Equal("ArchitectNode", activity.GetTagItem("workflow.node_name"));
        Assert.Equal("Build Task Manager Microservice", activity.GetTagItem("workflow.goal"));
        Assert.Equal("Spec: TaskManager API Contract", activity.GetTagItem("workflow.output_spec"));
    }
}
