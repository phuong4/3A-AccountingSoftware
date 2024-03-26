using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Sm.Windows.Controls;
using System.Windows;
using SmDefine;

namespace ARCTHD1
{
    public class StartUp : SmVoucherLib.StartUpTrans
    {
        static public SqlCommand TransFilterCmd;
        static public string M_Tilte = string.Empty;
        static public string M_ma_nt = string.Empty;
        //static public string M_Ma_dvcs=string.Empty;
        static public string M_Ngay_lct;

        static public string M_MA_THUE;
        static public string M_PHONE;

        static public string M_BP_BH;
        static public int M_AR_CK = 0;
        static public bool IsQLHD;
        public static string M_CHK_DATE_YN = "";
        public static DateTime? M_NGAY_BAT_DAU;
        public static DateTime? M_NGAY_KET_THUC;


        #region Run
        public override void Run()
        {
           Namespace = "ARCTHD1";
            //Khoi tao cac tham so can thiet
            filterId = "ARCTHD1";
            Ma_ct = "HD1";
            M_ma_nt0 = SysObj.GetOption("M_MA_NT0").ToString();
            
            //M_Ma_dvcs = SysObj.GetOption("M_MA_DVCS").ToString();
            Ws_Id = SysObj.GetOption("M_WS_ID").ToString();
            M_LAN = SysObj.GetOption("M_LAN").ToString();
            M_IN_HOI_CK = Convert.ToInt16(StartUp.SysObj.GetOption("M_IN_HOI_CK").ToString());
            M_ROUND = Convert.ToInt16(SysObj.GetSysVar("M_ROUND"));
            M_ROUND_NT = Convert.ToInt16(SysObj.GetSysVar("M_ROUND_NT"));
            M_User_Id = Convert.ToInt16(SysObj.UserInfo.Rows[0]["user_id"].ToString());
            M_MST_CHECK = SysObj.GetOption("M_MST_CHECK").ToString().Trim();

            M_MA_THUE = SysObj.GetOption("M_MA_THUE").ToString();
            M_PHONE = SysObj.GetOption("M_PHONE").ToString();


            CommandInfo = SmLib.SysFunc.GetCommandInfo(SysObj, Menu_Id);

            DmctInfo = SmDataLib.DataLoader.GetSqlFieldValue(SysObj, "dmct", "ma_ct", Ma_ct);
     
            M_BP_BH = DmctInfo["m_bp_bh"].ToString();
            M_Ngay_lct = DmctInfo["m_ngay_lct"].ToString();
            M_ong_ba = DmctInfo["m_ong_ba"].ToString();

            int.TryParse(DmctInfo["m_sl_ct0"].ToString(), out M_sl_ct0);
            FrmArcthd1 _Form = new FrmArcthd1();
            filterView = string.Format("{0};{1}", StartUp.DmctInfo["v_phdbf"].ToString().Trim(), StartUp.DmctInfo["v_ctdbf"].ToString().Trim());
            if (CommandInfo == null)
            {
                ExMessageBox.Show( 515,SysObj, "Chưa khai báo command hoặc command ngầm định sai!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                App.Current.Shutdown();
                return;
            }
            if (DmctInfo == null)
            {
                ExMessageBox.Show( 520,SysObj, "Chưa khai báo chứng từ hoặc chứng từ ngầm định sai!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                App.Current.Shutdown();
                return;
            }

            M_Tilte = SmLib.SysFunc.Cat_Dau(M_LAN.Equals("V") ? CommandInfo["bar"].ToString() : CommandInfo["bar2"].ToString());

            string conditionPH = " AND 1=1";
            if (!SmLib.SysFunc.CheckPermission(SysObj, ActionTask.View, Menu_Id))
                conditionPH = " AND user_id0 = " + SysObj.UserInfo.Rows[0]["user_id"].ToString();
            M_CHK_DATE_YN = SysObj.GetOption("M_CHK_DATE_YN").ToString().Trim();
            if (Editing_Stt_Rec.Equals(string.Empty) && M_CHK_DATE_YN == "1")
            {
                SmVoucherLib.SelectTime dlgSelTime = new SmVoucherLib.SelectTime();
                dlgSelTime.LanguageID = "ARCTHD1SelTime";

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
            //DsTrans.Tables[0].DefaultView.Sort = "ngay_ct asc, so_ct asc";
            //DsTrans.Tables[1].DefaultView.Sort = "stt_rec0";
            

            //Dòng để giữ focus khi xoá hoặc huỷ
            DataRow dr = DsTrans.Tables[0].NewRow();
            dr["stt_rec"] = string.Empty;
            dr["ma_nt"] = M_ma_nt0;
            DsTrans.Tables[0].Rows.InsertAt(dr, 0);

            tbStatus = SysObj.GetPostInfo(Ma_ct);

            //Kiểm tra cho in liên tục hay không
            SqlCommand CheckQLHDCmd = new SqlCommand("SELECT COUNT(*) FROM v_dmmauhd WHERE ma_ct_qs LIKE @ma_ct");
            CheckQLHDCmd.Parameters.Add("@ma_ct", SqlDbType.Char).Value = "%" + Ma_ct + "%";
            IsQLHD = Convert.ToDecimal(SysObj.ExcuteScalar(CheckQLHDCmd)) != 0;

        
            _Form.Title = M_Tilte;
            SmLib.SysFunc.LoadIcon(_Form);
            _Form.StartUpMain = this;
            _Form.ShowDialog();
      
            App.Current.Shutdown();
  
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
                if (StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0))
                    return "";
                else
                    return StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString();
            }
            if (code == "M_MA_NT0")
                return StartUp.M_ma_nt0;
            return code;
        }
        #endregion

        #region GetDmnt
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
        #endregion

        #region CheckData
        public static DataSet CheckData(int mode)
        {
            SqlCommand cmd = new SqlCommand("exec [dbo].[ARCTHD1-CheckData] @status, @mode, @stt_rec");
            cmd.Parameters.Add("@status", SqlDbType.Char, 1).Value = StartUp.DsTrans.Tables[0].DefaultView[0]["status"];
            cmd.Parameters.Add("@mode", SqlDbType.Int).Value = mode;
            cmd.Parameters.Add("@stt_rec", SqlDbType.Char, 11).Value = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"];
            return StartUp.SysObj.ExcuteReader(cmd);
        }
        #endregion

        #region DeleteVoucher
        public static void DeleteVoucher(string _stt_rec, string ma_qs, ActionTask action, bool isNd51)
        {
            SqlCommand cmd = new SqlCommand("exec [dbo].[DeleteVoucher] @ma_ct, @stt_rec");
            cmd.Parameters.Add("@ma_ct", SqlDbType.Char, 3).Value = StartUp.Ma_ct;
            cmd.Parameters.Add("@stt_rec", SqlDbType.Char, 11).Value = _stt_rec;
            StartUp.SysObj.ExcuteNonQuery(cmd);

            if (isNd51)
            {
                cmd = new SqlCommand("exec [dbo].[ARCTHD1-PostCTHHD] @stt_rec;");
                if (action == ActionTask.Delete)
                {
                    cmd.CommandText += " DECLARE @ngay_ct smalldatetime;";
                    cmd.CommandText += " DECLARE @so_ct numeric(16, 0), @so_ct1 numeric(16, 0);";
                    cmd.CommandText += " SELECT @so_ct = so_ct - 1, @so_ct1 = so_ct1 - 1 FROM dmqs WHERE ma_qs='" + ma_qs.Trim() + "';";
                    cmd.CommandText += " IF @so_ct = @so_ct1";
                    cmd.CommandText += "    SELECT @ngay_ct = ngay_qs1 FROM dmqs WHERE ma_qs='" + ma_qs.Trim() + "';";
                    cmd.CommandText += " ELSE";
                    cmd.CommandText += "    SELECT TOP 1 @ngay_ct = ngay_ct FROM cthhd WHERE ma_qs='" + ma_qs.Trim() + "' AND so_ct IS NOT NULL ORDER BY ngay_ct DESC;";
                    cmd.CommandText += " UPDATE dmqs SET so_ct = @so_ct, ngay_ct = ISNULL(@ngay_ct,ngay_qs1) WHERE ma_qs='" + ma_qs.Trim() + "';";
                }
                cmd.Parameters.Add("@stt_rec", SqlDbType.Char, 11).Value = _stt_rec;
                StartUp.SysObj.ExcuteNonQuery(cmd);
            }
        }
        #endregion

        public static void DeletePT(string _stt_rec, string ma_ct)
        {
            SqlCommand cmd = new SqlCommand("exec [dbo].[DeleteVoucher] @ma_ct, @stt_rec");
            cmd.Parameters.Add("@ma_ct", SqlDbType.Char, 3).Value = ma_ct;
            cmd.Parameters.Add("@stt_rec", SqlDbType.Char, 11).Value = _stt_rec;
            StartUp.SysObj.ExcuteNonQuery(cmd);
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
    }
}
