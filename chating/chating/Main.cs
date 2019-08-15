using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.IO.Compression;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;

// 서버 아이피 변경시 접속, 다운로드 IP 변경

namespace chating
{
    public partial class Main : Form
    {
        login log;
        Setting setting;

        const int textBuf = 254+3;
        const int userBuf = 12+3;
        const int stateBuf = 6+3;
        const string version = "1.0\n";
        const int verBuf = 10;
        const int dateBuf = 23;

        private static string ID;
        bool prog_exit = false;
        bool enter_key = false;
        bool text_change = false;
        bool control_key = false;
        bool enter_button = false;

        int input_text_X, input_text_Y;

        int monitor_x = Screen.PrimaryScreen.Bounds.Width;
        int monitor_y = Screen.PrimaryScreen.Bounds.Height;
        int Main_Width;
        int Main_Height;
        double div; // 해상도 나누기
        double x_div, y_div; // 알림창 해상도 나누기
        bool timer_stop;
        
        /* ini 파일 읽기 */
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        StringBuilder retCheck = new StringBuilder();

        /* 옵션 설정 */
        public static bool tray_check;
        public static bool notify_check;
        public static bool notify_msg_check;

        public static string user_name;
        public static string user_msg;

        public Main(login _log)
        {
            InitializeComponent();
            log = _log;
        }

        public Main(Setting _setting)
        {
            setting = _setting;
        }

        public void Screen_Fix()
        {
            //this.MaximumSize = new System.Drawing.Size((int)(this.Width / 1.22), (int)(this.Height / 1.3));
            /* 모니터 해상도 */
            /*
             * 1920 * 1080 나누기 3
             * 1366 * 768 나누기 2.5
             */

            Monitor_div(monitor_x, monitor_y);
            Notify_div(monitor_x, monitor_y);
            /* Main Form 크기 */
            Main_Width = (int)(monitor_x / div);
            Main_Height = (int)(monitor_y / div);       
            Size = new Size(Main_Width, Main_Height);
            
            /* textBox 위치 및 크기*/
            textBox.Location = new Point(Width / 100, Height / 15);
            textBox.Width = (int)(Width / 1.22);
            textBox.Height = (int)(Height / 1.4);
            
            /* 현재 사용자 위치 및 크기 */
            user_list.Location = new Point((int)(Width / 1.2) , Height / 15); // Width 1.18
            user_list.Width = (int)(Width / 8); // 7.77
            user_list.Height = (int)(Height / 1.4);

            /* input_text 위치 및 크기 */
            input_text.Location = new Point(Width / 100, (int)(Height / 1.27)); // Height 1.25
            input_text.Width = (int)(Width / 1.48);
            input_text.Height = (int)(Height / 14.2);   // Height 14.2
            
            /* send_box 위치 및 크기 */
            send_box.Location = new Point((int)(Width / 1.43), (int)(Height / 1.27)); // Height 1.25
            send_box.Width = (int)(Width / 8); // 7.77
            send_box.Height = (int)(Height / 14.2); // Height 14.2

            /* exit 위치 및 크기 */
            exit.Location = new Point((int)(Width / 1.2), (int)(Height / 1.27)); // Width 1.18  Height 1.25
            exit.Width = (int)(Width / 7.77);
            exit.Height = (int)(Height / 14.2);        // Height 14.2          
        }

