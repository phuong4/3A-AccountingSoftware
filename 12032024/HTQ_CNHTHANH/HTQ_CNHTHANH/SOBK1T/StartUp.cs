using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using SmLib.SM.FormBrowse;
using Infragistics.Windows.DataPresenter;
using Sm.Windows.Controls;
using System.Diagnostics;
using System.Windows;

namespace HTQ_CNHTHANH
{
    class StartUp : StartupBase
    {
        static public DataSet DataSourceReport = new DataSet();
        static public DateTime M_ngay_ct0;
        static public string M_ma_nt0;
        static private FrmLoc _frmSearch;
        static public FormBrowse oBrowse;
        static public DataRow CommandInfo;
        public static string ReportID = "";
        //0 tất cả, 1 VND, 2 Ngoại tệ
        static private int KindStyleReport = -1;
        static private SqlCommand cmd;
        static public string parameter = "";
        static public string strLan = string.Empty;
        static public DataTable tbDetail = null;
        public static string Kieu_in = "2";
        public override void Run()
        {
            Namespace = "HTQ_CNHTHANH";
            try
            {
                CommandInfo = SmLib.SysFunc.GetCommandInfo(SysObj, Menu_Id);
                if (CommandInfo == null)
                {
                    Sm.Windows.Controls.ExMessageBox.Show(145, SysObj, "Chưa khai báo command hoặc command ngầm định sai!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    if (!Process.GetCurrentProcess().ProcessName.Equals("SmProcess"))
                        App.Current.Shutdown();
                    return;
                }
                parameter = CommandInfo["parameter"].ToString();
                ReportID = CommandInfo["rep_file"].ToString();
                M_ma_nt0 = StartUp.SysObj.GetOption("M_MA_NT0").ToString();
                M_ngay_ct0 = (DateTime)SysObj.GetSysVar("M_NGAY_KY1");
                strLan = SysObj.GetOption("M_LAN").ToString().ToUpper();
                _frmSearch = new FrmLoc();
                _frmSearch.Title = SmLib.SysFunc.Cat_Dau(M_LAN.Equals("V") ? CommandInfo["bar"].ToString() : CommandInfo["bar2"].ToString());
                _frmSearch.ShowDialog();
                if (!Process.GetCurrentProcess().ProcessName.Equals("SmProcess"))
                    App.Current.Shutdown();
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }
        public static void CallGridVouchers(bool isFirstLoad, object StartDate, object EndDate, string filter, bool KindReport)
        {
            try
            {
                //DataTable tbDetail;
                if (isFirstLoad)
                {
                    cmd = new SqlCommand();
                    cmd.CommandText = "Exec " + CommandInfo["store_proc"] + " @StartDate , @EndDate , @Condition";
                    cmd.Parameters.Add("@StartDate", SqlDbType.VarChar).Value = string.IsNullOrEmpty(StartDate.ToString()) ? "" : String.Format("{0:yyyyMMdd}", (DateTime)StartDate);
                    cmd.Parameters.Add("@EndDate", SqlDbType.VarChar).Value = string.IsNullOrEmpty(EndDate.ToString()) ? "" : String.Format("{0:yyyyMMdd}", (DateTime)EndDate);
                    cmd.Parameters.Add("@Condition", SqlDbType.NVarChar).Value = filter;
                    DataSet ds = SysObj.ExcuteReader(cmd);
                    //tbDetail = SysObj.ExcuteReader(cmd).Tables[0].Copy();
                    tbDetail = ds.Tables[0].Copy();

                    tbDetail.TableName = "tbDetail";
                    DataSourceReport.Tables.Add(tbDetail);

                    KindStyleReport = KindReport ? 1 : 2;

                    oBrowse = new FormBrowse(SysObj, tbDetail.DefaultView, GetTableShow(KindStyleReport));
                    oBrowse.F5 += new FormBrowse.GridKeyUp_F5(oBrowse_F5);
                    oBrowse.F11 += new FormBrowse.GridKeyUp_F11(oBrowse_F11);
                    //oBrowse.F7 += new FormBrowse.GridKeyUp_F7(oBrowse_F7);
                    oBrowse.F7 += new FormBrowse.GridKeyUp_F7(_browser_F7);
                    oBrowse.CTRL_R += new FormBrowse.GridKeyUp_CTRL_R(oBrowse_CTRL_R);
                    oBrowse.frmBrw.oBrowse.FieldSettings.AllowEdit = false;
                    oBrowse.frmBrw.Title = SmLib.SysFunc.Cat_Dau(M_LAN.Equals("V") ? CommandInfo["bar"].ToString() : CommandInfo["bar2"].ToString());

                    System.Windows.Controls.ToolBar ToolBar = (oBrowse.frmBrw.ToolBar.FindName("tbReport") as System.Windows.Controls.ToolBar);
                    ToolBarButton btnDetail = ToolBar.FindName("btnDetail") as ToolBarButton;
                    btnDetail.Text = "Cập nhập hoá đơn";
                    btnDetail.ImagePath = "Images\\PreView.png";

                    //btnOption
                  //  System.Windows.Controls.ToolBar ToolBar2 = (oBrowse.frmBrw.ToolBar.FindName("") as System.Windows.Controls.ToolBar);
                    ToolBarButton btnOption = ToolBar.FindName("btnOption") as ToolBarButton;
                    btnOption.Text = "Cập nhập thanh toán hoàn thành ";
                    btnOption.ImagePath = "Images\\PreView.png";
                    //ToolBarButton btnNew = ToolBar.FindName("btnNew") as ToolBarButton;
                    //btnNew.Text = "Hoàn thành";

                }
                else
                {
                    DataSourceReport.Tables.Remove("tbDetail");
                    tbDetail = SysObj.ExcuteReader(cmd).Tables[0].Copy();
                    tbDetail.TableName = "tbDetail";

                    oBrowse.frmBrw.oBrowse.DataSource = tbDetail.DefaultView;
                    DataSourceReport.Tables.Add(tbDetail);
                    oBrowse.frmBrw.oBrowse.FieldLayouts[0].SummaryDefinitions.Clear();
                    oBrowse.UpdateSumaryFields();
                }
                oBrowse.SetRowColorByTag("chon", "1", System.Windows.Media.Colors.Green, true);
                oBrowse.SetRowColorByTag("chon", "2", System.Windows.Media.Colors.Chocolate, true);

                if (isFirstLoad)
                {
                    oBrowse.frmBrw.LanguageID = "SOBK1T_1";
                    oBrowse.ShowDialog();
                    _frmSearch.Close();
                }
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }

        private static void _browser_F7(object sender, EventArgs e)
        {
            {
                if (oBrowse.ActiveRecord != null)
                {
                    if (oBrowse.DataGrid.ActiveCell != null && oBrowse.DataGrid.ActiveCell.IsInEditMode)
                        oBrowse.DataGrid.ActiveCell.EndEditMode();
                    (oBrowse.ActiveRecord as DataRecord).Update();
                }
                if (!oBrowse.DataGrid.Records.Any(x => (x as DataRecord).Cells["tag"].Value.ToString() == "True"))
                {
                    ExMessageBox.Show(2140, SysObj, "Phải đánh dấu trước khi in!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                Arso1t2In oReport = new Arso1t2In();
                oReport.Records = oBrowse.DataGrid.Records;

                //oReport.Info = dtInfo.Copy();
                oReport.ShowDialog();
            }
        }
        //static void oBrowse_F7(object sender, EventArgs e)
        //{
        //    if (oBrowse.ActiveRecord != null)
        //    {
        //        if (oBrowse.DataGrid.ActiveCell != null && oBrowse.DataGrid.ActiveCell.IsInEditMode)
        //            oBrowse.DataGrid.ActiveCell.EndEditMode();
        //        (oBrowse.ActiveRecord as DataRecord).Update();
        //    }
        //    if (!oBrowse.DataGrid.Records.Any(x => (x as DataRecord).Cells["tag"].Value.ToString() == "True"))
        //    {
        //        ExMessageBox.Show(2140, SysObj, "Phải đánh dấu trước khi in!", "", MessageBoxButton.OK, MessageBoxImage.Information);
        //        return;
        //    }

        //    Arso1t2In oReport = new Arso1t2In();
        //    oReport.Records = oBrowse.DataGrid.Records;

        //    //oReport.Info = dtInfo.Copy();
        //    oReport.ShowDialog();
        //    //SmReport.ReportManager oReport = new SmReport.ReportManager(SysObj, CommandInfo["rep_file"].ToString(), KindStyleReport);
        //    //SmLib.SysFunc.DSCopyWithFilter(oBrowse.frmBrw.oBrowse, ref DataSourceReport, "tbDetail");

        //    //DataSourceReport.AcceptChanges();
        //    //DataSourceReport.Tables["tbDetail"].DefaultView.RowFilter = "tag=True";
        //    //var tnFilted = new DataView(DataSourceReport.Tables["tbDetail"], "tag=True", "", DataViewRowState.CurrentRows).ToTable();
        //    //if (tnFilted.Rows.Count > 0)
        //    //{
        //    //    DataSourceReport.Tables["tbDetail"].Clear();
        //    //    DataSourceReport.Tables["tbDetail"].Merge(tnFilted);
        //    //    oReport.Preview(DataSourceReport);
        //    //    SmLib.SysFunc.ResetFilter(ref DataSourceReport, "tbDetail");
        //    //}
        //    //else
        //    //{

        //    //}

        //}
        static void oBrowse_F7(object sender, EventArgs e)
        {

            if (oBrowse.ActiveRecord != null)
            {
                if (oBrowse.DataGrid.ActiveCell != null && oBrowse.DataGrid.ActiveCell.IsInEditMode)
                    oBrowse.DataGrid.ActiveCell.EndEditMode();
                (oBrowse.ActiveRecord as DataRecord).Update();
            }
            if (!oBrowse.DataGrid.Records.Any(x => (x as DataRecord).Cells["tag"].Value.ToString() == "True"))
            {
                ExMessageBox.Show(2140, SysObj, "Phải đánh dấu trước khi in!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }


            SmReport.ReportManager oReport = new SmReport.ReportManager(SysObj, CommandInfo["rep_file"].ToString(), KindStyleReport);
            SmLib.SysFunc.DSCopyWithFilter(oBrowse.frmBrw.oBrowse, ref DataSourceReport, "tbDetail");

            DataSourceReport.AcceptChanges();
            DataSourceReport.Tables["tbDetail"].DefaultView.RowFilter = "tag=True";
            var tnFilted = new DataView(DataSourceReport.Tables["tbDetail"], "tag=True", "", DataViewRowState.CurrentRows).ToTable();
            if (tnFilted.Rows.Count > 0)
            {
                DataSourceReport.Tables["tbDetail"].Clear();
                DataSourceReport.Tables["tbDetail"].Merge(tnFilted);
                oReport.Preview(DataSourceReport);
                SmLib.SysFunc.ResetFilter(ref DataSourceReport, "tbDetail");
            }
            else
            {

            }
        }


        static void oBrowse_CTRL_R(object sender, EventArgs e)
        {
            string filter = _frmSearch.GetFilter();
            bool loaibc = true;
            if (_frmSearch.cbMauBaoCao2.Value.ToString().Equals("1"))
                loaibc = false;
            StartUp.CallGridVouchers(false, _frmSearch.TxtStartDateTime.Value, _frmSearch.TxtEndDateTime.Value, filter, loaibc);
        }

        static void oBrowse_F11(object sender, EventArgs e)
        {

            int stt = 0;
            foreach (DataRow row in tbDetail.Rows)
            {
                if (row["tag"].ToString().Trim().Equals("True"))
                {
                    string stt_rec = row["stt_rec"].ToString().Trim();
                    string stt_rec0 = row["stt_rec0"].ToString().Trim();
                     stt += 1;

                    SqlCommand cmd = new SqlCommand("UPDATE ct71 SET hthanh = 1 WHERE stt_rec = @stt_rec and stt_rec0 = @stt_rec0");
                    cmd.Parameters.Add("@Stt_rec", SqlDbType.VarChar).Value = stt_rec;
                    cmd.Parameters.Add("@Stt_rec0", SqlDbType.VarChar).Value = stt_rec0;
                    StartUp.SysObj.ExcuteNonQuery(cmd);
                }
            }
                    MessageBox.Show("Cập nhập hoàn thành. Thành công!", "Thong bao", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        static void oBrowse_F5(object sender, EventArgs e)
        {
            int dem = 0;
            string zstt_rec = "";
            if (oBrowse.frmBrw.oBrowse.ActiveCell != null)
                oBrowse.frmBrw.oBrowse.ActiveCell.EndEditMode();
            var tbSelected = tbDetail.Clone();

            foreach (DataRow row in tbDetail.Rows)
            {
                if (row["tag"].ToString().Trim().Equals("True"))
                {
                    dem += 1;
                    tbSelected.ImportRow(row);
                }
                if (dem == 1)
                {
                    zstt_rec = row["stt_rec"].ToString().Trim();
                }
            }


            if (dem == 0)
            {
                ExMessageBox.Show(9488, StartUp.SysObj, "Chưa chọn phiếu!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }



            DataTable dt = new DataTable();
            dt.Columns.Add("stt_rec", typeof(string));
            dt.Columns.Add("stt_rec0", typeof(string));
            dt.Columns.Add("so_hd", typeof(string));
            dt.Columns.Add("ngay_hd", typeof(DateTime));

            DataRow dr = dt.NewRow();

            dt.Rows.Add(dr);

            FrmTaoHDA win = new FrmTaoHDA();
            win.DisplayLanguage = M_LAN;
            win.txtso_hd.Text = tbSelected.Rows[0]["so_hd"].ToString();

            //win.txtNgay_ct.dValue = tbSelected.Rows[0]["ngay_ct"].;
            //   win.txtma_ca.Text = "";
            SmLib.SysFunc.LoadIcon(win);
            win.Title = SmLib.SysFunc.Cat_Dau(win.Title);

            //win.tbInfoPX = dt;
            //win.DataContext = dt.DefaultView;

            try
            {
                win.ShowDialog();

                if (win.isOk)
                {

                    dt.Rows[0]["so_hd"] = win.txtso_hd.Text.Trim();
                    dt.Rows[0]["ngay_hd"] = win.txtNgay_hd.dValue;


                    int stt = 0;
                    foreach (DataRow row in tbDetail.Rows)
                    {
                        if (row["tag"].ToString().Trim().Equals("True"))
                        {
                            string stt_rec = row["stt_rec"].ToString().Trim();
                            string stt_rec0 = row["stt_rec0"].ToString().Trim();
                            stt += 1;

                            SqlCommand cmd = new SqlCommand("UPDATE ct71 SET so_hd = @so_hd,ngay_hd = @ngay_hd WHERE stt_rec = @stt_rec and stt_rec0 = @stt_rec0");
                            cmd.Parameters.Add("@Stt_rec", SqlDbType.VarChar).Value = stt_rec;
                            cmd.Parameters.Add("@Stt_rec0", SqlDbType.VarChar).Value = stt_rec0;
                            cmd.Parameters.Add("@ngay_hd", dt.Rows[0]["ngay_hd"]);
                            cmd.Parameters.Add("@so_hd", dt.Rows[0]["so_hd"].ToString().Trim());
                            StartUp.SysObj.ExcuteNonQuery(cmd);

                        }
                    }

                  
                    MessageBox.Show("Chương trình đã thực hiện xong!", "Thong bao");

                }
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }

        }



        public static string GetTableShow(int KindReport)
        {
            string strFieldShow = string.Empty;
            switch (strLan)
            {
                case ("V"):
                    {
                        switch (KindReport)
                        {
                            case 1:
                                {
                                    strFieldShow = CommandInfo["Vbrowse1"].ToString();
                                    break;
                                }
                            case 2:
                                {
                                    strFieldShow = CommandInfo["Vbrowse2"].ToString();
                                    break;
                                }
                        }
                        break;
                    }
                default:
                    {
                        switch (KindReport)
                        {
                            case 1:
                                {
                                    strFieldShow = CommandInfo["Ebrowse1"].ToString();
                                    break;
                                }
                            case 2:
                                {
                                    strFieldShow = CommandInfo["Ebrowse2"].ToString();
                                    break;
                                }
                        }
                        break;
                    }
            }
            return strFieldShow;
        }
    }
}
