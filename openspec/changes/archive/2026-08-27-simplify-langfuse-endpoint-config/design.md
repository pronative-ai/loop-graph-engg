## Context

To avoid vendor lock-in with any specific monitoring backend, configuration should follow OpenTelemetry official standard naming conventions (`OTEL_EXPORTER_OTLP_ENDPOINT`, `OTEL_EXPORTER_OTLP_HEADERS`, `OTEL_SERVICE_NAME`). See `proposal.md` for motivation.

## Goals / Non-Goals

**Goals:**
- Follow official OpenTelemetry specification standards for environment variable naming.
- Support any OTLP collector backend (Langfuse, SigNoz, Jaeger, Honeycomb, Aspire).
- Clear, vendor-agnostic `.env.example` guidance.

**Non-Goals:**
- Removing OpenTelemetry ActivitySource subscriptions for MAF and custom activities.

## Decisions

### Decision 1: Use Standard OTel Environment Variables
- *Design*:
  - Endpoint: `OTEL_EXPORTER_OTLP_ENDPOINT` (default `https://dev-monitoring.pronative.ai/api/public/otel`)
  - Headers: `OTEL_EXPORTER_OTLP_HEADERS` (e.g. `Authorization=Basic ...` or `signoz-access-token=...`)
  - Service Name: `OTEL_SERVICE_NAME` (default `loop-vs-graph`)
- *Rationale*: Maximum interoperability with industry standard tooling and containerized deployments.

## Risks / Trade-offs

- [Risk] Custom header format variation between backends.
  - *Mitigation*: Provide clear examples in `.env.example` for both Langfuse Basic Auth and SigNoz tokens.
