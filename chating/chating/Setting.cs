using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace chating
{
    public partial class Setting : Form
    {
        public Setting()
        {
            InitializeComponent();

            if (Main.tray_check == true)
            {
                tray_check.Checked = true;
            }
            else
            {
                tray_check.Checked = false;
            }

            if (Main.notify_check == true)
            {
                notify_check.Checked = true;                
            }
            else
            {
                notify_check.Checked = false;
                notify_msg_check.Enabled = false;
            }

            if (Main.notify_msg_check == true)
            {
                notify_msg_check.Checked = true;
            }
            else
            {
                notify_msg_check.Checked = false;
            }
        }

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        private void tray_check_Click(object sender, EventArgs e)
        {
            if(tray_check.Checked)
            {
                Main.tray_check = tray_check.Checked;
                WritePrivateProfileString("Option", "tray_check", "true", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Chating\Setting\config.ini");
            }
            else
            {
                Main.tray_check = tray_check.Checked;
                WritePrivateProfileString("Option", "tray_check", "false", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Chating\Setting\config.ini");
            }
        }

        private void Ok_button_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void notify_check_CheckedChanged(object sender, EventArgs e)
        {
            if(notify_check.Checked)
            {
                Notifications.notify_check = notify_check.Checked;
                Main.notify_check = notify_check.Checked;
                WritePrivateProfileString("Option", "notify_check", "true", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Chating\Setting\config.ini");
                notify_msg_check.Enabled = true;
            }
            else
            {
                Notifications.notify_check = notify_check.Checked;
                Main.notify_check = notify_check.Checked;
                WritePrivateProfileString("Option", "notify_check", "false", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Chating\Setting\config.ini");
                notify_msg_check.Enabled = false;
            }
        }

        private void notify_msg_check_CheckedChanged(object sender, EventArgs e)
        {
            if (notify_msg_check.Checked)
            {
                Notifications.notify_msg_check = notify_msg_check.Checked;
                Main.notify_msg_check = notify_msg_check.Checked;
                WritePrivateProfileString("Option", "notify_msg_check", "true", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Chating\Setting\config.ini");                
            }
            else
            {
                Notifications.notify_msg_check = notify_msg_check.Checked;
                Main.notify_msg_check = notify_msg_check.Checked;
                WritePrivateProfileString("Option", "notify_msg_check", "false", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Chating\Setting\config.ini");               
            }
        }
    }
}
