using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Configuration;
using System.ServiceProcess;

namespace HowToUploadFilesToFTP
{
    public partial class Form1 : Form
    {
        BackgroundWorker bg = null;
        NetworkCredential credentials = null;
        public string url { get; set; }
        public string localFilePath { get; set; }
        public string localPath { get; set; }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            bg = new BackgroundWorker();
            bg.DoWork += Bg_DoWork;
            bg.ProgressChanged += Bg_ProgressChanged;
            bg.WorkerReportsProgress = true;
            bg.RunWorkerCompleted += Bg_RunWorkerCompleted;
            this.Text += " " + ConfigurationManager.AppSettings["app-version"].ToString();
        }

        private void Bg_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                txtStatusUnZip.AppendText(Environment.NewLine);
                this.txtStatusUnZip.AppendText("Descarga Finalizada..");
                this.progressBar1.Value = 0;
                this.lblDescargando.Text = "Descargando Archivo:";
                MessageBox.Show("Descarga Finalizada, Ahora puedes abrir el sistema recuerda presiona F5 en el navegador...", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.txtStatusUnZip.Text = "";
                Process.Start("chrome.exe", ConfigurationManager.AppSettings["url-sitio"].ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Bg_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                txtStatusUnZip.AppendText(Environment.NewLine);
                this.txtStatusUnZip.AppendText(e.UserState.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void Bg_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                this.UnZip();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            //Get a FileInfo object for the file that will
            // be uploaded.
            FileInfo toUpload = new FileInfo(this.txtFile.Text);

            //Get a new FtpWebRequest object.
            FtpWebRequest request =
                (FtpWebRequest)WebRequest.Create(
                this.txtAddress.Text + "/" + toUpload.Name
                );

            //Method will be UploadFile.
            request.Method = WebRequestMethods.Ftp.UploadFile;

            //Set our credentials.
            request.Credentials =
                new NetworkCredential(this.txtUserName.Text,
                                        this.txtPassword.Text);

            //Setup a stream for the request and a stream for
            // the file we'll be uploading.
            Stream ftpStream = request.GetRequestStream();
            FileStream file = File.OpenRead(this.txtFile.Text);

            //Setup variables we'll use to read the file.
            int length = 1024;
            byte[] buffer = new byte[length];
            int bytesRead = 0;

            //Write the file to the request stream.
            do
            {
                bytesRead = file.Read(buffer, 0, length);
                ftpStream.Write(buffer, 0, bytesRead);
            }
            while (bytesRead != 0);

            //Close the streams.
            file.Close();
            ftpStream.Close();

            MessageBox.Show("Upload complete");
        }

        private void btnPickFile_Click(object sender, EventArgs e)
        {
            DialogResult result = fileDialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                this.txtFile.Text = fileDialog.FileName;
            }
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dialogResult = MessageBox.Show("La actualización del wms-lluvia cerrar tu navegador", "Mensaje", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialogResult == DialogResult.Yes)
                {

                    
                    cerrarNavegadores();
                    this.url = "ftp://" + this.txtAddress.Text + "/" + this.txtFileToDownload.Text;
                    this.txtStatusUnZip.Text = "Descargando archivo :" + this.url;
                    this.localFilePath = Path.Combine(this.txtDownloadPath.Text, "deploy.zip");
                    RespaldarZip();
                    this.credentials = new NetworkCredential(this.txtUserName.Text, this.txtPassword.Text);
                    FtpWebRequest listRequest = (FtpWebRequest)WebRequest.Create(this.url);
                    listRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                    listRequest.Credentials = credentials;
                    Task download = new Task(() => this.DescargarArchivo(this.url, listRequest));
                    download.RunSynchronously();
                    this.bg.RunWorkerAsync();
                }
                else if (dialogResult == DialogResult.No)
                {
                    //do something else
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void RespaldarZip()
        {
            try
            {
                if (File.Exists(this.localFilePath))
                {
                    Directory.Move(this.localFilePath, Path.Combine(this.txtDownloadPath.Text, "back" + DateTime.Now.ToString("yyyyMMddhms") + ".zip"));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void DescargarArchivo(string url, FtpWebRequest listRequest)
        {
            try
            {
                string fileUrl = url;
                WebRequest sizeRequest = WebRequest.Create(url);
                sizeRequest.Credentials = credentials;
                sizeRequest.Method = WebRequestMethods.Ftp.GetFileSize;
                int size = (int)sizeRequest.GetResponse().ContentLength;
             
                if (this.progressBar1.InvokeRequired) this.progressBar1.Invoke(new Action(() => progressBar1.Maximum = size)); else this.progressBar1.Maximum = size;

                // Download the file
                WebRequest request = WebRequest.Create(url);
                request.Credentials = credentials;
                request.Method = WebRequestMethods.Ftp.DownloadFile;

                using (Stream ftpStream = request.GetResponse().GetResponseStream())
                using (Stream fileStream = File.Create(this.localFilePath))
                {
                    byte[] buffer = new byte[10240];
                    int read;
                    while ((read = ftpStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        fileStream.Write(buffer, 0, read);
                        int position = (int)fileStream.Position;
                        progressBar1.Invoke(
                            (MethodInvoker)(() => progressBar1.Value = position));
                    }
                }
                this.lblDescargando.Text = "Archivo Descargado";
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void DescargarFolder(string url, FtpWebRequest listRequest, string localPath)
        {

            try
            {
                List<string> lines = new List<string>();
                using (FtpWebResponse listResponse = (FtpWebResponse)listRequest.GetResponse())
                using (Stream listStream = listResponse.GetResponseStream())
                using (StreamReader listReader = new StreamReader(listStream))
                {
                    while (!listReader.EndOfStream)
                    {
                        lines.Add(listReader.ReadLine());
                    }
                }

                foreach (string line in lines)
                {
                    string[] tokens =
                        line.Split(new[] { ' ' }, 9, StringSplitOptions.RemoveEmptyEntries);
                    string name = tokens[8];
                    string permissions = tokens[0];

                    string localFilePath = Path.Combine(localPath, name);
                    string fileUrl = url + "/" + name;

                    if (permissions[0] == 'd')
                    {
                        if (!Directory.Exists(localFilePath))
                        {
                            Directory.CreateDirectory(localFilePath);
                        }
                        if (!name.Equals(".") && !name.Equals("..") && !name.Equals("Codigos") && !name.Equals("bundles"))
                            DescargarFolder(fileUrl + "/", listRequest, localFilePath);
                    }
                    else
                    {
                        Console.WriteLine(fileUrl);
                        FtpWebRequest downloadRequest = (FtpWebRequest)WebRequest.Create(fileUrl);
                        downloadRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                        downloadRequest.Credentials = listRequest.Credentials;

                        using (FtpWebResponse downloadResponse =
                                  (FtpWebResponse)downloadRequest.GetResponse())
                        using (Stream sourceStream = downloadResponse.GetResponseStream())
                        using (Stream targetStream = File.Create(localFilePath))
                        {
                            byte[] buffer = new byte[10240];
                            int read;
                            while ((read = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                targetStream.Write(buffer, 0, read);
                            }
                            this.bg.ReportProgress(0, name);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }

        }

        public void UnZip()
        {
            int index = 1;
            try
            {
                using (ZipArchive archive = ZipFile.OpenRead((this.localFilePath)))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        string file = Path.Combine(this.txtDownloadPath.Text, entry.FullName);
                        if (System.IO.Path.GetExtension(file) == string.Empty)
                        {
                            if (Directory.Exists(file))
                            {
                                Directory.Delete(file, true);
                                Directory.CreateDirectory(file);
                            }
                            else
                            {
                                Directory.CreateDirectory(file);
                            }
                        }
                        else
                        {
                            if (File.Exists(file))
                            {
                                if (entry.FullName.Equals("Web.config"))
                                    File.Move(file, Path.Combine(this.txtDownloadPath.Text, "Webback" + DateTime.Now.ToString("yyyyMMddhms") + ".config"));

                                    File.Delete(file);
                                entry.ExtractToFile(file);
                            }
                            else
                            {
                                entry.ExtractToFile(file);
                            }
                        }
                        this.bg.ReportProgress(index, "\n UnZip: " + file);
                        index++;
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        public void cerrarNavegadores() {
            try
            {
                Process[] AllProcesses = Process.GetProcesses();
                foreach (var process in AllProcesses)
                {
                    if (process.MainWindowTitle != "")
                    {
                        string s = process.ProcessName.ToLower();
                        Console.WriteLine(s);
                        if (s == "iexplore" || s == "iexplorer" || s == "chrome" || s == "firefox" || s.ToLower().Contains("msedge"))
                            process.Kill();
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void startIIS() {
            ServiceController iisService = new ServiceController("W3SVC");

            if (iisService != null)

            {

                do

                {

                    iisService.Refresh();

                }

                while (iisService.Status == ServiceControllerStatus.ContinuePending ||

                       iisService.Status == ServiceControllerStatus.PausePending ||

                       iisService.Status == ServiceControllerStatus.StartPending ||

                       iisService.Status == ServiceControllerStatus.StopPending);

                if (iisService.Status == ServiceControllerStatus.Stopped)

                {

                    iisService.Start();

                    iisService.WaitForStatus(ServiceControllerStatus.Running);

                }

                else

                {

                    if (ServiceControllerStatus.Paused == iisService.Status)

                    {

                        iisService.Continue();

                        iisService.WaitForStatus(ServiceControllerStatus.Running);

                    }

                }

                iisService.Close();

            }
        }
        public void stopIIS() {
            try
            {
                ServiceController iisService = new ServiceController("W3SVC");
                if (iisService != null)
                {

                    do

                    {

                        iisService.Refresh();

                    }

                    while (iisService.Status == ServiceControllerStatus.ContinuePending ||

                           iisService.Status == ServiceControllerStatus.PausePending ||

                           iisService.Status == ServiceControllerStatus.StartPending ||

                           iisService.Status == ServiceControllerStatus.StopPending);

                    if (iisService.Status == ServiceControllerStatus.Running ||

                        iisService.Status == ServiceControllerStatus.Paused)

                    {

                        iisService.Stop();

                        iisService.WaitForStatus(ServiceControllerStatus.Stopped);

                    }
                    iisService.Close();
                }
            }
            catch (Exception ex)
            {
                txtStatusUnZip.AppendText(Environment.NewLine);
                this.txtStatusUnZip.AppendText(ex.Message +" ::::::::: "+ex.StackTrace);

            }
        }
        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void lblDescargando_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void txtAddress_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                this.stopIIS();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
          
        }

        private void btnStartIIS_Click(object sender, EventArgs e)
        {
            try
            {
                this.startIIS();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
        }
    }
}