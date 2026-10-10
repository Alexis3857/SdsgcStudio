using System.ComponentModel;

namespace AssetStudioGUI.Sdsgc;

partial class BundleSearcherForm
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        _topbar = new System.Windows.Forms.Panel();
        _searchButton = new System.Windows.Forms.Button();
        _regexCheckBox = new System.Windows.Forms.CheckBox();
        _bundleNameCheckBox = new System.Windows.Forms.CheckBox();
        _fullNameCheckBox = new System.Windows.Forms.CheckBox();
        _nameCheckBox = new System.Windows.Forms.CheckBox();
        _queryTextBox = new System.Windows.Forms.TextBox();
        _resultsTreeView = new BundleSearcherForm.CheckBoxTreeView();
        _bottomBar = new System.Windows.Forms.Panel();
        _downloadButton = new System.Windows.Forms.Button();
        _statusLabel = new System.Windows.Forms.Label();
        _topbar.SuspendLayout();
        _bottomBar.SuspendLayout();
        SuspendLayout();
        // 
        // _topbar
        // 
        _topbar.Controls.Add(_searchButton);
        _topbar.Controls.Add(_regexCheckBox);
        _topbar.Controls.Add(_bundleNameCheckBox);
        _topbar.Controls.Add(_fullNameCheckBox);
        _topbar.Controls.Add(_nameCheckBox);
        _topbar.Controls.Add(_queryTextBox);
        _topbar.Dock = System.Windows.Forms.DockStyle.Top;
        _topbar.Location = new System.Drawing.Point(0, 0);
        _topbar.Name = "_topbar";
        _topbar.Size = new System.Drawing.Size(800, 31);
        _topbar.TabIndex = 0;
        // 
        // _searchButton
        // 
        _searchButton.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right));
        _searchButton.Enabled = false;
        _searchButton.Location = new System.Drawing.Point(713, 3);
        _searchButton.Name = "_searchButton";
        _searchButton.Size = new System.Drawing.Size(84, 25);
        _searchButton.TabIndex = 5;
        _searchButton.Text = "Search";
        _searchButton.UseVisualStyleBackColor = true;
        _searchButton.Click += SearchButton_Click;
        // 
        // _regexCheckBox
        // 
        _regexCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right));
        _regexCheckBox.AutoSize = true;
        _regexCheckBox.Location = new System.Drawing.Point(650, 6);
        _regexCheckBox.Name = "_regexCheckBox";
        _regexCheckBox.Size = new System.Drawing.Size(57, 19);
        _regexCheckBox.TabIndex = 4;
        _regexCheckBox.Text = "Regex";
        _regexCheckBox.UseVisualStyleBackColor = true;
        // 
        // _bundleNameCheckBox
        // 
        _bundleNameCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right));
        _bundleNameCheckBox.AutoSize = true;
        _bundleNameCheckBox.Location = new System.Drawing.Point(546, 6);
        _bundleNameCheckBox.Name = "_bundleNameCheckBox";
        _bundleNameCheckBox.Size = new System.Drawing.Size(98, 19);
        _bundleNameCheckBox.TabIndex = 3;
        _bundleNameCheckBox.Text = "Bundle Name";
        _bundleNameCheckBox.UseVisualStyleBackColor = true;
        _bundleNameCheckBox.Click += RefreshSearchButtonState;
        // 
        // _fullNameCheckBox
        // 
        _fullNameCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right));
        _fullNameCheckBox.AutoSize = true;
        _fullNameCheckBox.Location = new System.Drawing.Point(467, 6);
        _fullNameCheckBox.Name = "_fullNameCheckBox";
        _fullNameCheckBox.Size = new System.Drawing.Size(78, 19);
        _fullNameCheckBox.TabIndex = 2;
        _fullNameCheckBox.Text = "Container";
        _fullNameCheckBox.UseVisualStyleBackColor = true;
        _fullNameCheckBox.Click += RefreshSearchButtonState;
        // 
        // _nameCheckBox
        // 
        _nameCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right));
        _nameCheckBox.AutoSize = true;
        _nameCheckBox.Checked = true;
        _nameCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
        _nameCheckBox.Location = new System.Drawing.Point(403, 6);
        _nameCheckBox.Name = "_nameCheckBox";
        _nameCheckBox.Size = new System.Drawing.Size(58, 19);
        _nameCheckBox.TabIndex = 1;
        _nameCheckBox.Text = "Name";
        _nameCheckBox.UseVisualStyleBackColor = true;
        _nameCheckBox.Click += RefreshSearchButtonState;
        // 
        // _queryTextBox
        // 
        _queryTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
        _queryTextBox.Location = new System.Drawing.Point(3, 4);
        _queryTextBox.Name = "_queryTextBox";
        _queryTextBox.Size = new System.Drawing.Size(394, 23);
        _queryTextBox.TabIndex = 0;
        _queryTextBox.TextChanged += RefreshSearchButtonState;
        // 
        // _resultsTreeView
        // 
        _resultsTreeView.CheckBoxes = true;
        _resultsTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
        _resultsTreeView.HideSelection = false;
        _resultsTreeView.Location = new System.Drawing.Point(0, 31);
        _resultsTreeView.Name = "_resultsTreeView";
        _resultsTreeView.Size = new System.Drawing.Size(800, 382);
        _resultsTreeView.TabIndex = 1;
        _resultsTreeView.BeforeCheck += ResultsTreeView_BeforeCheck;
        _resultsTreeView.AfterCheck += ResultsTreeView_AfterCheck;
        // 
        // _bottomBar
        // 
        _bottomBar.Controls.Add(_downloadButton);
        _bottomBar.Controls.Add(_statusLabel);
        _bottomBar.Dock = System.Windows.Forms.DockStyle.Bottom;
        _bottomBar.Location = new System.Drawing.Point(0, 413);
        _bottomBar.Name = "_bottomBar";
        _bottomBar.Size = new System.Drawing.Size(800, 37);
        _bottomBar.TabIndex = 2;
        // 
        // _downloadButton
        // 
        _downloadButton.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right));
        _downloadButton.Enabled = false;
        _downloadButton.Location = new System.Drawing.Point(668, 6);
        _downloadButton.Name = "_downloadButton";
        _downloadButton.Size = new System.Drawing.Size(129, 25);
        _downloadButton.TabIndex = 1;
        _downloadButton.Text = "Download 0 bundles";
        _downloadButton.UseVisualStyleBackColor = true;
        // 
        // _statusLabel
        // 
        _statusLabel.AutoSize = true;
        _statusLabel.Location = new System.Drawing.Point(12, 11);
        _statusLabel.Name = "_statusLabel";
        _statusLabel.Size = new System.Drawing.Size(60, 15);
        _statusLabel.TabIndex = 0;
        _statusLabel.Text = "No results";
        // 
        // BundleSearcherForm
        // 
        AcceptButton = _searchButton;
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(800, 450);
        Controls.Add(_resultsTreeView);
        Controls.Add(_bottomBar);
        Controls.Add(_topbar);
        MinimumSize = new System.Drawing.Size(600, 300);
        ShowIcon = false;
        StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        Text = "Sdsgc Bundle Searcher";
        _topbar.ResumeLayout(false);
        _topbar.PerformLayout();
        _bottomBar.ResumeLayout(false);
        _bottomBar.PerformLayout();
        ResumeLayout(false);
    }

    private System.Windows.Forms.Panel _topbar;
    private System.Windows.Forms.TextBox _queryTextBox;
    private System.Windows.Forms.CheckBox _nameCheckBox;
    private System.Windows.Forms.CheckBox _fullNameCheckBox;
    private System.Windows.Forms.CheckBox _bundleNameCheckBox;
    private System.Windows.Forms.CheckBox _regexCheckBox;
    private System.Windows.Forms.Button _searchButton;

    private BundleSearcherForm.CheckBoxTreeView _resultsTreeView;

    private System.Windows.Forms.Panel _bottomBar;
    private System.Windows.Forms.Label _statusLabel;
    private System.Windows.Forms.Button _downloadButton;

    #endregion
}
