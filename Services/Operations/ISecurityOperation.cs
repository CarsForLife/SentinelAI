namespace SentinelAI.Services.Operations;

public interface ISecurityOperation
{
    SecurityOperation Metadata { get; }

    string Run(LocalAnalysisService analyzer, string input)
    {
        return analyzer.Analyze(Metadata.Name, input);
    }
}