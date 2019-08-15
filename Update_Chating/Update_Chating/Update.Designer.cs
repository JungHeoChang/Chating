namespace Update_Chating
{
    partial class Update
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.ok_button = new System.Windows.Forms.Button();
            this.cancel_button = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.download_state = new System.Windows.Forms.Label();
            this.Persent = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ok_button
            // 
            this.ok_button.Location = new System.Drawing.Point(33, 144);
            this.ok_button.Name = "ok_button";
            this.ok_button.Size = new System.Drawing.Size(103, 35);
            this.ok_button.TabIndex = 0;
            this.ok_button.Text = "확인";
            this.ok_button.UseVisualStyleBackColor = true;
            this.ok_button.Click += new System.EventHandler(this.ok_button_Click);
            // 
            // cancel_button
            // 
            this.cancel_button.Location = new System.Drawing.Point(252, 144);
            this.cancel_button.Name = "cancel_button";
            this.cancel_button.Size = new System.Drawing.Size(105, 35);
            this.cancel_button.TabIndex = 1;
            this.cancel_button.Text = "취소";
            this.cancel_button.UseVisualStyleBackColor = true;
            this.cancel_button.Click += new System.EventHandler(this.cancel_button_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(33, 53);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(324, 23);
            this.progressBar.TabIndex = 2;
            // 
            // download_state
            // 
            this.download_state.AutoSize = true;
            this.download_state.Location = new System.Drawing.Point(172, 91);
            this.download_state.Name = "download_state";
            this.download_state.Size = new System.Drawing.Size(17, 12);
            this.download_state.TabIndex = 3;
            this.download_state.Text = "...";
            // 
            // Persent
            // 
            this.Persent.AutoSize = true;
            this.Persent.Location = new System.Drawing.Point(172, 119);
            this.Persent.Name = "Persent";
            this.Persent.Size = new System.Drawing.Size(17, 12);
            this.Persent.TabIndex = 4;
            this.Persent.Text = "...";
            // 
            // Update
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(382, 191);
            this.Controls.Add(this.Persent);
            this.Controls.Add(this.download_state);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.cancel_button);
            this.Controls.Add(this.ok_button);
            this.Name = "Update";
            this.Text = "업데이트";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Update_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ok_button;
        private System.Windows.Forms.Button cancel_button;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label download_state;
        private System.Windows.Forms.Label Persent;
    }
}

