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
using System.Net;
using System.IO.Compression;

namespace Update_Chating
{
    public partial class Update : Form
    {
        public Update()
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
                    //System.console.writeline(ex.Message.ToString());
                }
                return;
            }
            InitializeComponent();
        }

        private void ok_button_Click(object sender, EventArgs e)
        {
            Updating();
        }

        private void cancel_button_Click(object sender, EventArgs e)
        {
            Close();
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
        public void Updating()
        {       
            string url = "http://192.168.0.36/download.zip"; // 다운로드할 파일, 서버 IP 변경시 변경
            string path = Application.StartupPath + @"\temp\chating_download.zip"; // 파일 위치                
            //string file = Application.StartupPath + @"\temp"; // 압축을 푼 파일
                                                              //Environment.GetFolderPath(Environment.SpecialFolder.Desktop) // 데스크탑

            Folder_Delete(Application.StartupPath + @"\temp");
            File_Create();
            File_Recv(url, path);
        }

        /* 파일 만들기 */
        public void File_Create()
        {
            string dir = Application.StartupPath + @"\temp";

            if (Directory.Exists(dir))
            {
            }
            else
            {
                Directory.CreateDirectory(dir);
            }
        }

        /* 폴더 삭제 */
        public void Folder_Delete(string path)
        {
            DirectoryInfo folder = new DirectoryInfo(path);
            if(Directory.Exists(path))
            {
                folder.Delete(true);
            }
            
        }

        /* 파일 다운로드 */
        public void File_Recv(string url, string path)
        {
            try
            {
                WebClient webClient = new WebClient();
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(FileDownloadCompleted);
                webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(FileDownloadProgressChanged);
                //webClient.DownloadFile(url, path);
                ok_button.Enabled = false;
                webClient.DownloadFileAsync(new Uri(url), path);
            }
            catch (Exception)
            {
                MessageBox.Show("파일 다운로드를 완료하지 못했습니다.");
            }
        }
     
        /* 파일 압축 풀기 */
        public void Extract_ZIP_File(string zipFilePath, string backupFolder)
        {
            double persent;

            using (ZipArchive zipArchive = ZipFile.OpenRead(zipFilePath))
            {
                progressBar.Maximum = zipArchive.Entries.Count;
                download_state.Text = string.Format("0 / {0}", zipArchive.Entries.Count);
                Persent.Text = "0 %";

                foreach (ZipArchiveEntry zipArchiveEntry in zipArchive.Entries)
                {
                    try
                    {                                              
                        string folderPath = Path.GetDirectoryName(Path.Combine(backupFolder, zipArchiveEntry.FullName));
                        
                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }
                        
                        zipArchiveEntry.ExtractToFile(Path.Combine(backupFolder, zipArchiveEntry.FullName));
                        progressBar.Value++;
                        persent = ((double)progressBar.Value / zipArchive.Entries.Count) * 100;
                        Persent.Text = string.Format("{0} %", persent);
                        download_state.Text = string.Format("{0} / {1}", progressBar.Value, zipArchive.Entries.Count);
                    }
                    catch (PathTooLongException)
                    {
                        MessageBox.Show("압축 파일을 찾을 수 없습니다.");
                    }
                    catch(IOException)
                    {
                        MessageBox.Show("이미 파일이 존재합니다.");
                    }
                }
            }
        }
      
        /* 파일 실행 */
        public void Start_File()
        {
            string path = Application.StartupPath + @"\temp\Updating.exe";
            Process.Start(path);
        }

        private void Update_FormClosed(object sender, FormClosedEventArgs e)
        {
            Start_File();
        }

        //public event AsyncCompletedEventHandler DownloadFileCompleted;
        //public event DownloadProgressChangedEventHandler DownloadProgressChanged;

        //public delegate void AsyncCompletedEventHandler(Object sender, AsyncCompletedEventArgs e);
        //public delegate void DownloadProgressChangedEventHandler(Object sender, DownloadProgressChangedEventArgs e);

        //private delegate void CSafeSetText(string text);
        //private delegate void CsafeSetMaximum(Int32 value);
        //private delegate void CSafeSetValue(Int32 value);

        private void FileDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            download_state.Text = string.Format("{0} kb's / {1} kb's", e.BytesReceived, e.TotalBytesToReceive);
            progressBar.Value = e.ProgressPercentage;
            Persent.Text = string.Format("{0} %", e.ProgressPercentage);            
        }

        private void FileDownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            string path = Application.StartupPath + @"\temp\chating_download.zip"; // 파일 위치                
            string file = Application.StartupPath + @"\temp"; // 압축을 푼 파일

            if (e.Cancelled == true)
            {
                MessageBox.Show("다운로드 취소");
            }
            else
            {
                progressBar.Value = 0;
                Extract_ZIP_File(path, file);
                Close();
            }
        }
    }
}
