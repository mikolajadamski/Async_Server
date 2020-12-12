namespace ClientApp
{
    partial class CanalWindow
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
            this.UserInput = new System.Windows.Forms.TextBox();
            this.messagesBox = new System.Windows.Forms.TextBox();
            this.CanalName = new System.Windows.Forms.Label();
            this.ActiveUsers = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.LeaveButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // UserInput
            // 
            this.UserInput.Location = new System.Drawing.Point(32, 283);
            this.UserInput.Multiline = true;
            this.UserInput.Name = "UserInput";
            this.UserInput.Size = new System.Drawing.Size(411, 48);
            this.UserInput.TabIndex = 0;
            this.UserInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.UserInput_KeyDown);
            // 
            // messagesBox
            // 
            this.messagesBox.ForeColor = System.Drawing.SystemColors.MenuText;
            this.messagesBox.Location = new System.Drawing.Point(32, 40);
            this.messagesBox.Multiline = true;
            this.messagesBox.Name = "messagesBox";
            this.messagesBox.Size = new System.Drawing.Size(411, 221);
            this.messagesBox.TabIndex = 1;
            // 
            // CanalName
            // 
            this.CanalName.AutoSize = true;
            this.CanalName.Location = new System.Drawing.Point(29, 22);
            this.CanalName.Name = "CanalName";
            this.CanalName.Size = new System.Drawing.Size(33, 13);
            this.CanalName.TabIndex = 2;
            this.CanalName.Text = "canal";
            // 
            // ActiveUsers
            // 
            this.ActiveUsers.Location = new System.Drawing.Point(556, 56);
            this.ActiveUsers.Multiline = true;
            this.ActiveUsers.Name = "ActiveUsers";
            this.ActiveUsers.Size = new System.Drawing.Size(117, 129);
            this.ActiveUsers.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(553, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Użytkownicy na kanale:";
            // 
            // LeaveButton
            // 
            this.LeaveButton.Location = new System.Drawing.Point(584, 283);
            this.LeaveButton.Name = "LeaveButton";
            this.LeaveButton.Size = new System.Drawing.Size(75, 23);
            this.LeaveButton.TabIndex = 5;
            this.LeaveButton.Text = "Leave";
            this.LeaveButton.UseVisualStyleBackColor = true;
            this.LeaveButton.Click += new System.EventHandler(this.LeaveButton_Click);
            // 
            // CanalWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(702, 358);
            this.Controls.Add(this.LeaveButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ActiveUsers);
            this.Controls.Add(this.CanalName);
            this.Controls.Add(this.messagesBox);
            this.Controls.Add(this.UserInput);
            this.Name = "CanalWindow";
            this.Text = "Chatto";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.CanalWindow_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox UserInput;
        private System.Windows.Forms.TextBox messagesBox;
        private System.Windows.Forms.Label CanalName;
        private System.Windows.Forms.TextBox ActiveUsers;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button LeaveButton;
    }
}