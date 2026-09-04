# SentinelAI

Local-first C# security analysis assistant for SOC analysts, blue teamers, and security engineers.

> **Prototype status:** SentinelAI uses a configured OpenAI-compatible LLM for AI-assisted analysis. Its YARA menu currently uses deterministic text signatures and does not execute `.yar` rules.

## What It Does

| Capability | Description |
| --- | --- |
| Log analysis | Parses a log file and asks the LLM for a threat summary. |
| Script analysis | Detects selected suspicious patterns locally, then adds LLM analysis. |
| File scanning | Checks files for deterministic suspicious text signatures. |
| Memory analysis | Summarizes strings extracted from a memory dump. |
| MITRE mapping | Maps supplied indicators to MITRE ATT&CK techniques. |
| Malware classification | Combines suspicious strings and scan findings with LLM analysis. |
| Incident investigation | Produces a timeline, severity assessment, and recommendations. |

All file-based inputs are limited to 10 MiB by default. Generated reports are saved in `Reports/`.

## Requirements

- .NET 8 SDK
- An OpenAI-compatible local LLM endpoint for AI-assisted features

The default endpoint is:

```text
http://localhost:3000/v1/chat/completions
```

The endpoint must return a response containing `choices[0].message.content`.

## Run

From the repository root:

```bash
dotnet restore
dotnet run --project SentinelAI.csproj
```

SentinelAI presents a numbered menu. Select an operation and provide the path to the requested input file.

## Configuration

Edit [`Config/settings.json`](Config/settings.json) before running:

| Setting | Purpose | Default |
| --- | --- | --- |
| `LlmBaseUrl` | OpenAI-compatible chat endpoint | `http://localhost:3000/v1/chat/completions` |
| `LlmModel` | Model sent in the request | `gpt-4` |
| `LlmTimeoutSeconds` | Maximum LLM request duration | `30` |
| `MaxInputBytes` | Maximum size of an input file | `10485760` |
| `ReportsDirectory` | Destination for generated reports | `Reports` |
| `YaraRulesPath` | Location of the rules file | `Examples/yara_rules.yar` |

## Privacy And Safety

Input content is included in prompts sent to the configured LLM endpoint. Use an endpoint you control and avoid sending confidential data to untrusted services. Model output is advisory and should be verified against deterministic findings and other evidence.

## Limitations

- The scanner does not currently interpret or execute YARA rules.
- AI-assisted features require a reachable LLM endpoint.
- The example input files are placeholders and are currently empty.
- There is no automated test project yet.
- The project requires the .NET 8 SDK. A machine with only .NET 6 cannot build it.

## Verification

After installing the .NET 8 SDK, verify the project with:

```bash
dotnet restore
dotnet build
dotnet run --project SentinelAI.csproj
```

The source files edited during the review produced no VS Code diagnostics, and `Config/settings.json` passed JSON validation. Full compilation still requires the .NET 8 SDK.
