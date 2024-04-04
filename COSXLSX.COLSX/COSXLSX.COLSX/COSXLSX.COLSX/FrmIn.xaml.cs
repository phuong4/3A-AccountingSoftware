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
using Infragistics.Windows.DataPresenter;
using System.Windows.Interop;
using System.Data.SqlClient;

namespace COSXLSX.COLSX
{
    /// <summary>
    /// Interaction logic for FrmIn.xaml
    /// </summary>
    public partial class FrmIn : Form
    {
        DataSet dsSource = new DataSet();

        public FrmIn(string so_lsx)
        {
            InitializeComponent();
            SmLib.SysFunc.LoadIcon(this);
            this.ShowInTaskbar = false;
            GridSearch.LocalSysObj = StartUp.SysObj;
            GridSearch.ReportGroupName =StartUp.CommandInfo["rep_file"].ToString();

           //  = StartUp.DataSourceReport.Copy();

            dsSource  = Get_in(so_lsx);
            dsSource.Tables[0].TableName = "tblPh";
            dsSource.Tables[1].TableName = "tblCt";
            //dsSource.Tables[2].TableName = "tblQcin";
            //dsSource.Tables[3].TableName = "tblQcgc";


            StartUp.GetDmnt(dsSource);
            DataColumn newcolumn = new DataColumn("so_lien", typeof(int));
            newcolumn.DefaultValue = 1;
            dsSource.Tables[0].Columns.Add(newcolumn);

            newcolumn = new DataColumn("so_ct_goc", typeof(int));
            newcolumn.DefaultValue = 0;
            dsSource.Tables[0].Columns.Add(newcolumn);

            newcolumn = new DataColumn("ma_nx", typeof(string));
            newcolumn.DefaultValue = "";
            dsSource.Tables[1].Columns.Add(newcolumn);

            newcolumn = new DataColumn("stt", typeof(int));
            dsSource.Tables[1].Columns.Add(newcolumn);

            //string stt_recin = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString();


            //DataTable tbCtadd = StartUp.GetCtadd(stt_recin);

            //HOANGTQ
          

            //// them mot dong luu tien chiet khau
            //DataRow dr_new = dsSource.Tables[1].NewRow();
            //dr_new["stt_rec"] = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString();
            //dsSource.Tables[1].Rows.Add(dr_new);

            //dsSource.Tables[0].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString() + "'";
            //dsSource.Tables[1].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString() + "'";
          
            //gop data
           // dsSource.Merge(drin);


            GridSearch.DSource = dsSource;
            if (BindingSysObj.GetOption("M_LAN").ToString().Equals("V"))
            {
                btnExport.Content = BindingSysObj.GetSysVar("M_EXPORT_SIGN").ToString();
            }
            else
            {
                btnExport.Content = BindingSysObj.GetSysVar2("M_EXPORT_SIGN").ToString();
            }

        }

        private void Form_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Title = SmLib.SysFunc.Cat_Dau(this.Title);
                //DataTable tbIn = StartUp.GetPhIn();
                //if (tbIn.Rows.Count == 0)
                //{
                //    DataRow dr = tbIn.NewRow();
                //    dr["ma_ct"] = StartUp.Ma_ct;
                //    dr["stt_rec"] = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString().Trim();
                //    dr["so01"] = 0;
                //    dr["so02"] = StartUp.DmctInfo["so_lien"] == DBNull.Value ? 1 : StartUp.DmctInfo["so_lien"];
                //    tbIn.Rows.Add(dr);
                //}
                //this.DataContext = tbIn;
                txtctu0.Focus();
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        private void BtnIn_Click(object sender, RoutedEventArgs e)
        {
            if (txtlien.Value != null)
            {
                dsSource.Tables[0].DefaultView[0]["so_ct_goc"] = txtctu0.Text;
                int so_lien = 1;
                int so_lan_in = Convert.ToInt16(Math.Ceiling(Convert.ToDouble(txtlien.Text) / Convert.ToDouble((GridSearch.XGReport.ActiveRecord as DataRecord).Cells["so_lien"].Value)));
                dsSource.Tables[0].DefaultView[0]["so_lien"] = so_lan_in;
                while (so_lien <= so_lan_in)
                {
                    //Them so dong trang theo tham so so_dong_in
                    GridSearch.InsertSubRow(StartUp.Ma_ct, 1);

                    int stt = 1;
                    foreach (DataRowView dr in dsSource.Tables[1].DefaultView)
                    {
                        dr["stt"] = stt;
                        stt++;
                    }

                    GridSearch.V_In(1);
                    so_lien++;
                }
               // StartUp.SetPhIn(this.DataContext as DataTable);
            }
            this.Close();
        }

