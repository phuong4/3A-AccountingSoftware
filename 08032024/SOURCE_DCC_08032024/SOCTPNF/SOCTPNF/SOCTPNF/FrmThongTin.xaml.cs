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
using System.Data;
using SmVoucherLib;
using Infragistics.Windows.DataPresenter;

namespace SOCTPNF
{
    /// <summary>
    /// Interaction logic for FrmThongTin.xaml
    /// </summary>
    public partial class FrmThongTin : Form
    {
        public decimal so_luong = 0, so_luong_km = 0, t_tien_nt = 0, t_tien = 0, t_tien_nt2 = 0,
                t_ck_nt = 0, t_ck = 0, t_tien_nt2_ck_nt, t_tien2_ck = 0,
                t_tien2 = 0, t_thue_nt = 0, t_thue = 0, t_tt_nt = 0, t_tt = 0,
                t_tien_km_nt = 0, t_tien_km = 0, t_thue_km_nt = 0, t_thue_km = 0, tong_tien_nt = 0, tong_tien = 0;
        

        public FrmThongTin()
        {
            InitializeComponent();
            SmLib.SysFunc.LoadIcon(this);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Form_Loaded(object sender, RoutedEventArgs e)
        {
            string M_IP_SL = string.Empty;
            string M_IP_TIEN_NT = string.Empty;
            string M_IP_TIEN = string.Empty;
            string M_IP_Gia_NT = string.Empty;
            string M_IP_GIA = string.Empty;

            M_IP_SL = StartUp.SysObj.GetOption("M_IP_SL").ToString();
            M_IP_TIEN_NT = StartUp.SysObj.GetOption("M_IP_TIEN_NT").ToString();
            M_IP_TIEN = StartUp.SysObj.GetOption("M_IP_TIEN").ToString();

            StartUp.DsTrans.Tables[0].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[FrmSOCTPNF.iRow]["stt_rec"].ToString() + "'";
            StartUp.DsTrans.Tables[1].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[FrmSOCTPNF.iRow]["stt_rec"].ToString() + "'";
            
            if (StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].Equals(StartUp.M_ma_nt0))
            {
                M_IP_TIEN_NT = M_IP_TIEN;
            }

            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_so_luong"].ToString(), out so_luong);
            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien_nt"].ToString(), out t_tien_nt);
            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien"].ToString(), out t_tien);
            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien_nt2"].ToString(), out t_tien_nt2);
            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tien2"].ToString(), out t_tien2);

            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_ck_nt"].ToString(), out t_ck_nt);
            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_ck"].ToString(), out t_ck);

            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_thue_nt"].ToString(), out t_thue_nt);
            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_thue"].ToString(), out t_thue);
            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tt_nt"].ToString(), out t_tt_nt);
            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["t_tt"].ToString(), out t_tt);

            t_tien_nt2_ck_nt = t_tien_nt2 - t_ck_nt;
            t_tien2_ck = t_tien2 - t_ck;

            tong_tien_nt = t_tt_nt + t_tien_km_nt + t_thue_km_nt;
            tong_tien = t_tt + t_tien_km + t_thue_km;

            //So luong
            txtso_luong.Text = so_luong.ToString();//so_luong.ToString(M_IP_SL);
            txtso_luong.Format = M_IP_SL;
            txtso_luong_km.Text = so_luong_km.ToString();
            txtso_luong_km.Format = M_IP_SL;

            //Tien von
            txtt_tien_nt.Text = t_tien_nt.ToString();
            txtt_tien_nt.Format = M_IP_TIEN_NT;
            txtt_tien.Text = t_tien.ToString();
            txtt_tien.Format = M_IP_TIEN;

            //Tien hang
            txtt_tien_nt2.Text = t_tien_nt2.ToString();
            txtt_tien_nt2.Format = M_IP_TIEN_NT;
            txtt_tien2.Text = t_tien2.ToString();
            txtt_tien2.Format = M_IP_TIEN;

            //Tien chiet khau
            txtt_ck_nt.Text = t_ck_nt.ToString();
            txtt_ck_nt.Format = M_IP_TIEN_NT;
            txtt_ck.Text = t_ck.ToString();
            txtt_ck.Format = M_IP_TIEN;

            //Tien sau chiet khau
            txtt_tien_nt2_ck_nt.Text = t_tien_nt2_ck_nt.ToString();
            txtt_tien_nt2_ck_nt.Format = M_IP_TIEN_NT;
            txtt_tien2_ck.Text = t_tien2_ck.ToString();
            txtt_tien2_ck.Format = M_IP_TIEN;

            //Tien thue
            txtt_thue_nt.Text = t_thue_nt.ToString();
            txtt_thue_nt.Format = M_IP_TIEN_NT;
            txtt_thue.Text = t_thue.ToString();
            txtt_thue.Format = M_IP_TIEN;

            //Tong tt
            txtt_tt_nt.Text = t_tt_nt.ToString();
            txtt_tt_nt.Format = M_IP_TIEN_NT;
            txtt_tt.Text = t_tt.ToString();
            txtt_tt.Format = M_IP_TIEN;

            //Tien khuyen mai
            txtt_tien_km_nt.Text = t_tien_km_nt.ToString();
            txtt_tien_km_nt.Format = M_IP_TIEN_NT;
            txtt_tien_km.Text = t_tien_km.ToString();
            txtt_tien_km.Format = M_IP_TIEN;

            //Thue khuyen mai
            txtt_thue_km_nt.Text = t_thue_km_nt.ToString();
            txtt_thue_km_nt.Format = M_IP_TIEN_NT;
            txtt_thue_km.Text = t_thue_km.ToString();
            txtt_thue_km.Format = M_IP_TIEN;

            //Tong tien
            txttong_tien_nt.Text = tong_tien_nt.ToString();
            txttong_tien_nt.Format = M_IP_TIEN_NT;
            txttong_tien.Text = tong_tien.ToString();
            txttong_tien.Format = M_IP_TIEN;

            this.btnCancel.Focus();
        }
    }
}
