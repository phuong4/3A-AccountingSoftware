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
using System.Diagnostics;

namespace POBKHDM
{
    class StartUp : Sm.Windows.Controls.StartupBase
    {
        //static public SysLib.SysObject SysObj;
        static public DataSet DataSourceReport = new DataSet();
        //static private TransactionListFrm _frmSearch;
        static public DataTable dtInfo;
        static private FormBrowse2 oBrowse;
        //static private SmVoucherLib.FormBrowse2 oBrowse;
        static private DataRow CommandInfo;
        //static public string Menu_Id = "";
        // static public string sqlTableName = "ct70";
        static public string sqlTableView = "v_pobk1";
        static public string SqlTableKey = "stt_rec";//
        static public string SqlTableObjectName = "ten_kh";
        static public DateTime M_ngay_ct0;
        static public string M_ma_nt0;
        static SqlCommand cmd1 = new SqlCommand();
        static SqlCommand cmd = new SqlCommand();
        static FormLoc _frmLoc;
        static public string TableName = "ct70";
        static public string parameter = "";
        public override void Run()
        {
           Namespace = "POBKHDM";
            try
            {
                DateTime t1 = DateTime.Now;
                CommandInfo = SmLib.SysFunc.GetCommandInfo(SysObj, Menu_Id);
                M_ngay_ct0 = (DateTime)SysObj.GetSysVar("M_NGAY_KY1");
                M_ma_nt0 = StartUp.SysObj.GetOption("M_MA_NT0").ToString();
                dtInfo = new DataTable();
                dtInfo.TableName = "TbInfo";
                dtInfo.Columns.Add("StartDate");
                dtInfo.Columns.Add("EndDate");

                if (CommandInfo != null)
                {
                    parameter = CommandInfo["parameter"].ToString();
                    _frmLoc = new FormLoc();
                    _frmLoc.Title = SmLib.SysFunc.Cat_Dau(CommandInfo["bar"].ToString());
                    if(M_LAN != "V")
                        _frmLoc.Title = SmLib.SysFunc.Cat_Dau(CommandInfo["bar2"].ToString());

                    DateTime t2 = DateTime.Now;
                    _frmLoc.ShowDialog();

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public static void CallGridVouchers(object StartDate, object EndDate, string filter, int KindReport)
        {
            try
            {

                string strBrowse = "";
                string strBrowse2 = "";
                string strBrowse_nt = "";
                string strBrowse2_nt = "";
                string[] listStored = CommandInfo["store_proc"].ToString().Split('|');
                cmd = new SqlCommand();
                cmd.CommandText = "Exec " + listStored[0] + " @StartDate , @EndDate , @Condition";
                cmd.Parameters.Add("@StartDate", SqlDbType.VarChar).Value = string.IsNullOrEmpty(StartDate.ToString()) ? "" : String.Format("{0:yyyyMMdd}", (DateTime)StartDate);
                cmd.Parameters.Add("@EndDate", SqlDbType.VarChar).Value = string.IsNullOrEmpty(EndDate.ToString()) ? "" : String.Format("{0:yyyyMMdd}", (DateTime)EndDate);
                cmd.Parameters.Add("@Condition", SqlDbType.NVarChar).Value = filter;

                DataSet ds = SysObj.ExcuteReader(cmd);
                DataTable tbMain = ds.Tables[0].Copy();
                tbMain.TableName = "tbMain";
                DataTable tbDetail = ds.Tables[1].Copy();
                tbDetail.TableName = "tbDetail";

                DataTable reportHeaderTable = tbMain.Copy();
                DataTable reportDetailTable = tbDetail.Copy();
                reportHeaderTable.TableName = "tbPh";
                reportHeaderTable.Columns.Add("KHInfo", typeof(String), "TRIM(ma_kh)+' - '+ TRIM(ten_kh)");
                reportHeaderTable.Columns.Add("KHInfo2", typeof(String), "TRIM(ma_kh)+' - '+ TRIM(ten_kh2)");
                reportHeaderTable.Columns.Add("DGInfo", typeof(String), "Iif(Trim([dien_giai]) IS NULL OR LEN(Trim([dien_giai])) = 0,'',Trim([dien_giai]))+Iif(Trim([ma_vv]) IS NULL OR LEN(Trim([ma_vv])) = 0,'',' ('+Trim([ma_vv])+')')");
                reportDetailTable.TableName = "tbCt";
                reportDetailTable.Columns.Add("VTInfo", typeof(String), "TRIM(ma_vt)+' - '+ TRIM(ten_vt)");
                reportDetailTable.Columns.Add("VTInfo2", typeof(String), "TRIM(ma_vt)+' - '+ TRIM(ten_vt2)");

                DataSourceReport = new DataSet();
                DataSourceReport.Tables.Add(reportHeaderTable);
                DataSourceReport.Tables.Add(reportDetailTable);
                DataSourceReport.Tables.Add(dtInfo.Copy());
                // tạo relation cho 2 table Header va Detail trong dataset DataSourceReport
                DataColumn ParentCol = reportHeaderTable.Columns["stt_rec"];
                DataColumn ChildrenCol = reportDetailTable.Columns["stt_rec"];




                String RelationName = "Stt_rec_Relation";
                DataRelation dRelation = new DataRelation(RelationName, ParentCol, ChildrenCol, false);
                DataSourceReport.Relations.Add(dRelation);

                //TestWin twin = new TestWin();
                //twin.xamDataGrid1.DataSource = DataSourceReport.Tables[0].DefaultView ;
                //twin.ShowDialog();

                String[] arrStrBrowse1;
                String[] arrStrBrowse2;
                if (SysObj.GetOption("M_LAN").ToString().ToUpper().Equals("V"))
                {
                    arrStrBrowse1 = CommandInfo["VBrowse1"].ToString().Trim().Split('|');
                    arrStrBrowse2 = CommandInfo["VBrowse2"].ToString().Trim().Split('|');
                }
                else
                {
                    arrStrBrowse1 = CommandInfo["EBrowse1"].ToString().Trim().Split('|');
                    arrStrBrowse2 = CommandInfo["EBrowse2"].ToString().Trim().Split('|');
                }
                strBrowse = arrStrBrowse1[0];
                strBrowse2 = arrStrBrowse1[1];
                strBrowse_nt = arrStrBrowse2[0];
                strBrowse2_nt = arrStrBrowse2[1];

                if (KindReport == 0)
                {

                    oBrowse = new FormBrowse2(SysObj, tbMain.DefaultView, tbDetail.DefaultView, strBrowse, strBrowse2, "stt_rec");
                }
                else
                {
                    oBrowse = new FormBrowse2(SysObj, tbMain.DefaultView, tbDetail.DefaultView, strBrowse_nt, strBrowse2_nt, "stt_rec");
                }


                //SmReport.Browser.Browse(tbMain.DefaultView);
                //SmReport.Browser.Browse(tbDetail.DefaultView);

                oBrowse.F7 += new FormBrowse2.GridKeyUp_F7(oBrowse_F7);
                oBrowse.CTRL_R += new FormBrowse2.GridKeyUp_CTRL_R(oBrowse_CTRL_R);
                
                oBrowse.frmBrw.oBrowse.FieldSettings.AllowEdit = false;
                oBrowse.frmBrw.Title = SmLib.SysFunc.Cat_Dau(CommandInfo["bar"].ToString());
                oBrowse.Esc += new FormBrowse2.GridKeyUp_Esc(oBrowse_Esc);

                if(M_LAN != "V")
                    oBrowse.frmBrw.Title = SmLib.SysFunc.Cat_Dau(CommandInfo["bar2"].ToString());



                oBrowse.frmBrw.LanguageID  = "POBKHDMBrowse";
                oBrowse.ShowDialog();
                if (!Process.GetCurrentProcess().ProcessName.Equals("SmProcess"))
                    App.Current.Shutdown();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.InnerException.Message);
            }
        }

        #region QueryData
        public static void QueryData(bool isFirstLoad, object StartDate, object EndDate, string filter, int KindReport)
        {
            try
            {
                if (isFirstLoad)
                {

                    CallGridVouchers(StartDate, EndDate, filter, KindReport);

                }
                else
                {
                    DataSourceReport = new DataSet();
                    DataSet ds = SysObj.ExcuteReader(cmd);
                    DataTable tbMain = ds.Tables[0].Copy();
                    tbMain.TableName = "tbMain";

                    DataTable tbDetail = ds.Tables[1].Copy();
                    tbDetail.TableName = "tbDetail";

                    DataTable reportHeaderTable = tbMain.Copy();
                    DataTable reportDetailTable = tbDetail.Copy();
                    reportHeaderTable.TableName = "tbPh";
                    reportHeaderTable.Columns.Add("KHInfo", typeof(String), "TRIM(ma_kh)+' - '+ TRIM(ten_kh)");
                    reportHeaderTable.Columns.Add("KHInfo2", typeof(String), "TRIM(ma_kh)+' - '+ TRIM(ten_kh2)");
                    reportHeaderTable.Columns.Add("DGInfo", typeof(String), "Iif(Trim([dien_giai]) IS NULL OR LEN(Trim([dien_giai])) = 0,'',Trim([dien_giai]))+Iif(Trim([ma_vv]) IS NULL OR LEN(Trim([ma_vv])) = 0,'',' ('+Trim([ma_vv])+')')");
                    reportDetailTable.TableName = "tbCt";
                    reportDetailTable.Columns.Add("VTInfo", typeof(String), "TRIM(ma_vt)+' - '+ TRIM(ten_vt)");
                    reportDetailTable.Columns.Add("VTInfo2", typeof(String), "TRIM(ma_vt)+' - '+ TRIM(ten_vt2)");

                    DataSourceReport = new DataSet();
                    DataSourceReport.Tables.Add(reportHeaderTable);
                    DataSourceReport.Tables.Add(reportDetailTable);
                    DataSourceReport.Tables.Add(dtInfo.Copy());
                    // tạo relation cho 2 table Header va Detail trong dataset DataSourceReport
                    DataColumn ParentCol = reportHeaderTable.Columns["stt_rec"];
                    DataColumn ChildrenCol = reportDetailTable.Columns["stt_rec"];

                    String RelationName = "Stt_rec_Relation";
                    DataRelation dRelation = new DataRelation(RelationName, ParentCol, ChildrenCol, false);
                    DataSourceReport.Relations.Add(dRelation);

                    oBrowse.frmBrw.oBrowse.DataSource = tbMain.DefaultView;
                    oBrowse.frmBrw.oBrowseCt.DataSource = tbDetail.DefaultView;

                    oBrowse.frmBrw.oBrowse.FieldLayouts[0].SummaryDefinitions.Clear();
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
            StartUp.QueryData(false, (object)_frmLoc.TxtStartDateTime.Value, (object)_frmLoc.TxtEndDateTime.Value, filter, int.Parse(_frmLoc.cbMauBaoCao2.Value.ToString()));
        }
        #endregion

        static void oBrowse_Esc(object sender, EventArgs e)
        {
            oBrowse.frmBrw.Close();
            if (!Process.GetCurrentProcess().ProcessName.Equals("SmProcess"))
                App.Current.Shutdown();
        }



        static void oBrowse_F7(object sender, EventArgs e)
        {
            SmReport.ReportManager oReport = new SmReport.ReportManager(SysObj, CommandInfo["rep_file"].ToString());
            SmLib.SysFunc.DSCopyWithFilter(oBrowse.frmBrw.oBrowse, ref DataSourceReport, "tbPh");
            oReport.Preview(DataSourceReport);
            //SmLib.SysFunc.ResetFilter(ref DataSourceReport, "tbPh");
        }
        public static string GetTableShow(bool KindReport)
        {
            string strFieldShow = string.Empty;
            string strLan = SysObj.GetOption("M_LAN").ToString().ToUpper();
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
