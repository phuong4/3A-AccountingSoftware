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

namespace QLHD_BC01
{
    class StartUp:StartupBase
    {
        static public DataSet DataSourceReport = new DataSet();
        static public DateTime M_ngay_ct0;
        static public string M_ma_nt0;
        static private FrmLoc _frmSearch;
        static private FormBrowse oBrowse;
        static private DataRow CommandInfo;
        //0 tất cả, 1 VND, 2 Ngoại tệ
        static private int KindStyleReport = -1;
        static private SqlCommand cmd;
        static public string parameter = "";
        static public string strLan = string.Empty;
        public override void Run()
        {
           Namespace = "QLHD_BC01";
            try
            {
                CommandInfo = SmLib.SysFunc.GetCommandInfo(SysObj, Menu_Id);
                if (CommandInfo == null)
                {
                    Sm.Windows.Controls.ExMessageBox.Show( 145,SysObj, "Chưa khai báo command hoặc command ngầm định sai!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    if (!Process.GetCurrentProcess().ProcessName.Equals("SmProcess"))
                        App.Current.Shutdown();
                    return;
                }
                parameter = CommandInfo["parameter"].ToString();

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
                DataTable tbDetail;
                if (isFirstLoad)
                {
                    cmd = new SqlCommand();
                    cmd.CommandText = "Exec " + CommandInfo["store_proc"] + " @StartDate , @EndDate , @Condition";
                    cmd.Parameters.Add("@StartDate", SqlDbType.VarChar).Value = string.IsNullOrEmpty(StartDate.ToString()) ? "" : String.Format("{0:yyyyMMdd}", (DateTime)StartDate);
                    cmd.Parameters.Add("@EndDate", SqlDbType.VarChar).Value = string.IsNullOrEmpty(EndDate.ToString()) ? "" : String.Format("{0:yyyyMMdd}", (DateTime)EndDate);
                    cmd.Parameters.Add("@Condition", SqlDbType.NVarChar).Value = filter;
                    tbDetail = SysObj.ExcuteReader(cmd).Tables[0].Copy();
                    tbDetail.TableName = "tbDetail";
                    DataSourceReport.Tables.Add(tbDetail);

                    KindStyleReport = KindReport ? 1 : 2;

                    oBrowse = new FormBrowse(SysObj, tbDetail.DefaultView, GetTableShow(KindStyleReport));
                    oBrowse.F7 += new FormBrowse.GridKeyUp_F7(oBrowse_F7);
                    oBrowse.CTRL_R += new FormBrowse.GridKeyUp_CTRL_R(oBrowse_CTRL_R);
                    oBrowse.frmBrw.oBrowse.FieldSettings.AllowEdit = false;
                    oBrowse.frmBrw.Title = SmLib.SysFunc.Cat_Dau(M_LAN.Equals("V") ? CommandInfo["bar"].ToString() : CommandInfo["bar2"].ToString());

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

                if (isFirstLoad)
                {
                    oBrowse.frmBrw.LanguageID  = "QLHD_BC01_1";
                    oBrowse.ShowDialog();
                    _frmSearch.Close();
                }
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
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

        static void oBrowse_F7(object sender, EventArgs e)
        {
            SmReport.ReportManager oReport = new SmReport.ReportManager(SysObj, CommandInfo["rep_file"].ToString(), KindStyleReport);
            SmLib.SysFunc.DSCopyWithFilter(oBrowse.frmBrw.oBrowse, ref DataSourceReport, "tbDetail");
            oReport.Preview(DataSourceReport);
            SmLib.SysFunc.ResetFilter(ref DataSourceReport, "tbDetail");
        }
        public static string GetTableShow(int KindReport)
        {
            string strFieldShow = string.Empty;
            switch (strLan)
            {
                case("V"):
                    {
                        switch (KindReport)
                        {
                            case 1:
                                {
                                    strFieldShow =CommandInfo["Vbrowse1"].ToString();
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
