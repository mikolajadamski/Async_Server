namespace ClientApp
{
    partial class MenuWindow
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
            this.loggerBox = new System.Windows.Forms.TextBox();
            this.commandBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // loggerBox
            // 
            this.loggerBox.Location = new System.Drawing.Point(21, 12);
            this.loggerBox.Multiline = true;
            this.loggerBox.Name = "loggerBox";
            this.loggerBox.ReadOnly = true;
            this.loggerBox.Size = new System.Drawing.Size(655, 280);
            this.loggerBox.TabIndex = 1;
            // 
            // commandBox
            // 
            this.commandBox.Location = new System.Drawing.Point(21, 298);
            this.commandBox.Multiline = true;
            this.commandBox.Name = "commandBox";
            this.commandBox.Size = new System.Drawing.Size(655, 48);
            this.commandBox.TabIndex = 2;
            this.commandBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.commandBox_KeyDown);
            // 
            // MenuWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(702, 358);
            this.Controls.Add(this.commandBox);
            this.Controls.Add(this.loggerBox);
            this.Name = "MenuWindow";
            this.Text = "Chattoo";
            this.Load += new System.EventHandler(this.MenuWindow_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox loggerBox;
        private System.Windows.Forms.TextBox commandBox;
    }
}