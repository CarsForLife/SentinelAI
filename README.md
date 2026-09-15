# SentinelAI

Local-first C# security analysis assistant for SOC analysts, blue teamers, and security engineers. SentinelAI runs locally and does not require an AI model, AI editor extension, API key, network connection, or external service.

## What It Does

| Capability | Description |
| --- | --- |
| Log analysis | Scans log text for local threat indicators. |
| Script analysis | Detects suspicious patterns locally and maps them to ATT&CK techniques. |
| File scanning | Checks files for deterministic suspicious text signatures. |
| Memory analysis | Summarizes strings extracted from a memory dump. |
| MITRE mapping | Maps supplied indicators to MITRE ATT&CK techniques. |
| Malware classification | Produces a local risk level and recommended response steps. |
| Incident investigation | Produces local findings and recommendations. |

All file-based inputs are limited to 10 MiB by default. Generated reports are saved in `Reports/`.

## Requirements And Packages

- .NET 8 SDK

No AI extension, Python, Node.js, database, API key, or additional NuGet package is required. Verify the SDK with `dotnet --version`.

## Run

From the repository root:

```bash
dotnet restore
dotnet run --project SentinelAI.csproj
```

SentinelAI presents a numbered menu. Select an operation and provide the path to the requested input file. Reports are saved in `Reports/`.

This opens the Windows desktop GUI. Select an operation, browse to an input file, and select **Analyze**. To use the original terminal menu instead:

```powershell
dotnet run --project SentinelAI.csproj -- --cli
```
## Local History

Each analysis records its operation and matched indicators in `Reports/user_history.json`. Future reports include the most frequent prior indicators, so the application builds context from previous use locally. Input contents are not stored there and nothing is uploaded.

Delete `Reports/user_history.json` to reset this local history.

## Configuration

Edit [`Config/settings.json`](Config/settings.json) before running:

| Setting | Purpose | Default |
| --- | --- | --- |
| `MaxInputBytes` | Maximum size of an input file | `10485760` |
| `ReportsDirectory` | Destination for generated reports | `Reports` |
| `YaraRulesPath` | Location of the rules file | `Examples/yara_rules.yar` |

## Privacy And Safety

Findings are deterministic heuristics, not proof of compromise. Preserve original evidence and have an analyst validate important findings.

## Limitations

- The scanner does not currently interpret or execute YARA rules.
- The scanner does not currently interpret full YARA syntax.
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

The application builds with the .NET 8 SDK and can run without network access.
