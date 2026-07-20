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
        alwaysOnTopCheckBox = new CheckBox();
        hotKeyLabel = new Label();
        statusTextBox = new TextBox();
        candidateLabel = new Label();
        SuspendLayout();
        // 
        // toggleButton
        // 
        toggleButton.Location = new Point(12, 12);
        toggleButton.Name = "toggleButton";
        toggleButton.Size = new Size(418, 44);
        toggleButton.TabIndex = 0;
        toggleButton.Text = "对候选前台窗口启用/关闭防失焦";
        toggleButton.UseVisualStyleBackColor = true;
        toggleButton.Click += toggleButton_Click;
        // 
        // rebindHotKeyButton
        // 
        rebindHotKeyButton.Location = new Point(436, 12);
        rebindHotKeyButton.Name = "rebindHotKeyButton";
        rebindHotKeyButton.Size = new Size(188, 44);
        rebindHotKeyButton.TabIndex = 1;
        rebindHotKeyButton.Text = "改热键";
        rebindHotKeyButton.UseVisualStyleBackColor = true;
        rebindHotKeyButton.Click += rebindHotKeyButton_Click;
        //
        // alwaysOnTopCheckBox
        //
        alwaysOnTopCheckBox.AutoSize = true;
        alwaysOnTopCheckBox.Location = new Point(522, 66);
        alwaysOnTopCheckBox.Name = "alwaysOnTopCheckBox";
        alwaysOnTopCheckBox.Size = new Size(75, 21);
        alwaysOnTopCheckBox.TabIndex = 2;
        alwaysOnTopCheckBox.Text = "始终置顶";
        alwaysOnTopCheckBox.UseVisualStyleBackColor = true;
        alwaysOnTopCheckBox.CheckedChanged += alwaysOnTopCheckBox_CheckedChanged;
        // 
        // hotKeyLabel
        // 
        hotKeyLabel.AutoSize = true;
        hotKeyLabel.Location = new Point(12, 67);
        hotKeyLabel.Name = "hotKeyLabel";
        hotKeyLabel.Size = new Size(145, 17);
        hotKeyLabel.TabIndex = 3;
        hotKeyLabel.Text = "当前热键: Ctrl+Shift+Alt+T";
        // 
        // statusTextBox
        // 
        statusTextBox.Location = new Point(12, 116);
        statusTextBox.Multiline = true;
        statusTextBox.Name = "statusTextBox";
        statusTextBox.ReadOnly = true;
        statusTextBox.ScrollBars = ScrollBars.Vertical;
        statusTextBox.Size = new Size(612, 258);
        statusTextBox.TabIndex = 5;
        // 
        // candidateLabel
        // 
        candidateLabel.AutoSize = true;
        candidateLabel.Location = new Point(12, 92);
        candidateLabel.Name = "candidateLabel";
        candidateLabel.Size = new Size(96, 17);
        candidateLabel.TabIndex = 4;
        candidateLabel.Text = "候选窗口: None";
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(7F, 17F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(636, 386);
        Controls.Add(alwaysOnTopCheckBox);
        Controls.Add(hotKeyLabel);
        Controls.Add(rebindHotKeyButton);
        Controls.Add(candidateLabel);
        Controls.Add(statusTextBox);
        Controls.Add(toggleButton);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        KeyPreview = true;
        MaximizeBox = false;
        MinimizeBox = true;
        Name = "Form1";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Game Focus Guard";
        FormClosing += Form1_FormClosing;
        Load += Form1_Load;
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Button toggleButton;
    private Button rebindHotKeyButton;
    private CheckBox alwaysOnTopCheckBox;
    private Label hotKeyLabel;
    private TextBox statusTextBox;
    private Label candidateLabel;
}