        private void Main_Load(object sender, EventArgs e)
        {
            Screen_Fix();
            TCP();
            user_list.View = View.Details;
            user_list.GridLines = true;
            user_list.Columns.Add("현재 접속자", 100, HorizontalAlignment.Center);
            input_text_X = input_text.Location.X;
            input_text_Y = input_text.Location.Y;

            /* 트레이 아이콘 */
            notifyIcon.Icon = new Icon(Application.StartupPath + @"\Image\msg.ico");
            //Path.GetFullPath
            ContextMenu ctx = new ContextMenu();
            MenuItem open = new MenuItem();
            MenuItem close = new MenuItem();
            MenuItem bar = new MenuItem();

            ctx.MenuItems.Add(open);
            ctx.MenuItems.Add(bar);
            ctx.MenuItems.Add(close);

            open.Index = 0;
            open.Text = "열기";
            open.Click += delegate (object click, EventArgs eClick)
            {
                WindowState = FormWindowState.Normal;
                Visible = true;
                Show();
                Activate();
            };

            bar.Index = 1;
            bar.Text = "-";

            close.Index = 2;
            close.Text = "종료";
            close.Click += delegate (object click, EventArgs eClick)
            {
                tray_check = false;
                Close();
            };
            notifyIcon.ContextMenu = ctx;
            
            /* 설정 */
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Chating\Setting";
            DirectoryInfo di = new DirectoryInfo(path);

            if (di.Exists == false)
                di.Create();

            /* ini 파일 읽기 */
            /* tray_check */
            GetPrivateProfileString("Option", "tray_check", "(NONE)", retCheck, 32, Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Chating\Setting\config.ini");

            if (String.Compare(retCheck.ToString(), "true", false) == 0)
            {
                tray_check = true;
            }
            else
            {
                tray_check = false;
            }

            /* notify_check */
            GetPrivateProfileString("Option", "notify_check", "(NONE)", retCheck, 32, Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Chating\Setting\config.ini");

            if (String.Compare(retCheck.ToString(), "true", false) == 0)
            {
                notify_check = true;
            }
            else
            {
                notify_check = false;
            }

            /* notify_msg_check */
            GetPrivateProfileString("Option", "notify_msg_check", "(NONE)", retCheck, 32, Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Chating\Setting\config.ini");

            if (String.Compare(retCheck.ToString(), "true", false) == 0)
            {
                notify_msg_check = true;
            }
            else
            {
                notify_msg_check = false;
            }            
        }

        private void Input_text_Key_Up(object sender, KeyEventArgs e)
        {           
            if (e.KeyCode == Keys.Enter && control_key == false)
            {                
                enter_key = true;
                
                input_text.Location = new Point(Width / 100, (int)(Height / 1.27));
                input_text.Width = (int)(Width / 1.48);
                input_text.Height = (int)(Height / 14.2);

                input_text_X = input_text.Location.X;
                input_text_Y = input_text.Location.Y;

                textBox.Height = (int)(Height / 1.4);
            }
            else if(e.Modifiers == Keys.Control)
            {
                control_key = true;
                if (e.KeyCode == Keys.Enter && control_key == true)
                {
                    enter_key = false;
                    if (input_text.Height < (int)(Height / 4.54))
                    {
                        input_text_Y -= (int)(Height / 25);
                        input_text.Location = new Point(input_text_X, input_text_Y);
                        input_text.Height += (int)(Height / 25);
                        textBox.Height -= (int)(Height / 25);
                    }
                }
                Delay(500);
                control_key = false;
            }
        }
        
        public void TCP()
        {
            byte[] ID_buf = new byte[userBuf];
            byte[] enter_b = new byte[stateBuf];
            string enter = "ENTER";
            int size = enter.Length;
            byte[] data_size = BitConverter.GetBytes(size);
            string _text;           
            
            try
            {   
                Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                var sd = new IPEndPoint(IPAddress.Parse("192.168.0.36"), 11100); // 서버 IP 변경시 변경
                sock.Connect(sd);

                /* 업데이트 확인 */
                Check_Update(sock);

                /* 이전 메세지 출력 */
                Before_Message(sock);
                
                _text = Send_count(enter, stateBuf);           
                enter_b = Encoding.UTF8.GetBytes(_text);
                sock.Send(enter_b ,SocketFlags.None);
                _text = null;
                
                _text = Send_count(ID, userBuf);              
                ID_buf = Encoding.UTF8.GetBytes(_text);
                sock.Send(ID_buf, SocketFlags.None);
                _text = null;
                
                Current_User(sock);
                
                Thread(sock);                
            }
            catch
            {
                MessageBox.Show("서버와 연결되지 않았습니다.");
                Close();
                log.Close();
                
            }          
        }
        public static string ID_Send
        {
            get
            {
                return ID;
            }
            set
            {
                ID = value;
            }
        }       

        void Thread(Socket sock)
        {
            Thread send = new Thread(() => Send_Function(sock));
            Thread recv = new Thread(() => Recv_Function(sock));
            
            send.Start();
            recv.Start();
        }

        string Send_count(string text, int buf_size)
        {
            string _text;
            
            if (text.Length < 10)
            {
                _text = "00";
                _text += text.Length;
            }
            else if (text.Length < 100)
            {
                _text = "0";
                _text += text.Length;
            }
            else
            {
                _text = string.Format("{0}", text.Length);
            }
            
            _text += text; 
            
            for(int i=0; i < (buf_size-3) - text.Length; i++)
            {
                _text += " ";
            }
            return _text;
        }

        int CharToInt(char c)
        {
            int diff = 1 - '1';
            return c + diff;
        }

        string Read_count(string text)
        {
            string _text;
            string text_num;
            int i, read_num, num;
            i = read_num = num = 0;
            _text = null;

            for(i=0; i<3; i++)
            {
                text_num = text.Substring(i, 1);

                read_num = int.Parse(text_num);

                if (i == 0)
                {
                    num = 100 * read_num;
                }
                else if (i == 1)
                {
                    num += 10 * read_num;
                }
                else if (i == 2)
                {
                    num += read_num;
                }
                read_num = 0;
            }
            _text = text.Substring(3, num);

            return _text;
        }

        void Send_Function(Socket sock)
        {
            string send_true;
            string text;
            string _text;
            byte[] send_true_b = new byte[stateBuf];
            byte[] text_b = new byte[textBuf];
            byte[] user_b = new byte[userBuf];
            
            while (prog_exit == false)
            {               
                //Delay(100);
                if (enter_key == true)
                {
                    /*
                    input_text.ReadOnly = true; // 입력 시간 제한
                    Delay(300);
                    input_text.ReadOnly = false;
                    */
                    
                    input_text.MaxLength = 0;
                    Delay(300);
                    input_text.MaxLength = 32767;

                    send_true = "TRUE";
                    _text = Send_count(send_true, stateBuf);
                    send_true_b = Encoding.UTF8.GetBytes(_text);
                    sock.Send(send_true_b, 0, stateBuf, SocketFlags.None);
                    _text = null;

                    _text = Send_count(ID, userBuf);
                    user_b = Encoding.UTF8.GetBytes(_text);
                    sock.Send(user_b, userBuf, SocketFlags.None);
                    _text = null;

                    if(enter_button == true)
                        text = input_text.Text; // 보내기 버튼을 클릭할 시 엔터키 제거가 필요없다
                    else
                        text = input_text.Text.Substring(0, input_text.Text.Length - 2); // 엔터키 제거

                    _text = Send_count(text, textBuf);
                    text_b = Encoding.UTF8.GetBytes(_text);
                    sock.Send(text_b, textBuf, SocketFlags.None);

                    enter_key = false;
                    enter_button = false;
                    text = null;
                    input_text.Clear();
                }
                else
                {
                    send_true = "FALSE";
                    _text = Send_count(send_true, stateBuf);
                    send_true_b = Encoding.UTF8.GetBytes(_text);
                    sock.Send(send_true_b, SocketFlags.None);
                    _text = null;
                    System.Threading.Thread.Sleep(400);
                }
            }
            if (prog_exit == true)
                Exit(sock);
        }

        void Recv_Function(Socket sock)
        {
            string recv_true;
            byte[] recv_true_b = new byte[stateBuf];
            string recv_user;
            byte[] recv_user_b = new byte[userBuf];
            string recv_text;
            byte[] recv_text_b = new byte[textBuf];
            int socket_check = 0;
            int read_cnt;
            string msg_buf;
            string buf;
            
            while (prog_exit == false)
            {
                //Delay(100);
                socket_check = sock.Receive(recv_true_b);
                buf = Encoding.UTF8.GetString(recv_true_b, 0, socket_check);
                recv_true = Read_count(buf);
                buf = null;
                          
                if (socket_check <= 0)
                {
                    textBox.Text = "서버와 연결이 끊겼습니다.\n";
                    sock.Close();
                }
                            
                if (String.Compare(recv_true, "TRUE", false) == 0)
                {
                    read_cnt = sock.Receive(recv_user_b);
                    buf = Encoding.UTF8.GetString(recv_user_b, 0, read_cnt);
                    recv_user = Read_count(buf); 
                    buf = null;
                   
                    read_cnt = sock.Receive(recv_text_b);
                    buf = Encoding.UTF8.GetString(recv_text_b, 0, read_cnt);
                    recv_text = Read_count(buf);
                    buf = null;

                    //textBox.Text += recv_user + "\r\n"; // "\r\n" 엔터
                    //textBox.Text += recv_text+ Environment.NewLine; // 엔터
                    textBox.AppendText(recv_user + "\r\n");
                    textBox.AppendText(recv_text + Environment.NewLine);

                    if (notify_check == true)
                    {
                        Thread noti = new Thread(() => Notification(recv_user, recv_text));
                        noti.Start();
                    }

                    textBox.SelectionStart = textBox.Text.Length;
                    textBox.ScrollToCaret();
                }
                else if(String.Compare(recv_true, "EXIT", false) == 0)
                {
                    read_cnt = sock.Receive(recv_user_b);
                    buf = Encoding.UTF8.GetString(recv_user_b, 0, read_cnt);
                    recv_user = Read_count(buf);
                    buf = null;

                    textBox.Text += (recv_user + "님이 나갔습니다.\r\n");

                    textBox.SelectionStart = textBox.Text.Length;
                    textBox.ScrollToCaret();

                    Current_User(sock);
                }
                else if (String.Compare(recv_true, "UNEX", false) == 0)
                {
                    read_cnt = sock.Receive(recv_user_b);
                    buf = Encoding.UTF8.GetString(recv_user_b, 0, read_cnt);
                    recv_user = Read_count(buf);
                    buf = null;

                    textBox.Text += recv_user + "님의 접속이 끊겼습니다.\r\n";

                    textBox.SelectionStart = textBox.Text.Length;
                    textBox.ScrollToCaret();

                    Current_User(sock);
                }
                else if (String.Compare(recv_true, "ENTER", false) == 0)
                {
                    read_cnt = sock.Receive(recv_user_b);
                    buf = Encoding.UTF8.GetString(recv_user_b, 0, read_cnt);
                    recv_user = Read_count(buf);
                    buf = null;

                    msg_buf = recv_user;
                    msg_buf += "님이 입장했습니다.\r\n";
                    textBox.AppendText(msg_buf);

                    textBox.SelectionStart = textBox.Text.Length;
                    textBox.ScrollToCaret();

                    Current_User(sock);
                }
                msg_buf = null;
                read_cnt = 0;
                
                System.Threading.Thread.Sleep(300);
            }
        }

        private static DateTime Delay(int MS)
        {
            DateTime ThisMoment = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, MS);
            DateTime AfterWards = ThisMoment.Add(duration);
            
            while (AfterWards >= ThisMoment)
            {
                System.Windows.Forms.Application.DoEvents();
                ThisMoment = DateTime.Now;
            }
            
            return DateTime.Now;
        }

        public void Exit(Socket sock)
        {
            string send_true;
            string _text;
            byte[] send_true_b = new byte[stateBuf];
            byte[] ID_buf = new byte[userBuf];

            send_true = "EXIT";
            _text = Send_count(send_true, stateBuf);
            send_true_b = Encoding.UTF8.GetBytes(_text);
            sock.Send(send_true_b, 0, stateBuf, SocketFlags.None);
            _text = null;
            
            _text = Send_count(ID, userBuf);
            ID_buf = Encoding.UTF8.GetBytes(_text);
            sock.Send(ID_buf, SocketFlags.None);
            _text = null;
        }

        /* 현재 사용자 */
        public void Current_User(Socket sock)
        {
            string user;
            string[] arr = new string[2]; // 유저, 접속시간 (접속시간은 넣을지 말지 고민중)
            int number;
            int read_cnt;
            string buf;
            byte[] _user = new byte[userBuf];
            byte[] count = new byte[2]; // 인원수 3byte 0~99
            ListViewItem data = new ListViewItem(arr);

            user_list.Items.Clear();
            
            number = Current_user_count(sock);
            
            for(int i=0; i < number; i++)
            {
                read_cnt = sock.Receive(_user);
                buf = Encoding.UTF8.GetString(_user, 0, read_cnt);
                user = Read_count(buf);
                buf = null;
                arr[0] = user;
                data = new ListViewItem(arr);
                user_list.Items.Add(data);
            }
            
        }

        /* 현재 사용자 인원 */
        public int Current_user_count(Socket sock)
        {
            byte[] count = new byte[2];
            string number;
            int num;
            int read_cnt;
 
            read_cnt = sock.Receive(count);
            number = Encoding.UTF8.GetString(count, 0, read_cnt);
            
            if (string.Compare(number.Substring(0, 1), "0") == 0)
                num = int.Parse(number.Substring(1, 1));
            else
            {
                num = int.Parse(number.Substring(0, 1)) * 10;
                num += int.Parse(number.Substring(1, 1));
            }           
            return num;
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            if (tray_check == false)
            {
                prog_exit = true;
                log.Visible = true;
                Close();
            }
            else
            {
                Visible = false;
                //this.Hide();
            }
        }

        private void Send_box_Click(object sender, EventArgs e)
        {
            enter_key = true;
            enter_button = true;
            
            input_text.Location = new Point(Width / 100, (int)(Height / 1.27));
            input_text.Width = (int)(this.Width / 1.48);
            input_text.Height = (int)(Height / 14.2);

            input_text_X = input_text.Location.X;
            input_text_Y = input_text.Location.Y;

            textBox.Height = (int)(Height / 1.4);
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (tray_check == true)
            {
                e.Cancel = true;
                //this.Hide();
                this.Visible = false;
            }
            else
            {
                e.Cancel = false;
                log.Visible = true;
                prog_exit = true;
            }
        }

        /* 메세지 길이 제한 */
        private void Input_text_TextChanged(object sender, EventArgs e)
        {
            int max_size = 254;
            string text = input_text.Text;
            Byte[] _text;

            if(!text_change)
            {
                text_change = true;

                if(input_text.Text.Length > 0)
                {
                    _text = Encoding.UTF8.GetBytes(text);

                    if(_text.Length > max_size)
                    {
                        while(_text.Length > max_size)
                        {
                            text = text.Substring(0, text.Length - 1);
                            _text = Encoding.UTF8.GetBytes(text);
                        }
                        input_text.Text = text;
                        input_text.SelectionStart = input_text.Text.Length;
                        input_text.Focus();
                    }
                }
                text_change = false;
            }
        }

        /* 업데이트 확인 */
        public void Check_Update(Socket sock)
        {
            string server_ver;
            string buf;
            byte[] client_ver_b = new byte[verBuf];
            byte[] server_ver_b = new byte[verBuf];
            int read_cnt;

            read_cnt = sock.Receive(server_ver_b);
            buf = Encoding.UTF8.GetString(server_ver_b, 0, read_cnt);
            server_ver = Read_count(buf);           
            buf = null;
            
            if(String.Compare(version, server_ver, false) != 0)
            {
                string path = Directory.GetCurrentDirectory() + @"\Update_Chating.exe";               
                //@"C:\Users\happy\source\repos\Update_Chating\Update_Chating\bin\Release\Update_Chating.exe";

                Process.Start(path);
                Exit(sock);
                Close();
                log.Close();
            }
        }

        public void Monitor_div(int x, int y)
        {
            if(x == 1920)
            {
                if(y == 1080)
                {
                    div = 3;
                }
            }
            else if(x == 1366)
            {
                if(y == 768)
                {
                    div = 2.5;
                }
            }
            else
            {
                div = 2.5;
            }
        }

        public void Notify_div(int x, int y)
        {
            if(x == 1920)
            {
                if(y == 1080)
                {
                    x_div = 5.2;
                    y_div = 7.6;
                }
            }
            else if(x == 1366)
            {
                if(y == 768)
                {
                    x_div = 5.2;
                    y_div = 7.6;
                }
            }
            else
            {
                x_div = 5.2;
                y_div = 7.6;
            }
        }

        private void Main_Resize(object sender, EventArgs e)
        {           
            if (WindowState == FormWindowState.Minimized)
            {
                notifyIcon.Visible = true;
            }
            else if (WindowState == FormWindowState.Normal)
            {
                ShowInTaskbar = true;
            }
        }

        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                WindowState = FormWindowState.Normal;
                Visible = true;
                Show();
                Activate();
            }
        }

