using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using SmDataLib;
using SmLib.SM.FormBrowse;
using System.Data;
using System.Runtime.InteropServices;
using System.Windows;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using SmVoucherLib;
using Sm.Windows.Controls;
using SmDefine;

namespace APCTPN1
{
    public class StartUp : StartUpTrans
    {
        //static public string Ws_Id;
        //static public string Ma_ct = "PN1";


        static public SqlCommand TransFilterCmd;
        //static public DataRow CommandInfo;
        //static public DataRow DmctInfo;

        //static public DataTable tbStatus;
        //static public DataSet DsTrans;
        //static public DataTable DtDmnt;

        static public string titleWindow = string.Empty;
        static public string M_ma_ms = string.Empty;
        static public int so_dong_in = 0;
        //static public string filterId = "APCTPN1";
        //static public string filterView = string.Empty;
        //static public string M_ma_nt0;
        //static public string M_MST_CHECK;
        //static public int M_User_Id = 0;
        //static public string M_ngay_lct = string.Empty;
        //static public string M_ong_ba = string.Empty;
        //static public string M_trung_so = string.Empty;
        //static public int M_sl_ct0 = 0;
        public static string M_CHK_DATE_YN = "";
        public static DateTime? M_NGAY_BAT_DAU;
        public static DateTime? M_NGAY_KET_THUC;



