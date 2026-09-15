using SentinelAI.Helpers;
using SentinelAI.Services;

namespace SentinelAI;

public sealed class MainForm : Form
{
    private readonly ComboBox _operation = new();
    private readonly TextBox _path = new();
    private readonly RichTextBox _output = new();
    private readonly Label _status = new();

    public MainForm()
    {
        Text = "SentinelAI - Local Security Analysis";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(820, 560);
        Size = new Size(1080, 720);
        BackColor = Color.FromArgb(22, 28, 36);
        ForeColor = Color.WhiteSmoke;

        var heading = new Label
        {
            Text = "SENTINELAI",
            AutoSize = true,
            Font = new Font("Segoe UI", 20, FontStyle.Bold),
            ForeColor = Color.FromArgb(116, 220, 184),
            Location = new Point(28, 22)
        };

        var subtitle = new Label
        {
            Text = "Local threat analysis with private, persistent history",
            AutoSize = true,
            Font = new Font("Segoe UI", 10),
            ForeColor = Color.FromArgb(178, 190, 202),
            Location = new Point(31, 60)
        };

        _operation.DropDownStyle = ComboBoxStyle.DropDownList;
        _operation.Items.AddRange(new object[]
        {
            "Log analysis",
            "Script analysis",
            "YARA text scan",
            "Memory strings",
            "MITRE mapping",
            "Malware classification",
            "Incident investigation"
        });
        _operation.SelectedIndex = 0;
        _operation.BackColor = Color.FromArgb(38, 47, 59);
        _operation.ForeColor = Color.WhiteSmoke;
        _operation.Dock = DockStyle.Fill;

        _path.Dock = DockStyle.Fill;
        _path.BackColor = Color.FromArgb(38, 47, 59);
        _path.ForeColor = Color.WhiteSmoke;
        _path.BorderStyle = BorderStyle.FixedSingle;

        var browse = CreateButton("Browse", BrowseForFile);
        var analyze = CreateButton("Analyze", AnalyzeInput);
        analyze.BackColor = Color.FromArgb(35, 143, 115);

        _output.Dock = DockStyle.Fill;
        _output.ReadOnly = true;
        _output.BackColor = Color.FromArgb(15, 20, 26);
        _output.ForeColor = Color.FromArgb(222, 231, 237);
        _output.Font = new Font("Cascadia Mono", 10);
        _output.BorderStyle = BorderStyle.FixedSingle;

        _status.Text = "Ready. Choose an operation and an input file.";
        _status.AutoSize = true;
        _status.ForeColor = Color.FromArgb(178, 190, 202);
        _status.Dock = DockStyle.Fill;
        _status.TextAlign = ContentAlignment.MiddleLeft;

        var inputLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 92,
            ColumnCount = 3,
            RowCount = 2,
            Padding = new Padding(0, 16, 0, 8)
        };
        inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 190));
        inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        inputLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));
        inputLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        inputLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        inputLayout.Controls.Add(CreateLabel("Operation"), 0, 0);
        inputLayout.Controls.Add(_operation, 1, 0);
        inputLayout.Controls.Add(analyze, 2, 0);
        inputLayout.Controls.Add(CreateLabel("Input file"), 0, 1);
        inputLayout.Controls.Add(_path, 1, 1);
        inputLayout.Controls.Add(browse, 2, 1);

        var content = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Padding = new Padding(28, 0, 28, 22)
        };
        content.RowStyles.Add(new RowStyle(SizeType.Absolute, 92));
        content.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        content.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        content.Controls.Add(inputLayout, 0, 0);
        content.Controls.Add(_output, 0, 1);
        content.Controls.Add(_status, 0, 2);

        Controls.Add(content);
        Controls.Add(subtitle);
        Controls.Add(heading);
    }

    private static Label CreateLabel(string text) => new()
    {
        Text = text,
        AutoSize = true,
        TextAlign = ContentAlignment.MiddleLeft,
        Dock = DockStyle.Fill,
        ForeColor = Color.FromArgb(178, 190, 202),
        Padding = new Padding(0, 7, 0, 0)
    };

    private static Button CreateButton(string text, EventHandler handler)
    {
        var button = new Button
        {
            Text = text,
            Dock = DockStyle.Fill,
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(51, 64, 79),
            ForeColor = Color.WhiteSmoke,
            FlatAppearance = { BorderColor = Color.FromArgb(85, 104, 124) }
        };
        button.Click += handler;
        return button;
    }

    private void BrowseForFile(object? sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog { Title = "Choose evidence file" };
        if (dialog.ShowDialog(this) == DialogResult.OK)
            _path.Text = dialog.FileName;
    }

    private void AnalyzeInput(object? sender, EventArgs e)
    {
        try
        {
            var settings = ConfigurationLoader.Load();
            var knowledge = new LocalKnowledgeBase(settings.ReportsDirectory);
            var operation = _operation.SelectedItem?.ToString() ?? "Log analysis";
            var input = FileHelper.Read(_path.Text, settings.MaxInputBytes);
            var engine = new LocalAnalysisService(knowledge);
            string result;

            if (operation == "YARA text scan")
            {
                var findings = new YaraScanner().ScanFile(_path.Text, settings.MaxInputBytes);
                result = findings.Count == 0 ? "No deterministic signatures matched." : string.Join(Environment.NewLine, findings.Select(finding => $"- {finding}"));
            }
            else if (operation == "MITRE mapping")
            {
                result = engine.Map(File.ReadLines(_path.Text));
            }
            else if (operation == "Malware classification")
            {
                result = engine.Classify(File.ReadLines(_path.Text));
            }
            else
            {
                result = engine.Analyze(operation, input);
            }

            _output.Text = result;
            _status.Text = $"Completed {operation}. History is stored locally in {settings.ReportsDirectory}.";
        }
        catch (Exception exception)
        {
            _output.Text = exception.Message;
            _status.Text = "Analysis failed. Check the input path and file size.";
        }
    }
}