        private void BtnInLT_Click(object sender, RoutedEventArgs e)
        {
            return;
            if (txtlien.Value != null)
            {
                if (StartUp.M_IN_HOI_CK == 1)
                {
                    if (ExMessageBox.Show( 2210,StartUp.SysObj, "Có chắc chắn in tất cả các chứng từ đã được lọc ?", "Fast Book 11 .NET", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                    {
                        return;
                    }
                }

                //int iRowTmp = FrmPoctpna.iRow;
                int so_lien = 1;
                dsSource.Tables[0].DefaultView[0]["so_ct_goc"] = txtctu0.Text; int so_lan_in = Convert.ToInt16(Math.Ceiling(Convert.ToDouble(txtlien.Text) / Convert.ToDouble((GridSearch.XGReport.ActiveRecord as DataRecord).Cells["so_lien"].Value)));
                dsSource.Tables[0].DefaultView[0]["so_lien"] = so_lien;
                while (so_lien <= so_lan_in)
                {
                    for (int i = 1; i < StartUp.DsTrans.Tables[0].Rows.Count; i++)
                    {
                        dsSource.Tables[0].DefaultView.RowFilter = "stt_rec= '" + dsSource.Tables[0].Rows[i]["stt_rec"].ToString() + "'";
                        dsSource.Tables[1].DefaultView.RowFilter = "stt_rec= '" + dsSource.Tables[0].Rows[i]["stt_rec"].ToString() + "'";
                        //Them so dong trang theo tham so so_dong_in
                        GridSearch.InsertSubRow(StartUp.Ma_ct, 1);

                        int stt = 1;
                        foreach (DataRowView dr in dsSource.Tables[1].DefaultView)
                        {
                            dr["stt"] = stt;
                            stt++;
                        }

                        GridSearch.V_In(1);
                    }
                    so_lien++;
                }
                StartUp.SetPhIn(this.DataContext as DataTable);

                //StartUp.DsTrans.Tables[0].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[iRowTmp]["stt_rec"].ToString() + "'";
                //StartUp.DsTrans.Tables[1].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].Rows[iRowTmp]["stt_rec"].ToString() + "'";
            }
            this.Close();
        }

        private void BtnXem_Click(object sender, RoutedEventArgs e)
        {
            dsSource.Tables[0].DefaultView[0]["so_ct_goc"] = txtctu0.Text;
            //Them so dong trang theo tham so so_dong_in
          //  GridSearch.InsertSubRow(StartUp.Ma_ct, 1);
            GridSearch.V_Xem();
          //  StartUp.SetPhIn(this.DataContext as DataTable);
        }

        private void BtnThoat_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void txtctu0_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!txtctu0.IsFocusWithin)
            {
                if (txtctu0.Value.ToString() == "")
                    txtctu0.Value = 0;
                dsSource.Tables[0].DefaultView[0]["so_ct_goc"] = txtctu0.Value;
            }
        }

        private void GridSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            //Them so dong trang theo tham so so_dong_in
          //  GridSearch.InsertSubRow(StartUp.Ma_ct, 1);

        }

        private void txtlien_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!txtlien.IsFocusWithin)
            {
                if (txtlien.Text.ToString() == "")
                    txtlien.Value = 0;
            }
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            if (dsSource.Tables[0].DefaultView.Count == 1)
            {
                string file_name = StartUp.GetFileNameExportWithSignature(dsSource.Tables[0].DefaultView[0]);
                IntPtr windowHandle = new WindowInteropHelper(this).Handle;
                GridSearch.V_XuatPdf(file_name, windowHandle);
            }
        }

        public static DataSet Get_in(string stt_rec)
        {
            SqlCommand sqlCommand = new SqlCommand("EXEC dbo.AA_Getctin @stt_rec,@ma_dvcs");
            sqlCommand.Parameters.Add("@stt_rec", SqlDbType.Char).Value = stt_rec;
            sqlCommand.Parameters.Add("@ma_dvcs", SqlDbType.Char, 8).Value = StartUp.SysObj.M_ma_dvcs;

            DataSet dataSet = StartupBase.SysObj.ExcuteReader(sqlCommand);
            //if (dataSet.Tables[0].Rows.Count != 1)
            //{
            //    return null;
            //}
            return dataSet;
        }
    }
}
