using System.Drawing;
using System.Windows.Forms;

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
        headerPanel = new Panel();
        headerStatusLabel = new Label();
        headerSubtitleLabel = new Label();
        headerTitleLabel = new Label();
        targetCard = new UiCard();
        targetMetaLabel = new Label();
        targetTitleLabel = new Label();
        candidateLabel = new Label();
        targetProcessLabel = new Label();
        rebindHotKeyButton = new Button();
        toggleButton = new Button();
        settingsCard = new UiCard();
        hotKeyLabel = new Label();
        settingsHotkeyCaption = new Label();
        alwaysOnTopToggle = new ToggleSwitch();
        settingsAlwaysOnTopLabel = new Label();
        statusCard = new UiCard();
        statusTextBox = new TextBox();
        lastErrorLabel = new Label();
        enabledStatusLabel = new Label();
        SuspendLayout();
        // 
        // headerPanel
        // 
        headerPanel.BackColor = UiTheme.Card;
        headerPanel.Controls.Add(headerStatusLabel);
        headerPanel.Controls.Add(headerSubtitleLabel);
        headerPanel.Controls.Add(headerTitleLabel);
        headerPanel.Dock = DockStyle.Top;
        headerPanel.Location = new Point(0, 0);
        headerPanel.Name = "headerPanel";
        headerPanel.Size = new Size(812, 64);
        headerPanel.TabIndex = 9;
        // 
        // headerStatusLabel
        // 
        headerStatusLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        headerStatusLabel.AutoSize = true;
        headerStatusLabel.Font = UiTheme.Body(8.5f, FontStyle.Bold);
        headerStatusLabel.ForeColor = UiTheme.Muted;
        headerStatusLabel.Location = new Point(664, 24);
        headerStatusLabel.Name = "headerStatusLabel";
        headerStatusLabel.Size = new Size(110, 17);
        headerStatusLabel.TabIndex = 3;
        headerStatusLabel.Text = "● 待命 · STANDBY";
        // 
        // headerSubtitleLabel
        // 
        headerSubtitleLabel.AutoSize = true;
        headerSubtitleLabel.Font = UiTheme.Body(8.5f);
        headerSubtitleLabel.ForeColor = UiTheme.Muted;
        headerSubtitleLabel.Location = new Point(20, 40);
        headerSubtitleLabel.Name = "headerSubtitleLabel";
        headerSubtitleLabel.Size = new Size(96, 17);
        headerSubtitleLabel.TabIndex = 2;
        headerSubtitleLabel.Text = "目标窗口焦点保护";
        // 
        // headerTitleLabel
        // 
        headerTitleLabel.AutoSize = true;
        headerTitleLabel.Font = UiTheme.Display(15f, FontStyle.Bold);
        headerTitleLabel.ForeColor = UiTheme.Ink;
        headerTitleLabel.Location = new Point(20, 14);
        headerTitleLabel.Name = "headerTitleLabel";
        headerTitleLabel.Size = new Size(168, 24);
        headerTitleLabel.TabIndex = 1;
        headerTitleLabel.Text = "Game Focus Guard";
        // 
        // targetCard
        // 
        targetCard.BackColor = UiTheme.Card;
        targetCard.Controls.Add(targetMetaLabel);
        targetCard.Controls.Add(targetTitleLabel);
        targetCard.Controls.Add(candidateLabel);
        targetCard.Controls.Add(targetProcessLabel);
        targetCard.Controls.Add(rebindHotKeyButton);
        targetCard.Controls.Add(toggleButton);
        targetCard.Eyebrow = "TARGET WINDOW";
        targetCard.Location = new Point(20, 80);
        targetCard.Name = "targetCard";
        targetCard.Padding = new Padding(16, 44, 16, 14);
        targetCard.Size = new Size(460, 330);
        targetCard.TabIndex = 5;
        targetCard.Title = "目标窗口";
        // 
        // targetMetaLabel
        // 
        targetMetaLabel.Font = UiTheme.Mono(8.5f);
        targetMetaLabel.ForeColor = UiTheme.Muted;
        targetMetaLabel.Location = new Point(24, 152);
        targetMetaLabel.Name = "targetMetaLabel";
        targetMetaLabel.Size = new Size(416, 52);
        targetMetaLabel.TabIndex = 5;
        targetMetaLabel.Text = "HWND -- · PID -- · TID --";
        // 
        // targetTitleLabel
        // 
        targetTitleLabel.Font = UiTheme.Body(10.5f);
        targetTitleLabel.ForeColor = UiTheme.Muted;
        targetTitleLabel.Location = new Point(24, 94);
        targetTitleLabel.Name = "targetTitleLabel";
        targetTitleLabel.Size = new Size(416, 28);
        targetTitleLabel.TabIndex = 4;
        targetTitleLabel.Text = "切换到目标游戏窗口后锁定";
        // 
        // candidateLabel
        // 
        candidateLabel.Font = UiTheme.Body(8.5f);
        candidateLabel.ForeColor = UiTheme.Faint;
        candidateLabel.Location = new Point(24, 126);
        candidateLabel.Name = "candidateLabel";
        candidateLabel.Size = new Size(416, 22);
        candidateLabel.TabIndex = 3;
        candidateLabel.Text = "候选窗口: None";
        // 
        // targetProcessLabel
        // 
        targetProcessLabel.Font = UiTheme.Display(18f, FontStyle.Bold);
        targetProcessLabel.ForeColor = UiTheme.Ink;
        targetProcessLabel.Location = new Point(24, 54);
        targetProcessLabel.Name = "targetProcessLabel";
        targetProcessLabel.Size = new Size(416, 38);
        targetProcessLabel.TabIndex = 2;
        targetProcessLabel.Text = "等待前台窗口";
        targetProcessLabel.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // rebindHotKeyButton
        // 
        rebindHotKeyButton.Location = new Point(24, 266);
        rebindHotKeyButton.Name = "rebindHotKeyButton";
        rebindHotKeyButton.Size = new Size(196, 38);
        rebindHotKeyButton.TabIndex = 1;
        rebindHotKeyButton.Text = "改热键";
        rebindHotKeyButton.UseVisualStyleBackColor = true;
        rebindHotKeyButton.Click += rebindHotKeyButton_Click;
        // 
        // toggleButton
        // 
        toggleButton.Location = new Point(24, 214);
        toggleButton.Name = "toggleButton";
        toggleButton.Size = new Size(412, 44);
        toggleButton.TabIndex = 0;
        toggleButton.Text = "锁定当前前台窗口";
        toggleButton.UseVisualStyleBackColor = true;
        toggleButton.Click += toggleButton_Click;
        // 
        // settingsCard
        // 
        settingsCard.BackColor = UiTheme.Card;
        settingsCard.Controls.Add(hotKeyLabel);
        settingsCard.Controls.Add(settingsHotkeyCaption);
        settingsCard.Controls.Add(alwaysOnTopToggle);
        settingsCard.Controls.Add(settingsAlwaysOnTopLabel);
        settingsCard.Eyebrow = "SETTINGS";
        settingsCard.Location = new Point(496, 80);
        settingsCard.Name = "settingsCard";
        settingsCard.Padding = new Padding(16, 44, 16, 14);
        settingsCard.Size = new Size(296, 168);
        settingsCard.TabIndex = 7;
        settingsCard.Title = "快捷设置";
        // 
        // hotKeyLabel
        // 
        hotKeyLabel.Font = UiTheme.Body(10.5f, FontStyle.Bold);
        hotKeyLabel.ForeColor = UiTheme.Ink;
        hotKeyLabel.Location = new Point(20, 102);
        hotKeyLabel.Name = "hotKeyLabel";
        hotKeyLabel.Size = new Size(252, 30);
        hotKeyLabel.TabIndex = 3;
        hotKeyLabel.Text = "Ctrl+Shift+Alt+T";
        hotKeyLabel.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // settingsHotkeyCaption
        // 
        settingsHotkeyCaption.Font = UiTheme.Body(8f);
        settingsHotkeyCaption.ForeColor = UiTheme.Faint;
        settingsHotkeyCaption.Location = new Point(20, 82);
        settingsHotkeyCaption.Name = "settingsHotkeyCaption";
        settingsHotkeyCaption.Size = new Size(170, 20);
        settingsHotkeyCaption.TabIndex = 2;
        settingsHotkeyCaption.Text = "当前热键";
        // 
        // alwaysOnTopToggle
        // 
        alwaysOnTopToggle.BackColor = UiTheme.Card;
        alwaysOnTopToggle.Checked = false;
        alwaysOnTopToggle.Location = new Point(222, 40);
        alwaysOnTopToggle.Name = "alwaysOnTopToggle";
        alwaysOnTopToggle.Size = new Size(48, 28);
        alwaysOnTopToggle.TabIndex = 1;
        alwaysOnTopToggle.Text = "alwaysOnTopToggle";
        alwaysOnTopToggle.CheckedChanged += alwaysOnTopToggle_CheckedChanged;
        // 
        // settingsAlwaysOnTopLabel
        // 
        settingsAlwaysOnTopLabel.Font = UiTheme.Body(9.5f, FontStyle.Bold);
        settingsAlwaysOnTopLabel.ForeColor = UiTheme.Ink;
        settingsAlwaysOnTopLabel.Location = new Point(20, 46);
        settingsAlwaysOnTopLabel.Name = "settingsAlwaysOnTopLabel";
        settingsAlwaysOnTopLabel.Size = new Size(170, 26);
        settingsAlwaysOnTopLabel.TabIndex = 0;
        settingsAlwaysOnTopLabel.Text = "始终置顶";
        settingsAlwaysOnTopLabel.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // statusCard
        // 
        statusCard.BackColor = UiTheme.Card;
        statusCard.Controls.Add(statusTextBox);
        statusCard.Controls.Add(lastErrorLabel);
        statusCard.Controls.Add(enabledStatusLabel);
        statusCard.Eyebrow = "STATUS";
        statusCard.Location = new Point(496, 260);
        statusCard.Name = "statusCard";
        statusCard.Padding = new Padding(16, 44, 16, 14);
        statusCard.Size = new Size(296, 230);
        statusCard.TabIndex = 8;
        statusCard.Title = "运行状态";
        // 
        // statusTextBox
        // 
        statusTextBox.BackColor = UiTheme.Terminal;
        statusTextBox.BorderStyle = BorderStyle.None;
        statusTextBox.Font = UiTheme.Mono(8.5f);
        statusTextBox.ForeColor = UiTheme.TerminalText;
        statusTextBox.Location = new Point(20, 102);
        statusTextBox.Multiline = true;
        statusTextBox.Name = "statusTextBox";
        statusTextBox.ReadOnly = true;
        statusTextBox.ScrollBars = ScrollBars.Vertical;
        statusTextBox.Size = new Size(252, 110);
        statusTextBox.TabIndex = 2;
        statusTextBox.TabStop = false;
        // 
        // lastErrorLabel
        // 
        lastErrorLabel.Font = UiTheme.Body(8.5f);
        lastErrorLabel.ForeColor = UiTheme.Faint;
        lastErrorLabel.Location = new Point(20, 76);
        lastErrorLabel.Name = "lastErrorLabel";
        lastErrorLabel.Size = new Size(252, 22);
        lastErrorLabel.TabIndex = 1;
        lastErrorLabel.Text = "最近状态: Idle";
        // 
        // enabledStatusLabel
        // 
        enabledStatusLabel.Font = UiTheme.Body(10.5f, FontStyle.Bold);
        enabledStatusLabel.ForeColor = UiTheme.Muted;
        enabledStatusLabel.Location = new Point(20, 46);
        enabledStatusLabel.Name = "enabledStatusLabel";
        enabledStatusLabel.Size = new Size(252, 28);
        enabledStatusLabel.TabIndex = 0;
        enabledStatusLabel.Text = "● 待命 · STANDBY";
        enabledStatusLabel.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(7F, 17F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = UiTheme.Background;
        ClientSize = new Size(812, 536);
        Controls.Add(statusCard);
        Controls.Add(settingsCard);
        Controls.Add(targetCard);
        Controls.Add(headerPanel);
        Font = UiTheme.Body(9f);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        KeyPreview = true;
        MaximizeBox = false;
        MinimizeBox = true;
        Name = "Form1";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Game Focus Guard";
        FormClosing += Form1_FormClosing;
        Load += Form1_Load;
        UiTheme.StylePrimaryButton(toggleButton);
        UiTheme.StyleSecondaryButton(rebindHotKeyButton);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Panel headerPanel;
    private Label headerStatusLabel;
    private Label headerSubtitleLabel;
    private Label headerTitleLabel;
    private UiCard targetCard;
    private Label targetMetaLabel;
    private Label targetTitleLabel;
    private Label candidateLabel;
    private Label targetProcessLabel;
    private Button rebindHotKeyButton;
    private Button toggleButton;
    private UiCard settingsCard;
    private Label hotKeyLabel;
    private Label settingsHotkeyCaption;
    private ToggleSwitch alwaysOnTopToggle;
    private Label settingsAlwaysOnTopLabel;
    private UiCard statusCard;
    private TextBox statusTextBox;
    private Label lastErrorLabel;
    private Label enabledStatusLabel;
}
