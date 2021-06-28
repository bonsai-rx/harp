
namespace Bonsai.Harp.Design
{
    partial class DeviceConfigurationDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DeviceConfigurationDialog));
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.connectionStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.deviceInfoGroupBox = new System.Windows.Forms.GroupBox();
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.bootloaderButton = new System.Windows.Forms.Button();
            this.bootloaderGroupBox = new System.Windows.Forms.GroupBox();
            this.resetNameButton = new System.Windows.Forms.Button();
            this.resetSettingsButton = new System.Windows.Forms.Button();
            this.firmwarePropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.warningTextBox = new System.Windows.Forms.RichTextBox();
            this.selectFirmwareButton = new System.Windows.Forms.Button();
            this.updateFirmwareButton = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.statusStrip.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            this.deviceInfoGroupBox.SuspendLayout();
            this.bootloaderGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectionStatusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 227);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(832, 26);
            this.statusStrip.TabIndex = 0;
            this.statusStrip.Text = "statusStrip1";
            // 
            // connectionStatusLabel
            // 
            this.connectionStatusLabel.Name = "connectionStatusLabel";
            this.connectionStatusLabel.Size = new System.Drawing.Size(50, 20);
            this.connectionStatusLabel.Text = "Ready";
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 37F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 63F));
            this.tableLayoutPanel.Controls.Add(this.deviceInfoGroupBox, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.bootloaderGroupBox, 1, 0);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 1;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(832, 227);
            this.tableLayoutPanel.TabIndex = 1;
            // 
            // deviceInfoGroupBox
            // 
            this.deviceInfoGroupBox.Controls.Add(this.propertyGrid);
            this.deviceInfoGroupBox.Controls.Add(this.bootloaderButton);
            this.deviceInfoGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.deviceInfoGroupBox.Location = new System.Drawing.Point(3, 3);
            this.deviceInfoGroupBox.Name = "deviceInfoGroupBox";
            this.deviceInfoGroupBox.Size = new System.Drawing.Size(301, 221);
            this.deviceInfoGroupBox.TabIndex = 0;
            this.deviceInfoGroupBox.TabStop = false;
            this.deviceInfoGroupBox.Text = "Device Info";
            // 
            // propertyGrid
            // 
            this.propertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGrid.CommandsVisibleIfAvailable = false;
            this.propertyGrid.HelpVisible = false;
            this.propertyGrid.Location = new System.Drawing.Point(9, 21);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.propertyGrid.Size = new System.Drawing.Size(286, 154);
            this.propertyGrid.TabIndex = 1;
            this.propertyGrid.ToolbarVisible = false;
            // 
            // bootloaderButton
            // 
            this.bootloaderButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bootloaderButton.Location = new System.Drawing.Point(182, 181);
            this.bootloaderButton.Name = "bootloaderButton";
            this.bootloaderButton.Size = new System.Drawing.Size(113, 34);
            this.bootloaderButton.TabIndex = 0;
            this.bootloaderButton.Text = "Bootloader >>";
            this.bootloaderButton.UseVisualStyleBackColor = true;
            this.bootloaderButton.Click += new System.EventHandler(this.bootloaderButton_Click);
            // 
            // bootloaderGroupBox
            // 
            this.bootloaderGroupBox.Controls.Add(this.resetNameButton);
            this.bootloaderGroupBox.Controls.Add(this.resetSettingsButton);
            this.bootloaderGroupBox.Controls.Add(this.firmwarePropertyGrid);
            this.bootloaderGroupBox.Controls.Add(this.warningTextBox);
            this.bootloaderGroupBox.Controls.Add(this.selectFirmwareButton);
            this.bootloaderGroupBox.Controls.Add(this.updateFirmwareButton);
            this.bootloaderGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bootloaderGroupBox.Location = new System.Drawing.Point(310, 3);
            this.bootloaderGroupBox.Name = "bootloaderGroupBox";
            this.bootloaderGroupBox.Size = new System.Drawing.Size(519, 221);
            this.bootloaderGroupBox.TabIndex = 1;
            this.bootloaderGroupBox.TabStop = false;
            this.bootloaderGroupBox.Text = "Update Firmware";
            // 
            // resetNameButton
            // 
            this.resetNameButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.resetNameButton.Location = new System.Drawing.Point(365, 21);
            this.resetNameButton.Name = "resetNameButton";
            this.resetNameButton.Size = new System.Drawing.Size(145, 48);
            this.resetNameButton.TabIndex = 6;
            this.resetNameButton.Text = "Reset Device Name...";
            this.resetNameButton.UseVisualStyleBackColor = true;
            // 
            // resetSettingsButton
            // 
            this.resetSettingsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.resetSettingsButton.Location = new System.Drawing.Point(365, 75);
            this.resetSettingsButton.Name = "resetSettingsButton";
            this.resetSettingsButton.Size = new System.Drawing.Size(145, 48);
            this.resetSettingsButton.TabIndex = 5;
            this.resetSettingsButton.Text = "Reset Device Settings...";
            this.resetSettingsButton.UseVisualStyleBackColor = true;
            // 
            // firmwarePropertyGrid
            // 
            this.firmwarePropertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.firmwarePropertyGrid.CommandsVisibleIfAvailable = false;
            this.firmwarePropertyGrid.HelpVisible = false;
            this.firmwarePropertyGrid.Location = new System.Drawing.Point(6, 21);
            this.firmwarePropertyGrid.Name = "firmwarePropertyGrid";
            this.firmwarePropertyGrid.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.firmwarePropertyGrid.Size = new System.Drawing.Size(353, 154);
            this.firmwarePropertyGrid.TabIndex = 4;
            this.firmwarePropertyGrid.ToolbarVisible = false;
            // 
            // warningTextBox
            // 
            this.warningTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.warningTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.warningTextBox.Location = new System.Drawing.Point(6, 181);
            this.warningTextBox.Name = "warningTextBox";
            this.warningTextBox.ReadOnly = true;
            this.warningTextBox.Size = new System.Drawing.Size(353, 34);
            this.warningTextBox.TabIndex = 3;
            this.warningTextBox.Text = "Caution: Selecting a firmware that was not designed for this Harp device may caus" +
    "e permanent damage.";
            // 
            // selectFirmwareButton
            // 
            this.selectFirmwareButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.selectFirmwareButton.Location = new System.Drawing.Point(362, 181);
            this.selectFirmwareButton.Name = "selectFirmwareButton";
            this.selectFirmwareButton.Size = new System.Drawing.Size(71, 34);
            this.selectFirmwareButton.TabIndex = 2;
            this.selectFirmwareButton.Text = "Open...";
            this.selectFirmwareButton.UseVisualStyleBackColor = true;
            this.selectFirmwareButton.Click += new System.EventHandler(this.selectFirmwareButton_Click);
            // 
            // updateFirmwareButton
            // 
            this.updateFirmwareButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.updateFirmwareButton.Enabled = false;
            this.updateFirmwareButton.Location = new System.Drawing.Point(439, 181);
            this.updateFirmwareButton.Name = "updateFirmwareButton";
            this.updateFirmwareButton.Size = new System.Drawing.Size(71, 34);
            this.updateFirmwareButton.TabIndex = 1;
            this.updateFirmwareButton.Text = "Update";
            this.updateFirmwareButton.UseVisualStyleBackColor = true;
            this.updateFirmwareButton.Click += new System.EventHandler(this.updateFirmwareButton_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "HEX files|*.hex|All files|*.*";
            // 
            // DeviceConfigurationDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(832, 253);
            this.Controls.Add(this.tableLayoutPanel);
            this.Controls.Add(this.statusStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(850, 300);
            this.Name = "DeviceConfigurationDialog";
            this.Text = "Device Setup";
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.tableLayoutPanel.ResumeLayout(false);
            this.deviceInfoGroupBox.ResumeLayout(false);
            this.bootloaderGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel connectionStatusLabel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.GroupBox deviceInfoGroupBox;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Button bootloaderButton;
        private System.Windows.Forms.GroupBox bootloaderGroupBox;
        private System.Windows.Forms.RichTextBox warningTextBox;
        private System.Windows.Forms.Button selectFirmwareButton;
        private System.Windows.Forms.Button updateFirmwareButton;
        private System.Windows.Forms.PropertyGrid propertyGrid;
        private System.Windows.Forms.PropertyGrid firmwarePropertyGrid;
        private System.Windows.Forms.Button resetNameButton;
        private System.Windows.Forms.Button resetSettingsButton;
    }
}