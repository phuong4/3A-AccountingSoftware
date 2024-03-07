using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SmLib.SM.FormBrowse;
using System.Data.SqlClient;
using System.Windows;
using SmLib.SM.FormBrowse2;
using Infragistics.Windows.DataPresenter;
using Microsoft.Win32;
using System.Windows.Controls;
using Sm.Windows.Controls;
using SmDataLib;
using System.Windows.Input;
using Infragistics.Windows.DataPresenter.Events;

namespace AAA_QLHD3A
{
    class StartUp : Sm.Windows.Controls.StartupBase
    {

        static public DataSet DataSourceReport = new DataSet();

        static public DataTable dtInfo;
        static private FormBrowse2 oBrowse;

        static private DataRow CommandInfo;

        static public string sqlTableView = "v_AAA_QLHD3A";
        static public string SqlTableKey = "stt_rec";//
        static public string SqlTableObjectName = "ten_kh";
        static public string pListVoucher = "HDA;HDX";

        static public DateTime M_ngay_ct0;
        static public string M_ma_nt0;
        static public string M_MA_DVCS;

        public static DateTime zStartdate;
        public static DateTime zEndDate;

        public static int zzkieu_loc ;

        static SqlCommand cmd1 = new SqlCommand();
        static SqlCommand cmd = new SqlCommand();
        static FormLoc _frmLoc;
        static public DataTable tbMain = null;
        static public string strLan = string.Empty;
        public static DataTable dtNhom;

        public static String zfilter = "";
        public static int zkieu_loc = 0;
        static public string TableName = "ct70";



