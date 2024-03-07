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
using System.Data.SqlClient;

namespace AAA_QLHD3A
{
    /// <summary>
    /// Interaction logic for FrmPoctpnaCopy.xaml
    /// </summary>
    public partial class Frmcreate : Form
    {
        public Frmcreate()
        {
            InitializeComponent();
            this.BindingSysObj = StartUp.SysObj;
            SmLib.SysFunc.LoadIcon(this);
        }

        public bool isOk = false;
        //public FrmView frm;
        //public DataSet dsHdm;

        private void Form_Loaded(object sender, RoutedEventArgs e)
        {
            //txtNgay_ct_old.Focus();
        }

        private void ConfirmGridView_OnOk(object sender, RoutedEventArgs e)
        {
            #region Hoa don ban hang
            if (radioButton_hda.IsChecked == true)
            {
                //MessageBox.Show(StartUp.zzkieu_loc.ToString());

                if (StartUp.zzkieu_loc ==1)
                {
                    MessageBox.Show("Kiểu lọc dữ liệu không đúng");
                    return;
                }

                string listSttRec = "";
                int stt = 0;
                foreach (DataRow row in StartUp.tbMain.Rows)
                {
                    if (row["tag"].ToString().Trim().Equals("True") && string.IsNullOrEmpty(row["stt_rec"].ToString().Trim()))
                    {
                        string stt_rec_hd = row["stt_rec_hd"].ToString().Trim();
                        stt += 1;

                        listSttRec = listSttRec + stt_rec_hd + ",";

                    }
                }

                //var tbSelected2 = StartUp.tbMain.Clone();
                SqlCommand cmd = new SqlCommand("exec [dbo].[QLHD_create_HDA] @Startdate, @EndDate, @ma_dvcs,@listSttRec");
                cmd.Parameters.Add("@Startdate", SqlDbType.VarChar).Value = (string.IsNullOrEmpty(StartUp.zStartdate.ToString()) ? "" : string.Format("{0:yyyyMMdd}", (DateTime)StartUp.zStartdate));
                cmd.Parameters.Add("@EndDate", SqlDbType.VarChar).Value = (string.IsNullOrEmpty(StartUp.zEndDate.ToString()) ? "" : string.Format("{0:yyyyMMdd}", (DateTime)StartUp.zEndDate));
                cmd.Parameters.Add("@ma_dvcs", SqlDbType.VarChar).Value = StartUp.M_MA_DVCS.Trim();
                cmd.Parameters.Add("@listSttRec", SqlDbType.VarChar).Value = listSttRec;
                StartUp.SysObj.ExcuteNonQuery(cmd);

                
                ExMessageBox.Show(9392, StartupBase.SysObj, "Tạo Hoá đơn bán hàng. Thành công!", "", MessageBoxButton.OK, MessageBoxImage.Asterisk);


            }
            #endregion

            #region Hoa don dich_vu
            if (radioButton_hd1.IsChecked == true)
            {
                if (StartUp.zzkieu_loc == 1)
                {
                    MessageBox.Show("Kiểu lọc dữ liệu không đúng");
                    return;
                }

                string listSttRec = "";
                int stt = 0;
                foreach (DataRow row in StartUp.tbMain.Rows)
                {
                    if (row["tag"].ToString().Trim().Equals("True") && string.IsNullOrEmpty(row["stt_rec"].ToString().Trim()))
                    {
                        string stt_rec_hd = row["stt_rec_hd"].ToString().Trim();
                        stt += 1;

                        listSttRec = listSttRec + stt_rec_hd + ",";

                    }
                }

                //var tbSelected2 = StartUp.tbMain.Clone();
                SqlCommand cmd = new SqlCommand("exec [dbo].[QLHD_create_HD1] @Startdate, @EndDate, @ma_dvcs,@listSttRec");
                cmd.Parameters.Add("@Startdate", SqlDbType.VarChar).Value = (string.IsNullOrEmpty(StartUp.zStartdate.ToString()) ? "" : string.Format("{0:yyyyMMdd}", (DateTime)StartUp.zStartdate));
                cmd.Parameters.Add("@EndDate", SqlDbType.VarChar).Value = (string.IsNullOrEmpty(StartUp.zEndDate.ToString()) ? "" : string.Format("{0:yyyyMMdd}", (DateTime)StartUp.zEndDate));
                cmd.Parameters.Add("@ma_dvcs", SqlDbType.VarChar).Value = StartUp.M_MA_DVCS.Trim();
                cmd.Parameters.Add("@listSttRec", SqlDbType.VarChar).Value = listSttRec;
                StartUp.SysObj.ExcuteNonQuery(cmd);

                ExMessageBox.Show(9391, StartupBase.SysObj, "Tạo Hoá đơn Dich vu. Thành công!", "", MessageBoxButton.OK, MessageBoxImage.Asterisk);


            }
            #endregion

            #region Nhap mua
            if (radioButton_pna.IsChecked == true)
            {
                if (StartUp.zzkieu_loc == 2)
                {
                    MessageBox.Show("Kiểu lọc dữ liệu không đúng");
                    return;
                }

                string listSttRec = "";
                int stt = 0;
                foreach (DataRow row in StartUp.tbMain.Rows)
                {
                    if (row["tag"].ToString().Trim().Equals("True") && string.IsNullOrEmpty(row["stt_rec"].ToString().Trim()))
                    {
                        string stt_rec_hd = row["stt_rec_hd"].ToString().Trim();
                        stt += 1;

                        listSttRec = listSttRec + stt_rec_hd + ",";

                    }
                }

                SqlCommand cmd = new SqlCommand("exec [dbo].[QLHD_create_PNA] @Startdate, @EndDate, @ma_dvcs,@listSttRec");
                cmd.Parameters.Add("@Startdate", SqlDbType.VarChar).Value = (string.IsNullOrEmpty(StartUp.zStartdate.ToString()) ? "" : string.Format("{0:yyyyMMdd}", (DateTime)StartUp.zStartdate));
                cmd.Parameters.Add("@EndDate", SqlDbType.VarChar).Value = (string.IsNullOrEmpty(StartUp.zEndDate.ToString()) ? "" : string.Format("{0:yyyyMMdd}", (DateTime)StartUp.zEndDate));
                cmd.Parameters.Add("@ma_dvcs", SqlDbType.VarChar).Value = StartUp.M_MA_DVCS.Trim();
                cmd.Parameters.Add("@listSttRec", SqlDbType.VarChar).Value = listSttRec;
                StartUp.SysObj.ExcuteNonQuery(cmd);

                ExMessageBox.Show(9393, StartupBase.SysObj, "Phiếu nhập mua hàng. Thành công!", "", MessageBoxButton.OK, MessageBoxImage.Asterisk);

            }
            #endregion

            #region Mua dich vu

            if (radioButton_pn1.IsChecked == true)
            {
                if (StartUp.zzkieu_loc == 2)
                {
                    MessageBox.Show("Kiểu lọc dữ liệu không đúng ");
                    return;
                }
                string listSttRec = "";
                int stt = 0;
                foreach (DataRow row in StartUp.tbMain.Rows)
                {
                    if (row["tag"].ToString().Trim().Equals("True") && string.IsNullOrEmpty(row["stt_rec"].ToString().Trim()))
                    {
                        string stt_rec_hd = row["stt_rec_hd"].ToString().Trim();
                        stt += 1;

                        listSttRec = listSttRec + stt_rec_hd + ",";

                    }
                }
                    
                SqlCommand cmd = new SqlCommand("exec [dbo].[QLHD_create_PN1] @Startdate, @EndDate, @ma_dvcs,@listSttRec");
                cmd.Parameters.Add("@Startdate", SqlDbType.VarChar).Value = (string.IsNullOrEmpty(StartUp.zStartdate.ToString()) ? "" : string.Format("{0:yyyyMMdd}", (DateTime)StartUp.zStartdate));
                cmd.Parameters.Add("@EndDate", SqlDbType.VarChar).Value = (string.IsNullOrEmpty(StartUp.zEndDate.ToString()) ? "" : string.Format("{0:yyyyMMdd}", (DateTime)StartUp.zEndDate));
                cmd.Parameters.Add("@ma_dvcs", SqlDbType.VarChar).Value = StartUp.M_MA_DVCS.Trim();
                cmd.Parameters.Add("@listSttRec", SqlDbType.VarChar).Value = listSttRec;
                StartUp.SysObj.ExcuteNonQuery(cmd);

                ExMessageBox.Show(9394, StartupBase.SysObj, "Tạo Hoá đơn mua dịch vụ. Thành công!", "", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            #endregion

            #region Phieu chi
            if (radioButton_pc1.IsChecked == true)
            {
                if (StartUp.zzkieu_loc == 2)
                {
                    MessageBox.Show("Kiểu lọc dữ liệu không đúng");
                    return;
                }
                string listSttRec = "";
                int stt = 0;
                foreach (DataRow row in StartUp.tbMain.Rows)
                {
                    if (row["tag"].ToString().Trim().Equals("True") && string.IsNullOrEmpty(row["stt_rec"].ToString().Trim()))
                    {
                        string stt_rec_hd = row["stt_rec_hd"].ToString().Trim();
                        stt += 1;

                        listSttRec = listSttRec + stt_rec_hd + ",";

                    }
                }

                SqlCommand cmd = new SqlCommand("exec [dbo].[QLHD_create_PC1] @Startdate, @EndDate, @ma_dvcs,@listSttRec");
                cmd.Parameters.Add("@Startdate", SqlDbType.VarChar).Value = (string.IsNullOrEmpty(StartUp.zStartdate.ToString()) ? "" : string.Format("{0:yyyyMMdd}", (DateTime)StartUp.zStartdate));
                cmd.Parameters.Add("@EndDate", SqlDbType.VarChar).Value = (string.IsNullOrEmpty(StartUp.zEndDate.ToString()) ? "" : string.Format("{0:yyyyMMdd}", (DateTime)StartUp.zEndDate));
                cmd.Parameters.Add("@ma_dvcs", SqlDbType.VarChar).Value = StartUp.M_MA_DVCS.Trim();
                cmd.Parameters.Add("@listSttRec", SqlDbType.VarChar).Value = listSttRec;
                StartUp.SysObj.ExcuteNonQuery(cmd);

                ExMessageBox.Show(9395, StartupBase.SysObj, "Tạo Phiếu chi. Thành công!", "", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            
        }
            #endregion

            #region Nhap hang ban tra lai
            if (radioButton_PNF.IsChecked == true)
            {
                if (StartUp.zzkieu_loc == 2)
                {
                    MessageBox.Show("Kiểu lọc dữ liệu không đúng");
                    return;
                }

                string listSttRec = "";
                int stt = 0;
                foreach (DataRow row in StartUp.tbMain.Rows)
                {
                    if (row["tag"].ToString().Trim().Equals("True") && string.IsNullOrEmpty(row["stt_rec"].ToString().Trim()))
                    {
                        string stt_rec_hd = row["stt_rec_hd"].ToString().Trim();
                        stt += 1;

                        listSttRec = listSttRec + stt_rec_hd + ",";

                    }
                }

                SqlCommand cmd = new SqlCommand("exec [dbo].[QLHD_create_PNF] @Startdate, @EndDate, @ma_dvcs,@listSttRec");
                cmd.Parameters.Add("@Startdate", SqlDbType.VarChar).Value = (string.IsNullOrEmpty(StartUp.zStartdate.ToString()) ? "" : string.Format("{0:yyyyMMdd}", (DateTime)StartUp.zStartdate));
                cmd.Parameters.Add("@EndDate", SqlDbType.VarChar).Value = (string.IsNullOrEmpty(StartUp.zEndDate.ToString()) ? "" : string.Format("{0:yyyyMMdd}", (DateTime)StartUp.zEndDate));
                cmd.Parameters.Add("@ma_dvcs", SqlDbType.VarChar).Value = StartUp.M_MA_DVCS.Trim();
                cmd.Parameters.Add("@listSttRec", SqlDbType.VarChar).Value = listSttRec;
                StartUp.SysObj.ExcuteNonQuery(cmd);

                ExMessageBox.Show(9399, StartupBase.SysObj, "Phiếu nhập trả lại. Thành công!", "", MessageBoxButton.OK, MessageBoxImage.Asterisk);

            }
            #endregion



            this.Close();

        }

        private void ConfirmGridView_OnCancel(object sender, RoutedEventArgs e)
        {
            isOk = false;
            this.Close();
        }

        private void Form_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                isOk = false;
                this.Close();
            }
        }


        private string GetFilter()
        {
            string filter = "1=1";

            return filter;
        }


        private void gotHDA(object sender, MouseEventArgs e)
        {
            if (StartUp.zzkieu_loc == 1)
            {
                MessageBox.Show("Kiểu lọc dữ liệu không đúng");
                radioButton_hda.IsChecked = false;
                return;
            }
        }

        private void gotHD1(object sender, MouseEventArgs e)
        {
            if (StartUp.zzkieu_loc == 1)
            {
                MessageBox.Show("Kiểu lọc dữ liệu không đúng");
                radioButton_hd1.IsChecked = false;
                return;
            }
        }

        private void gotPNA(object sender, MouseEventArgs e)
        {
            if (StartUp.zzkieu_loc == 2)
            {
                MessageBox.Show("Kiểu lọc dữ liệu không đúng");
                radioButton_pna.IsChecked = false;
                return;
            }
        }
        private void gotPN1(object sender, MouseEventArgs e)
        {
            if (StartUp.zzkieu_loc == 2)
            {
                MessageBox.Show("Kiểu lọc dữ liệu không đúng");
                radioButton_pn1.IsChecked = false;
                return;
            }
        }

        private void gotPC1(object sender, MouseEventArgs e)
        {
            if (StartUp.zzkieu_loc == 2)
            {
                MessageBox.Show("Kiểu lọc dữ liệu không đúng");
                radioButton_pc1.IsChecked = false;
                return;
            }
        }

        private void gotPNF(object sender, MouseEventArgs e)
        {
            if (StartUp.zzkieu_loc == 2)
            {
                MessageBox.Show("Kiểu lọc dữ liệu không đúng");
                radioButton_PNF.IsChecked = false;
                return;
            }
        }
    }
}
