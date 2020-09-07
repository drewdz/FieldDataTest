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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.ActionMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.FieldMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.GenerateMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.LoadDataMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.SideViewMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.ResetMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.ClearTrackingMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.streamToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.StartMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.StopMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.StreamFileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.ExecuteMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.RunMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.StartWaitMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.SimMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.GetPlayerMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.GetPlayerActivityMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.ViewActivitiesMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.SettingsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.BaseAddressMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.timeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenFile = new System.Windows.Forms.OpenFileDialog();
            this.SaveFile = new System.Windows.Forms.SaveFileDialog();
            this.PauseMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
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
            this.Status});
            this.MainStatus.Location = new System.Drawing.Point(0, 347);
            this.MainStatus.Name = "MainStatus";
            this.MainStatus.Size = new System.Drawing.Size(784, 22);
            this.MainStatus.TabIndex = 1;
            this.MainStatus.Text = "statusStrip1";
            // 
            // Status
            // 
            this.Status.Name = "Status";
            this.Status.Size = new System.Drawing.Size(769, 17);
            this.Status.Spring = true;
            this.Status.Text = "Test";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ActionMenu,
            this.streamToolStripMenuItem,
            this.ExecuteMenu,
            this.SimMenu,
            this.SettingsMenu});
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
            this.GenerateMenu,
            this.LoadDataMenu,
            this.toolStripMenuItem1,
            this.SideViewMenu,
            this.toolStripSeparator1,
            this.ResetMenu,
            this.ClearTrackingMenu,
            this.toolStripMenuItem2});
            this.ActionMenu.Name = "ActionMenu";
            this.ActionMenu.Size = new System.Drawing.Size(59, 20);
            this.ActionMenu.Text = "Actions";
            // 
            // FieldMenu
            // 
            this.FieldMenu.Name = "FieldMenu";
            this.FieldMenu.Size = new System.Drawing.Size(172, 22);
            this.FieldMenu.Text = "Load Field";
            this.FieldMenu.Click += new System.EventHandler(this.FieldMenu_Click);
            // 
            // GenerateMenu
            // 
            this.GenerateMenu.Name = "GenerateMenu";
            this.GenerateMenu.Size = new System.Drawing.Size(172, 22);
            this.GenerateMenu.Text = "Generate Data";
            this.GenerateMenu.Click += new System.EventHandler(this.GenerateMenu_Click);
            // 
            // LoadDataMenu
            // 
            this.LoadDataMenu.Name = "LoadDataMenu";
            this.LoadDataMenu.Size = new System.Drawing.Size(172, 22);
            this.LoadDataMenu.Text = "Load Data";
            this.LoadDataMenu.Click += new System.EventHandler(this.LoadDataMenu_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(169, 6);
            // 
            // SideViewMenu
            // 
            this.SideViewMenu.Name = "SideViewMenu";
            this.SideViewMenu.Size = new System.Drawing.Size(172, 22);
            this.SideViewMenu.Text = "Side View";
            this.SideViewMenu.Click += new System.EventHandler(this.SideViewMenu_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(169, 6);
            // 
            // ResetMenu
            // 
            this.ResetMenu.Name = "ResetMenu";
            this.ResetMenu.Size = new System.Drawing.Size(172, 22);
            this.ResetMenu.Text = "Reset";
            this.ResetMenu.Click += new System.EventHandler(this.ResetMenu_Click);
            // 
            // ClearTrackingMenu
            // 
            this.ClearTrackingMenu.Name = "ClearTrackingMenu";
            this.ClearTrackingMenu.Size = new System.Drawing.Size(172, 22);
            this.ClearTrackingMenu.Text = "Clear TrackingData";
            this.ClearTrackingMenu.Click += new System.EventHandler(this.ClearTrackingMenu_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(169, 6);
            // 
            // streamToolStripMenuItem
            // 
            this.streamToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StartMenu,
            this.StopMenu,
            this.toolStripMenuItem5,
            this.StreamFileMenu});
            this.streamToolStripMenuItem.Name = "streamToolStripMenuItem";
            this.streamToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
            this.streamToolStripMenuItem.Text = "Stream";
            // 
            // StartMenu
            // 
            this.StartMenu.Name = "StartMenu";
            this.StartMenu.Size = new System.Drawing.Size(147, 22);
            this.StartMenu.Text = "Start Stream";
            this.StartMenu.Click += new System.EventHandler(this.StartMenu_Click);
            // 
            // StopMenu
            // 
            this.StopMenu.Name = "StopMenu";
            this.StopMenu.Size = new System.Drawing.Size(147, 22);
            this.StopMenu.Text = "Stop Stream";
            this.StopMenu.Click += new System.EventHandler(this.StopMenu_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(144, 6);
            // 
            // StreamFileMenu
            // 
            this.StreamFileMenu.Name = "StreamFileMenu";
            this.StreamFileMenu.Size = new System.Drawing.Size(147, 22);
            this.StreamFileMenu.Text = "Stream To File";
            this.StreamFileMenu.Click += new System.EventHandler(this.StreamFileMenu_Click);
            // 
            // ExecuteMenu
            // 
            this.ExecuteMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.RunMenu,
            this.StartWaitMenu,
            this.toolStripMenuItem3,
            this.PauseMenu,
            this.toolStripMenuItem6});
            this.ExecuteMenu.Name = "ExecuteMenu";
            this.ExecuteMenu.Size = new System.Drawing.Size(60, 20);
            this.ExecuteMenu.Text = "Execute";
            // 
            // RunMenu
            // 
            this.RunMenu.Name = "RunMenu";
            this.RunMenu.Size = new System.Drawing.Size(180, 22);
            this.RunMenu.Text = "Start No Scanner";
            this.RunMenu.Click += new System.EventHandler(this.RunMenu_Click);
            // 
            // StartWaitMenu
            // 
            this.StartWaitMenu.Name = "StartWaitMenu";
            this.StartWaitMenu.Size = new System.Drawing.Size(180, 22);
            this.StartWaitMenu.Text = "Start With Scanner";
            this.StartWaitMenu.Click += new System.EventHandler(this.RunMenu_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(177, 6);
            // 
            // SimMenu
            // 
            this.SimMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.GetPlayerMenu,
            this.GetPlayerActivityMenu,
            this.toolStripMenuItem4,
            this.ViewActivitiesMenu});
            this.SimMenu.Name = "SimMenu";
            this.SimMenu.Size = new System.Drawing.Size(39, 20);
            this.SimMenu.Text = "Sim";
            // 
            // GetPlayerMenu
            // 
            this.GetPlayerMenu.Name = "GetPlayerMenu";
            this.GetPlayerMenu.Size = new System.Drawing.Size(170, 22);
            this.GetPlayerMenu.Text = "Get Player";
            this.GetPlayerMenu.Click += new System.EventHandler(this.GetPlayerMenu_Click);
            // 
            // GetPlayerActivityMenu
            // 
            this.GetPlayerActivityMenu.Name = "GetPlayerActivityMenu";
            this.GetPlayerActivityMenu.Size = new System.Drawing.Size(170, 22);
            this.GetPlayerActivityMenu.Text = "Get Player Activity";
            this.GetPlayerActivityMenu.Click += new System.EventHandler(this.GetPlayerActivityMenu_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(167, 6);
            // 
            // ViewActivitiesMenu
            // 
            this.ViewActivitiesMenu.Name = "ViewActivitiesMenu";
            this.ViewActivitiesMenu.Size = new System.Drawing.Size(170, 22);
            this.ViewActivitiesMenu.Text = "View Activities";
            this.ViewActivitiesMenu.Click += new System.EventHandler(this.ViewActivitiesMenu_Click);
            // 
            // SettingsMenu
            // 
            this.SettingsMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.BaseAddressMenu,
            this.timeToolStripMenuItem});
            this.SettingsMenu.Name = "SettingsMenu";
            this.SettingsMenu.Size = new System.Drawing.Size(61, 20);
            this.SettingsMenu.Text = "Settings";
            // 
            // BaseAddressMenu
            // 
            this.BaseAddressMenu.Name = "BaseAddressMenu";
            this.BaseAddressMenu.Size = new System.Drawing.Size(143, 22);
            this.BaseAddressMenu.Text = "Base Address";
            this.BaseAddressMenu.Click += new System.EventHandler(this.BaseAddressMenu_Click);
            // 
            // timeToolStripMenuItem
            // 
            this.timeToolStripMenuItem.Name = "timeToolStripMenuItem";
            this.timeToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.timeToolStripMenuItem.Text = "Time";
            this.timeToolStripMenuItem.Click += new System.EventHandler(this.timeToolStripMenuItem_Click);
            // 
            // PauseMenu
            // 
            this.PauseMenu.Name = "PauseMenu";
            this.PauseMenu.Size = new System.Drawing.Size(180, 22);
            this.PauseMenu.Text = "Pause Only";
            this.PauseMenu.Click += new System.EventHandler(this.PauseMenu_Click);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(177, 6);
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
        private System.Windows.Forms.ToolStripMenuItem GenerateMenu;
        private System.Windows.Forms.OpenFileDialog OpenFile;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem SideViewMenu;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem ResetMenu;
        private System.Windows.Forms.ToolStripMenuItem streamToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem StartMenu;
        private System.Windows.Forms.ToolStripMenuItem StopMenu;
        private System.Windows.Forms.ToolStripMenuItem ExecuteMenu;
        private System.Windows.Forms.ToolStripMenuItem RunMenu;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem StartWaitMenu;
        private System.Windows.Forms.ToolStripMenuItem SettingsMenu;
        private System.Windows.Forms.ToolStripMenuItem BaseAddressMenu;
        private System.Windows.Forms.ToolStripMenuItem SimMenu;
        private System.Windows.Forms.ToolStripMenuItem GetPlayerMenu;
        private System.Windows.Forms.ToolStripMenuItem GetPlayerActivityMenu;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem ViewActivitiesMenu;
        private System.Windows.Forms.ToolStripMenuItem LoadDataMenu;
        private System.Windows.Forms.ToolStripMenuItem ClearTrackingMenu;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem StreamFileMenu;
        private System.Windows.Forms.ToolStripMenuItem timeToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog SaveFile;
        private System.Windows.Forms.ToolStripMenuItem PauseMenu;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem6;
    }
}