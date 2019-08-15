namespace chating
{
    partial class Main
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
            this.components = new System.ComponentModel.Container();
            this.input_text = new System.Windows.Forms.TextBox();
            this.send_box = new System.Windows.Forms.Button();
            this.exit = new System.Windows.Forms.Button();
            this.user_list = new System.Windows.Forms.ListView();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.menu = new System.Windows.Forms.MenuStrip();
            this.메뉴ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.설정ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.textBox = new System.Windows.Forms.RichTextBox();
            this.menu.SuspendLayout();
            this.SuspendLayout();
            // 
            // input_text
            // 
            this.input_text.Location = new System.Drawing.Point(17, 369);
            this.input_text.Multiline = true;
            this.input_text.Name = "input_text";
            this.input_text.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.input_text.Size = new System.Drawing.Size(536, 28);
            this.input_text.TabIndex = 0;
            this.input_text.TextChanged += new System.EventHandler(this.Input_text_TextChanged);
            this.input_text.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Input_text_Key_Up);
            // 
            // send_box
            // 
            this.send_box.Location = new System.Drawing.Point(572, 369);
            this.send_box.Name = "send_box";
            this.send_box.Size = new System.Drawing.Size(105, 32);
            this.send_box.TabIndex = 1;
            this.send_box.Text = "보내기";
            this.send_box.UseVisualStyleBackColor = true;
            this.send_box.Click += new System.EventHandler(this.Send_box_Click);
            // 
            // exit
            // 
            this.exit.Location = new System.Drawing.Point(682, 370);
            this.exit.Name = "exit";
            this.exit.Size = new System.Drawing.Size(105, 32);
            this.exit.TabIndex = 4;
            this.exit.Text = "종료";
            this.exit.UseVisualStyleBackColor = true;
            this.exit.Click += new System.EventHandler(this.Exit_Click);
            // 
            // user_list
            // 
            this.user_list.Location = new System.Drawing.Point(684, 27);
            this.user_list.Name = "user_list";
            this.user_list.Size = new System.Drawing.Size(104, 326);
            this.user_list.TabIndex = 9;
            this.user_list.UseCompatibleStateImageBehavior = false;
            // 
            // notifyIcon
            // 
            this.notifyIcon.Text = "Chating";
            this.notifyIcon.Visible = true;
            this.notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseClick);
            // 
            // menu
            // 
            this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.메뉴ToolStripMenuItem});
            this.menu.Location = new System.Drawing.Point(0, 0);
            this.menu.Name = "menu";
            this.menu.Size = new System.Drawing.Size(798, 24);
            this.menu.TabIndex = 10;
            this.menu.Text = "메뉴";
            // 
            // 메뉴ToolStripMenuItem
            // 
            this.메뉴ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.설정ToolStripMenuItem});
            this.메뉴ToolStripMenuItem.Name = "메뉴ToolStripMenuItem";
            this.메뉴ToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.메뉴ToolStripMenuItem.Text = "메뉴";
            // 
            // 설정ToolStripMenuItem
            // 
            this.설정ToolStripMenuItem.Name = "설정ToolStripMenuItem";
            this.설정ToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            this.설정ToolStripMenuItem.Text = "설정";
            this.설정ToolStripMenuItem.Click += new System.EventHandler(this.설정ToolStripMenuItem_Click);
            // 
            // textBox
            // 
            this.textBox.Location = new System.Drawing.Point(13, 27);
            this.textBox.Name = "textBox";
            this.textBox.ReadOnly = true;
            this.textBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.textBox.Size = new System.Drawing.Size(665, 326);
            this.textBox.TabIndex = 11;
            this.textBox.Text = "";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(798, 426);
            this.Controls.Add(this.textBox);
            this.Controls.Add(this.user_list);
            this.Controls.Add(this.exit);
            this.Controls.Add(this.send_box);
            this.Controls.Add(this.input_text);
            this.Controls.Add(this.menu);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MainMenuStrip = this.menu;
            this.MaximizeBox = false;
            this.Name = "Main";
            this.Text = "채팅";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.Load += new System.EventHandler(this.Main_Load);
            this.Resize += new System.EventHandler(this.Main_Resize);
            this.menu.ResumeLayout(false);
            this.menu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox input_text;
        private System.Windows.Forms.Button exit;
        public System.Windows.Forms.Button send_box;
        private System.Windows.Forms.ListView user_list;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.MenuStrip menu;
        private System.Windows.Forms.ToolStripMenuItem 메뉴ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 설정ToolStripMenuItem;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.RichTextBox textBox;
    }
}