        private void 설정ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Setting setting = new Setting();
            setting.ShowDialog();
        }

        public void time_stop()
        {
            Delay(3000);
            timer_stop = true;
        }

        public void Notification(string user, string msg)
        {
            double notify_width, notify_height;

            notify_width = monitor_x / x_div;
            notify_height = monitor_y / y_div;
            
            if(this.Visible == false || this.WindowState == FormWindowState.Minimized)
            {
                user_name = user;
                user_msg = msg;

                Notifications Notify = new Notifications(this, setting);
                Notify.Size = new Size((int)notify_width, (int)notify_height);

                Rectangle ScreenRectangle = Screen.PrimaryScreen.WorkingArea;

                int xPos = ScreenRectangle.Width - Notify.Bounds.Width - 5;
                int yPos = ScreenRectangle.Height - Notify.Bounds.Height - 5;
                
                Notify.Show();            
                Notify.SetBounds(xPos, yPos, Notify.Size.Width, Notify.Size.Height, BoundsSpecified.Location);
                Notify.BringToFront();
                          
                time_stop();
            
                if (timer_stop == true)
                {
                    Notify.Close();
                    timer_stop = false;
                }                                    
            }            
        }

        public void Before_Message(Socket sock)
        {      
            string recv_true;
            byte[] recv_true_b = new byte[stateBuf];
            string recv_user;
            byte[] recv_user_b = new byte[userBuf];
            string recv_text;
            byte[] recv_text_b = new byte[textBuf];
            string recv_date;
            byte[] recv_date_b = new byte[dateBuf];

            int socket_check = 0;
            int read_cnt;
            string msg_buf;
            string buf;
            string record_date = "";
            string date;

            int num = 0;

            while (num < 100)
            {
                read_cnt = sock.Receive(recv_date_b);
                buf = Encoding.UTF8.GetString(recv_date_b, 0, read_cnt);
                recv_date = Read_count(buf);
                buf = null;
                
                date = recv_date.Substring(0, 8);
                
                if (num == 0)
                {                   
                    record_date = date;
                    textBox.Text += "-----" + date + "-----" + Environment.NewLine;
                    textBox.SelectionStart = textBox.Text.Length;
                    textBox.ScrollToCaret();
                }
                else if(String.Compare(date, record_date, false) != 0)
                {
                    textBox.Text += "-----" + date + "-----" + Environment.NewLine;
                    //textBox.SelectionStart = textBox.Text.Length;
                    //textBox.ScrollToCaret();

                    record_date = date;
                }
                
                socket_check = sock.Receive(recv_true_b);
                buf = Encoding.UTF8.GetString(recv_true_b, 0, socket_check);
                recv_true = Read_count(buf);
                buf = null;
                       
                if (socket_check <= 0)
                {
                    textBox.Text = "서버와 연결이 끊겼습니다.\n";
                    sock.Close();
                }
                
                if (String.Compare(recv_true, "TRUE", false) == 0)
                {
                    read_cnt = sock.Receive(recv_user_b);
                    buf = Encoding.UTF8.GetString(recv_user_b, 0, read_cnt);
                    recv_user = Read_count(buf); 
                    buf = null;
                   
                    read_cnt = sock.Receive(recv_text_b);
                    buf = Encoding.UTF8.GetString(recv_text_b, 0, read_cnt);
                    recv_text = Read_count(buf);
                    buf = null;
                    
                    //textBox.Text += recv_user + "\r\n"; // "\r\n" 엔터
                    //textBox.Text += recv_text+ Environment.NewLine; // 엔터
                    textBox.AppendText(recv_user + "\r\n");
                    textBox.AppendText(recv_text + Environment.NewLine);

                    //textBox.SelectionStart = textBox.Text.Length;
                    //textBox.ScrollToCaret();
                }
                else if(String.Compare(recv_true, "EXIT", false) == 0)
                {
                    read_cnt = sock.Receive(recv_user_b);
                    buf = Encoding.UTF8.GetString(recv_user_b, 0, read_cnt);
                    recv_user = Read_count(buf);
                    buf = null;

                    //textBox.Text += (recv_user + "님이 나갔습니다.\r\n");
                    textBox.AppendText(recv_user + "님이 나갔습니다.\r\n");

                    //textBox.SelectionStart = textBox.Text.Length;
                    //textBox.ScrollToCaret();
                }
                else if (String.Compare(recv_true, "UNEX", false) == 0)
                {
                    read_cnt = sock.Receive(recv_user_b);
                    buf = Encoding.UTF8.GetString(recv_user_b, 0, read_cnt);
                    recv_user = Read_count(buf);
                    buf = null;

                    //textBox.Text += recv_user + "님의 접속이 끊겼습니다.\r\n";
                    textBox.AppendText(recv_user + "님의 접속이 끊겼습니다.\r\n");

                    //textBox.SelectionStart = textBox.Text.Length;
                    //textBox.ScrollToCaret();
                }
                else if (String.Compare(recv_true, "ENTER", false) == 0)
                {
                    read_cnt = sock.Receive(recv_user_b);
                    buf = Encoding.UTF8.GetString(recv_user_b, 0, read_cnt);
                    recv_user = Read_count(buf);
                    buf = null;

                    msg_buf = recv_user;
                    msg_buf += "님이 입장했습니다.\r\n";
                    textBox.AppendText(msg_buf);

                    //textBox.SelectionStart = textBox.Text.Length;
                    //textBox.ScrollToCaret();
                }
                msg_buf = null;
                read_cnt = 0;
                num++;                
            }
            textBox.AppendText("-----" + DateTime.Now.ToString("yyyy-MM-dd") + "-----" + Environment.NewLine);
            textBox.SelectionStart = textBox.Text.Length;
            textBox.ScrollToCaret();           
        }       
    }
}
