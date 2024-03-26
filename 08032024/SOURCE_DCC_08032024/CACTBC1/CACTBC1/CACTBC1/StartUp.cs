using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using SmDataLib;
using SmLib.SM.FormBrowse;
using System.Data;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Input;
using Sm.Windows.Controls;
using Infragistics.Windows.DataPresenter;
using System.Diagnostics;
using SmVoucherLib;
using Arttpb;
using SmDefine;

namespace CACTBC1
{
    public class StartUp : StartUpTrans
    {
        static public SqlCommand TransFilterCmd;
        //static public DataTable DtDmnt;

        static public SmDefine.ActionTask currActionTask = SmDefine.ActionTask.None;
       // static public string titleWindow = string.Empty;
        static public string M_ma_nt = string.Empty;

        public static string stringBrowse1 = "";//"ngay_ct:fl:100:h=Ngày c.từ;so_ct:fl:100:h=Số c.từ;t_tien_nt:n1:140:h=Tổng ps nt;ma_nt:80:h=Mã nt;ty_gia:140:h=Tỷ giá:r;t_tien:n0:140:h=Tổng ps;ma_kh:100:h=Mã khách;ten_kh:225:h=Tên khách;tk:100:h=Tài khoản nợ;[date]:140:h=Ngày cập nhật;[time]:140:h=Giờ cập nhật;[user_name]:180:h=Tên NSD";
        public static string stringBrowse2 = "";//"tk_i:fl:100:h=Tk có;tien_nt:140:n1:h=Ps có nt;dien_giaii:225:h=Diễn giải;tien:140:n0:h=Ps có";
        static public string[] M_Gd_2Tg_List = "4,5,6,7,8,9".Split(new char[] { ',' });

