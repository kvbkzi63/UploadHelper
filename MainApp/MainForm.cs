using MainApp.Helper;
using MainApp.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UploadHelper.Helper;

namespace UploadHelper
{
     public partial class MainForm : Form
    {
        private bool error = false;
        List<FileInfo> fileInfos = null;
        private MyFormBase _waitForm;
        private AuthModel auth = null;
        private TokenModel token = null;
        private ScanerHook listener = new ScanerHook();

        public MainForm()
        {
            InitializeComponent();
            ECHelper.Instance.KillProcess();
            listener.ScanerEvent += Listener_ScanerEventAsync;
            listener.Start();
        }
        private async void Listener_ScanerEventAsync(ScanerHook.ScanerCodes codes)
        {
            ECHelper.Instance.WriteLog("Listener_ScanerEventAsync : start " + codes.Result);

            if ((codes.Result.StartsWith("finish") || codes.Result.StartsWith("query")))
            {
                ECHelper.Instance.WriteLog("Listener_ScanerEventAsync :" + codes.Result);
                if (codes.Result.StartsWith("finish"))
                {
                    string finish = await ECHelper.Instance.PickupOrderItems(codes.Result.Replace("finish", ""), token.content.token);
                    MessageBox.Show(finish);  
                } else if (codes.Result.StartsWith("query"))
                {
                    string query = await ECHelper.Instance.QueryOrderStatus(codes.Result.Replace("query", ""), token.content.token);
                    MessageBox.Show(query);
                }
            }
        }

