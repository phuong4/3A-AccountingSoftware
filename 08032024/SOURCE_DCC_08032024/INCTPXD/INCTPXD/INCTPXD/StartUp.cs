using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Sm.Windows.Controls;
using System.Windows;
using SmErrorLib;
using System.Diagnostics;
using SmVoucherLib;
using SmDefine;

namespace Inctpxd
{
    public class StartUp:StartUpTrans
    {
       
        static public SqlCommand TransFilterCmd;
       
        static public string M_Tilte = string.Empty;
        static public string M_ma_nt = string.Empty;
      
        static public string M_CHK_TON_VT;

        static public string M_MA_THUE;
        static public string M_PHONE;
        FrmInctpxd _Form;
        static public string stringBrowse1 = "";
        static public string stringBrowse2 = "";
        static public DateTime M_ngay_ct0;
        static public DateTime ngay_gia_px;
        static public string tableList = "v_PH84;v_CT84";
        public static string M_CHK_DATE_YN = "";
        public static DateTime? M_NGAY_BAT_DAU;
        public static DateTime? M_NGAY_KET_THUC;

        #region Run
        public override void Run()
        {
            Namespace = "Inctpxd";
            Ma_ct = "PXD";
            filterId = "INCTPXD";
            //Khoi tao cac tham so can thiet
            M_ma_nt0 = SysObj.GetOption("M_MA_NT0").ToString();
            Ws_Id = SysObj.GetOption("M_WS_ID").ToString();
            M_LAN = SysObj.GetOption("M_LAN").ToString();
            M_ROUND = Convert.ToInt16(SysObj.GetSysVar("M_ROUND"));
            M_ROUND_NT = Convert.ToInt16(SysObj.GetSysVar("M_ROUND_NT"));
            M_ROUND_GIA = Convert.ToInt16(StartUp.SysObj.GetSysVar("M_ROUND_GIA"));
            M_ROUND_GIA_NT = Convert.ToInt16(StartUp.SysObj.GetSysVar("M_ROUND_GIA_NT"));

            M_User_Id = Convert.ToInt16(SysObj.UserInfo.Rows[0]["user_id"].ToString());
            M_MST_CHECK = SysObj.GetOption("M_MST_CHECK").ToString().Trim();

            M_CHK_TON_VT = SysObj.GetOption("M_CHK_TON_VT").ToString();

            M_MA_THUE = SysObj.GetOption("M_MA_THUE").ToString();
            M_PHONE = SysObj.GetOption("M_PHONE").ToString();

            M_ngay_ct0 = Convert.ToDateTime(SysObj.GetSysVar("M_NGAY_KY1"));
            object ngay = SysObj.GetOption("M_NGAY_GIA_PX");
            ngay_gia_px = DateTime.Parse(ngay.ToString());
            CommandInfo = SmLib.SysFunc.GetCommandInfo(SysObj, Menu_Id);

            DmctInfo = SmDataLib.DataLoader.GetSqlFieldValue(SysObj, "dmct", "ma_ct", Ma_ct);

            M_ngay_lct = DmctInfo["m_ngay_lct"].ToString();
            M_ong_ba = DmctInfo["m_ong_ba"].ToString();

            int.TryParse(DmctInfo["m_sl_ct0"].ToString(), out M_sl_ct0);
            _Form = new FrmInctpxd();
         
          //  filterView = string.Format("{0};{1};{2}", StartUp.DmctInfo["v_phdbf"].ToString().Trim(), StartUp.DmctInfo["v_ctdbf"].ToString().Trim(), StartUp.DmctInfo["v_ctgtdbf"].ToString().Trim());
            if (CommandInfo == null)
            {
                ExMessageBox.Show( 1115,SysObj, "Chưa khai báo command hoặc command ngầm định sai!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                if (!Process.GetCurrentProcess().ProcessName.Equals("SmProcess"))
                    App.Current.Shutdown();
                return;
            }
            if (DmctInfo == null)
            {
                ExMessageBox.Show( 1120,SysObj, "Chưa khai báo chứng từ hoặc chứng từ ngầm định sai!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                if (!Process.GetCurrentProcess().ProcessName.Equals("SmProcess"))
                    App.Current.Shutdown();
                return;
            }
            if (!CommandInfo["ma_phan_he"].ToString().Trim().Equals("IN"))
            {
                App.Current.Shutdown();
            }
            string[] strBrowses = StartUp.M_LAN.Equals("V") ? CommandInfo["Vbrowse2"].ToString().Split('|') : CommandInfo["Ebrowse2"].ToString().Split('|');
            
            if (strBrowses != null)
            {
                stringBrowse1 = strBrowses[0];
                stringBrowse2 = strBrowses[1];
            }


            M_Tilte = M_LAN.Equals("V") ? SmLib.SysFunc.Cat_Dau(CommandInfo["bar"].ToString()) : SmLib.SysFunc.Cat_Dau(CommandInfo["bar2"].ToString());

            string conditionPH = " AND 1=1";
            if (!SmLib.SysFunc.CheckPermission(SysObj, ActionTask.View, Menu_Id))
                conditionPH = " AND user_id0 = " + SysObj.UserInfo.Rows[0]["user_id"].ToString();

            M_CHK_DATE_YN = SysObj.GetOption("M_CHK_DATE_YN").ToString().Trim();
            if (Editing_Stt_Rec.Equals(string.Empty) && M_CHK_DATE_YN == "1")
            {
                SmVoucherLib.SelectTime dlgSelTime = new SmVoucherLib.SelectTime();
                dlgSelTime.LanguageID = "INCTPXDSelTime";

                SmLib.SysFunc.LoadIcon(dlgSelTime);
                dlgSelTime.ShowDialog();
                if (dlgSelTime.IsOK)
                {
                    M_NGAY_BAT_DAU = (DateTime)dlgSelTime.M_NGAY_CT1;
                    M_NGAY_KET_THUC = (DateTime)dlgSelTime.M_NGAY_CT2;
                    conditionPH += string.Format(" AND ngay_ct BETWEEN '{0:yyyyMMdd}' AND '{1:yyyyMMdd}'", M_NGAY_BAT_DAU, M_NGAY_KET_THUC);
                }
            }
            TransFilterCmd = new SqlCommand("exec LoadVoucher @ma_ct, @PhFilter, @CtFilter, @GtFilter, @Sl_ct");
            TransFilterCmd.Parameters.Add("@ma_ct", SqlDbType.Char).Value = Ma_ct;
            TransFilterCmd.Parameters.Add("@PhFilter", SqlDbType.NVarChar, 4000).Value = (Editing_Stt_Rec.Equals(string.Empty) ? "1=1" + " AND ma_dvcs = '" + SysObj.M_ma_dvcs.Trim() + "'" : "stt_rec = '" + Editing_Stt_Rec + "'") + conditionPH;
            TransFilterCmd.Parameters.Add("@CtFilter", SqlDbType.NVarChar, 4000).Value = "1=1";
            TransFilterCmd.Parameters.Add("@GtFilter", SqlDbType.NVarChar, 4000).Value = "1=1";
            TransFilterCmd.Parameters.Add("@Sl_ct", SqlDbType.Int).Value = M_sl_ct0;
            DsTrans = SmVoucherLib.DataProvider.FillCommand(SysObj, TransFilterCmd);

            DsTrans.Tables[0].DefaultView.Sort = "ngay_ct asc, so_ct asc";
            DsTrans.Tables[1].DefaultView.Sort = "stt_rec0";

            //Dòng để giữ focus khi xoá hoặc huỷ
            DataRow dr = DsTrans.Tables[0].NewRow();
            dr["stt_rec"] = string.Empty;
            dr["ma_nt"] = M_ma_nt0;
            DsTrans.Tables[0].Rows.InsertAt(dr, 0);

            tbStatus = SysObj.GetPostInfo(Ma_ct);
            _Form.Title = M_Tilte;
            SmLib.SysFunc.LoadIcon(_Form);
            _Form.StartUpMain = this;
            _Form.ShowDialog();
        }
        #endregion

        
        #region GetRates
        public static decimal GetRates(string _ma_nt, DateTime _ngay)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("select [dbo].[GetRates](@ma_nt,@ngay_ct)");
                cmd.Parameters.Add("@ma_nt", SqlDbType.Char, 3).Value = _ma_nt;
                cmd.Parameters.Add("@ngay_ct", SqlDbType.VarChar, 8).Value = string.Format("{0:yyyyMMdd}", _ngay);
                object o_ty_gia = SysObj.ExcuteScalar(cmd);
                decimal _ty_gia = Convert.ToDecimal(o_ty_gia.Equals(DBNull.Value) ? 0 : o_ty_gia);
                return _ty_gia;
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
            return 0;
        }
        #endregion

        //#region UpdateRates
        //public static int UpdateRates(string _ma_nt, DateTime _ngay, decimal _ty_gia)
        //{
        //    try
        //    {
        //        SqlCommand cmd = new SqlCommand("exec [dbo].[SetRates] @ma_nt,@ngay_ct,@ty_gia, @user_id");
        //        cmd.Parameters.Add("@ma_nt", SqlDbType.Char, 3).Value = _ma_nt;
        //        cmd.Parameters.Add("@ngay_ct", SqlDbType.VarChar, 8).Value = string.Format("{0:yyyyMMdd}", _ngay);
        //        cmd.Parameters.Add("@ty_gia", SqlDbType.Decimal).Value = _ty_gia;
        //        cmd.Parameters.Add("@user_id", SqlDbType.Decimal).Value = M_User_Id;
        //        int _rno = StartUp.SysObj.ExcuteNonQuery(cmd);
        //        return _rno;
        //    }
        //    catch (Exception ex)
        //    {
        //        SmErrorLib.ErrorLog.CatchMessage(ex);
        //    }
        //    return 0;
        //}
        //#endregion

        #region DataFilter
        public static void DataFilter(string stt_rec)
        {
            //Refresh lai form
            StartUp.DsTrans.Tables[0].DefaultView.RowFilter = "stt_rec= '" + stt_rec + "'";
            StartUp.DsTrans.Tables[1].DefaultView.RowFilter = "stt_rec= '" + stt_rec + "'";
           
            StartUp.DsTrans.Tables[1].DefaultView.Sort = "stt_rec0";
           
        }
        #endregion

        #region GetLanguageString
        public static string GetLanguageString(string code, string language)
        {
            if (code == "M_MA_NT")
            {
                if (StartUp.DsTrans.Tables[0].Rows[FrmInctpxd.iRow]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0))
                    return "";
                else
                    return StartUp.DsTrans.Tables[0].Rows[FrmInctpxd.iRow]["ma_nt"].ToString();
            }
            if (code == "M_MA_NT0")
                return StartUp.M_ma_nt0;
            return code;
        }
        #endregion
        public static DataSet CheckData()
        {
            SqlCommand cmd = new SqlCommand("exec [dbo].[INCTPXD-CheckData] @status, @stt_rec");
            cmd.Parameters.Add("@status", SqlDbType.Char, 1).Value = StartUp.DsTrans.Tables[0].DefaultView[0]["status"];
            cmd.Parameters.Add("@stt_rec", SqlDbType.Char, 11).Value = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"];
            return StartUp.SysObj.ExcuteReader(cmd);
        }


