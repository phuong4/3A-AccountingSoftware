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
using Sm.Windows.Controls;
using System.Windows.Media.Imaging;
using SysLib;
using SmVoucherLib;
using SmErrorLib;
using System.Diagnostics;
using System.Threading;
using SmDefine;

namespace TTPoctpna
{
    public delegate void DLPost();

    public class StartUp : StartUpTrans
    {
        static public string M_User_name = string.Empty;
        static public SqlCommand TransFilterCmd;

        //static public DataTable DtDmnt;

        static public string titleWindow = string.Empty;
        static public string M_ma_nt = string.Empty;

        static public int M_so_lien = 0;
        static public int M_loc_nsd = 0;
        static public int M_IN_HOI_CK = 0;
        static public int M_MA_MS = 0;
    
        static public string M_CHK_TON_VT = string.Empty;
        static public int so_dong_in = 0;
        static public string stringBrowse1 = "";
        static public string stringBrowse2 = "";
        static public DataTable dtRegInfo;
        public static string M_CHK_DATE_YN = "";
        public static DateTime? M_NGAY_BAT_DAU;
        public static DateTime? M_NGAY_KET_THUC;

        //Dùng trong form tìm
        static public string tableList = "v_ph71;v_ct71;v_ct71gt";
        static public DateTime M_ngay_ct0;
        //static public int rowIndex = -1;

        //static public Stopwatch swMain = new Stopwatch();