        //static public int rowIndex = -1;
        static private string M_IP_TIEN;
        static private string M_IP_TIEN_NT;
        static private string M_IP_TY_GIA;
        static public string M_IP_TIEN_HD;
        static public int M_CHK_ZERO;
        static public DataTable dtRegInfo;
        //static public int M_IN_HOI_CK = 0;
        public static string M_CHK_DATE_YN = "";
        public static DateTime? M_NGAY_BAT_DAU;
        public static DateTime? M_NGAY_KET_THUC;
        public override void Run()
        {
           Namespace = "CACTBC1";
            try
            {
                SysObj.SynchroFile(".", "CatgLib.dll");
                
                Ma_ct = "BC1";
                filterId = "CACTBC1";

                M_Gd_2Tg_List = SysObj.GetSysVar("M_GD_2TG_LIST").ToString().Trim().Split(new char[] { ',' });
                
                //Khoi tao cac tham so can thiet
                M_ma_nt0 = SysObj.GetOption("M_MA_NT0").ToString();
                Ws_Id = SysObj.GetOption("M_WS_ID").ToString();
                M_LAN = StartUp.SysObj.GetOption("M_LAN").ToString();
                M_MST_CHECK = SysObj.GetOption("M_MST_CHECK").ToString().Trim();
                M_User_Id = Convert.ToInt16(SysObj.UserInfo.Rows[0]["user_id"].ToString());
                M_CHK_ZERO = Convert.ToInt16(StartUp.SysObj.GetOption("M_CHK_ZERO").ToString());
                M_IP_TIEN = SysObj.GetOption("M_IP_TIEN").ToString();
                M_IP_TIEN_NT = SysObj.GetOption("M_IP_TIEN_NT").ToString();
                M_IP_TY_GIA = SysObj.GetOption("M_IP_TY_GIA").ToString();
                M_IN_HOI_CK = Convert.ToInt16(StartUp.SysObj.GetOption("M_IN_HOI_CK").ToString());
                M_IP_TIEN_HD = SysObj.GetOption("M_IP_TIEN_NT").ToString();

                CommandInfo = SmLib.SysFunc.GetCommandInfo(SysObj, Menu_Id);
                dtRegInfo = StartUp.SysObj.GetRegInfo();

                DmctInfo = SmDataLib.DataLoader.GetSqlFieldValue(SysObj, "dmct", "ma_ct", Ma_ct);
                filterView = string.Format("{0};{1}", StartUp.DmctInfo["v_phdbf"].ToString().Trim(), StartUp.DmctInfo["v_ctdbf"].ToString().Trim(), StartUp.DmctInfo["v_ctgtdbf"].ToString().Trim());
                if (CommandInfo == null)
                {
                    Sm.Windows.Controls.ExMessageBox.Show( 205,StartUp.SysObj, "Chưa khai báo command hoặc command ngầm định sai!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    if (!Process.GetCurrentProcess().ProcessName.Equals("SmProcess"))
                        App.Current.Shutdown();
                    return;
                }
                if (DmctInfo == null)
                {
                    Sm.Windows.Controls.ExMessageBox.Show( 210,StartUp.SysObj, "Chưa khai báo chứng từ hoặc chứng từ ngầm định sai!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    if (!Process.GetCurrentProcess().ProcessName.Equals("SmProcess"))
                        App.Current.Shutdown();
                    return;
                }
                FrmCACTBC1 _Form = new FrmCACTBC1();
            
                string[] strBrowses = null;
                if (M_LAN.Equals("V"))
                {
                    if (CommandInfo["Vbrowse2"].ToString().Trim() != "")
                        strBrowses = CommandInfo["Vbrowse2"].ToString().Split('|');
                }
                else
                {
                    if (CommandInfo["Ebrowse2"].ToString().Trim() != "")
                        strBrowses = CommandInfo["Ebrowse2"].ToString().Split('|');
                }
                if (strBrowses != null)
                {
                    stringBrowse1 = strBrowses[0];
                    stringBrowse2 = strBrowses[1];
                }

                //các tham số chứng từ
                M_ngay_lct = DmctInfo["m_ngay_lct"].ToString();
                M_ong_ba = DmctInfo["m_ong_ba"].ToString();
                int.TryParse(DmctInfo["m_sl_ct0"].ToString(), out M_sl_ct0);

                string conditionPH = " AND 1=1";
                if (!SmLib.SysFunc.CheckPermission(SysObj, ActionTask.View, Menu_Id))
                    conditionPH = " AND user_id0 = " + SysObj.UserInfo.Rows[0]["user_id"].ToString();

                M_CHK_DATE_YN = SysObj.GetOption("M_CHK_DATE_YN").ToString().Trim();
                if (Editing_Stt_Rec.Equals(string.Empty) && M_CHK_DATE_YN == "1")
                {
                    SmVoucherLib.SelectTime dlgSelTime = new SmVoucherLib.SelectTime();
                    dlgSelTime.LanguageID = "CACTBC1SelTime";

                    SmLib.SysFunc.LoadIcon(dlgSelTime);
                    dlgSelTime.ShowDialog();
                    if (dlgSelTime.IsOK)
                    {
                        M_NGAY_BAT_DAU = (DateTime)dlgSelTime.M_NGAY_CT1;
                        M_NGAY_KET_THUC = (DateTime)dlgSelTime.M_NGAY_CT2;
                        conditionPH += string.Format(" AND ngay_ct BETWEEN '{0:yyyyMMdd}' AND '{1:yyyyMMdd}'", M_NGAY_BAT_DAU, M_NGAY_KET_THUC);
                    }
                }
                TransFilterCmd = new SqlCommand("exec LoadVoucher @ma_ct, @PhFilter, @CtFilter, @GtFilter,@sl_ct");
                TransFilterCmd.Parameters.Add("@ma_ct", SqlDbType.Char).Value = Ma_ct;
                TransFilterCmd.Parameters.Add("@PhFilter", SqlDbType.NVarChar, 4000).Value = (Editing_Stt_Rec.Equals(string.Empty) ? "1=1" + " AND ma_dvcs = '" + SysObj.M_ma_dvcs.Trim() + "'" : "stt_rec = '" + Editing_Stt_Rec + "'") + conditionPH;
                TransFilterCmd.Parameters.Add("@CtFilter", SqlDbType.NVarChar, 4000).Value = "1=1";
                TransFilterCmd.Parameters.Add("@GtFilter", SqlDbType.NVarChar, 4000).Value = "";
                TransFilterCmd.Parameters.Add("@sl_ct", SqlDbType.Int).Value = M_sl_ct0;
                DsTrans = SmVoucherLib.DataProvider.FillCommand(SysObj, TransFilterCmd);
                DsTrans.Tables[0].DefaultView.Sort = "ngay_ct asc, so_ct asc";
                DsTrans.Tables[1].DefaultView.Sort = "stt_rec0";
                //Dòng để giữ focus khi xoá hoặc huỷ
                DataRow dr = DsTrans.Tables[0].NewRow();
                dr["stt_rec"] = string.Empty;
                dr["ma_nt"] = M_ma_nt0;
                DsTrans.Tables[0].Rows.InsertAt(dr, 0);

                tbStatus = SysObj.GetPostInfo(Ma_ct);

                //Load Icon
                SmLib.SysFunc.LoadIcon(_Form);
                _Form.Title = SmLib.SysFunc.Cat_Dau(M_LAN.Equals("V") ? CommandInfo["bar"].ToString() : CommandInfo["bar2"].ToString());
                _Form.StartUpMain = this;
                _Form.ShowDialog();
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }
        public static void DeleteVoucher(string _stt_rec)
        {
            try
            {
                if (StartUp.DsTrans.Tables[0].DefaultView[0]["ma_gd"].ToString().Trim().Equals("1"))
                {
                    string sql = "exec [dbo].[ArttpbDel] '{0}','{1}','{2}';\n";
                    string sqlCmd = "";
                    foreach (DataRowView row in StartUp.DsTrans.Tables[1].DefaultView)
                        sqlCmd += string.Format(sql, _stt_rec, Ma_ct, row["tk_i"].ToString().Trim());

                    Debug.WriteLine(sqlCmd);
                    SqlCommand cmdDel = new SqlCommand(sqlCmd);
                    StartUp.SysObj.ExcuteNonQuery(cmdDel);
                }
                
                SqlCommand cmd = new SqlCommand("exec [dbo].[DeleteVoucher] @cMa_ct,@stt_rec");
                cmd.Parameters.Add("@cMa_ct", SqlDbType.Char, 3).Value = Ma_ct;
                cmd.Parameters.Add("@stt_rec", SqlDbType.Char, 11).Value = _stt_rec;

                StartUp.SysObj.ExcuteNonQuery(cmd);
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }
        public static double GetRates(string _ma_nt, DateTime _ngay)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("select [dbo].[GetRates](@ma_nt,@ngay_ct)");
                cmd.Parameters.Add("@ma_nt", SqlDbType.Char, 3).Value = _ma_nt;
                cmd.Parameters.Add("@ngay_ct", SqlDbType.VarChar, 8).Value = string.Format("{0:yyyyMMdd}", _ngay);
                object o_ty_gia = SysObj.ExcuteScalar(cmd);
                double _ty_gia = Convert.ToDouble(o_ty_gia.Equals(DBNull.Value) ? 0 : o_ty_gia);
                return _ty_gia;
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
            return 0;
        }
        public static int UpdateRates(string _ma_nt, DateTime _ngay, decimal _ty_gia)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("exec [dbo].[SetRates] @ma_nt,@ngay_ct,@ty_gia,@user_id");
                cmd.Parameters.Add("@ma_nt", SqlDbType.Char, 3).Value = _ma_nt;
                cmd.Parameters.Add("@ngay_ct", SqlDbType.VarChar, 8).Value = string.Format("{0:yyyyMMdd}", _ngay);
                cmd.Parameters.Add("@ty_gia", SqlDbType.Decimal).Value = _ty_gia;
                cmd.Parameters.Add("@user_id", SqlDbType.Decimal).Value = StartUp.SysObj.UserInfo.Rows[0]["user_id"].ToString();

                int _rno = StartUp.SysObj.ExcuteNonQuery(cmd);
                return _rno;
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
            return 0;
        }

        public static DataSet CheckData()
        {
            SqlCommand cmd = new SqlCommand("exec [dbo].[CACTBC1-CheckData] @status, @stt_rec");
            cmd.Parameters.Add("@status", SqlDbType.Char, 1).Value = StartUp.DsTrans.Tables[0].DefaultView[0]["status"];
            cmd.Parameters.Add("@stt_rec", SqlDbType.Char, 11).Value = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"];
            return StartUp.SysObj.ExcuteReader(cmd);
        }
        public static void In()
        {
            try
            {
                FrmIn oReport = new FrmIn();
                oReport.ShowDialog();

            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }
        public static void GetDmnt(DataSet _ds)
        {
            SqlCommand cmd = new SqlCommand("Select *,@ma_nt as ma_nt0, @Type as read_num_type from dmnt");
            cmd.Parameters.Add("@ma_nt", SqlDbType.Char, 3).Value = StartUp.M_ma_nt0;
            cmd.Parameters.Add("@Type", SqlDbType.Char, 1).Value = SysObj.GetOption("M_READ_NUM");


            DataTable tbNT = StartUp.SysObj.ExcuteReader(cmd).Tables[0].Copy();
            tbNT.TableName = "TableNTInfo";
            if (_ds.Tables.IndexOf("TableNTInfo") >= 0)
                _ds.Tables.Remove("TableNTInfo");
            _ds.Tables.Add(tbNT);
        }

        #region CreateTableInfo
        static public void CreateTableInfo(DataSet _ds)
        {
            DataTable dt = new DataTable();
            dt.TableName = "TableInfo";

            DataColumn newcolumn = new DataColumn("M_IP_TIEN", typeof(string));
            newcolumn.DefaultValue = StartUp.M_IP_TIEN;
            dt.Columns.Add(newcolumn);

            newcolumn = new DataColumn("M_IP_TY_GIA", typeof(string));
            newcolumn.DefaultValue = StartUp.M_IP_TY_GIA;
            dt.Columns.Add(newcolumn);

            newcolumn = new DataColumn("M_IP_TIEN_NT", typeof(string));
            newcolumn.DefaultValue = StartUp.M_IP_TIEN_NT;
            dt.Columns.Add(newcolumn);

            newcolumn = new DataColumn("M_MA_NT0", typeof(string));
            newcolumn.DefaultValue = StartUp.M_ma_nt0;
            dt.Columns.Add(newcolumn);

            DataRow dr = dt.NewRow();
            dt.Rows.Add(dr);
            if (_ds.Tables.IndexOf("TableInfo") >= 0)
                _ds.Tables.Remove("TableInfo");
            _ds.Tables.Add(dt);
        }
        #endregion

        static void DataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            BasicGridView grid = (BasicGridView)sender;
            for (int i = 0; i < grid.Records.Count; i++)
            {
                if (((grid.Records[i] as DataRecord).DataItem as DataRowView)["bold"].ToString().Equals("1"))
                {
                    RecordPresenter rp = RecordPresenter.FromRecord(grid.Records[i]);
                    rp.FontWeight = FontWeights.Bold;
                }
            }
            grid.Loaded -= new RoutedEventHandler(DataGrid_Loaded);
        }

        static void frmBrw_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //SmReport.ReportManager oReport = new SmReport.ReportManager(SysObj, CommandInfo["rep_file"].ToString());
            //oReport.Preview(StartUp.DsTrans);
            FrmIn oReport = new FrmIn();
            oReport.ShowDialog();
            (sender as SmLib.SM.SMFormBrowse.FrmBrowse).Closing -= new System.ComponentModel.CancelEventHandler(frmBrw_Closing);
        }

        static void oBrowse_F7(object sender, EventArgs e)
        {
            FrmIn oReport = new FrmIn();
            oReport.ShowDialog();
        }

        static void oBrowse_Esc(object sender, EventArgs e)
        {
            (sender as FormBrowse).frmBrw.Close();
        }

    }

    public class PbInfo : IPhanbo
    {
        public PbInfo(object ngay_ct1, object ngay_ct2, object tk, string ma_kh, string ma_dvcs)
        {
            M_NGAY_CT1 = ngay_ct1;
            M_NGAY_CT2 = ngay_ct2;
            M_TK = tk;
            Ma_Dvcs = ma_dvcs;
            Ma_kh = ma_kh;
        }

        #region IPhanbo Members

        public object M_NGAY_CT1 { get; set; }

        public object M_NGAY_CT2 { get; set; }

        public object M_TK { get; set; }

        public string Ma_Dvcs { get; set; }

        public string Ma_kh { get; set; }

        public string TitleView { get; set; }

        #endregion
    }
}
