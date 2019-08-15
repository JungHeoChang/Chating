using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace chating
{
    public partial class login : Form
    {
        bool text_change = false;

        public login()
        {
            InitializeComponent();
        }

        private void Login_button_Click(object sender, EventArgs e)
        {
            Main chating = new Main(this);
            Main.ID_Send = user_id.Text;
            //this.Close();
            
            chating.Show();
            Visible = false;
            //this.Hide();           
        }

        private void User_id_TextChanged(object sender, EventArgs e)
        {
            int max_size = 12;
            string text = user_id.Text;
            Byte[] _text;
            
            if (!text_change)
            {
                text_change = true;

                if (user_id.Text.Length > 0)
                {
                    _text = Encoding.UTF8.GetBytes(text);

                    if (_text.Length > max_size)
                    {
                        while (_text.Length > max_size)
                        {
                            text = text.Substring(0, text.Length - 1);
                            _text = Encoding.UTF8.GetBytes(text);
                        }
                        user_id.Text = text;
                        user_id.SelectionStart = user_id.Text.Length;
                        user_id.Focus();
                    }
                }
                text_change = false;
            }
        }

        // 쓰지않음
        public void Delete_File()
        {
            FileInfo file = new FileInfo(Application.StartupPath + @"\chating_old.exe");
            FileInfo zipfile = new FileInfo(Application.StartupPath + @"\chating_download.zip");

            try
            {
                if (file.Exists || zipfile.Exists) // 삭제할 파일이 있는지
                {
                    file.IsReadOnly = false;
                    file.Delete(); // 없어도 에러안남
                    zipfile.Delete();
                }
            }
            catch(UnauthorizedAccessException)
            {
                FileAttributes attr = (new FileInfo(Application.StartupPath + "\\chating_old.exe")).Attributes;
                file.IsReadOnly = false;
                
                if ((attr & FileAttributes.ReadOnly) > 0)
                    MessageBox.Show("파일이 읽기 전용으로 되어있습니다.");
            }
        }
    }
}
