using Microsoft.Win32;
using SentinelAI.Helpers;
using SentinelAI.Services;
using SentinelAI.Services.Operations;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Reflection;

namespace SentinelAI;

public partial class MainWindow : Window
{
    private readonly List<string> _features = new()
    {
        "01  Deterministic log threat triage with bounded file reads",
        "02  Script behavior detection for downloads, encoding, and Defender tampering",
        "03  Suspicious file text scanning with size validation",
        "04  Memory-string threat triage",
        "05  MITRE ATT&CK technique mapping with matched evidence",
        "06  Local malware risk classification",
        "07  Incident investigation summaries and response guidance",
        "08  SHA-256 evidence fingerprinting for chain-of-custody notes",
        "09  Pasted evidence analysis without creating a temporary file",
        "10  Drag-and-drop evidence loading",
        "11  Configurable maximum input size to reduce accidental overload",
        "12  Local-only operation with no API key or network dependency",
        "13  Persistent finding history stored as inspectable JSON",
        "14  Frequent-finding context from previous local investigations",
        "15  History reset for privacy and workstation handoff",
        "16  Searchable analysis history tab",
        "17  Full exception capture in a copyable error workspace",
        "18  Report copy to clipboard for tickets and case notes",
        "19  Plain-text report export for evidence packages",
        "20  Responsive off-thread analysis for larger inputs",
        "21  CLI fallback for automation and headless workflows",
        "22  UTF-8 evidence reading with clear missing-file errors",
        "23  Deterministic severity levels: Low, High, and Critical",
        "24  Network download indicator detection mapped to T1105",
        "25  Obfuscation indicator detection mapped to T1027",
        "26  Security-tool tampering detection mapped to T1562.001",
        "27  Credential access indicator detection mapped to T1555",
        "28  Persistence indicator detection mapped to T1547",
        "29  Theme-aware dark and light workspaces",
        "30  Analyst-verifiable recommendations instead of opaque decisions",
        "31  Input clearing without deleting investigation history",
        "32  Configurable report directory for case separation"
    };
    private bool _lightTheme;

    private ListBox OperationSelection => FindControl<ListBox>("OperationBox");
    private TextBox EvidencePath => FindControl<TextBox>("PathBox");
    private TextBox EvidenceInput => FindControl<TextBox>("InputBox");
    private TextBox ReportOutput => FindControl<TextBox>("ReportBox");
    private TextBox HistoryOutput => FindControl<TextBox>("HistoryBox");
    private TextBox ErrorOutput => FindControl<TextBox>("ErrorsBox");
    private TextBox FeatureSearch => FindControl<TextBox>("FeatureSearchBox");
    private ListBox FeatureResults => FindControl<ListBox>("FeatureList");
    private TextBlock EvidenceIndicator => FindControl<TextBlock>("EvidenceStatus");
    private TextBlock StatusIndicator => FindControl<TextBlock>("StatusText");
    private Button ThemeToggle => FindControl<Button>("ThemeButton");

    public MainWindow()
    {
        LoadGeneratedView();
        OperationSelection.ItemsSource = SecurityOperationRegistry.All.Select(operation => new OperationSelectionItem(operation)).ToList();
        FeatureResults.ItemsSource = _features;
        RefreshHistory();
    }

    private void LoadGeneratedView()
    {
        var loader = GetType().GetMethod("InitializeComponent", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (loader is null)
            throw new InvalidOperationException("The WPF view loader was not generated. Run a project build before starting SentinelAI.");

        loader.Invoke(this, null);
    }

    private T FindControl<T>(string name) where T : class
    {
        return FindName(name) as T
            ?? throw new InvalidOperationException($"The WPF view control '{name}' was not found.");
    }

    private void Browse_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog { Title = "Choose evidence file" };
        if (dialog.ShowDialog() == true)
        {
            EvidencePath.Text = dialog.FileName;
            EvidenceInput.Clear();
        }
    }

    private async void Analyze_Click(object sender, RoutedEventArgs e)
    {
        SetBusy(true);
        try
        {
            var settings = ConfigurationLoader.Load();
            var selected = OperationSelection.Items.Cast<OperationSelectionItem>().Where(item => item.IsSelected).ToList();
            if (selected.Count == 0)
                throw new ArgumentException("Select at least one operation before running analysis.");

            var input = GetInput(settings.MaxInputBytes);
            var sourcePath = string.IsNullOrWhiteSpace(EvidenceInput.Text) ? EvidencePath.Text : string.Empty;
            var results = await Task.WhenAll(selected.Select(item => RunOperationAsync(item.Operation, input, sourcePath, settings.MaxInputBytes, settings.ReportsDirectory)));
            var result = string.Join($"{Environment.NewLine}{Environment.NewLine}{new string('-', 72)}{Environment.NewLine}{Environment.NewLine}", results);

            if (!string.IsNullOrWhiteSpace(sourcePath))
            {
                var hash = await Task.Run(() => FileHelper.Sha256(sourcePath));
                result = $"Evidence SHA-256: {hash}{Environment.NewLine}{Environment.NewLine}{result}";
                EvidenceIndicator.Text = $"Loaded {Path.GetFileName(sourcePath)} | SHA-256 {hash[..12]}...";
            }
            else
            {
                EvidenceIndicator.Text = $"Pasted evidence | {input.Length:N0} characters";
            }

            ReportOutput.Text = result;
            RefreshHistory();
            StatusIndicator.Text = $"Completed {selected.Count} operation(s) concurrently. Local history updated.";
        }
        catch (Exception exception)
        {
            ErrorOutput.AppendText($"[{DateTime.Now:G}] {exception}\n\n");
            StatusIndicator.Text = "Analysis failed. The full error is available in the Errors tab.";
        }
        finally
        {
            SetBusy(false);
        }
    }

