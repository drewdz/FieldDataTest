namespace FieldDataTest
{
    partial class InputForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.Cancel = new System.Windows.Forms.Button();
            this.Ok = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.Data = new System.Windows.Forms.TextBox();
            this.Prompt = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.Cancel);
            this.panel1.Controls.Add(this.Ok);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(214, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(3);
            this.panel1.Size = new System.Drawing.Size(70, 161);
            this.panel1.TabIndex = 0;
            // 
            // Cancel
            // 
            this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel.Dock = System.Windows.Forms.DockStyle.Top;
            this.Cancel.Location = new System.Drawing.Point(3, 26);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(64, 23);
            this.Cancel.TabIndex = 2;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // Ok
            // 
            this.Ok.Dock = System.Windows.Forms.DockStyle.Top;
            this.Ok.Location = new System.Drawing.Point(3, 3);
            this.Ok.Name = "Ok";
            this.Ok.Size = new System.Drawing.Size(64, 23);
            this.Ok.TabIndex = 1;
            this.Ok.Text = "OK";
            this.Ok.UseVisualStyleBackColor = true;
            this.Ok.Click += new System.EventHandler(this.Ok_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.Data);
            this.panel2.Controls.Add(this.Prompt);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(3);
            this.panel2.Size = new System.Drawing.Size(214, 161);
            this.panel2.TabIndex = 1;
            // 
            // Data
            // 
            this.Data.Dock = System.Windows.Forms.DockStyle.Top;
            this.Data.Location = new System.Drawing.Point(3, 22);
            this.Data.Name = "Data";
            this.Data.Size = new System.Drawing.Size(208, 20);
            this.Data.TabIndex = 0;
            // 
            // Prompt
            // 
            this.Prompt.Dock = System.Windows.Forms.DockStyle.Top;
            this.Prompt.Location = new System.Drawing.Point(3, 3);
            this.Prompt.Name = "Prompt";
            this.Prompt.Padding = new System.Windows.Forms.Padding(3);
            this.Prompt.Size = new System.Drawing.Size(208, 19);
            this.Prompt.TabIndex = 3;
            this.Prompt.Text = "#";
            // 
            // InputForm
            // 
            this.AcceptButton = this.Ok;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Cancel;
            this.ClientSize = new System.Drawing.Size(284, 161);
            this.ControlBox = false;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "InputForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Input";
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.Button Ok;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox Data;
        private System.Windows.Forms.Label Prompt;
    }
}