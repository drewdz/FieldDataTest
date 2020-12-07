namespace FieldDataTest
{
    partial class MainForm 
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
            this.MainImage = new System.Windows.Forms.PictureBox();
            this.MainStatus = new System.Windows.Forms.StatusStrip();
            this.Status = new System.Windows.Forms.ToolStripStatusLabel();
            this.StreamStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.SimStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.MainMenu = new System.Windows.Forms.MenuStrip();
            this.FileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.FileOpenMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.GetDataMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.ActionMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.StartMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.RunMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.PauseMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.ExitMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.SetupMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenFile = new System.Windows.Forms.OpenFileDialog();
            this.SaveFile = new System.Windows.Forms.SaveFileDialog();
            this.ClearDataMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            ((System.ComponentModel.ISupportInitialize)(this.MainImage)).BeginInit();
            this.MainStatus.SuspendLayout();
            this.MainMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainImage
            // 
            this.MainImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainImage.Location = new System.Drawing.Point(0, 24);
            this.MainImage.Name = "MainImage";
            this.MainImage.Padding = new System.Windows.Forms.Padding(3);
            this.MainImage.Size = new System.Drawing.Size(784, 323);
            this.MainImage.TabIndex = 0;
            this.MainImage.TabStop = false;
            this.MainImage.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MainImage_MouseUp);
            this.MainImage.Resize += new System.EventHandler(this.MainImage_Resize);
            // 
            // MainStatus
            // 
            this.MainStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Status,
            this.StreamStatus,
            this.SimStatus});
            this.MainStatus.Location = new System.Drawing.Point(0, 347);
            this.MainStatus.Name = "MainStatus";
            this.MainStatus.Size = new System.Drawing.Size(784, 22);
            this.MainStatus.TabIndex = 1;
            this.MainStatus.Text = "statusStrip1";
            // 
            // Status
            // 
            this.Status.Name = "Status";
            this.Status.Size = new System.Drawing.Size(529, 17);
            this.Status.Spring = true;
            this.Status.Text = "Test";
            // 
            // StreamStatus
            // 
            this.StreamStatus.AutoSize = false;
            this.StreamStatus.BackColor = System.Drawing.SystemColors.Control;
            this.StreamStatus.Name = "StreamStatus";
            this.StreamStatus.Size = new System.Drawing.Size(120, 17);
            this.StreamStatus.Text = "Not Streaming";
            // 
            // SimStatus
            // 
            this.SimStatus.AutoSize = false;
            this.SimStatus.BackColor = System.Drawing.SystemColors.Control;
            this.SimStatus.Name = "SimStatus";
            this.SimStatus.Size = new System.Drawing.Size(120, 17);
            this.SimStatus.Text = "Simulate Scanner";
            // 
            // MainMenu
            // 
            this.MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileMenu,
            this.ActionMenu,
            this.SetupMenu});
            this.MainMenu.Location = new System.Drawing.Point(0, 0);
            this.MainMenu.Name = "MainMenu";
            this.MainMenu.Size = new System.Drawing.Size(784, 24);
            this.MainMenu.TabIndex = 2;
            this.MainMenu.Text = "menuStrip1";
            // 
            // FileMenu
            // 
            this.FileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileOpenMenu,
            this.toolStripMenuItem2,
            this.GetDataMenu,
            this.ClearDataMenu});
            this.FileMenu.Name = "FileMenu";
            this.FileMenu.Size = new System.Drawing.Size(37, 20);
            this.FileMenu.Text = "File";
            // 
            // FileOpenMenu
            // 
            this.FileOpenMenu.Name = "FileOpenMenu";
            this.FileOpenMenu.Size = new System.Drawing.Size(180, 22);
            this.FileOpenMenu.Text = "Load Data";
            this.FileOpenMenu.Click += new System.EventHandler(this.FileOpenMenu_Click);
            // 
            // GetDataMenu
            // 
            this.GetDataMenu.Name = "GetDataMenu";
            this.GetDataMenu.Size = new System.Drawing.Size(180, 22);
            this.GetDataMenu.Text = "Get Data";
            this.GetDataMenu.Click += new System.EventHandler(this.GetDataMenu_Click);
            // 
            // ActionMenu
            // 
            this.ActionMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StartMenu,
            this.toolStripMenuItem5,
            this.RunMenu,
            this.PauseMenu,
            this.toolStripMenuItem1,
            this.ExitMenu});
            this.ActionMenu.Name = "ActionMenu";
            this.ActionMenu.Size = new System.Drawing.Size(59, 20);
            this.ActionMenu.Text = "Actions";
            // 
            // StartMenu
            // 
            this.StartMenu.Name = "StartMenu";
            this.StartMenu.Size = new System.Drawing.Size(163, 22);
            this.StartMenu.Text = "Start Stream";
            this.StartMenu.Click += new System.EventHandler(this.StartMenu_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(160, 6);
            // 
            // RunMenu
            // 
            this.RunMenu.Name = "RunMenu";
            this.RunMenu.Size = new System.Drawing.Size(163, 22);
            this.RunMenu.Text = "Start";
            this.RunMenu.Visible = false;
            this.RunMenu.Click += new System.EventHandler(this.RunMenu_Click);
            // 
            // PauseMenu
            // 
            this.PauseMenu.Name = "PauseMenu";
            this.PauseMenu.Size = new System.Drawing.Size(163, 22);
            this.PauseMenu.Text = "Use Scanner App";
            this.PauseMenu.Click += new System.EventHandler(this.PauseMenu_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(160, 6);
            // 
            // ExitMenu
            // 
            this.ExitMenu.Name = "ExitMenu";
            this.ExitMenu.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.ExitMenu.Size = new System.Drawing.Size(163, 22);
            this.ExitMenu.Text = "Exit";
            this.ExitMenu.Click += new System.EventHandler(this.ExitMenu_Click);
            // 
            // SetupMenu
            // 
            this.SetupMenu.Name = "SetupMenu";
            this.SetupMenu.Size = new System.Drawing.Size(92, 20);
            this.SetupMenu.Text = "Activity Setup";
            // 
            // ClearDataMenu
            // 
            this.ClearDataMenu.Name = "ClearDataMenu";
            this.ClearDataMenu.Size = new System.Drawing.Size(180, 22);
            this.ClearDataMenu.Text = "Clear Data";
            this.ClearDataMenu.Click += new System.EventHandler(this.ClearDataMenu_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(177, 6);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 369);
            this.Controls.Add(this.MainImage);
            this.Controls.Add(this.MainMenu);
            this.Controls.Add(this.MainStatus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MainMenuStrip = this.MainMenu;
            this.Name = "MainForm";
            this.Text = "Main";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.MainImage)).EndInit();
            this.MainStatus.ResumeLayout(false);
            this.MainStatus.PerformLayout();
            this.MainMenu.ResumeLayout(false);
            this.MainMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox MainImage;
        private System.Windows.Forms.StatusStrip MainStatus;
        private System.Windows.Forms.ToolStripStatusLabel Status;
        private System.Windows.Forms.MenuStrip MainMenu;
        private System.Windows.Forms.ToolStripMenuItem ActionMenu;
        private System.Windows.Forms.OpenFileDialog OpenFile;
        private System.Windows.Forms.SaveFileDialog SaveFile;
        private System.Windows.Forms.ToolStripStatusLabel StreamStatus;
        private System.Windows.Forms.ToolStripMenuItem StartMenu;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
        private System.Windows.Forms.ToolStripStatusLabel SimStatus;
        private System.Windows.Forms.ToolStripMenuItem RunMenu;
        private System.Windows.Forms.ToolStripMenuItem PauseMenu;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem ExitMenu;
        private System.Windows.Forms.ToolStripMenuItem SetupMenu;
        private System.Windows.Forms.ToolStripMenuItem FileMenu;
        private System.Windows.Forms.ToolStripMenuItem FileOpenMenu;
        private System.Windows.Forms.ToolStripMenuItem GetDataMenu;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem ClearDataMenu;
    }
}