        public override void Run()
        {
           Namespace = "APCTPN1";           
            try
            {
                Debug.WriteLine(string.Format("#2: {0}", DateTime.Now.ToString()));

                Ma_ct = "PN1";
                filterId = "APCTPN1";
                //Khoi tao cac tham so can thiet
                M_ma_nt0 = SysObj.GetOption("M_MA_NT0").ToString();
                Ws_Id = SysObj.GetOption("M_WS_ID").ToString();
                M_ma_ms = SysObj.GetOption("M_MA_MS").ToString().Trim();
                M_MST_CHECK = SysObj.GetOption("M_MST_CHECK").ToString().Trim();
                M_User_Id = Convert.ToInt16(SysObj.UserInfo.Rows[0]["user_id"].ToString());
                M_IN_HOI_CK = Convert.ToInt16(StartUp.SysObj.GetOption("M_IN_HOI_CK").ToString());
                M_CHK_HD_VAO = Convert.ToInt16(StartUp.SysObj.GetOption("M_CHK_HD_VAO").ToString());

                CommandInfo = SmLib.SysFunc.GetCommandInfo(SysObj, Menu_Id);
                DmctInfo = SmDataLib.DataLoader.GetSqlFieldValue(SysObj, "dmct", "ma_ct", Ma_ct);
                filterView = string.Format("{0};{1};{2}", StartUp.DmctInfo["v_phdbf"].ToString().Trim(), StartUp.DmctInfo["v_ctdbf"].ToString().Trim(), StartUp.DmctInfo["v_ctgtdbf"].ToString().Trim());
                so_dong_in = SysObj.GetDmctInfo(Ma_ct).Rows[0]["so_dong_in"] != DBNull.Value ? Convert.ToInt16(SysObj.GetDmctInfo(Ma_ct).Rows[0]["so_dong_in"]) : 0;
                if (CommandInfo == null)
                {
                    Sm.Windows.Controls.ExMessageBox.Show( 250,SysObj, "Chưa khai báo command hoặc command ngầm định sai!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    if (!Process.GetCurrentProcess().ProcessName.Equals("SmProcess"))
                        App.Current.Shutdown();
                    return;
                }
                if (DmctInfo == null)
                {
                    Sm.Windows.Controls.ExMessageBox.Show( 255,SysObj, "Chưa khai báo chứng từ hoặc chứng từ ngầm định sai!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    if (!Process.GetCurrentProcess().ProcessName.Equals("SmProcess"))
                        App.Current.Shutdown();
                    return;
                }
                FrmAPCTPN1 _Form = new FrmAPCTPN1();
                
                //các tham số chứng từ
                M_ngay_lct = DmctInfo["m_ngay_lct"].ToString();
                M_ong_ba = DmctInfo["m_ong_ba"].ToString();
 
                int.TryParse(DmctInfo["m_sl_ct0"].ToString(), out M_sl_ct0);
                        
                //DtDmnt = SmVoucherLib.DataProvider.FillCommand(SysObj, new SqlCommand("select ma_nt, ten_nt from vdmnt")).Tables[0];
                //DtDmnt.DefaultView.Sort = "ma_nt";
                string conditionPH = " AND 1=1";
                if (!SmLib.SysFunc.CheckPermission(SysObj, ActionTask.View, Menu_Id))
                    conditionPH = " AND user_id0 = " + SysObj.UserInfo.Rows[0]["user_id"].ToString();

                M_CHK_DATE_YN = SysObj.GetOption("M_CHK_DATE_YN").ToString().Trim();
                if (Editing_Stt_Rec.Equals(string.Empty) && M_CHK_DATE_YN == "1")
                {
                    SmVoucherLib.SelectTime dlgSelTime = new SelectTime();
                    dlgSelTime.LanguageID = "APCTPN1SelTime";

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

                Debug.WriteLine(string.Format("#3: {0}", DateTime.Now.ToString()));
                DsTrans = SmVoucherLib.DataProvider.FillCommand(SysObj, TransFilterCmd);
                DsTrans.Tables[0].DefaultView.Sort = "ngay_ct asc, so_ct asc";
                DsTrans.Tables[1].DefaultView.Sort = "stt_rec0";
                DsTrans.Tables[2].DefaultView.Sort = "stt_rec0";
                //Dòng để giữ focus khi xoá hoặc huỷ
                DataRow dr = DsTrans.Tables[0].NewRow();
                dr["stt_rec"] = string.Empty;
                dr["ma_nt"] = DmctInfo["ma_nt"];
                DsTrans.Tables[0].Rows.InsertAt(dr, 0);
  
                tbStatus=SysObj.GetPostInfo(Ma_ct);

                //System.Windows.MessageBox.Show(SysObj.SysCultureInfo.NumberFormat.NumberGroupSeparator);

                //System.Windows.MessageBox.Show(SysObj.GetOption("M_MA_NT0").ToString());

                //_Form.GrdCt.FieldLayoutSettings.AutoGenerateFields = false;
                //_Form.GrdCt.DataSource = DsTrans.Tables[1].DefaultView;
                _Form.Title = SmLib.SysFunc.Cat_Dau(M_LAN.Equals("V") ? CommandInfo["bar"].ToString() : CommandInfo["bar2"].ToString());
                Debug.WriteLine(string.Format("#4: {0}", DateTime.Now.ToString()));
                _Form.StartUpMain = this;
                //Load Icon
                SmLib.SysFunc.LoadIcon(_Form);
                _Form.ShowDialog();
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }
        //public void FastLoader(object[] parameters, SysLib.SysObject SysO)
        //{
        //    try
        //    {
        //        StartUp.SysObj = SysO;
        //        StartUp.Menu_Id = parameters.Length > 0 ? parameters[0].ToString() : "05.02.02";
        //        StartUp.Editing_Stt_Rec = parameters.Length > 1 ? parameters[1].ToString() : string.Empty;

        //        Run();
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Windows.MessageBox.Show("FastLoader error");
        //        SmErrorLib.ErrorLog.CatchMessage(ex);
        //    }
        //}
        public static void DeleteVoucher(string _stt_rec)
        {
            try
            {
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
        //public static int UpdateRates(string _ma_nt, DateTime _ngay, decimal _ty_gia)
        //{
        //    try
        //    {
        //        SqlCommand cmd = new SqlCommand("exec [dbo].[SetRates] @ma_nt,@ngay_ct,@ty_gia,@user_id");
        //        cmd.Parameters.Add("@ma_nt", SqlDbType.Char, 3).Value = _ma_nt;
        //        cmd.Parameters.Add("@ngay_ct", SqlDbType.VarChar, 8).Value = string.Format("{0:yyyyMMdd}", _ngay);
        //        cmd.Parameters.Add("@ty_gia", SqlDbType.Decimal).Value = _ty_gia;
        //        cmd.Parameters.Add("@user_id", SqlDbType.Decimal).Value = StartUp.SysObj.UserInfo.Rows[0]["user_id"].ToString();

        //        int _rno = StartUp.SysObj.ExcuteNonQuery(cmd);
        //        return _rno;
        //    }
        //    catch (Exception ex)
        //    {
        //        SmErrorLib.ErrorLog.CatchMessage(ex);
        //    }
        //    return 0;
        //}
        public static DataSet CheckData()
        {
            SqlCommand cmd = new SqlCommand("exec [dbo].[APCTPN1-CheckData] @status, @stt_rec");
            cmd.Parameters.Add("@status", SqlDbType.Char, 1).Value = StartUp.DsTrans.Tables[0].DefaultView[0]["status"];
            cmd.Parameters.Add("@stt_rec", SqlDbType.Char, 11).Value = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"];
            return StartUp.SysObj.ExcuteReader(cmd);
        }
        
        #region GetLanguageString
        public static string GetLanguageString(string code, string language)
        {
            if (code == "M_MA_NT")
                if (!StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0))
                    return StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString();
                else
                    return "";
            if (code == "M_MA_NT0")
                return StartUp.M_ma_nt0;
            return code;
        }
        #endregion
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
        static public bool IsTkMe(string tk)
        {
            bool result = false;
            try
            {
                SqlCommand cmdGet = new SqlCommand("exec CheckIsTkMe @tk");
                cmdGet.Parameters.Add("@tk", SqlDbType.Char).Value = tk;
                if ((int)StartUp.SysObj.ExcuteScalar(cmdGet) > 0)
                    result = true;
            }
            catch (SqlException ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
            return result;
        }

        public static bool CheckExistHDVao(string stt_rec, string so_ct0, string so_seri0, string ngay_ct0, string ma_so_thue)
        {
            SqlCommand cmd = new SqlCommand("Exec CheckExistsHDVao @stt_rec, @so_ct0, @so_seri0, @ngay_ct0, @ma_so_thue");
            cmd.Parameters.Add("@stt_rec", SqlDbType.VarChar).Value = stt_rec;
            cmd.Parameters.Add("@so_ct0", SqlDbType.VarChar).Value = so_ct0;
            cmd.Parameters.Add("@so_seri0", SqlDbType.VarChar).Value = so_seri0;
            cmd.Parameters.Add("@ngay_ct0", SqlDbType.VarChar).Value = ngay_ct0;
            cmd.Parameters.Add("@ma_so_thue", SqlDbType.VarChar).Value = ma_so_thue;
            int result = (int)StartUp.SysObj.ExcuteScalar(cmd);
            if (result == 1)
                return true;
            else
                return false;
        }
    }
}