        #region GetDmnt
        public static DataTable GetDmnt()
        {
            SqlCommand cmd = new SqlCommand("Select *,@ma_nt as ma_nt0, @Type as read_num_type from dmnt");
            cmd.Parameters.Add("@ma_nt", SqlDbType.Char, 3).Value = StartUp.M_ma_nt0;
            cmd.Parameters.Add("@Type", SqlDbType.Char, 1).Value = SysObj.GetOption("M_READ_NUM");


            DataTable tbNT = StartUp.SysObj.ExcuteReader(cmd).Tables[0].Copy();
            tbNT.TableName = "TableNTInfo";
            if (DsTrans.Tables.IndexOf("TableNTInfo") >= 0)
                DsTrans.Tables.Remove("TableNTInfo");
            return tbNT;
        }
        #endregion

        #region IsTkMe
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
                ErrorLog.CatchMessage(ex);
            }
            return result;
        }
        #endregion

        #region GetPN
        public static DataTable GetPN(string ma_vt, string ma_kho, object ngay_ct)
        {
            DataTable tbNT = new DataTable();
            try
            {
                SqlCommand cmd = new SqlCommand("Exec GetPN @ma_vt,@ma_kho, @ngay_ct");
                cmd.Parameters.Add("@ma_vt", SqlDbType.VarChar).Value = ma_vt;
                cmd.Parameters.Add("@ma_kho", SqlDbType.VarChar).Value = ma_kho;
                cmd.Parameters.Add("@ngay_ct", SqlDbType.DateTime).Value = ngay_ct;

                DataSet ds = StartUp.SysObj.ExcuteReader(cmd);
                if (ds != null || ds.Tables.Count > 0)
                {
                    tbNT = ds.Tables[0].Copy();
                }
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
            return tbNT;
        }
        #endregion

    }
}
