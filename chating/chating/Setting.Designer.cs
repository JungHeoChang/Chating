namespace chating
{
    partial class Setting
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
            this.tray_check = new System.Windows.Forms.CheckBox();
            this.Ok_button = new System.Windows.Forms.Button();
            this.notify_check = new System.Windows.Forms.CheckBox();
            this.notify_msg_check = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // tray_check
            // 
            this.tray_check.AutoSize = true;
            this.tray_check.Location = new System.Drawing.Point(7, 12);
            this.tray_check.Name = "tray_check";
            this.tray_check.Size = new System.Drawing.Size(168, 16);
            this.tray_check.TabIndex = 0;
            this.tray_check.Text = "종료시 트레이 상태로 전환";
            this.tray_check.UseVisualStyleBackColor = true;
            this.tray_check.Click += new System.EventHandler(this.tray_check_Click);
            // 
            // Ok_button
            // 
            this.Ok_button.Location = new System.Drawing.Point(56, 271);
            this.Ok_button.Name = "Ok_button";
            this.Ok_button.Size = new System.Drawing.Size(75, 23);
            this.Ok_button.TabIndex = 1;
            this.Ok_button.Text = "확인";
            this.Ok_button.UseVisualStyleBackColor = true;
            this.Ok_button.Click += new System.EventHandler(this.Ok_button_Click);
            // 
            // notify_check
            // 
            this.notify_check.AutoSize = true;
            this.notify_check.Location = new System.Drawing.Point(7, 51);
            this.notify_check.Name = "notify_check";
            this.notify_check.Size = new System.Drawing.Size(76, 16);
            this.notify_check.TabIndex = 2;
            this.notify_check.Text = "알람 켜기";
            this.notify_check.UseVisualStyleBackColor = true;
            this.notify_check.CheckedChanged += new System.EventHandler(this.notify_check_CheckedChanged);
            // 
            // notify_msg_check
            // 
            this.notify_msg_check.AutoSize = true;
            this.notify_msg_check.Location = new System.Drawing.Point(7, 89);
            this.notify_msg_check.Name = "notify_msg_check";
            this.notify_msg_check.Size = new System.Drawing.Size(116, 16);
            this.notify_msg_check.TabIndex = 3;
            this.notify_msg_check.Text = "알람 내용 감추기";
            this.notify_msg_check.UseVisualStyleBackColor = true;
            this.notify_msg_check.CheckedChanged += new System.EventHandler(this.notify_msg_check_CheckedChanged);
            // 
            // Setting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(187, 306);
            this.Controls.Add(this.notify_msg_check);
            this.Controls.Add(this.notify_check);
            this.Controls.Add(this.Ok_button);
            this.Controls.Add(this.tray_check);
            this.Name = "Setting";
            this.Text = "설정창";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox tray_check;
        private System.Windows.Forms.Button Ok_button;
        private System.Windows.Forms.CheckBox notify_check;
        private System.Windows.Forms.CheckBox notify_msg_check;
    }
}