namespace FocusTool.Ui;

partial class Form1
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        toggleButton = new Button();
        rebindHotKeyButton = new Button();
        hotKeyLabel = new Label();
        statusTextBox = new TextBox();
        candidateLabel = new Label();
        languageLabel = new Label();
        languageComboBox = new ComboBox();
        trayButton = new Button();
        contextMenuStrip = new ContextMenuStrip(components);
        trayMenuRestore = new ToolStripMenuItem();
        trayMenuSeparator = new ToolStripSeparator();
        trayMenuQuit = new ToolStripMenuItem();
        notifyIcon = new NotifyIcon(components);
        SuspendLayout();
        //
        // toggleButton
        //
        toggleButton.Location = new Point(12, 12);
        toggleButton.Name = "toggleButton";
        toggleButton.Size = new Size(418, 44);
        toggleButton.TabIndex = 0;
        toggleButton.Text = Strings.ToggleButton;
        toggleButton.UseVisualStyleBackColor = true;
        toggleButton.Click += toggleButton_Click;
        //
        // rebindHotKeyButton
        //
        rebindHotKeyButton.Location = new Point(436, 12);
        rebindHotKeyButton.Name = "rebindHotKeyButton";
        rebindHotKeyButton.Size = new Size(188, 44);
        rebindHotKeyButton.TabIndex = 1;
        rebindHotKeyButton.Text = Strings.RebindHotKeyButton;
        rebindHotKeyButton.UseVisualStyleBackColor = true;
        rebindHotKeyButton.Click += rebindHotKeyButton_Click;
        //
        // hotKeyLabel
        //
        hotKeyLabel.AutoSize = true;
        hotKeyLabel.Location = new Point(12, 67);
        hotKeyLabel.Name = "hotKeyLabel";
        hotKeyLabel.Size = new Size(145, 17);
        hotKeyLabel.TabIndex = 2;
        hotKeyLabel.Text = $"{Strings.CurrentHotKeyPrefix}: Ctrl+Shift+Alt+T";
        //
        // candidateLabel
        //
        candidateLabel.AutoSize = true;
        candidateLabel.Location = new Point(12, 92);
        candidateLabel.Name = "candidateLabel";
        candidateLabel.Size = new Size(96, 17);
        candidateLabel.TabIndex = 4;
        candidateLabel.Text = $"{Strings.CandidateWindowPrefix}: {Strings.None}";
        //
        // languageLabel
        //
        languageLabel.AutoSize = true;
        languageLabel.Location = new Point(12, 119);
        languageLabel.Name = "languageLabel";
        languageLabel.TabIndex = 5;
        languageLabel.Text = Strings.LanguageLabel;
        //
        // languageComboBox
        //
        languageComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        languageComboBox.Location = new Point(88, 115);
        languageComboBox.Name = "languageComboBox";
        languageComboBox.Size = new Size(148, 23);
        languageComboBox.TabIndex = 6;
        languageComboBox.Items.AddRange(new object[] { "English", "Français", "中文" });
        languageComboBox.SelectedIndex = 0;
        languageComboBox.SelectedIndexChanged += languageComboBox_SelectedIndexChanged;
        //
        // trayButton
        //
        trayButton.Location = new Point(440, 115);
        trayButton.Name = "trayButton";
        trayButton.Size = new Size(184, 23);
        trayButton.TabIndex = 7;
        trayButton.Text = Strings.TrayButton;
        trayButton.UseVisualStyleBackColor = true;
        trayButton.Click += trayButton_Click;
        //
        // contextMenuStrip (tray right-click menu)
        //
        trayMenuRestore.Text = Strings.TrayMenuRestore;
        trayMenuRestore.Font = new Font(trayMenuRestore.Font, FontStyle.Bold);
        trayMenuRestore.Click += trayMenuRestore_Click;
        trayMenuQuit.Text = Strings.TrayMenuQuit;
        trayMenuQuit.Click += trayMenuQuit_Click;
        contextMenuStrip.Items.AddRange(new ToolStripItem[]
        {
            trayMenuRestore,
            trayMenuSeparator,
            trayMenuQuit
        });
        //
        // notifyIcon
        //
        notifyIcon.ContextMenuStrip = contextMenuStrip;
        notifyIcon.Text = Strings.TrayTooltip;
        // L'icône est chargée dans Form1_Load depuis icon.ico
        notifyIcon.Visible = false;
        notifyIcon.DoubleClick += notifyIcon_DoubleClick;
        //
        // statusTextBox
        //
        statusTextBox.Location = new Point(12, 144);
        statusTextBox.Multiline = true;
        statusTextBox.Name = "statusTextBox";
        statusTextBox.ReadOnly = true;
        statusTextBox.ScrollBars = ScrollBars.Vertical;
        statusTextBox.Size = new Size(612, 258);
        statusTextBox.TabIndex = 3;
        //
        // Form1
        //
        AutoScaleDimensions = new SizeF(7F, 17F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(636, 414);
        Controls.Add(hotKeyLabel);
        Controls.Add(rebindHotKeyButton);
        Controls.Add(candidateLabel);
        Controls.Add(languageLabel);
        Controls.Add(languageComboBox);
        Controls.Add(trayButton);
        Controls.Add(statusTextBox);
        Controls.Add(toggleButton);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        KeyPreview = true;
        MaximizeBox = false;
        MinimizeBox = true;
        Name = "Form1";
        StartPosition = FormStartPosition.CenterScreen;
        Text = Strings.AppTitle;
        FormClosing += Form1_FormClosing;
        Load += Form1_Load;
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Button toggleButton;
    private Button rebindHotKeyButton;
    private Label hotKeyLabel;
    private TextBox statusTextBox;
    private Label candidateLabel;
    private Label languageLabel;
    private ComboBox languageComboBox;
    private Button trayButton;
    private ContextMenuStrip contextMenuStrip;
    private ToolStripMenuItem trayMenuRestore;
    private ToolStripSeparator trayMenuSeparator;
    private ToolStripMenuItem trayMenuQuit;
    private NotifyIcon notifyIcon;
}