        public override void Run()
        {
            //swMain.Start();
           Namespace = "TTPoctpna";
            try
            {
                Ma_ct = "PNA";
                filterId = "Poctpna";
                //Khoi tao cac tham so can thiet
                dtRegInfo = StartUp.SysObj.GetRegInfo();

                FrmPoctpna _Form = new FrmPoctpna();

                M_ma_nt0 = SysObj.GetOption("M_MA_NT0").ToString();
                M_User_Id = Convert.ToInt16( SysObj.UserInfo.Rows[0]["user_id"].ToString());
                M_User_name = SysObj.UserInfo.Rows[0]["user_name"].ToString().Trim();
                Ws_Id = SysObj.GetOption("M_WS_ID").ToString();
                M_ROUND = Convert.ToInt32(StartUp.SysObj.GetSysVar("M_ROUND"));
                M_ROUND_NT = Convert.ToInt32(StartUp.SysObj.GetSysVar("M_ROUND_NT"));
                M_ROUND_GIA = Convert.ToInt32(StartUp.SysObj.GetSysVar("M_ROUND_GIA"));
                M_ROUND_GIA_NT = Convert.ToInt32(StartUp.SysObj.GetSysVar("M_ROUND_GIA_NT"));
              
                M_MST_CHECK = SysObj.GetOption("M_MST_CHECK").ToString().Trim();
         
                M_IN_HOI_CK = Convert.ToInt16(StartUp.SysObj.GetOption("M_IN_HOI_CK").ToString());
                M_MA_MS = Convert.ToInt16(StartUp.SysObj.GetOption("M_MA_MS").ToString());
                M_CHK_HD_VAO = Convert.ToInt16(StartUp.SysObj.GetOption("M_CHK_HD_VAO").ToString());

                M_ngay_ct0 = Convert.ToDateTime(SysObj.GetSysVar("M_NGAY_KY1"));

                CommandInfo = SmLib.SysFunc.GetCommandInfo(SysObj, Menu_Id);
                DmctInfo = SmDataLib.DataLoader.GetSqlFieldValue(SysObj, "dmct", "ma_ct", Ma_ct);

                if (CommandInfo == null)
                {
                    ExMessageBox.Show( 555,StartUp.SysObj, "Chưa khai báo command hoặc command ngầm định sai!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                if (DmctInfo == null)
                {
                    ExMessageBox.Show( 560,StartUp.SysObj, "Chưa khai báo chứng từ hoặc chứng từ ngầm dịnh sai!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                

                string[] strBrowses = CommandInfo["Vbrowse1"].ToString().Split('|');
                if (M_LAN != "V")
                    strBrowses = CommandInfo["Ebrowse1"].ToString().Split('|');
                if (strBrowses != null)
                {
                    stringBrowse1 = strBrowses[0];
                    stringBrowse2 = strBrowses[1];
                }

                M_ngay_lct = DmctInfo["m_ngay_lct"].ToString();
                M_ong_ba = DmctInfo["m_ong_ba"].ToString();
      
                int.TryParse(DmctInfo["m_sl_ct0"].ToString(), out M_sl_ct0);
                int.TryParse(DmctInfo["so_lien"].ToString(), out M_so_lien);
                int.TryParse(DmctInfo["m_loc_nsd"].ToString(), out M_loc_nsd);
                int.TryParse(DmctInfo["so_dong_in"].ToString(), out so_dong_in);

                string conditionPH = " AND 1=1";
                if (!SmLib.SysFunc.CheckPermission(SysObj, ActionTask.View, Menu_Id))
                    conditionPH = " AND user_id0 = " + SysObj.UserInfo.Rows[0]["user_id"].ToString();

                M_CHK_DATE_YN = SysObj.GetOption("M_CHK_DATE_YN").ToString().Trim();
                if (Editing_Stt_Rec.Equals(string.Empty) && M_CHK_DATE_YN == "1")
                {
                    SmVoucherLib.SelectTime dlgSelTime = new SmVoucherLib.SelectTime();
                    dlgSelTime.LanguageID = "PoctpnaSelTime";

                    SmLib.SysFunc.LoadIcon(dlgSelTime);
                    dlgSelTime.ShowDialog();
                    if (dlgSelTime.IsOK)
                    {
                        M_NGAY_BAT_DAU = (DateTime)dlgSelTime.M_NGAY_CT1;
                        M_NGAY_KET_THUC = (DateTime)dlgSelTime.M_NGAY_CT2;
                        conditionPH += string.Format(" AND ngay_ct BETWEEN '{0:yyyyMMdd}' AND '{1:yyyyMMdd}'", M_NGAY_BAT_DAU, M_NGAY_KET_THUC);
                    }
                }

                TransFilterCmd = new SqlCommand("exec LoadVoucher @ma_ct, @PhFilter, @CtFilter, @GtFilter, @sl_ct");
                TransFilterCmd.Parameters.Add("@ma_ct", SqlDbType.Char).Value = Ma_ct;
                TransFilterCmd.Parameters.Add("@PhFilter", SqlDbType.NVarChar, 4000).Value = (Editing_Stt_Rec.Equals(string.Empty) ? "1=1" + " AND ma_dvcs = '" + SysObj.M_ma_dvcs.Trim() + "'" : "stt_rec = '" + Editing_Stt_Rec + "'") + conditionPH;
                TransFilterCmd.Parameters.Add("@CtFilter", SqlDbType.NVarChar, 4000).Value = "1=1";
                TransFilterCmd.Parameters.Add("@GtFilter", SqlDbType.NVarChar, 4000).Value = "";
                TransFilterCmd.Parameters.Add("@sl_ct", SqlDbType.Int).Value = M_sl_ct0;

                DsTrans = SmVoucherLib.DataProvider.FillCommand(SysObj, TransFilterCmd);
                DsTrans.Tables[0].DefaultView.Sort = "ngay_ct asc, so_ct asc";
                DsTrans.Tables[1].DefaultView.Sort = "stt_rec0 ASC";
                //Dòng để giữ focus khi xoá hoặc huỷ
                DataRow dr = DsTrans.Tables[0].NewRow();
                dr["stt_rec"] = string.Empty;
                dr["ma_nt"] = M_ma_nt0;
                DsTrans.Tables[0].Rows.InsertAt(dr, 0);

                tbStatus = SmVoucherLib.DataProvider.FillCommand(SysObj, new SqlCommand("Select * from dmPost where ma_ct like '%" + Ma_ct + "%'")).Tables[0];
                _Form.Title = SmLib.SysFunc.Cat_Dau(CommandInfo["bar"].ToString()).ToString();
                if(M_LAN != "V")
                    _Form.Title = SmLib.SysFunc.Cat_Dau(CommandInfo["bar2"].ToString()).ToString();

                SmLib.SysFunc.LoadIcon(_Form);
                _Form.StartUpMain = this;
                _Form.ShowInTaskbar = true;
                
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
                SqlCommand cmd = new SqlCommand("[dbo].[DeleteVoucher]");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@cMa_ct", SqlDbType.Char, 3).Value = Ma_ct;
                cmd.Parameters.Add("@stt_rec", SqlDbType.Char, 11).Value = _stt_rec;
                //cmd.Parameters.Add("@strList", SqlDbType.VarChar, 255).Value = StartUp.DmctInfo["m_phdbf"].ToString() + "," + StartUp.DmctInfo["m_ctdbf"].ToString() + "," + StartUp.DmctInfo["m_ctgtdbf"].ToString();
                StartUp.SysObj.ExcuteNonQuery(cmd);
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }
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
        public static int UpdateRates(string _ma_nt, DateTime _ngay, decimal _ty_gia)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("exec [dbo].[SetRates] @ma_nt,@ngay_ct,@ty_gia,@user_id");
                cmd.Parameters.Add("@ma_nt", SqlDbType.Char, 3).Value = _ma_nt;
                cmd.Parameters.Add("@ngay_ct", SqlDbType.VarChar, 8).Value = string.Format("{0:yyyyMMdd}", _ngay);
                cmd.Parameters.Add("@ty_gia", SqlDbType.Decimal).Value = _ty_gia;
                cmd.Parameters.Add("@user_id", SqlDbType.VarChar).Value = M_User_Id;
                int _rno = StartUp.SysObj.ExcuteNonQuery(cmd);
                return _rno;
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
            return 0;
        }

        #region DataFilter
        public static void DataFilter(string stt_rec)
        {
            //Refresh lai form
            StartUp.DsTrans.Tables[0].DefaultView.RowFilter = "stt_rec= '" + stt_rec + "'";
            StartUp.DsTrans.Tables[1].DefaultView.RowFilter = "stt_rec= '" + stt_rec + "'";
            StartUp.DsTrans.Tables[1].DefaultView.Sort = "stt_rec0";
            StartUp.DsTrans.Tables[2].DefaultView.RowFilter = "stt_rec= '" + stt_rec + "'";
        }
        #endregion 

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

        public static DataSet CheckData()
        {
            SqlCommand cmd = new SqlCommand("exec [dbo].[Poctpna-CheckData] @status, @stt_rec");
            cmd.Parameters.Add("@status", SqlDbType.Char, 1).Value = StartUp.DsTrans.Tables[0].DefaultView[0]["status"];
            cmd.Parameters.Add("@stt_rec", SqlDbType.Char, 11).Value = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"];
            return StartUp.SysObj.ExcuteReader(cmd);
        }

        
        //static void Post2()
        //{
        //    SqlCommand PostCmd = new SqlCommand("exec [TTPoctpna-Post] @stt_rec");
        //    PostCmd.Parameters.Add("@stt_rec", SqlDbType.VarChar).Value = StartUp.DsTrans.Tables[0].DefaultView[0]["stt_rec"].ToString();
        //        StartUp.SysObj.ExcuteNonQuery(PostCmd);
        //        _Form.txtso_du_kh.Value = ArapLib.ArFuncLib.GetSdkh13(StartUp.SysObj, StartUp.DsTrans.Tables[0].DefaultView[0]["ma_kh"].ToString(), StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nx"].ToString());
        //}

        public static DataSet GetHdm(string filter)
        {
            SqlCommand cmd = new SqlCommand("exec LoadVoucher @ma_ct, @PhFilter, @CtFilter, @GtFilter, @sl_ct");
            cmd.Parameters.Add("@ma_ct", SqlDbType.Char).Value = "HDM";
            cmd.Parameters.Add("@PhFilter", SqlDbType.NVarChar, 4000).Value = filter;
            cmd.Parameters.Add("@CtFilter", SqlDbType.NVarChar, 4000).Value = "1=1";
            cmd.Parameters.Add("@GtFilter", SqlDbType.NVarChar, 4000).Value = "";
            cmd.Parameters.Add("@sl_ct", SqlDbType.Int).Value = 0;

            return SmVoucherLib.DataProvider.FillCommand(SysObj, cmd);
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
