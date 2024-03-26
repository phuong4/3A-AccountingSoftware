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
using Infragistics.Windows.Editors;
using System.Data;
using Infragistics.Windows.DataPresenter;
using System.Windows.Interop;

namespace APCTPN1
{
    /// <summary>
    /// Interaction logic for FrmIn.xaml
    /// </summary>
    public partial class FrmIn : Form
    {
        private DataSet dsSource = new DataSet();
        public FrmIn()
        {
            InitializeComponent();
            SmLib.SysFunc.LoadIcon(this);
            GridSearch.LocalSysObj = StartUp.SysObj;
            GridSearch.ReportGroupName = StartUp.CommandInfo["rep_file"].ToString();

            dsSource = StartUp.DsTrans.Copy();
            StartUp.GetDmnt(dsSource);
            dsSource.Tables[0].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString() + "'";
            dsSource.Tables[1].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString() + "'";
            dsSource.Tables[1].DefaultView.Sort = "stt_rec0";
            dsSource.Tables[2].DefaultView.RowFilter = "stt_rec= '" + StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString() + "'";
            GridSearch.DSource = dsSource;
            GridSearch.ReportPreviewMouseDoubleClick += new SmReport.ControlFilterReport.MouseClick(GridSearch_ReportPreviewMouseDoubleClick);
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
            this.Title = SmLib.SysFunc.Cat_Dau(this.Title);
            DataTable tbIn = StartUp.GetPhIn();
            if (tbIn.Rows.Count == 0)
            {
                DataRow dr = tbIn.NewRow();
                dr["ma_ct"] = StartUp.Ma_ct;
                dr["stt_rec"] = dsSource.Tables[0].DefaultView[0]["stt_rec"].ToString().Trim();
                dr["so01"] = 0;
                dr["so02"] = StartUp.DmctInfo["so_lien"] == DBNull.Value ? 1 : StartUp.DmctInfo["so_lien"];
                tbIn.Rows.Add(dr);
            }
            this.DataContext = tbIn;
            txtctu0.Focus();
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
                int so_lien = 1;
                int so_lan_in = Convert.ToInt16(Math.Ceiling(Convert.ToDouble(txtlien.Text) / Convert.ToDouble((GridSearch.XGReport.ActiveRecord as DataRecord).Cells["so_lien"].Value)));
                while (so_lien <= so_lan_in)
                {
                    GridSearch.InsertSubRow(StartUp.Ma_ct, 1);
                    GridSearch.V_In(1);
                    so_lien++;
                }
                StartUp.SetPhIn(this.DataContext as DataTable);
            }
        	this.Close();
		}

        private void BtnInLT_Click(object sender, RoutedEventArgs e)
        {
            if (StartUp.M_IN_HOI_CK == 1)
            {
                if (ExMessageBox.Show( 220,StartUp.SysObj, "Có chắc chắn in tất cả các chứng từ đã được lọc ?", "Xac nhan nhap lieu", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                {
                    return;
                }
            }
            if (txtlien.Value != null)
            {
                int iRowTmp = FrmAPCTPN1.iRow;
                int so_lien = 1;
                int so_lan_in = Convert.ToInt16(Math.Ceiling(Convert.ToDouble(txtlien.Text) / Convert.ToDouble((GridSearch.XGReport.ActiveRecord as DataRecord).Cells["so_lien"].Value)));
                while (so_lien <= so_lan_in)
                {
                    for (int i = 1; i < dsSource.Tables[0].Rows.Count; i++)
                    {
                        dsSource.Tables[0].DefaultView.RowFilter = "stt_rec= '" + dsSource.Tables[0].Rows[i]["stt_rec"].ToString() + "'";
                        dsSource.Tables[1].DefaultView.RowFilter = "stt_rec= '" + dsSource.Tables[0].Rows[i]["stt_rec"].ToString() + "'";
                        dsSource.Tables[1].DefaultView.Sort = "stt_rec0";
                        dsSource.Tables[2].DefaultView.RowFilter = "stt_rec= '" + dsSource.Tables[0].Rows[i]["stt_rec"].ToString() + "'";
                        GridSearch.InsertSubRow(StartUp.Ma_ct, 1);
                        GridSearch.V_In(1);
                    }
                    so_lien++;
                }
                StartUp.SetPhIn(this.DataContext as DataTable);

                dsSource.Tables[0].DefaultView.RowFilter = "stt_rec= '" + dsSource.Tables[0].Rows[iRowTmp]["stt_rec"].ToString() + "'";
                dsSource.Tables[1].DefaultView.RowFilter = "stt_rec= '" + dsSource.Tables[0].Rows[iRowTmp]["stt_rec"].ToString() + "'";
                dsSource.Tables[1].DefaultView.Sort = "stt_rec0";
                dsSource.Tables[2].DefaultView.RowFilter = "stt_rec= '" + dsSource.Tables[0].Rows[iRowTmp]["stt_rec"].ToString() + "'";
            }
            
        	this.Close();
		}

        void GridSearch_ReportPreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            BtnXem_Click(sender, e);
        }

        private void BtnXem_Click(object sender, RoutedEventArgs e)
        {
            GridSearch.InsertSubRow(StartUp.Ma_ct, 1);
            GridSearch.V_Xem();
            StartUp.SetPhIn(this.DataContext as DataTable);
        }

        private void BtnThoat_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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
    }
}
