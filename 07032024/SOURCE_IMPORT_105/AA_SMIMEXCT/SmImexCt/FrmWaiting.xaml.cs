using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Sm.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Interop;
using System.Threading;
using System.Diagnostics;
using System.Windows.Threading;
using System.Runtime.InteropServices;

namespace AA_SMIMEXCT
{
    /// <summary>
    /// Interaction logic for FrmWaiting.xaml
    /// </summary>
    public partial class FrmWaiting : Form
    {
        System.ComponentModel.BackgroundWorker mWorker;
        public double pgValue { get { return PBar.Maximum; } set { PBar.Maximum = value; } }
        public FrmWaiting(double maximum)
        {
            InitializeComponent();
            this.ShowInTaskbar = false;
            PBar.Maximum = maximum;
            PBar.Value = 0;
            this.Topmost = true;
            SmLib.SysFunc.LoadIcon(this);
            this.ActiveMainForm = false;
            lblMessage.Text = (StartUp.M_LAN == "V" ? "Đang kiểm tra dữ liệu..." : "Processing...");

            txtVer.Text = StartUp.SysObj.VersionInfo.Rows[0]["product"].ToString();
            
        }

        public void Set(double value)
        {
            mWorker = new System.ComponentModel.BackgroundWorker();
            mWorker.WorkerReportsProgress = true;
            mWorker.WorkerSupportsCancellation = true;
            mWorker.RunWorkerAsync();
            if (!mWorker.CancellationPending)
            {
                int n_post_error = 0;
                if (C_ImportVoucher.tb_Post_Error != null)
                    n_post_error = C_ImportVoucher.tb_Post_Error.Rows.Count;
                PBar.Value = value;
                lblMessage.Text = (StartUp.M_LAN == "V" ? "Đang thực hiện...  " : "Processing...  ") + PBar.Value.ToString() + "/" + PBar.Maximum.ToString() + (n_post_error.Equals(0)? "": " (Lỗi " + n_post_error.ToString() + ")");
            }
            System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background,
                                       new System.Threading.ThreadStart(delegate { }));
        }

        public void Set_Delete(double value)
        {
            mWorker = new System.ComponentModel.BackgroundWorker();
            mWorker.WorkerReportsProgress = true;
            mWorker.WorkerSupportsCancellation = true;
            mWorker.RunWorkerAsync();
            if (!mWorker.CancellationPending)
            {
                PBar.Value = value;
                lblMessage.Text = (StartUp.M_LAN == "V" ? "Đang xóa dữ liệu trùng...  " : "Processing...  ") + PBar.Value.ToString() + "/" + PBar.Maximum.ToString();
            }
            System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background,
                                       new System.Threading.ThreadStart(delegate { }));
        }

        public void Set_Post_Error(double value)
        {
            mWorker = new System.ComponentModel.BackgroundWorker();
            mWorker.WorkerReportsProgress = true;
            mWorker.WorkerSupportsCancellation = true;
            mWorker.RunWorkerAsync();
            if (!mWorker.CancellationPending)
            {
                PBar.Value = value;
                lblMessage.Text = (StartUp.M_LAN == "V" ? "Chương trình hủy số liệu đưa vào...  " : "Processing...  ") + PBar.Value.ToString() + "/" + PBar.Maximum.ToString();
            }
            System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background,
                                       new System.Threading.ThreadStart(delegate { }));
        }

        protected override void OnClosed(EventArgs e)
        {
            //base.OnClosed(e);
        }
    }
}