    private static async Task<string> RunOperationAsync(ISecurityOperation operation, string input, string sourcePath, int maxBytes, string reportsDirectory)
    {
        var knowledge = new LocalKnowledgeBase(reportsDirectory);
        var analyzer = new LocalAnalysisService(knowledge);
        if (operation.Metadata.Name == "YARA Text Scan")
        {
            if (string.IsNullOrWhiteSpace(sourcePath))
                throw new ArgumentException("YARA Text Scan requires a file path, not pasted text.");

            var findings = await Task.Run(() => new YaraScanner().ScanFile(sourcePath, maxBytes));
            return $"Local {operation.Metadata.Name}{Environment.NewLine}{string.Join(Environment.NewLine, findings.DefaultIfEmpty("No deterministic signatures matched." ).Select(finding => $"- {finding}"))}";
        }

        return await Task.Run(() => operation.Run(analyzer, input));
    }

    private void SelectAllOperations_Click(object sender, RoutedEventArgs e)
    {
        foreach (var item in OperationSelection.Items.Cast<OperationSelectionItem>())
            item.IsSelected = true;
        OperationSelection.Items.Refresh();
    }

    private void ClearOperationSelection_Click(object sender, RoutedEventArgs e)
    {
        foreach (var item in OperationSelection.Items.Cast<OperationSelectionItem>())
            item.IsSelected = false;
        OperationSelection.Items.Refresh();
    }

    private void SetBusy(bool busy)
    {
        Mouse.OverrideCursor = busy ? System.Windows.Input.Cursors.Wait : null;
        if (busy)
            StatusIndicator.Text = "Analyzing locally...";
    }

    private void ClearInput_Click(object sender, RoutedEventArgs e)
    {
        EvidencePath.Clear();
        EvidenceInput.Clear();
        EvidenceIndicator.Text = "No evidence loaded";
        StatusIndicator.Text = "Input cleared.";
    }

    private void CopyReport_Click(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(ReportOutput.Text))
            Clipboard.SetText(ReportOutput.Text);
    }

    private void ExportReport_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(ReportOutput.Text))
            return;

        var dialog = new SaveFileDialog { Filter = "Text report (*.txt)|*.txt|All files (*.*)|*.*", FileName = "sentinelai_report.txt" };
        if (dialog.ShowDialog() == true)
        {
            File.WriteAllText(dialog.FileName, ReportOutput.Text);
            StatusIndicator.Text = $"Report exported to {dialog.FileName}.";
        }
    }

    private string GetInput(int maxBytes)
    {
        if (!string.IsNullOrWhiteSpace(EvidenceInput.Text))
            return EvidenceInput.Text;
        EnsureFilePath();
        return FileHelper.Read(EvidencePath.Text, maxBytes);
    }

    private void EnsureFilePath()
    {
        if (string.IsNullOrWhiteSpace(EvidencePath.Text))
            throw new ArgumentException("Paste evidence into the input box or choose an input file.");
    }

    private void PathBox_Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
            EvidencePath.Text = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
    }

    private void Window_Drop(object sender, DragEventArgs e) => PathBox_Drop(sender, e);

    private void RefreshHistory_Click(object sender, RoutedEventArgs e) => RefreshHistory();

    private void RefreshHistory()
    {
        var settings = ConfigurationLoader.Load();
        var knowledge = new LocalKnowledgeBase(settings.ReportsDirectory);
        HistoryOutput.Text = knowledge.ReadHistory();
    }

    private void ResetHistory_Click(object sender, RoutedEventArgs e)
    {
        var settings = ConfigurationLoader.Load();
        new LocalKnowledgeBase(settings.ReportsDirectory).Reset();
        RefreshHistory();
        StatusIndicator.Text = "Local history reset.";
    }

    private void CopyHistory_Click(object sender, RoutedEventArgs e) => Clipboard.SetText(HistoryOutput.Text);

    private void ClearErrors_Click(object sender, RoutedEventArgs e) => ErrorOutput.Clear();

    private void CopyErrors_Click(object sender, RoutedEventArgs e) => Clipboard.SetText(ErrorOutput.Text);

    private void FeatureSearchBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        var query = FeatureSearch.Text.Trim();
        FeatureResults.ItemsSource = string.IsNullOrWhiteSpace(query)
            ? _features
            : _features.Where(feature => feature.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    private void ThemeToggle_Click(object sender, RoutedEventArgs e)
    {
        _lightTheme = !_lightTheme;
        if (_lightTheme)
        {
            SetBrush("WindowBrush", "#F4F7F8");
            SetBrush("PanelBrush", "#FFFFFF");
            SetBrush("InputBrush", "#FFFFFF");
            SetBrush("OutputBrush", "#EAF0F2");
            SetBrush("BorderBrush", "#B8C7CE");
            SetBrush("PrimaryTextBrush", "#17242A");
            SetBrush("MutedTextBrush", "#52636B");
            SetBrush("ButtonBrush", "#DCE7EA");
            ThemeToggle.Content = "Dark mode";
        }
        else
        {
            SetBrush("WindowBrush", "#161C24");
            SetBrush("PanelBrush", "#1D2631");
            SetBrush("InputBrush", "#26313D");
            SetBrush("OutputBrush", "#0F141A");
            SetBrush("BorderBrush", "#526579");
            SetBrush("PrimaryTextBrush", "#F2F5F7");
            SetBrush("MutedTextBrush", "#B2BECA");
            SetBrush("ButtonBrush", "#33404F");
            ThemeToggle.Content = "Light mode";
        }
    }

    private void SetBrush(string key, string color)
    {
        Resources[key] = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
    }
}