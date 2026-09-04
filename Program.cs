using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using SentinelAI.Services;
using SentinelAI.Helpers;
using SentinelAI.Models;

namespace SentinelAI
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            Console.Title = "SentinelAI  Cybersecurity Assistant";

            Console.WriteLine("==== SentinelAI ====");
            Console.WriteLine("1. Analyze Log File");
            Console.WriteLine("2. Analyze Script");
            Console.WriteLine("3. YARA Scan File");
            Console.WriteLine("4. Memory Dump Summary");
            Console.WriteLine("5. MITRE ATT&CK Mapping");
            Console.WriteLine("6. Malware Classification");
            Console.WriteLine("7. Investigate Incident");
            Console.Write("Select option: ");

            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    await AnalyzeLog();
                    break;

                case "2":
                    await AnalyzeScript();
                    break;

                case "3":
                    await RunYaraScan();
                    break;

                case "4":
                    await AnalyzeMemoryDump();
                    break;

                case "5":
                    await RunMitreMapping();
                    break;

                case "6":
                    await RunMalwareClassification();
                    break;

                case "7":
                    await RunIncidentInvestigation();
                    break;

                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }

        // --------------------- OPERATIONS ---------------------

        static async Task AnalyzeLog()
        {
            Console.Write("Enter log file path: ");
            var path = Console.ReadLine();

            var analyzer = new LogAnalyzer();
            var events = analyzer.ParseLogFile(path ?? string.Empty);

            Console.WriteLine($"Parsed {events.Count} log events.");

            var llm = new LlmClient();
            var summary = await llm.AskAsync($"Summarize these logs and identify any threats:\n{string.Join("\n", events)}");

            Console.WriteLine(summary);
            FileHelper.WriteReport(ConfigurationLoader.Load().ReportsDirectory, "log_report.txt", summary);
        }

        static async Task AnalyzeScript()
        {
            Console.Write("Enter script path: ");
            var path = Console.ReadLine();

            var script = FileHelper.Read(path ?? string.Empty);
            var analyzer = new ScriptAnalyzer();
            var indicators = analyzer.Scan(script);
            Console.WriteLine("Deterministic findings:");
            indicators.ForEach(indicator => Console.WriteLine($" - {indicator}"));

            var llm = new LlmClient();
            var result = await llm.AskAsync($"Analyze this script for malicious behavior. Deterministic findings:\n{string.Join("\n", indicators)}\n\nScript:\n{script}");

            Console.WriteLine(result);
            FileHelper.WriteReport(ConfigurationLoader.Load().ReportsDirectory, "script_report.txt", result);
        }

        static async Task RunYaraScan()
        {
            Console.Write("File to scan: ");
            var path = Console.ReadLine();

            var yara = new YaraScanner();
            var results = yara.ScanFile(path);

            Console.WriteLine("YARA Results:");
            results.ForEach(r => Console.WriteLine($" - {r}"));
        }

        static async Task AnalyzeMemoryDump()
        {
            Console.Write("Enter memory strings file: ");
            var path = Console.ReadLine();

            var strings = FileHelper.ReadLines(path ?? string.Empty);
            var memory = new MemoryDumpAnalyzer();

            var output = await memory.Summarize(strings);
            Console.WriteLine(output);
        }

        static async Task RunMitreMapping()
        {
            Console.Write("Indicators file: ");
            var path = Console.ReadLine();

            var indicators = FileHelper.ReadLines(path ?? string.Empty);
            var mitre = new MitreMapper();

            var output = await mitre.Map(indicators);
            Console.WriteLine(output);
        }

        static async Task RunMalwareClassification()
        {
            Console.Write("Suspicious strings file: ");
            var path = Console.ReadLine();

            var suspicious = FileHelper.ReadLines(path ?? string.Empty);
            var classifier = new MalwareClassifier();

            var output = await classifier.Classify(new List<string>(), new List<string>(suspicious));
            Console.WriteLine(output);
        }

        static async Task RunIncidentInvestigation()
        {
            Console.Write("Log or script file: ");
            var path = Console.ReadLine();

            var content = FileHelper.Read(path ?? string.Empty);
            var invest = new IncidentInvestigator();

            var output = await invest.Investigate(content);
            Console.WriteLine(output);
        }
    }
}
