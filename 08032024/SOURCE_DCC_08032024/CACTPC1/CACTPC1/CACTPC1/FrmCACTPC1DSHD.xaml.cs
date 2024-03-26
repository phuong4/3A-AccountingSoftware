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
using System.Data;
using System.Data.SqlClient;
using System.Windows.Threading;
using Infragistics.Windows.DataPresenter;
using Sm.Windows.Controls;

namespace CACTPC1
{
    /// <summary>
    /// Interaction logic for FrmCACTPC1DSHD.xaml
    /// </summary>
    public partial class FrmCACTPC1DSHD : Form
    {
        bool isMePress = false;
        public FrmCACTPC1DSHD()
        {
            InitializeComponent();
            SmLib.SysFunc.LoadIcon(this);
            LoadData();
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
            //if (grdDSHD.Records.Count > 0)
            //{
            //    grdDSHD.Focus();
            //    grdDSHD.ActiveRecord = grdDSHD.Records[0];
            //}}));
        }

        public void LoadData()
        {
            string stt_rec = string.Empty;
            string ma_kh = string.Empty;
            string ma_dvcs = string.Empty;

            stt_rec = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString();
            ma_kh = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_kh"].ToString();
            ma_dvcs = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_dvcs"].ToString();
            SqlCommand cmd = new SqlCommand("Exec [CACTPC1-InitTt] @stt_rec, @ma_kh, @Ma_dvcs");
            cmd.Parameters.Add("@stt_rec", SqlDbType.Char, 11).Value = stt_rec;
            cmd.Parameters.Add("@ma_kh", SqlDbType.Char, 16).Value = ma_kh;
            cmd.Parameters.Add("@Ma_dvcs", SqlDbType.Char, 16).Value =  ma_dvcs;
            DataTable dt = StartUp.SysObj.ExcuteReader(cmd).Tables[0].Copy();
            dt.DefaultView.Sort = "ngay_ct, so_ct";
            grdDSHD.DataSource = dt.DefaultView;
            
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (!isMePress)
                return;
            if (e.Key == Key.Escape || e.Key==Key.Enter)
            {
                this.Close();
            }
        }

        void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            isMePress = true;
        }

        public void ShowHd(string so_ct0, string stt_rec)
        {
            DataRecord r;
            foreach (Record rec in grdDSHD.Records)
            {
                r = rec as DataRecord;
                if (r == null || r.DataItem == null)
                    continue;
                if (r.Cells["so_ct"].Value.ToString().Trim().ToUpper() == so_ct0.Trim().ToUpper())// && r.Cells["stt_rec"].Value.ToString() == stt_rec)
                {
                    grdDSHD.ActiveRecord = rec;
                    grdDSHD.Focus();
                    return ;
                }
            }
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                if (grdDSHD.Records.Count > 0)
                {
                    grdDSHD.Focus();
                    grdDSHD.ActiveRecord = grdDSHD.Records[0];
                }
            }));

            ShowDialog();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            SmLib.WinAPISenkey.SenKey(ModifierKeys.None, Key.Tab);
        }
    }
}
