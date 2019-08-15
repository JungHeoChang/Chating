using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace chating
{
    public partial class Notifications : Form
    {
        Main main;
        Setting setting;

        public static bool notify_check;
        public static bool notify_msg_check;

        int monitor_x = Screen.PrimaryScreen.Bounds.Width;
        int monitor_y = Screen.PrimaryScreen.Bounds.Height;

        double x_div, y_div;

        public Notifications(Main _main, Setting _setting)
        {
            InitializeComponent();
            Screen_Fix();
            Notification();
            main = _main;
            setting = _setting;
        }

        public void Screen_Fix()
        {
            double notify_width, notify_height;           

            Notify_div(monitor_x, monitor_y);

            notify_width = monitor_x / x_div;
            notify_height = monitor_y / y_div;

            Size = new Size((int)notify_width, (int)notify_height);
            
            /* 답장 버튼 */           
            answer.Location = new Point((0), (Height - answer.Height));           
            answer.Width = Width / 2;
            answer.Height = Height / (Height / 23);
            
            /* 확인 버튼 */           
            identify.Location = new Point((Width - answer.Width), (Height - identify.Height));
            identify.Width = Width / 2;
            identify.Height = Height / (Height / 23); //6
            
            /* 이름 */           
            name.Location = new Point((Width % 110), (Height / 10));

            /* 메세지 */
            msg.Location = new Point((Width % 110), (Height / 3));
        }

        public void Notification()
        {
            if (notify_msg_check == true)
            {
                name.Text = "";
                msg.Text = "메세지가 도착했습니다.";
            }
            else
            {
                name.Text = Main.user_name;
                msg.Text = Main.user_msg;
            }
        }

        private void answer_Click(object sender, EventArgs e)
        {
            main.WindowState = FormWindowState.Normal;
            main.Show();
            main.Activate();
            Close();
            
        }

        private void identify_Click(object sender, EventArgs e)
        {
            Close();
        }

        public void Notify_div(int x, int y)
        {
            if (x == 1920)
            {
                if (y == 1080)
                {
                    x_div = 5.2;
                    y_div = 7.6;
                }
            }
            else if (x == 1366)
            {
                if (y == 768)
                {
                    x_div = 5.4;
                    y_div = 7.8;
                }
            }
            else
            {
                x_div = 5.2;
                y_div = 7.6;
            }
        }
    }
}
