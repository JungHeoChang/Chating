using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Principal;
using System.Diagnostics;
using System.IO;

namespace Updating
{
    public partial class Updating : Form
    {
        public Updating()
        {
            if (IsAdministrator() == false)
            {
                try
                {
                    ProcessStartInfo procInfo = new ProcessStartInfo();
                    procInfo.UseShellExecute = true;
                    procInfo.FileName = Application.ExecutablePath;
                    procInfo.WorkingDirectory = Environment.CurrentDirectory;
                    procInfo.Verb = "runas";
                    Process.Start(procInfo);
                    Close();
                }
                catch (Exception)
                {
                }
                return;
            }
            InitializeComponent();            
        }

        public static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();

            if (null != identity)
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            return false;
        }

        /* 업데이트 */
        public void Update_chating()
        {
            File_Copy();          
            progressBar.Value = 100;
            Start_File();
            Close();
        }        

        /* 파일 복사 덮어쓰기 */
        public void File_Copy()
        {
            Process[] processes = Process.GetProcessesByName("chating");
            FileInfo file = new FileInfo(Application.StartupPath + @"\chating.exe");
            progressBar.Value = 50;

            try
            {
                //file.CopyTo(Path.GetDirectoryName(Environment.CurrentDirectory) + @"\chating.exe", true);
                file.CopyTo(Path.GetDirectoryName(Application.StartupPath) + @"\chating.exe", true);
                MessageBox.Show("업데이트 완료");
            }
            catch(FileNotFoundException)
            {
                MessageBox.Show("파일을 찾을 수 없습니다.");
                Close();
            }
            catch(IOException)
            {
                foreach (Process p in processes)
                    p.Kill();
                MessageBox.Show("파일이 실행 중이므로 덮어쓸 수 없습니다.");
                Close();
            }
            
        }
        
        /* 파일 실행 */
        public void Start_File()
        {
            string path = Path.GetDirectoryName(Application.StartupPath) + @"\chating.exe";
            try
            {
                Process.Start(path);
            }
            catch(Exception)
            {
                MessageBox.Show("지정된 파일을 찾을 수 없습니다.");
                Close();
            }
        }

        private void Updating_Load(object sender, EventArgs e)
        {
            Update_chating();
        }

        // 쓰지않거나 다른것으로 교체 가능
        /* 파일 이름 변경 */
        public void Re_Name()
        {
            FileInfo fileRename = new FileInfo(Path.GetDirectoryName(Environment.CurrentDirectory) + @"\chating.exe");
            //FileInfo fileRename = new FileInfo(@"sample3.txt");
            if (fileRename.Exists)
            {
                try
                {
                    fileRename.MoveTo(Path.GetDirectoryName(Environment.CurrentDirectory) + @"\chating_old.exe"); // 이미있으면 에러
                    fileRename.IsReadOnly = false;
                }
                catch (Exception)
                {
                    MessageBox.Show("동일한 이름의 파일이 존재합니다!");
                }
            }
        }

        /* 파일 이동 */
        public void Move_File()
        {
            string file_path = Application.StartupPath + @"\chating.exe";
            string path = Path.GetDirectoryName(Environment.CurrentDirectory) + @"\chating.exe";

            File.Move(file_path, path);
            System.IO.File.Move(file_path, path);
        }

        /* 폴더 삭제 */
        public void Folder_Delete(string path)
        {
            DirectoryInfo folder = new DirectoryInfo(path);
            folder.Delete(true);
        }

        /* 파일 삭제 */
        public void Delete_File(string path)
        {
            //FileInfo file = new FileInfo(Application.StartupPath + "\\chating_old.exe"); // 실행 중 삭제할 경우 오류
            FileInfo zipfile = new FileInfo(Application.StartupPath + path);

            if (zipfile.Exists) // 삭제할 파일이 있는지
            {
                zipfile.Delete();
            }
            else
            {
                MessageBox.Show("삭제할 파일을 찾지 못했습니다.");
            }
        }      
    }
}