        public override void Run()
        {
            Namespace = "AAA_QLHD3A";
            try
            {
                DateTime t1 = DateTime.Now;
                M_ma_nt0 = StartUp.SysObj.GetOption("M_MA_NT0").ToString();
                CommandInfo = SmLib.SysFunc.GetCommandInfo(SysObj, Menu_Id);
                M_ngay_ct0 = (DateTime)SysObj.GetSysVar("M_NGAY_KY1");
                strLan = SysObj.GetOption("M_LAN").ToString().ToUpper();

                dtInfo = new DataTable();
                dtInfo.TableName = "TbInfo";
                dtInfo.Columns.Add("StartDate", typeof(DateTime));
                dtInfo.Columns.Add("EndDate", typeof(DateTime));
                dtInfo.Rows.Add(DateTime.Now.Date, DateTime.Now.Date);
                if (CommandInfo != null)
                {
                    //_frmSearch = new TransactionListFrm();
                    //_frmSearch.ShowDialog();
                    _frmLoc = new FormLoc();
                    _frmLoc.Title = SmLib.SysFunc.Cat_Dau(M_LAN.Equals("V") ? CommandInfo["bar"].ToString() : CommandInfo["bar2"].ToString());
                    DateTime t2 = DateTime.Now;
                    // MessageBox.Show((t2 - t1).TotalMilliseconds.ToString());
                    _frmLoc.ShowDialog();

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public static void CallGridVouchers(object StartDate, object EndDate, string filter, int KieuLoc)

        {
            try
            {
                StartUp.zzkieu_loc = KieuLoc + 1;
                string strBrowse = "";
                string strBrowse2 = "";
                string strBrowse_nt = "";
                string strBrowse2_nt = "";
                string[] listStored = CommandInfo["store_proc"].ToString().Split('|');
                StartUp.zStartdate = (DateTime)StartDate;
                StartUp.zEndDate = (DateTime)EndDate;
                cmd = new SqlCommand();
                cmd.CommandText = "Exec " + listStored[0] + " @StartDate , @EndDate , @Condition, @pListVoucher, @KindFilter";
                cmd.Parameters.Add("@StartDate", SqlDbType.VarChar).Value = string.IsNullOrEmpty(StartDate.ToString()) ? "" : String.Format("{0:yyyyMMdd}", (DateTime)StartDate);
                cmd.Parameters.Add("@EndDate", SqlDbType.VarChar).Value = string.IsNullOrEmpty(EndDate.ToString()) ? "" : String.Format("{0:yyyyMMdd}", (DateTime)EndDate);
                cmd.Parameters.Add("@Condition", SqlDbType.NVarChar).Value = filter;
                cmd.Parameters.Add("@pListVoucher", SqlDbType.NVarChar).Value = pListVoucher;
                cmd.Parameters.Add("@KindFilter", SqlDbType.Int).Value = KieuLoc;
                DataSet ds = SysObj.ExcuteReader(cmd);
                DataTable tbDetail = ds.Tables[1].Copy();
                tbMain = ds.Tables[0].Copy();
                //tbMain.DefaultView.Sort = "ngay_ct,ma_ct_in,so_ct";
                tbMain.TableName = "tbMain";

                DataTable reportHeaderTable = tbMain.Copy();
                DataTable reportDetailTable = tbDetail.Copy();
                reportHeaderTable.TableName = "tbPh";
                reportHeaderTable.Columns.Add("KHInfo", typeof(String), "TRIM(ma_kh)+' - '+ TRIM(ten_kh)");
                reportDetailTable.TableName = "tbCt";
                // Thêm cột Vật tư Info (khỏi phải xài CalculatedField)
                reportDetailTable.Columns.Add("VTInfo", typeof(String), "TRIM(ma_vt)+' - '+ TRIM(ten_vt)");
                DataSourceReport = new DataSet();
                DataSourceReport.Tables.Add(reportHeaderTable);
                DataSourceReport.Tables.Add(reportDetailTable);
                DataSourceReport.Tables.Add(dtInfo.Copy());
                // tạo relation cho 2 table Header va Detail trong dataset DataSourceReport
                DataColumn ParentCol = reportHeaderTable.Columns["stt_rec_hd"];
                DataColumn ChildrenCol = reportDetailTable.Columns["stt_rec_hd"];

                //String RelationName ="Stt_rec_Relation";
                //DataRelation dRelation = new DataRelation(RelationName, ParentCol, ChildrenCol,false);
                //DataSourceReport.Relations.Add(dRelation);
                String[] arrStrBrowse1, arrStrBrowse2;
                if (strLan.Equals("V"))
                {
                    arrStrBrowse1 = CommandInfo["VBrowse1"].ToString().Trim().Split('|');
                    arrStrBrowse2 = CommandInfo["VBrowse2"].ToString().Trim().Split('|');
                }
                else
                {
                    arrStrBrowse1 = CommandInfo["EBrowse1"].ToString().Trim().Split('|');
                    arrStrBrowse2 = CommandInfo["EBrowse2"].ToString().Trim().Split('|');
                }


                strBrowse = arrStrBrowse1[2];
                strBrowse2 = arrStrBrowse1[3];
                strBrowse_nt = arrStrBrowse2[2];
                strBrowse2_nt = arrStrBrowse2[3];

                oBrowse = new FormBrowse2(SysObj, tbMain.DefaultView, tbDetail.DefaultView, strBrowse, strBrowse2, "stt_rec_hd");



                oBrowse.F7 += new FormBrowse2.GridKeyUp_F7(oBrowse_F7);
                oBrowse.F5 += new FormBrowse2.GridKeyUp_F5(oBrowse_F5);
                oBrowse.CTRL_R += new FormBrowse2.GridKeyUp_CTRL_R(oBrowse_CTRL_R);
                oBrowse.frmBrw.PreviewKeyUp += new KeyEventHandler(zfrmBrw_PreviewKeyUp);
                oBrowse.DataGrid.FieldLayoutInitialized += new EventHandler<FieldLayoutInitializedEventArgs>(zDataGrid_FieldLayoutInitialized);

                oBrowse.frmBrw.oBrowse.FieldSettings.AllowEdit = false;
                oBrowse.frmBrw.Title = SmLib.SysFunc.Cat_Dau(M_LAN.Equals("V") ? CommandInfo["bar"].ToString() : CommandInfo["bar2"].ToString());
                //oBrowse.Esc += new FormBrowse2.GridKeyUp_Esc(oBrowse_Esc);

                ToolBar toolBar = StartUp.oBrowse.frmBrw.ToolBar.FindName("tbReport") as ToolBar;

                if (toolBar != null)
                {
                    //(toolBar.FindName("btnEdit") as Sm.Windows.Controls.ToolBarButton).Text = "Tạo phiếu nhập";
                    (toolBar.FindName("btnDetail") as Sm.Windows.Controls.ToolBarButton).Text = "Tạo phiếu";
                    (toolBar.FindName("btnDetail") as Sm.Windows.Controls.ToolBarButton).ImagePath = "Images\\Edit.png";
                }
                if (toolBar != null)
                {
                    //(toolBar.FindName("btnEdit") as Sm.Windows.Controls.ToolBarButton).Text = "Tạo phiếu nhập";
                    (toolBar.FindName("btnPrint") as Sm.Windows.Controls.ToolBarButton).Text = "Xoá phiếu";
                    (toolBar.FindName("btnPrint") as Sm.Windows.Controls.ToolBarButton).ImagePath = "Images\\Delete.png";
                }


                oBrowse.frmBrw.LanguageID = "AAA_QLHD3A_1";
                oBrowse.ShowDialog();
                App.Current.Shutdown();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        #region QueryData
        public static void QueryData(bool isFirstLoad, object StartDate, object EndDate, string filter, int KieuLoc)
        {
            try
            {
                if (isFirstLoad)
                {
                    CallGridVouchers(StartDate, EndDate, filter, KieuLoc);
                }
                else
                {

                    DataTable tbInfo = DataSourceReport.Tables["tbInfo"].Copy();
                    DataSourceReport = new DataSet();
                    DataSet ds = SysObj.ExcuteReader(cmd);
                    DataTable tbDetail = ds.Tables[1].Copy();
                    tbMain = ds.Tables[0].Copy();
                    tbDetail.TableName = "tbDetail";
                    tbMain.TableName = "tbMain";
                    DataTable reportHeaderTable = tbMain.Copy();
                    DataTable reportDetailTable = tbDetail.Copy();
                    reportHeaderTable.TableName = "tbPh";
                    reportHeaderTable.Columns.Add("KHInfo", typeof(String), "TRIM(ma_kh)+' - '+ TRIM(ten_kh)");
                    reportDetailTable.TableName = "tbCt";
                    // Thêm cột Vật tư Info (khỏi phải xài CalculatedField)
                    reportDetailTable.Columns.Add("VTInfo", typeof(String), "TRIM(ma_vt)+' - '+ TRIM(ten_vt)");
                    DataSourceReport = new DataSet();
                    DataSourceReport.Tables.Add(reportHeaderTable);
                    DataSourceReport.Tables.Add(reportDetailTable);
                    DataSourceReport.Tables.Add(tbInfo.Copy());
                    // tạo relation cho 2 table Header va Detail trong dataset DataSourceReport
                    DataColumn ParentCol = reportHeaderTable.Columns["stt_rec_hd"];
                    DataColumn ChildrenCol = reportDetailTable.Columns["stt_rec_hd"];

                    String RelationName = "stt_rec_hd";
                    DataRelation dRelation = new DataRelation(RelationName, ParentCol, ChildrenCol, false);
                    DataSourceReport.Relations.Add(dRelation);

                    //oBrowse.frmBrw.oBrowse.DataSource = null;

                    oBrowse.frmBrw.oBrowse.DataSource = tbMain.DefaultView;
                    oBrowse.frmBrw.oBrowseCt.DataSource = tbDetail.DefaultView;
                    oBrowse.ObrowseViewCt = tbDetail.DefaultView;
                    DataSourceReport.Tables.Add(tbDetail);
                    oBrowse.frmBrw.oBrowse.FieldLayouts[0].SummaryDefinitions.Clear();
                    //if (KieuLoc == 1)
                    //{
                    //    //oBrowse.ObrowseViewCt.RowFilter = " ma_vt ='" + MaVT + "'";
                    //    if (!String.IsNullOrEmpty(MaVT))
                    //        oBrowse.constCtFilter = " ma_vt ='" + MaVT + "'";
                    //}
                    oBrowse.UpdateSumaryFields();
                }
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }
        static void oBrowse_CTRL_R(object sender, EventArgs e)
        {
            string filter = _frmLoc.GetFilter();
            StartUp.QueryData(false, (object)_frmLoc.TxtStartDateTime.Value, (object)_frmLoc.TxtEndDateTime.Value, filter, Convert.ToInt32(_frmLoc.cbKieuLoc.Value.ToString()));
        }
        #endregion


        static void zDataGrid_FieldLayoutInitialized(object sender, FieldLayoutInitializedEventArgs e)
        {
            try
            {
                XamDataGrid grid = sender as XamDataGrid;

                grid.FieldLayouts[0].Fields["tag"].Settings.AllowEdit = true;
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }

        static void zfrmBrw_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.None)
            {
                switch (e.Key)
                {

                    case Key.Space:
                        SelectEntry();
                        break;
                }
            }
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                switch (e.Key)
                {
                    case Key.A:
                        SelectAll(true);
                        break;
                    case Key.U:
                        SelectAll(false);
                        break;
                }
            }
        }


        static private void SelectEntry()
        {
            DataRecord rec = oBrowse.DataGrid.ActiveRecord as DataRecord;
            if (rec == null || rec.RecordType != RecordType.DataRecord)
                return;
            if (oBrowse.DataGrid.ActiveCell != null)
                oBrowse.DataGrid.ActiveCell = null;

            Cell cell = rec.Cells["tag"];
            if (cell.Value == null || cell.Value is DBNull)
                cell.Value = false;
            else
                cell.Value = !((bool)cell.Value);
        }

        static private void SelectAll(bool tag)
        {
            if (oBrowse.DataGrid.ActiveCell != null)
                oBrowse.DataGrid.ActiveCell = null;

         
            foreach (DataRecord rec in oBrowse.DataGrid.RecordManager.GetFilteredInDataRecords())
            {
                rec.Cells["tag"].Value = tag;
            }


        }


        //static void oBrowse_Esc(object sender, EventArgs e)
        //{
        //    oBrowse.frmBrw.Close();
        //    App.Current.Shutdown();
        //}



        static void oBrowse_F7(object sender, EventArgs e)
        {
            //SmReport.ReportManager oReport = new SmReport.ReportManager(SysObj, CommandInfo["rep_file"].ToString());
            //// Set the parameter's value.
            //SmLib.SysFunc.DSCopyWithFilter(oBrowse.frmBrw.oBrowse, ref DataSourceReport, "tbPh");
            //oReport.Preview(DataSourceReport);
            //SmLib.SysFunc.ResetFilter(ref DataSourceReport, "tbPh");

            ////Xoá phiếu được tạo
            int dem = 0;
            string zstt_rec = "";
            if (oBrowse.frmBrw.oBrowse.ActiveCell != null)
                oBrowse.frmBrw.oBrowse.ActiveCell.EndEditMode();
            var tbSelected = tbMain.Clone();

            foreach (DataRow row in tbMain.Rows)
            {
                if (row["tag"].ToString().Trim().Equals("True"))
                {
                    dem += 1;
                    tbSelected.ImportRow(row);
                }
                if (dem == 1)
                {
                    zstt_rec = row["stt_rec_hd"].ToString().Trim();
                }
            }

            if (dem == 0)
            {
                ExMessageBox.Show(7488, StartUp.SysObj, "Chưa chọn dữ liệu!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }


            string listSttRec = "";
            int stt = 0;
            foreach (DataRow row in StartUp.tbMain.Rows)
            {
                if (row["tag"].ToString().Trim().Equals("True"))
                {
                    string stt_rec_hd = row["stt_rec_hd"].ToString().Trim();
                    stt += 1;

                    listSttRec = listSttRec + stt_rec_hd + ",";

                }
            }

                if (ExMessageBox.Show(9475, StartUp.SysObj, "Có chắc chắn xoá chứng từ này không?", "Xac nhan nhap lieu", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                {
                    return;
                }
                try
                {
                    SqlCommand cmd = new SqlCommand("exec [dbo].[QLHD_delete_ctu] @Startdate, @EndDate, @ma_dvcs,@listSttRec");
                    cmd.Parameters.Add("@Startdate", SqlDbType.VarChar).Value = (string.IsNullOrEmpty(StartUp.zStartdate.ToString()) ? "" : string.Format("{0:yyyyMMdd}", (DateTime)StartUp.zStartdate));
                    cmd.Parameters.Add("@EndDate", SqlDbType.VarChar).Value = (string.IsNullOrEmpty(StartUp.zEndDate.ToString()) ? "" : string.Format("{0:yyyyMMdd}", (DateTime)StartUp.zEndDate));
                    cmd.Parameters.Add("@ma_dvcs", SqlDbType.VarChar).Value = StartUp.M_MA_DVCS.Trim();
                    cmd.Parameters.Add("@listSttRec", SqlDbType.VarChar).Value = listSttRec;
                    StartUp.SysObj.ExcuteNonQuery(cmd);

                    ExMessageBox.Show(9315, StartupBase.SysObj, "Xoá chứng từ . Thành công!", "", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }
                catch (Exception ex)
                {
                    SmErrorLib.ErrorLog.CatchMessage(ex);
                }
            




        }

        static void oBrowse_F5(object sender, EventArgs e)
        {

            int dem = 0;
            string zstt_rec = "";
            if (oBrowse.frmBrw.oBrowse.ActiveCell != null)
                oBrowse.frmBrw.oBrowse.ActiveCell.EndEditMode();
            var tbSelected = tbMain.Clone();

            foreach (DataRow row in tbMain.Rows)
            {
                if (row["tag"].ToString().Trim().Equals("True"))
                {
                    dem += 1;
                    tbSelected.ImportRow(row);
                }
                if (dem == 1)
                {
                    zstt_rec = row["stt_rec_hd"].ToString().Trim();
                }
            }

            if (dem == 0)
            {
                ExMessageBox.Show(7488, StartUp.SysObj, "Chưa chọn dữ liệu!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            //đoạn này tạo mới danh mục khách hàng

          var  zcmd = new SqlCommand("SELECT REPLACE(ma_so_thue, '-', '_') as ma_kh,max(ten_kh) as ten_kh,max(dia_chi) as dia_chi,ma_so_thue FROM Qlhd_header WHERE ngay_hd >= @Startdate and ngay_hd <= @EndDate and ma_so_thue not in (select distinct ma_so_thue from dmkh where ma_so_thue <> '') group by ma_so_thue  ");
            zcmd.Parameters.Add("@Startdate", SqlDbType.VarChar).Value = (string.IsNullOrEmpty(StartUp.zStartdate.ToString()) ? "" : string.Format("{0:yyyyMMdd}", (DateTime)StartUp.zStartdate));
           zcmd.Parameters.Add("@EndDate", SqlDbType.VarChar).Value = (string.IsNullOrEmpty(StartUp.zEndDate.ToString()) ? "" : string.Format("{0:yyyyMMdd}", (DateTime)StartUp.zEndDate));

            dtNhom = StartUp.SysObj.ExcuteReader(zcmd).Tables[0];

            if (dtNhom.Rows.Count != 0)
            {
                Frmcreate_kh create_kh = new Frmcreate_kh(ref dtNhom);
                create_kh.ShowDialog();

                if (create_kh.isOk == true)
                {

                    SqlCommand cmd2 = new SqlCommand("Select top 1 * from dmkh");
                    var dsDmKh = StartUp.SysObj.ExcuteReader(cmd2);
                    dsDmKh.Tables[0].Clear();
                    int M_User_Id = int.Parse(StartUp.SysObj.UserInfo.Rows[0]["user_id"].ToString());
                    string M_User_Name = StartUp.SysObj.UserInfo.Rows[0]["user_name"].ToString().Trim();

                    foreach (DataRow rKh in dtNhom.Rows)
                    {
                        var ma_kh = rKh["ma_kh"].ToString().Trim();

                        if (ma_kh != "")
                        {
                            var tblClone = dsDmKh.Tables[0].Clone();
                            var newKh = tblClone.NewRow();
                            newKh["ma_kh"] = ma_kh;
                            newKh["ten_kh"] = rKh["ten_kh"];
                            newKh["ma_so_thue"] = rKh["ma_so_thue"];
                            newKh["dia_chi"] = rKh["dia_chi"];
                            newKh["status"] = 1;

                            newKh["date"] = DateTime.Now;
                            newKh["time"] = DateTime.Now.ToString("HH:mm:ss");
                            newKh["user_id"] = M_User_Id;
                            newKh["user_name"] = M_User_Name;


                            newKh["date0"] = DateTime.Now;
                            newKh["time0"] = DateTime.Now.ToString("HH:mm:ss");
                            newKh["user_id0"] = M_User_Id;
                            newKh["user_name0"] = M_User_Name;

                            tblClone.Rows.Add(newKh);
                            if (tblClone.Columns.Contains("search"))
                            {
                                SmLib.SysFunc.SetStrSearch(StartUp.SysObj, "dmkh", ref tblClone);
                            }
                            dsDmKh.Tables[0].ImportRow(newKh);
                        }
                    }
                    DataTable tbkh = dsDmKh.Tables[0];


                    foreach (DataRow rKh in tbkh.Rows)
                    {
                        string msg = "";
                        if (create_kh.CheckValid(rKh["ma_kh"].ToString(), out msg))
                            ListFunc.inserRowInDataBase("dmkh", rKh, StartUp.SysObj);
                        else
                            MessageBox.Show(msg);
                    }
                    //string zma_kh = txtma_kh.Text.Trim();


                    MessageBox.Show("Tạo danh sách khách hàng / NCC. Thành công");
                    //return;
                }
            }

            if (!SmLib.SysFunc.CheckValidNgayKs(StartUp.SysObj, Convert.ToDateTime(StartUp.zStartdate)))
            {
                MessageBox.Show("Chứng từ phải sau ngày khóa sổ!");
                //ExMessageBox.Show(8600, StartUp.SysObj, "Chứng từ phải sau ngày khóa sổ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            try
            {
                //đoan này tạo chứng tư
                Frmcreate form = new Frmcreate();
                form.ShowDialog();
                StartUp.QueryData(true, StartUp.zStartdate, StartUp.zEndDate, StartUp.zfilter, StartUp.zkieu_loc);
            }

            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }
        public static string GetTableShow(bool KindReport)
        {
            string strFieldShow = string.Empty;
            strFieldShow = "ngay_ct;Ma_ct0;so_ct;ma_kh;";
            if (strLan.Equals("V"))
            {
                strFieldShow += "ten_kh;dien_giai;tk;tk_du;";
            }
            else
            {
                strFieldShow += "ten_kh2;dien_giai;tk;tk_du;";
            }

            if (KindReport)
            {
                strFieldShow += "ps_no;ps_co;ma_vv;ma_phi;";
            }
            else
            {
                strFieldShow += "ps_no_nt;ps_co_nt;ma_vv;ma_phi;";
            }

            if (strLan.Equals("V"))
            {
                strFieldShow += "ten_tk;ten_tk_du;ma_ct;ma_dvcs";
            }
            else
            {
                strFieldShow += "ten_tk2;ten_tk2_du;ma_ct;ma_dvcs";
            }
            return strFieldShow;
        }
        //public static string GetStrBrowse1(bool KindReport,int loai)
        //{
        //    string ketqua="";
        //    if (KindReport == true)
        //    { 

        //    }
        //    return ketqua;
        //}
        //public static string GetStrBrowse2(bool KindReport)
        //{
        //    string ketqua = "";
        //    return ketqua;
        //}
    }
}