        /// <summary>
        /// 初始化界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, System.EventArgs e)
        {
            try
            {
                //ECHelper.Instance.SetAutoRun();
                auth = ECHelper.Instance.GetAuthModel();
                if (auth == null)
                {
                    Validations validations = new Validations();
                    if (validations.ShowDialog() == DialogResult.OK)
                    {

                        auth = validations.AuthModel;
                        string s = ECHelper.Instance.ToXML<AuthModel>(auth);
                        ECHelper.Instance.SaveXml(s, ECHelper.xmlName.lineces);
                    }
                    else
                    {
                        MessageBox.Show("驗證失敗，程式關閉", "驗證");
                        this.Close();
                    }
                }
                else if (ECHelper.Instance.CheckAuthIsActived(auth.api_key, auth.password, ref token))
                {
                     fileInfos = ECHelper.Instance.GetUploadFiles(auth.uploadPath);
                    //指定使用的容器
                    this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
                    //建立NotifyIcon 
                    this.notifyIcon1.Text = "電子商務拋轉";
                    this.StartWork();
                }
                else
                {
                    throw new Exception("未經授權驗證");
                }
                ECHelper.Instance.CreateUploadFolder(auth.uploadPath); 
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                ECHelper.Instance.WriteLog(ex.ToString());
                this.notifyIcon1.ShowBalloonTip(5000, "開啟失敗",ex.ToString(), ToolTipIcon.Info);

                ECHelper.Instance.KillProcess();
                error = true;
                this.Close();
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
 
            this.Hide();
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.ShowBalloonTip(5000, "開啟成功", "開啟成功", ToolTipIcon.Info);
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker1.DoWork += new DoWorkEventHandler(worker_DoWorkAsync);
            backgroundWorker1.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
        }
        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                this.notifyIcon1.Visible = true;
            }
        }  
        private void notifyIcon1_MouseDoubleClick(Object sender, MouseEventArgs e)
        {
            //讓Form再度顯示，並寫狀態設為Normal
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }
        System.Timers.Timer timersTimer = new System.Timers.Timer();
        System.Timers.Timer timersTimer2 = new System.Timers.Timer();
        private void StartWork( )
        {
            this.lblMsg.Text = "監控中"; 
            timersTimer.Interval =10000;
            timersTimer.Enabled = true;
            timersTimer.Elapsed += TimersTimer_Elapsed;
            timersTimer.Start();
            timersTimer2.Interval = 900000;
            timersTimer2.Enabled = true;
            timersTimer2.Elapsed += TimersTimer2_Elapsed;
            timersTimer2.Start();
        }
        private void StopWork()
        {
            this.lblMsg.Text = "暫停監控"; 
            timersTimer.Elapsed -= TimersTimer_Elapsed;
            timersTimer.Stop();
        }
        private void TimersTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        { 
            ECHelper.Instance.CreateUploadFolder(auth.uploadPath); 
            this.BeginInvoke(new Action(() =>
                {
                    if (!backgroundWorker1.IsBusy)
                    {
                        backgroundWorker1.RunWorkerAsync();
                    }
                }), null); 
        }
        private void TimersTimer2_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!(ECHelper.Instance.CheckAuthIsActived(auth.api_key, auth.password, ref token)))
            {
                this.lblMsg.Text = "驗證失敗，暫停監控";
                StopWork();
            }
        }
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.WindowState != FormWindowState.Minimized && !error)
            {
                e.Cancel = true;
                this.WindowState = FormWindowState.Minimized;
                notifyIcon1.Tag = string.Empty;
                notifyIcon1.ShowBalloonTip(3000, this.Text,
                     "程式並未結束，要結束請在圖示上按右鍵，選取結束功能!",
                     ToolTipIcon.Info);
            }
        }
        #region     登陸檢核
        protected void ShowWaitForm(string message)
        {
            // don't display more than one wait form at a time
            if (_waitForm != null && !_waitForm.IsDisposed)
            {
                return;
            }
         
            _waitForm = new MyFormBase();
            _waitForm.SetMessage(message); // "Loading data. Please wait..."
            _waitForm.TopMost = true;
            _waitForm.StartPosition = FormStartPosition.CenterScreen;
            _waitForm.Show();
            _waitForm.Refresh();

            // force the wait window to display for at least 700ms so it doesn't just flash on the screen
            System.Threading.Thread.Sleep(3000);
            Application.Idle += OnLoaded;
        }
        private void OnLoaded(object sender, EventArgs e)
        {
            Application.Idle -= OnLoaded;
            _waitForm.Close();
        }
        #endregion

        #region     背景執行檔案上傳
        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.ProgressBar1.Value = e.ProgressPercentage;
            this.lblMsg.Text = "上傳中：" + e.UserState ;
            this.notifyIcon1.ShowBalloonTip(1000, "上傳中", e.UserState.ToString(), ToolTipIcon.Info);

        }
        private void worker_DoWorkAsync(object sender, DoWorkEventArgs e)
        {
            try
            {
                fileInfos = ECHelper.Instance.GetUploadFiles(auth.uploadPath);

                if (fileInfos.Count != 0)
                {
                    int i = 1;

                    foreach (FileInfo file in fileInfos)
                    {
                        int p = Convert.ToInt16(Math.Round((decimal)i / (decimal)fileInfos.Count(), 2) * 100);
                        Thread.Sleep(1000); 
                        backgroundWorker1.ReportProgress(p, file.Name);
                        ECHelper.Instance.CheckAuthIsActived(auth.api_key, auth.password, ref token);
                        ECHelper.Instance.UploadFileCompleteAsync(file, token.content.token);
                        i++;
                    } 
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        //執行完成
        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if ((e.Cancelled == true))
                {
                    this.lblMsg.Text = "取消!";

                }
                else if (!(e.Error == null))
                {
                    ECHelper.Instance.WriteLog(e.ToString());
                    //ECHelper.Instance.WriteLog("上傳失敗：" + currentCount.ToString() + "　/　" + Comments.Count.ToString()); 
                    this.lblMsg.Text = ("Error: " + e.Error.Message + " "); 

                }
                else
                {
                    this.lblMsg.Text = "上傳完畢!"; 
                    ECHelper.Instance.WriteLog("監控中");
                    this.lblMsg.Text = "監控中";
                    this.ProgressBar1.Value = 0;

                }
            }
            catch (Exception ex)
            {
                lblMsg.Text = ex.ToString();

            }
            finally
            {
                fileInfos = null;
            }
            //unLockUI();
        }

        #endregion

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            notifyIcon1.Dispose();
            notifyIcon1.Icon = null;
        }
        #region     MenuFunction
        private void close_Click(object sender, EventArgs e)
        {
            ECHelper.Instance.KillProcess();
            error = true;
            this.Close();
        } 

        private void StopScan_Click_1(object sender, EventArgs e)
        {
            StopWork();
        }
        #endregion

        private void StartScan_Click(object sender, EventArgs e)
        {
            StartWork();
        }

        private void UploadPathChange_Click(object sender, EventArgs e)
        {
            try
            {
                StopWork();
                Validations validations = new Validations(auth);
                if (validations.ShowDialog() == DialogResult.OK)
                { 
                    auth = validations.AuthModel;
                    string s = ECHelper.Instance.ToXML<AuthModel>(auth);
                    ECHelper.Instance.SaveXml(s, ECHelper.xmlName.lineces);
                }
            }
            catch (Exception ex)
            {
                ECHelper.Instance.WriteLog(ex.ToString());
            }
            finally
            {
                StartWork();
            }  
        }

        private void SetMinimized_Click(object sender, EventArgs e)
        { 
            this.WindowState = FormWindowState.Minimized;
            notifyIcon1.Tag = string.Empty;
            notifyIcon1.ShowBalloonTip(3000, this.Text, "程式並未結束，要結束請在圖示上按右鍵，選取結束功能!", ToolTipIcon.Info);
        }

 
    }
}
