namespace chating
{
    partial class Notifications
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
            this.answer = new System.Windows.Forms.Button();
            this.identify = new System.Windows.Forms.Button();
            this.name = new System.Windows.Forms.Label();
            this.msg = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // answer
            // 
            this.answer.Location = new System.Drawing.Point(1, 118);
            this.answer.Name = "answer";
            this.answer.Size = new System.Drawing.Size(189, 23);
            this.answer.TabIndex = 0;
            this.answer.Text = "답장";
            this.answer.UseVisualStyleBackColor = true;
            this.answer.Click += new System.EventHandler(this.answer_Click);
            // 
            // identify
            // 
            this.identify.Location = new System.Drawing.Point(190, 118);
            this.identify.Name = "identify";
            this.identify.Size = new System.Drawing.Size(175, 23);
            this.identify.TabIndex = 1;
            this.identify.Text = "확인";
            this.identify.UseVisualStyleBackColor = true;
            this.identify.Click += new System.EventHandler(this.identify_Click);
            // 
            // name
            // 
            this.name.AutoSize = true;
            this.name.Location = new System.Drawing.Point(25, 25);
            this.name.Name = "name";
            this.name.Size = new System.Drawing.Size(17, 12);
            this.name.TabIndex = 2;
            this.name.Text = "...";
            // 
            // msg
            // 
            this.msg.AutoSize = true;
            this.msg.Location = new System.Drawing.Point(25, 63);
            this.msg.Name = "msg";
            this.msg.Size = new System.Drawing.Size(17, 12);
            this.msg.TabIndex = 3;
            this.msg.Text = "...";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(173, 63);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "label1";
            // 
            // Notifications
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(365, 141);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.msg);
            this.Controls.Add(this.name);
            this.Controls.Add(this.identify);
            this.Controls.Add(this.answer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Notifications";
            this.Text = "Notifications";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label name;
        private System.Windows.Forms.Label msg;
        public System.Windows.Forms.Button answer;
        public System.Windows.Forms.Button identify;
        private System.Windows.Forms.Label label1;
    }
}