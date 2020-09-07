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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.ActionMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.FieldMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.StartMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.OpenFile = new System.Windows.Forms.OpenFileDialog();
            this.SaveFile = new System.Windows.Forms.SaveFileDialog();
            this.RunMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.PauseMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.SimStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.ExitMenu = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.MainImage)).BeginInit();
            this.MainStatus.SuspendLayout();
            this.menuStrip1.SuspendLayout();
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
            this.Status.Size = new System.Drawing.Size(498, 17);
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
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ActionMenu});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(784, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // ActionMenu
            // 
            this.ActionMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FieldMenu,
            this.toolStripMenuItem2,
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
            // FieldMenu
            // 
            this.FieldMenu.Name = "FieldMenu";
            this.FieldMenu.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.FieldMenu.Size = new System.Drawing.Size(180, 22);
            this.FieldMenu.Text = "Load Field";
            this.FieldMenu.Visible = false;
            this.FieldMenu.Click += new System.EventHandler(this.FieldMenu_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(192, 6);
            this.toolStripMenuItem2.Visible = false;
            // 
            // StartMenu
            // 
            this.StartMenu.Name = "StartMenu";
            this.StartMenu.Size = new System.Drawing.Size(195, 22);
            this.StartMenu.Text = "Start Stream";
            this.StartMenu.Click += new System.EventHandler(this.StartMenu_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(192, 6);
            // 
            // RunMenu
            // 
            this.RunMenu.Name = "RunMenu";
            this.RunMenu.Size = new System.Drawing.Size(180, 22);
            this.RunMenu.Text = "Start";
            this.RunMenu.Visible = false;
            this.RunMenu.Click += new System.EventHandler(this.RunMenu_Click);
            // 
            // PauseMenu
            // 
            this.PauseMenu.Name = "PauseMenu";
            this.PauseMenu.Size = new System.Drawing.Size(180, 22);
            this.PauseMenu.Text = "Use Scanner App";
            this.PauseMenu.Click += new System.EventHandler(this.PauseMenu_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(192, 6);
            // 
            // SimStatus
            // 
            this.SimStatus.AutoSize = false;
            this.SimStatus.BackColor = System.Drawing.SystemColors.Control;
            this.SimStatus.Name = "SimStatus";
            this.SimStatus.Size = new System.Drawing.Size(120, 17);
            this.SimStatus.Text = "Simulate Scanner";
            // 
            // ExitMenu
            // 
            this.ExitMenu.Name = "ExitMenu";
            this.ExitMenu.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.ExitMenu.Size = new System.Drawing.Size(195, 22);
            this.ExitMenu.Text = "Exit";
            this.ExitMenu.Click += new System.EventHandler(this.ExitMenu_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 369);
            this.Controls.Add(this.MainImage);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.MainStatus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Main";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.MainImage)).EndInit();
            this.MainStatus.ResumeLayout(false);
            this.MainStatus.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox MainImage;
        private System.Windows.Forms.StatusStrip MainStatus;
        private System.Windows.Forms.ToolStripStatusLabel Status;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem ActionMenu;
        private System.Windows.Forms.ToolStripMenuItem FieldMenu;
        private System.Windows.Forms.OpenFileDialog OpenFile;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.SaveFileDialog SaveFile;
        private System.Windows.Forms.ToolStripStatusLabel StreamStatus;
        private System.Windows.Forms.ToolStripMenuItem StartMenu;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
        private System.Windows.Forms.ToolStripStatusLabel SimStatus;
        private System.Windows.Forms.ToolStripMenuItem RunMenu;
        private System.Windows.Forms.ToolStripMenuItem PauseMenu;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem ExitMenu;
    }
}