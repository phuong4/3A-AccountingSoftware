using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Sm.Windows.Controls;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Windows;
using SmLib.SM.FormBrowse;
using System.Threading;


namespace AA_SMIMEXCT
{
    class C_ImportVoucher
    {

        public static string strException = "";


        //private const int RF_PROCESSMESSAGE = 0xA123;
        //private const int RF_PROCESSWAITINGSHOW = 0xA126;
        //private const int RF_PROCESSWAITING = 0xA125;

        //[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        //public static extern int SendMessage(IntPtr hwnd, [MarshalAs(UnmanagedType.U4)] int Msg, IntPtr wParam, IntPtr lParam);


     //   public static FrmWaiting waiting;
        public static DataTable tbStt_rec = new DataTable();
        public static DataTable tbDuLieuTrung = new DataTable();
        public static DataTable tb_Post_Error = new DataTable();
        public static DataTable tb_Post_Ok = new DataTable();

        public static bool _flag_post = true;
        public static bool UploadTable(string tableName, DataSet db, string strbrowse)
        {
            strException = "";
            //xoa du lieu cu
            DeleteTableOld(tableName);

            DataTable tbError = db.Tables["DataExcel"].Clone();
            tbError.Columns.Add("ly_do");
            foreach (DataRow row in db.Tables["DataExcel"].Rows)
            {
                SqlConnection con = new SqlConnection(GetConnectionString(10000));
                con.Open();
                SqlCommand cmd;
                try
                {
                    cmd = GetUploadCommand(tableName, row);
                    cmd.Connection = con;
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException se)
                {
                    strException += (se.Message + "\n");
                    DataRow r = tbError.NewRow();
                    r.ItemArray = row.ItemArray;
                    r["ly_do"] = se.Message;
                    tbError.Rows.Add(r);
                }
                catch (Exception ex)
                {
                    SmErrorLib.ErrorLog.CatchMessage(ex);
                }
                finally
                {
                    con.Close();
                }
            }
            if (tbError.Rows.Count > 0)
            {
                //xoa nhung dong da insert
                DeleteTableOld(tableName);
                if (StartUp.waiting != null)
                    StartUp.waiting.Close();
                //SendMessage(StartUp.SysObj.HandleWaiting, RF_PROCESSWAITING, IntPtr.Zero, new IntPtr((int)'1'));
                FormBrowse br = new FormBrowse(StartUp.SysObj, tbError.DefaultView, strbrowse + ";ly_do:320:H=Lý do lỗi");
                br.frmBrw.Title = StartUp.M_LAN.Equals("V") ? "So lieu loi" : "The data errors";
                br.frmBrw.LanguageID = "AA_SMIMEXCT_5";
                br.ShowDialog();
                
                return false;
            }

            return true;
        }

       

        private static SqlCommand GetUploadCommand(string tableName, DataRow row)
        {
            string parameters;
            parameters = "";
            SqlCommand cmd = new SqlCommand();
            for(int i = 0; i < row.Table.Columns.Count; i++)
            {
                string columnName = row.Table.Columns[i].ColumnName.Trim();
                if (parameters == "")
                    parameters = string.Format("@{0}", columnName);
                else
                    parameters += string.Format(",@{0}", columnName);

                cmd.Parameters.Add(new SqlParameter(string.Format("@{0}", columnName), row[columnName]));
                cmd.CommandText = string.Format("INSERT INTO {0} ({1}) VALUES ({2})", tableName, parameters.Replace("@", ""), parameters);
            }

            return cmd;
        }

        private static void DeleteTableOld(string tableName)
        {
            SqlConnection con = new SqlConnection(GetConnectionString(10000));
            con.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = string.Format("Delete from {0}", tableName);
            cmd.Connection = con;
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public static string GetConnectionString(int _time)
        {
            string conStr = "";
            DataSet fastDb = new DataSet();
            try
            {
                conStr = StartUp.SysObj.M_ConnectString;
                if (!conStr.ToUpper().Contains("TIMEOUT"))
                    conStr += ";Connect Timeout = " + _time;

            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
                return "";
            }
            return conStr;
        }


        public static bool Check_Data(ImportInfo info)
        {
            bool _flag = false;
            try
            {
                SqlConnection con = new SqlConnection(GetConnectionString(500000));
                SqlCommand cmd1 = new SqlCommand();
                cmd1.CommandText = string.Format("EXEC {0} {1}, '{2}', '{3}', '{4}'", (info.PostProc.Split(','))[0].ToString(), StartUp._User_id, info.Ma_qs, StartupBase.SysObj.M_ma_dvcs.Trim(), info.Xy_ly.Trim());
                cmd1.Connection = con;
                cmd1.CommandTimeout = 600000;
                SqlDataAdapter ad = new SqlDataAdapter(cmd1);
                DataSet dsError = new DataSet();
                ad.Fill(dsError);
                con.Close();
                _flag = ShowError(dsError, info.Ma_Imex);
                if (_flag)
                {
                    tbStt_rec = dsError.Tables[dsError.Tables.Count - 1].Copy();
                    tbDuLieuTrung = dsError.Tables[dsError.Tables.Count - 2].Copy();
                }
            }

            catch (Exception ex)
            {
                if (StartUp.waiting != null)
                    StartUp.waiting.Close();
                ExMessageBox.Show(873, StartUp.SysObj, ex.Message, "", MessageBoxButton.OK, MessageBoxImage.Information);
                _flag = false;
            }
           
            return _flag;
        }


        public static bool Post(ImportInfo info)
        {

            tb_Post_Error = tbStt_rec.Clone();
            tb_Post_Error.Columns.Add("ly_do_loi", typeof(string));

            int n_so_ct = tbStt_rec.Rows.Count;
            StartUp.waiting.pgValue = n_so_ct;
            StartUp.waiting.Set(0);
            //  DataTable tb_Post_Ok = tbStt_rec.Clone();
            bool _flag = true;
            SqlConnection con = new SqlConnection(GetConnectionString(500000));
            SqlCommand cmd1 = new SqlCommand();
            cmd1.CommandTimeout = 600000;
            cmd1.Connection = con;
            con.Open();
            int dem = 0;
            foreach (DataRow temp in tbStt_rec.Rows)
            {
                StartUp.waiting.Set(dem);
                try
                {
                    cmd1.CommandText = string.Format("EXEC {0} '{1}'", (info.PostProc.Split(','))[1].ToString(), temp["stt_rec"].ToString().Trim());
                    cmd1.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    //Lưu lại stt_rec bị lỗi
                    DataRow r = tb_Post_Error.NewRow();
                    r.ItemArray = temp.ItemArray;
                    r["ly_do_loi"] = ex.Message;
                    tb_Post_Error.Rows.Add(r);
                    _flag = false;
                }
                finally
                {
                    dem++;
                    StartUp.waiting.Set(dem);
                }
            }

            StartUp.waiting.Set(StartUp.waiting.pgValue); // hoan tat post

            if (_flag == false) // Post bị lỗi: undo dữ liêu
            {

                int n_value = n_so_ct;
                //undo : xóa tất cả những phiếu đã post
                foreach (DataRow temp in tbStt_rec.Rows)
                {
                    cmd1.CommandText = string.Format("EXEC [dbo].[DeleteVoucher] '{0}', '{1}'", info.Ma_ct.Trim(), temp["stt_rec"].ToString().Trim());
                    cmd1.ExecuteNonQuery();
                    StartUp.waiting.Set_Post_Error(n_value--);
                }
                if (StartUp.waiting != null)
                    StartUp.waiting.Close();
                //các chứng từ bị lỗi
                FormBrowse br = new FormBrowse(StartUp.SysObj, tb_Post_Error.DefaultView, "ngay_ct:H=Ngày c.từ:130;ma_qs:H=Mã quyển sổ:130;so_ct:H=Số c.từ:130;ly_do_loi:H=Lý do lỗi;stt_rec:0:H=stt_rec");
                br.frmBrw.Title = StartUp.M_LAN.Equals("V") ? "Các chứng từ bị lỗi khi post" : "The post data errors";
                br.frmBrw.LanguageID = "AA_SMIMEXCT_15";
                br.ShowDialog();
            }
            else if (info.Xy_ly.Trim() == "1") //post thành công thì xóa các chứng từ trùng
            {
                int dem_xoa = 0;
                StartUp.waiting.pgValue = tbDuLieuTrung.Rows.Count;
                foreach (DataRow temp in tbDuLieuTrung.Rows)
                {
                    cmd1.CommandText = string.Format("EXEC [dbo].[DeleteVoucher] '{0}', '{1}'", info.Ma_ct.Trim(), temp["stt_rec"].ToString().Trim());
                    cmd1.ExecuteNonQuery();
                    StartUp.waiting.Set_Delete(dem_xoa++);
                }
                StartUp.waiting.Set_Delete(StartUp.waiting.pgValue);
            }
            con.Close();
            return _flag;
        }
        
        public static bool Post_All(ImportInfo info)
        {
            int dem = 0;
            int n_so_ct = tbStt_rec.Rows.Count;
            StartUp.waiting.pgValue = n_so_ct;
            StartUp.waiting.Set(0);
            //waiting = new FrmWaiting(tbStt_rec.Rows.Count);         
            //waiting.Show();
            
            int _numberThread_0 = 10; //số tiền trình post
            
            tb_Post_Ok = tbStt_rec.Clone();
            tb_Post_Error = tbStt_rec.Clone();
            tb_Post_Error.Columns.Add("ly_do_loi", typeof(string));
          
            bool _flag_post = true;

            int n = n_so_ct / _numberThread_0;
            int du = n_so_ct % _numberThread_0;
        
            ManualResetEvent[] resetEvents;
            for (int j = 0; j <= n ; j++)
            {
              
                int numberThread_Tmp = _numberThread_0;
               
                if(j == n && du == 0) //đã post hết
                    break;
                if(j == n && du != 0) //post phần dư
                    numberThread_Tmp = du;

                resetEvents = new ManualResetEvent[numberThread_Tmp];

                for (int k = 0; k < numberThread_Tmp; k++)
                {
                    int vi_tri_stt_rec = (j * _numberThread_0) + k;
                   
                    resetEvents[k] = new ManualResetEvent(false);

                    ThreadPool.QueueUserWorkItem((data) =>
                    {
                        int index_stt_rec =  int.Parse(((string)data).Split(';')[0].Trim());
                        int index = int.Parse(((string)data).Split(';')[1].Trim());

                        SqlConnection connection = new SqlConnection(GetConnectionString(50000));
                        connection.Open();
                        try
                        {
                    
                            string stt_rec = tbStt_rec.Rows[index_stt_rec]["stt_rec"].ToString();
                            SqlCommand Cmd_Post = new SqlCommand();
                            Cmd_Post.Connection = connection;
                            Cmd_Post.CommandText = string.Format("EXEC {0} '{1}'", info.PostProc.Split(',')[1].Trim(), stt_rec);
                          
                            Cmd_Post.ExecuteNonQuery();

                            DataRow r = tb_Post_Ok.NewRow();
                            r.ItemArray = tbStt_rec.Rows[index_stt_rec].ItemArray;
                            tb_Post_Ok.Rows.Add(r);
                        }
                        catch (Exception ex)
                        {
                            //Lưu lại stt_rec bị lỗi
                            DataRow r = tb_Post_Error.NewRow();
                            r.ItemArray = tbStt_rec.Rows[index_stt_rec].ItemArray;
                            r["ly_do_loi"] = ex.Message;
                            tb_Post_Error.Rows.Add(r);
                            _flag_post = false;
                        }
                        finally
                        {
                           
                            connection.Close();
                            dem++;
                            resetEvents[index].Set();
                        }

                    }, vi_tri_stt_rec.ToString() + ";" + k.ToString());
                }

                foreach (var e in resetEvents)
                { 
                   
                    e.WaitOne();
                    StartUp.waiting.Set(dem);
                }
            }

            StartUp.waiting.Set(StartUp.waiting.pgValue); //hoàn tất post

            if (_flag_post == false) // Post bị lỗi thì undo dữ liệu
            {
                SqlConnection con = new SqlConnection(GetConnectionString(500000));
                SqlCommand cmd_undo = new SqlCommand();
                cmd_undo.Connection = con;
                con.Open();
                int n_value = n_so_ct;
                //undo : xóa tất cả những phiếu đã post
                foreach (DataRow temp in tbStt_rec.Rows)
                {
                    n_value--;
                    cmd_undo.CommandText = string.Format("EXEC [dbo].[DeleteVoucher] '{0}', '{1}'", info.Ma_ct.Trim(), temp["stt_rec"].ToString().Trim());
                    cmd_undo.ExecuteNonQuery();
                    StartUp.waiting.Set_Post_Error(n_value);
                }
                con.Close();
                //các chứng từ bị lỗi
                if (StartUp.waiting != null)
                    StartUp.waiting.Close();
                FormBrowse br = new FormBrowse(StartUp.SysObj, tb_Post_Error.DefaultView, "ma_qs:H=Mã quyển sổ:130;so_ct:H=Số c.từ:130;ly_do_loi:H=Lý do lỗi:400");
                br.frmBrw.Title = StartUp.M_LAN.Equals("V") ? "Các chứng từ bị lỗi khi post" : "The post data errors";
                br.frmBrw.LanguageID = "AA_SMIMEXCT_15";
                br.ShowDialog();
            }
            else if (info.Xy_ly.Trim() == "1") //post thành công thì xóa các chứng từ trùng
            {

                StartUp.waiting.pgValue = tbDuLieuTrung.Rows.Count;
                SqlConnection con2 = new SqlConnection(GetConnectionString(500000));
                SqlCommand cmd_delete = new SqlCommand();
                cmd_delete.Connection = con2;
                con2.Open();
                int dem_xoa = 0;
                foreach (DataRow temp in tbDuLieuTrung.Rows)
                {
                   
                    dem_xoa++;
                    cmd_delete.CommandText = string.Format("EXEC [dbo].[DeleteVoucher] '{0}', '{1}'", info.Ma_ct.Trim(), temp["stt_rec"].ToString().Trim());
                    cmd_delete.ExecuteNonQuery();
                    StartUp.waiting.Set_Delete(dem_xoa);
                    //StartUp.SysObj.ExcuteNonQuery(cmd);
                }
                con2.Close();
                StartUp.waiting.Set_Delete(StartUp.waiting.pgValue);
            }
            return _flag_post;
        }



        private static bool ShowError(DataSet dsError, string maImex)
        {
            switch (maImex.Trim())
            {
                case "PK1":
                    return ShowError_PK1(dsError);
                case "HDA":
                    return ShowError_HDA(dsError);
                case "PNF":
                    return ShowError_PNF(dsError);
                case "PND":
                    return ShowError_PND(dsError);
                case "PXD":
                    return ShowError_PXD(dsError);
                case "PNA":
                    return ShowError_PNA(dsError);
                case "PXE":
                    return ShowError_PXE(dsError);
                case "PNG":
                    return ShowError_PNG(dsError);
                case "PC1":
                    return ShowError_PC1(dsError);
                case "PT1":
                    return ShowError_PT1(dsError);
                case "PNB":
                    return ShowError_PNB(dsError);
                case "BN1":
                    return ShowError_BN1(dsError);
                case "BC1":
                    return ShowError_BC1(dsError);
                case "HD1":
                    return ShowError_HD1(dsError);
                case "HD4":
                    return ShowError_HD4(dsError);
                case "PN1":
                    return ShowError_PN1(dsError);
            }

            return false;
        }

        private static bool ShowError_PK1(DataSet dsError)
        {

            bool result = true;
            foreach (DataTable tbl in dsError.Tables)
            {
                if (tbl.Rows.Count == 0)
                    continue;
                switch (tbl.Columns[0].ColumnName)
                {
                    case "ma_kh":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_kh:H=Mã khách hàng", "Danh sách mã khách hàng không có trong danh mục khách hàng");
                        else
                            BrowseError(tbl, "ma_kh:H=Mã khách hàng", "Danh sách mã khách hàng không có trong danh mục khách hàng e");

                        result = false;
                        break;

                    case "ma_qs":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_qs:H=Quyển sổ", "Danh sách quyển sổ không có trong danh mục quyển sổ hoặc chứng từ không thuộc quyển sổ này");
                        else
                            BrowseError(tbl, "ma_qs:H=Quyển sổ e", "Danh sách quyển sổ không có trong danh mục quyển sổ hoặc chứng từ không thuộc quyển sổ này e");
                        result = false;
                        break;
                    case "so_ct":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_qs:H=Quyển sổ;so_ct:H=Số chứng từ", "Danh sách số chứng từ trùng số");
                        else
                            BrowseError(tbl, "ma_qs:H=Quyển sổ e;so_ct:H=Số chứng từ e", "Danh sách số chứng từ trùng số e");
                        result = false;
                        break;


                    case "tk_i":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "tk_i:H=Tài khoản;ten_tk:H=Tên tài khoản", "Danh sách tài khoản không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");
                        else
                            BrowseError(tbl, "tk_i:H=Tài khoản e;ten_tk2:H=Tên tài khoản e", "Danh sách tài khoản không có trong danh mục tài khoản hoặc là tài khoản tổng hợp e");

                        result = false;
                        break;

                    case "ma_dvcs":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_dvcs:H=Mã ĐVCS", "Danh sách mã ĐVCS không có trong danh mục ĐVCS");
                        else
                            BrowseError(tbl, "ma_dvcs:H=Mã ĐVCS", "Danh sách mã ĐVCS không có trong danh mục ĐVCS e");
                        result = false;
                        break;
                    case "ngay_ct":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ngay_ct:H=Ngày chứng từ:D", "Danh sách ngày chứng từ nhỏ hơn ngày mở sổ");
                        else
                            BrowseError(tbl, "ngay_ct:H=Ngày chứng từ:D", "Danh sách ngày chứng từ nhỏ hơn ngày mở sổ");

                        result = false;
                        break;

                    case "ma_vv_i":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_vv_i:H=Mã dự án", "Danh sách mã dự án không có trong danh mục dự án");
                        else
                            BrowseError(tbl, "ma_vv_i:H=Mã dự án", "Danh sách mã dự án không có trong danh mục dự án");

                        result = false;
                        break;
                    case "nh_dk":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ngay_ct:H=Ngày c.từ:D;so_ct:H=Số c.từ;ma_qs:H=Mã quyển sổ;nh_dk:H=Nhóm định khoản;t_ps_no:H=Tổng ps nợ:N0;t_ps_co:H=Tổng ps có:N0", "Tổng phát sinh nợ khác tổng phát sinh có trong 1 nhóm định khoản");
                        else
                            BrowseError(tbl, "ngay_ct:H=Ngày c.từ:D;so_ct:H=Số c.từ;ma_qs:H=Mã quyển sổ;nh_dk:H=Nhóm định khoản;t_ps_no:H=Tổng ps nợ:N0;t_ps_co:H=Tổng ps có:N0", "Tổng phát sinh nợ khác tổng phát sinh có trong 1 nhóm định khoản");

                        result = false;
                        break;
                    case "tk_cn":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "tk_cn:H=Tài khoản;ma_kh_i:H=Mã khách hàng", "Danh sách tài khoản công nợ chưa vào mã khách hàng");
                        else
                            BrowseError(tbl, "tk_cn:H=Tài khoản;ma_kh_i:H=Mã khách hàng", "Danh sách tài khoản công nợ chưa vào mã khách hàng");

                        result = false;
                        break;

                    case "so_ct_khac_ngay":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "so_ct_khac_ngay:H=Số c.từ;ma_qs:H=Mã quyển sổ", "Danh sách c.từ có ngày c.từ khác nhau");
                        else
                            BrowseError(tbl, "so_ct_khac_ngay:H=Số c.từ;ma_qs:H=Mã quyển sổ", "Danh sách c.từ có ngày c.từ khác nhau");

                        result = false;
                        break;
                    case "ma_phi":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_phi:H=Mã phí", "Danh sách mã phí không có trong danh mục khoản mục phí");
                        else
                            BrowseError(tbl, "ma_phi:H=Mã phí", "Danh sách mã phí không có trong danh mục khoản mục phí");

                        result = false;
                        break;
                }
            }
            return result;
        }

        private static bool ShowError_HDA(DataSet dsError)
        {
            bool result = true;
            foreach (DataTable tbl in dsError.Tables)
            {
                if (tbl.Rows.Count == 0)
                    continue;
                switch (tbl.Columns[0].ColumnName)
                {
                    case "ma_kh":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_kh:H=Mã khách hàng", "Danh sách mã khách hàng không có trong danh mục khách hàng");
                        else
                            BrowseError(tbl, "ma_kh:H=Mã khách hàng", "Danh sách mã khách hàng không có trong danh mục khách hàng e");

                        result = false;
                        break;
                    case "ma_kho":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_kho:H=Mã kho;ma_dvcs:H=Mã ĐVCS", "Danh sách mã kho không có trong danh mục kho hoặc không thuộc mã ĐVCS của chứng từ");
                        else
                            BrowseError(tbl, "ma_kho:H=Mã kho;ma_dvcs:H=Mã ĐVCS", "Danh sách mã kho không có trong danh mục kho hoặc không thuộc mã ĐVCS của chứng từ");

                        result = false;
                        break;
                    case "ma_vt":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_vt:H=Mã vật tư", "Danh sách mã vật tư không có trong danh mục vật tư");
                        else
                            BrowseError(tbl, "ma_vt:H=Mã vật tư e", "Danh sách mã vật tư không có trong danh mục vật tư e");

                        result = false;
                        break;
                    case "ma_nx":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_nx:H=Mã nhập xuất (Tk nợ);ten_tk:H=Tên nhập xuất", "Danh sách mã nhập xuất (Tk nợ) không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");
                        else
                            BrowseError(tbl, "ma_nx:H=Mã nhập xuất (Tk nợ);ten_tk:H=Tên nhập xuất", "Danh sách mã nhập xuất (Tk nợ) không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");

                        result = false;
                        break;
                    case "ma_qs":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_qs:H=Quyển sổ", "Danh sách quyển sổ không có trong danh mục quyển sổ hoặc chứng từ không thuộc quyển sổ này");
                        else
                            BrowseError(tbl, "ma_qs:H=Quyển sổ e", "Danh sách quyển sổ không có trong danh mục quyển sổ hoặc chứng từ không thuộc quyển sổ này e");
                        result = false;
                        break;
                    case "so_ct":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_qs:H=Quyển sổ;so_ct:H=Số chứng từ", "Danh sách số chứng từ trùng số");
                        else
                            BrowseError(tbl, "ma_qs:H=Quyển sổ e;so_ct:H=Số chứng từ e", "Danh sách số chứng từ trùng số e");
                        result = false;
                        break;
                    case "tk_dt":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "tk_dt:H=Tk doanh thu;ten_tk:H=Tên tài khoản", "Danh sách tài khoản doanh thu không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");
                        else
                            BrowseError(tbl, "tk_dt:H=Tk doanh thu;ten_tk:H=Tên tài khoản", "Danh sách tài khoản doanh thu không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");

                        result = false;
                        break;
                    case "tk_vt":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "tk_vt:H=Tk vật tư;ten_tk:H=Tên tài khoản", "Danh sách tài khoản vật tư không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");
                        else
                            BrowseError(tbl, "tk_vt:H=Tk vật tư;ten_tk:H=Tên tài khoản", "Danh sách tài khoản vật tư không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");

                        result = false;
                        break;
                    case "tk_gv":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "tk_gv:H=Tk giá vốn;ten_tk:H=Tên tài khoản", "Danh sách tài khoản giá vốn không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");
                        else
                            BrowseError(tbl, "tk_gv:H=Tk giá vốn;ten_tk:H=Tên tài khoản", "Danh sách tài khoản giá vốn không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");

                        result = false;
                        break;
                    case "tk_ck":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "tk_ck:H=Tk chiết khấu;ten_tk:H=Tên tài khoản", "Danh sách tài khoản chiết khấu không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");
                        else
                            BrowseError(tbl, "tk_ck:H=Tk chiết khấu;ten_tk:H=Tên tài khoản", "Danh sách tài khoản chiết khấu không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");

                        result = false;
                        break;

                    case "ma_gd":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_gd:H=Mã giao dịch", "Danh sách mã giao dịch không hợp lệ");
                        else
                            BrowseError(tbl, "ma_gd:H=Mã giao dịch e", "Danh sách mã giao dịch không hợp lệ e");

                        result = false;
                        break;

                    case "ma_dvcs":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_dvcs:H=Mã ĐVCS", "Danh sách mã ĐVCS không có trong danh mục ĐVCS");
                        else
                            BrowseError(tbl, "ma_dvcs:H=Mã ĐVCS", "Danh sách mã ĐVCS không có trong danh mục ĐVCS e");

                        result = false;
                        break;
                    case "ngay_ct":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ngay_ct:H=Ngày chứng từ:D", "Danh sách ngày chứng từ nhỏ hơn ngày mở sổ");
                        else
                            BrowseError(tbl, "ngay_ct:H=Ngày chứng từ:D", "Danh sách ngày chứng từ nhỏ hơn ngày mở sổ");

                        result = false;
                        break;

                    case "tk_thue_co":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "tk_thue_co:H=Tài khoản thuế;ten_tk:H=Tên tài khoản", "Danh sách tài khoản thuế không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");
                        else
                            BrowseError(tbl, "tk_thue_co:H=Tài khoản thuế;ten_tk:H=Tên tài khoản", "Danh sách tài khoản thuế không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");

                        result = false;
                        break;
                    case "tk_km_i":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "tk_km_i:H=Tài khoản cp km;ten_tk:H=Tên tài khoản", "Danh sách tài khoản cp km không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");
                        else
                            BrowseError(tbl, "tk_km_i:H=Tài khoản cp km;ten_tk:H=Tên tài khoản", "Danh sách tài khoản cp km không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");

                        result = false;
                        break;

                    case "ma_vv_i":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_vv_i:H=Mã dự án", "Danh sách mã dự án không có trong danh mục dự án");
                        else
                            BrowseError(tbl, "ma_vv_i:H=Mã dự án", "Danh sách mã dự án không có trong danh mục dự án");

                        result = false;
                        break;
                    case "ma_bp":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_bp:H=Mã NVBH", "Danh sách mã NVBH không có trong danh mục bộ phận bán hàng");
                        else
                            BrowseError(tbl, "ma_bp:H=Mã NVBH", "Danh sách mã NVBH không có trong danh mục bộ phận bán hàng");

                        result = false;
                        break;
                    case "so_ct_khac_ngay":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "so_ct_khac_ngay:H=Số c.từ;ma_qs:H=Mã quyển sổ", "Danh sách c.từ có ngày c.từ khác nhau");
                        else
                            BrowseError(tbl, "so_ct_khac_ngay:H=Số c.từ;ma_qs:H=Mã quyển sổ", "Danh sách c.từ có ngày c.từ khác nhau");

                        result = false;
                        break;

                }
            }
            return result;
        }
  

        private static bool ShowError_PNF(DataSet dsError)
        {
            bool result = true;
            foreach (DataTable tbl in dsError.Tables)
            {
                if (tbl.Rows.Count == 0)
                    continue;
                switch (tbl.Columns[0].ColumnName)
                {
                    case "ma_kh":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_kh:H=Mã khách hàng", "Danh sách mã khách hàng không có trong danh mục khách hàng");
                        else
                            BrowseError(tbl, "ma_kh:H=Mã khách hàng", "Danh sách mã khách hàng không có trong danh mục khách hàng e");

                        result = false;
                        break;
                    case "ma_kho":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_kho:H=Mã kho;ma_dvcs:H=Mã ĐVCS", "Danh sách mã kho không có trong danh mục kho hoặc không thuộc mã ĐVCS của chứng từ");
                        else
                            BrowseError(tbl, "ma_kho:H=Mã kho;ma_dvcs:H=Mã ĐVCS", "Danh sách mã kho không có trong danh mục kho hoặc không thuộc mã ĐVCS của chứng từ");

                        result = false;
                        break;
                    case "ma_vt":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_vt:H=Mã vật tư", "Danh sách mã vật tư không có trong danh mục vật tư");
                        else
                            BrowseError(tbl, "ma_vt:H=Mã vật tư e", "Danh sách mã vật tư không có trong danh mục vật tư e");

                        result = false;
                        break;
                    case "ma_nx":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_nx:H=Mã nhập xuất (Tk nợ);ten_tk:H=Tên nhập xuất", "Danh sách mã nhập xuất (Tk nợ) không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");
                        else
                            BrowseError(tbl, "ma_nx:H=Mã nhập xuất (Tk nợ);ten_tk:H=Tên nhập xuất", "Danh sách mã nhập xuất (Tk nợ) không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");

                        result = false;
                        break;
                    case "ma_qs":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_qs:H=Quyển sổ", "Danh sách quyển sổ không có trong danh mục quyển sổ hoặc chứng từ không thuộc quyển sổ này");
                        else
                            BrowseError(tbl, "ma_qs:H=Quyển sổ e", "Danh sách quyển sổ không có trong danh mục quyển sổ hoặc chứng từ không thuộc quyển sổ này e");
                        result = false;
                        break;
                    case "so_ct":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_qs:H=Quyển sổ;so_ct:H=Số chứng từ", "Danh sách số chứng từ trùng số");
                        else
                            BrowseError(tbl, "ma_qs:H=Quyển sổ e;so_ct:H=Số chứng từ e", "Danh sách số chứng từ trùng số e");
                        result = false;
                        break;
                    case "tk_tl":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "tk_tl:H=Tk trả lại;ten_tk:H=Tên tài khoản", "Danh sách tài khoản trả lại không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");
                        else
                            BrowseError(tbl, "tk_tl:H=Tk trả lại;ten_tk:H=Tên tài khoản", "Danh sách tài khoản trả lại không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");

                        result = false;
                        break;
                    case "tk_vt":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "tk_vt:H=Tk vật tư;ten_tk:H=Tên tài khoản", "Danh sách tài khoản vật tư không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");
                        else
                            BrowseError(tbl, "tk_vt:H=Tk vật tư;ten_tk:H=Tên tài khoản", "Danh sách tài khoản vật tư không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");

                        result = false;
                        break;
                    case "tk_gv":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "tk_gv:H=Tk giá vốn;ten_tk:H=Tên tài khoản", "Danh sách tài khoản giá vốn không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");
                        else
                            BrowseError(tbl, "tk_gv:H=Tk giá vốn;ten_tk:H=Tên tài khoản", "Danh sách tài khoản giá vốn không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");

                        result = false;
                        break;
                    case "tk_ck":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "tk_ck:H=Tk chiết khấu;ten_tk:H=Tên tài khoản", "Danh sách tài khoản chiết khấu không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");
                        else
                            BrowseError(tbl, "tk_ck:H=Tk chiết khấu;ten_tk:H=Tên tài khoản", "Danh sách tài khoản chiết khấu không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");

                        result = false;
                        break;

                    //case "ma_gd":
                    //    if (StartUp.M_LAN == "V")
                    //        BrowseError(tbl, "ma_gd:H=Mã giao dịch", "Danh sách mã giao dịch không hợp lệ");
                    //    else
                    //        BrowseError(tbl, "ma_gd:H=Mã giao dịch e", "Danh sách mã giao dịch không hợp lệ e");

                    //    result = false;
                    //    break;

                    case "ma_dvcs":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_dvcs:H=Mã ĐVCS", "Danh sách mã ĐVCS không có trong danh mục ĐVCS");
                        else
                            BrowseError(tbl, "ma_dvcs:H=Mã ĐVCS", "Danh sách mã ĐVCS không có trong danh mục ĐVCS e");

                        result = false;
                        break;
                    case "ngay_ct":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ngay_ct:H=Ngày chứng từ:D", "Danh sách ngày chứng từ nhỏ hơn ngày mở sổ");
                        else
                            BrowseError(tbl, "ngay_ct:H=Ngày chứng từ:D", "Danh sách ngày chứng từ nhỏ hơn ngày mở sổ");

                        result = false;
                        break;

                    //case "tk_thue_no":
                    //    if (StartUp.M_LAN == "V")
                    //        BrowseError(tbl, "tk_thue_no:H=Tài khoản thuế;ten_tk:H=Tên tài khoản", "Danh sách tài khoản thuế không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");
                    //    else
                    //        BrowseError(tbl, "tk_thue_no:H=Tài khoản thuế;ten_tk:H=Tên tài khoản", "Danh sách tài khoản thuế không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");

                    //    result = false;
                    //    break;
                    case "tk_km_i":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "tk_km_i:H=Tài khoản cp km;ten_tk:H=Tên tài khoản", "Danh sách tài khoản cp km không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");
                        else
                            BrowseError(tbl, "tk_km_i:H=Tài khoản cp km;ten_tk:H=Tên tài khoản", "Danh sách tài khoản cp km không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");

                        result = false;
                        break;

                    case "ma_vv_i":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_vv_i:H=Mã dự án", "Danh sách mã dự án không có trong danh mục dự án");
                        else
                            BrowseError(tbl, "ma_vv_i:H=Mã dự án", "Danh sách mã dự án không có trong danh mục dự án");

                        result = false;
                        break;
                    case "ma_bp":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_bp:H=Mã NVBH", "Danh sách mã NVBH không có trong danh mục bộ phận bán hàng");
                        else
                            BrowseError(tbl, "ma_bp:H=Mã NVBH", "Danh sách mã NVBH không có trong danh mục bộ phận bán hàng");

                        result = false;
                        break;
                    case "so_ct_khac_ngay":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "so_ct_khac_ngay:H=Số c.từ;ma_qs:H=Mã quyển sổ", "Danh sách c.từ có ngày c.từ khác nhau");
                        else
                            BrowseError(tbl, "so_ct_khac_ngay:H=Số c.từ;ma_qs:H=Mã quyển sổ", "Danh sách c.từ có ngày c.từ khác nhau");

                        result = false;
                        break;

                }
            }
            return result;
        }

        private static bool ShowError_PNA(DataSet dsError)
        {
            bool result = true;
            foreach (DataTable tbl in dsError.Tables)
            {
                if (tbl.Rows.Count == 0)
                    continue;
                switch (tbl.Columns[0].ColumnName)
                {
                    case "ma_kh":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_kh:H=Mã khách hàng", "Danh sách mã khách hàng không có trong danh mục khách hàng");
                        else
                            BrowseError(tbl, "ma_kh:H=Mã khách hàng", "Danh sách mã khách hàng không có trong danh mục khách hàng e");

                        result = false;
                        break;
                    case "ma_kho":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_kho:H=Mã kho;ma_dvcs:H=Mã ĐVCS", "Danh sách mã kho không có trong danh mục kho hoặc không thuộc mã ĐVCS của chứng từ");
                        else
                            BrowseError(tbl, "ma_kho:H=Mã kho;ma_dvcs:H=Mã ĐVCS", "Danh sách mã kho không có trong danh mục kho hoặc không thuộc mã ĐVCS của chứng từ");

                        result = false;
                        break;
                    case "ma_vt":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_vt:H=Mã vật tư", "Danh sách mã vật tư không có trong danh mục vật tư");
                        else
                            BrowseError(tbl, "ma_vt:H=Mã vật tư e", "Danh sách mã vật tư không có trong danh mục vật tư e");

                        result = false;
                        break;
                    case "ma_nx":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_nx:H=Mã nhập xuất (Tk nợ);ten_tk:H=Tên nhập xuất", "Danh sách mã nhập xuất (Tk nợ) không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");
                        else
                            BrowseError(tbl, "ma_nx:H=Mã nhập xuất (Tk nợ);ten_tk:H=Tên nhập xuất", "Danh sách mã nhập xuất (Tk nợ) không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");

                        result = false;
                        break;
                    case "ma_qs":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_qs:H=Quyển sổ", "Danh sách quyển sổ không có trong danh mục quyển sổ hoặc chứng từ không thuộc quyển sổ này");
                        else
                            BrowseError(tbl, "ma_qs:H=Quyển sổ e", "Danh sách quyển sổ không có trong danh mục quyển sổ hoặc chứng từ không thuộc quyển sổ này e");
                        result = false;
                        break;
                    case "so_ct":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_qs:H=Quyển sổ;so_ct:H=Số chứng từ", "Danh sách số chứng từ trùng số");
                        else
                            BrowseError(tbl, "ma_qs:H=Quyển sổ e;so_ct:H=Số chứng từ e", "Danh sách số chứng từ trùng số e");
                        result = false;
                        break;
                    case "tk_vt":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "tk_vt:H=Tk vật tư;ten_tk:H=Tên tài khoản", "Danh sách tài khoản vật tư không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");
                        else
                            BrowseError(tbl, "tk_vt:H=Tk vật tư;ten_tk:H=Tên tài khoản", "Danh sách tài khoản vật tư không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");

                        result = false;
                        break;
                    case "ma_dvcs":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_dvcs:H=Mã ĐVCS", "Danh sách mã ĐVCS không có trong danh mục ĐVCS");
                        else
                            BrowseError(tbl, "ma_dvcs:H=Mã ĐVCS", "Danh sách mã ĐVCS không có trong danh mục ĐVCS e");

                        result = false;
                        break;
                    case "ngay_ct":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ngay_ct:H=Ngày chứng từ:D", "Danh sách ngày chứng từ nhỏ hơn ngày mở sổ");
                        else
                            BrowseError(tbl, "ngay_ct:H=Ngày chứng từ:D", "Danh sách ngày chứng từ nhỏ hơn ngày mở sổ");

                        result = false;
                        break;

                    case "tk_thue_no":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "tk_thue_no:H=Tài khoản thuế;ten_tk:H=Tên tài khoản", "Danh sách tài khoản thuế không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");
                        else
                            BrowseError(tbl, "tk_thue_no:H=Tài khoản thuế;ten_tk:H=Tên tài khoản", "Danh sách tài khoản thuế không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");

                        result = false;
                        break;
                    case "ma_vv_i":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_vv_i:H=Mã dự án", "Danh sách mã dự án không có trong danh mục dự án");
                        else
                            BrowseError(tbl, "ma_vv_i:H=Mã dự án", "Danh sách mã dự án không có trong danh mục dự án");

                        result = false;
                        break;
                    case "so_ct_khac_ngay":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "so_ct_khac_ngay:H=Số c.từ;ma_qs:H=Mã quyển sổ", "Danh sách c.từ có ngày c.từ khác nhau");
                        else
                            BrowseError(tbl, "so_ct_khac_ngay:H=Số c.từ;ma_qs:H=Mã quyển sổ", "Danh sách c.từ có ngày c.từ khác nhau");

                        result = false;
                        break;

                }
            }
            return result;
        }

        private static bool ShowError_PNB(DataSet dsError)
        {
            bool result = true;
            foreach (DataTable tbl in dsError.Tables)
            {
                if (tbl.Rows.Count == 0)
                    continue;
                switch (tbl.Columns[0].ColumnName)
                {
                    case "ma_kh":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_kh:H=Mã khách hàng", "Danh sách mã khách hàng không có trong danh mục khách hàng");
                        else
                            BrowseError(tbl, "ma_kh:H=Mã khách hàng", "Danh sách mã khách hàng không có trong danh mục khách hàng e");

                        result = false;
                        break;
                    case "ma_kho":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_kho:H=Mã kho;ma_dvcs:H=Mã ĐVCS", "Danh sách mã kho không có trong danh mục kho hoặc không thuộc mã ĐVCS của chứng từ");
                        else
                            BrowseError(tbl, "ma_kho:H=Mã kho;ma_dvcs:H=Mã ĐVCS", "Danh sách mã kho không có trong danh mục kho hoặc không thuộc mã ĐVCS của chứng từ");

                        result = false;
                        break;
                    case "ma_vt":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_vt:H=Mã vật tư", "Danh sách mã vật tư không có trong danh mục vật tư");
                        else
                            BrowseError(tbl, "ma_vt:H=Mã vật tư e", "Danh sách mã vật tư không có trong danh mục vật tư e");

                        result = false;
                        break;
                    case "ma_nx":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_nx:H=Mã nhập xuất (Tk nợ);ten_tk:H=Tên nhập xuất", "Danh sách mã nhập xuất (Tk nợ) không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");
                        else
                            BrowseError(tbl, "ma_nx:H=Mã nhập xuất (Tk nợ);ten_tk:H=Tên nhập xuất", "Danh sách mã nhập xuất (Tk nợ) không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");

                        result = false;
                        break;
                    case "ma_qs":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_qs:H=Quyển sổ", "Danh sách quyển sổ không có trong danh mục quyển sổ hoặc chứng từ không thuộc quyển sổ này");
                        else
                            BrowseError(tbl, "ma_qs:H=Quyển sổ e", "Danh sách quyển sổ không có trong danh mục quyển sổ hoặc chứng từ không thuộc quyển sổ này e");
                        result = false;
                        break;
                    case "so_ct":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_qs:H=Quyển sổ;so_ct:H=Số chứng từ", "Danh sách số chứng từ trùng số");
                        else
                            BrowseError(tbl, "ma_qs:H=Quyển sổ e;so_ct:H=Số chứng từ e", "Danh sách số chứng từ trùng số e");
                        result = false;
                        break;
                    case "tk_vt":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "tk_vt:H=Tk vật tư;ten_tk:H=Tên tài khoản", "Danh sách tài khoản vật tư không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");
                        else
                            BrowseError(tbl, "tk_vt:H=Tk vật tư;ten_tk:H=Tên tài khoản", "Danh sách tài khoản vật tư không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");

                        result = false;
                        break;
                    case "ma_dvcs":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_dvcs:H=Mã ĐVCS", "Danh sách mã ĐVCS không có trong danh mục ĐVCS");
                        else
                            BrowseError(tbl, "ma_dvcs:H=Mã ĐVCS", "Danh sách mã ĐVCS không có trong danh mục ĐVCS e");

                        result = false;
                        break;
                    case "ngay_ct":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ngay_ct:H=Ngày chứng từ:D", "Danh sách ngày chứng từ nhỏ hơn ngày mở sổ");
                        else
                            BrowseError(tbl, "ngay_ct:H=Ngày chứng từ:D", "Danh sách ngày chứng từ nhỏ hơn ngày mở sổ");

                        result = false;
                        break;

                    case "tk_thue_no":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "tk_thue_no:H=Tài khoản thuế;ten_tk:H=Tên tài khoản", "Danh sách tài khoản thuế không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");
                        else
                            BrowseError(tbl, "tk_thue_no:H=Tài khoản thuế;ten_tk:H=Tên tài khoản", "Danh sách tài khoản thuế không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");

                        result = false;
                        break;
                    case "ma_vv_i":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_vv_i:H=Mã dự án", "Danh sách mã dự án không có trong danh mục dự án");
                        else
                            BrowseError(tbl, "ma_vv_i:H=Mã dự án", "Danh sách mã dự án không có trong danh mục dự án");

                        result = false;
                        break;
                    case "so_ct_khac_ngay":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "so_ct_khac_ngay:H=Số c.từ;ma_qs:H=Mã quyển sổ", "Danh sách c.từ có ngày c.từ khác nhau");
                        else
                            BrowseError(tbl, "so_ct_khac_ngay:H=Số c.từ;ma_qs:H=Mã quyển sổ", "Danh sách c.từ có ngày c.từ khác nhau");

                        result = false;
                        break;

                }
            }
            return result;
        }

        private static bool ShowError_PND(DataSet dsError)
        {
            bool result = true;
            foreach (DataTable tbl in dsError.Tables)
            {
                if (tbl.Rows.Count == 0)
                    continue;
                switch (tbl.Columns[0].ColumnName)
                {
                    case "ma_kh":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_kh:H=Mã khách hàng", "Danh sách mã khách hàng không có trong danh mục khách hàng");
                        else
                            BrowseError(tbl, "ma_kh:H=Mã khách hàng", "Danh sách mã khách hàng không có trong danh mục khách hàng e");

                        result = false;
                        break;
                    case "ma_kho":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_kho:H=Mã kho;ma_dvcs:H=Mã ĐVCS", "Danh sách mã kho không có trong danh mục kho hoặc không thuộc mã ĐVCS của chứng từ");
                        else
                            BrowseError(tbl, "ma_kho:H=Mã kho;ma_dvcs:H=Mã ĐVCS", "Danh sách mã kho không có trong danh mục kho hoặc không thuộc mã ĐVCS của chứng từ");

                        result = false;
                        break;
                    case "ma_vt":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_vt:H=Mã vật tư", "Danh sách mã vật tư không có trong danh mục vật tư");
                        else
                            BrowseError(tbl, "ma_vt:H=Mã vật tư e", "Danh sách mã vật tư không có trong danh mục vật tư e");

                        result = false;
                        break;

                    case "ma_qs":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_qs:H=Quyển sổ", "Danh sách quyển sổ không có trong danh mục quyển sổ hoặc chứng từ không thuộc quyển sổ này");
                        else
                            BrowseError(tbl, "ma_qs:H=Quyển sổ e", "Danh sách quyển sổ không có trong danh mục quyển sổ hoặc chứng từ không thuộc quyển sổ này e");
                        result = false;
                        break;
                    case "so_ct":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_qs:H=Quyển sổ;so_ct:H=Số chứng từ", "Danh sách số chứng từ trùng số");
                        else
                            BrowseError(tbl, "ma_qs:H=Quyển sổ e;so_ct:H=Số chứng từ e", "Danh sách số chứng từ trùng số e");
                        result = false;
                        break;



                    case "ma_gd":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_gd:H=Mã giao dịch", "Danh sách mã giao dịch không hợp lệ");
                        else
                            BrowseError(tbl, "ma_gd:H=Mã giao dịch e", "Danh sách mã giao dịch không hợp lệ e");

                        result = false;
                        break;
                    case "tk_no":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "tk_no:H=Tài khoản nợ;ten_tk:H=Tên tài khoản", "Danh sách tài khoản nợ không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");
                        else
                            BrowseError(tbl, "tk_no:H=Tài khoản nợ;ten_tk:H=Tên tài khoản", "Danh sách tài khoản nợ không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");

                        result = false;
                        break;
                    case "tk_co":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "tk_co:H=Tài khoản có;ten_tk:H=Tên tài khoản", "Danh sách tài khoản có không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");
                        else
                            BrowseError(tbl, "tk_co:H=Tài khoản có;ten_tk:H=Tên tài khoản", "Danh sách tài khoản có không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");

                        result = false;
                        break;

                    case "ma_dvcs":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_dvcs:H=Mã ĐVCS", "Danh sách mã ĐVCS không có trong danh mục ĐVCS");
                        else
                            BrowseError(tbl, "ma_dvcs:H=Mã ĐVCS", "Danh sách mã ĐVCS không có trong danh mục ĐVCS e");

                        result = false;
                        break;

                    case "ngay_ct":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ngay_ct:H=Ngày chứng từ:D", "Danh sách ngày chứng từ nhỏ hơn ngày mở sổ");
                        else
                            BrowseError(tbl, "ngay_ct:H=Ngày chứng từ:D", "Danh sách ngày chứng từ nhỏ hơn ngày mở sổ");

                        result = false;
                        break;

                    case "ma_vv_i":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_vv_i:H=Mã dự án", "Danh sách mã dự án không có trong danh mục dự án");
                        else
                            BrowseError(tbl, "ma_vv_i:H=Mã dự án", "Danh sách mã dự án không có trong danh mục dự án");

                        result = false;
                        break;
                    case "so_ct_khac_ngay":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "so_ct_khac_ngay:H=Số c.từ;ma_qs:H=Mã quyển sổ", "Danh sách c.từ có ngày c.từ khác nhau");
                        else
                            BrowseError(tbl, "so_ct_khac_ngay:H=Số c.từ;ma_qs:H=Mã quyển sổ", "Danh sách c.từ có ngày c.từ khác nhau");

                        result = false;
                        break;
                    case "ma_px_i":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_px_i:H=Mã phân xưởng", "Danh sách mã phân xưởng không có trong danh mục phân xưởng");
                        else
                            BrowseError(tbl, "ma_px_i:H=Mã phân xưởng", "Danh sách mã phân xưởng không có trong danh mục phân xưởng");

                        result = false;
                        break;
                    case "ma_sp":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_sp:H=Mã sản phẩm", "Danh sách mã sản phẩm không có trong danh mục sản phẩm");
                        else
                            BrowseError(tbl, "ma_sp:H=Mã sản phẩm", "Danh sách mã sản phẩm không có trong danh mục sản phẩm");

                        result = false;
                        break;
                    case "so_lsx_i":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "so_lsx_i:H=Số lệnh sản xuất", "Danh sách lệnh sản xuất không có trong danh mục lệnh sản xuất");
                        else
                            BrowseError(tbl, "so_lsx_i:H=Số lệnh sản xuất", "Danh sách lệnh sản xuất không có trong danh mục lệnh sản xuất");

                        result = false;
                        break;
                    case "ma_bpht_i":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_bpht_i:H=Mã bộ phận hạch toán", "Danh sách mã bộ phận hạch toán không có trong danh mục bộ phận hạch toán");
                        else
                            BrowseError(tbl, "ma_bpht_i:H=Mã bộ phận hạch toán", "Danh sách mã bộ phận hạch toán không có trong danh mục bộ phận hạch toán");

                        result = false;
                        break;
                }
            }
            return result;
        }

        private static bool ShowError_PXD(DataSet dsError)
        {
            bool result = true;
            foreach (DataTable tbl in dsError.Tables)
            {
                if (tbl.Rows.Count == 0)
                    continue;
                switch (tbl.Columns[0].ColumnName)
                {
                    case "ma_kh":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_kh:H=Mã khách hàng", "Danh sách mã khách hàng không có trong danh mục khách hàng");
                        else
                            BrowseError(tbl, "ma_kh:H=Mã khách hàng", "Danh sách mã khách hàng không có trong danh mục khách hàng e");

                        result = false;
                        break;
                    case "ma_kho":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_kho:H=Mã kho;ma_dvcs:H=Mã ĐVCS", "Danh sách mã kho không có trong danh mục kho hoặc không thuộc mã ĐVCS của chứng từ");
                        else
                            BrowseError(tbl, "ma_kho:H=Mã kho;ma_dvcs:H=Mã ĐVCS", "Danh sách mã kho không có trong danh mục kho hoặc không thuộc mã ĐVCS của chứng từ");

                        result = false;
                        break;
                    case "ma_vt":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_vt:H=Mã vật tư", "Danh sách mã vật tư không có trong danh mục vật tư");
                        else
                            BrowseError(tbl, "ma_vt:H=Mã vật tư e", "Danh sách mã vật tư không có trong danh mục vật tư e");

                        result = false;
                        break;

                    case "ma_qs":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_qs:H=Quyển sổ", "Danh sách quyển sổ không có trong danh mục quyển sổ hoặc chứng từ không thuộc quyển sổ này");
                        else
                            BrowseError(tbl, "ma_qs:H=Quyển sổ e", "Danh sách quyển sổ không có trong danh mục quyển sổ hoặc chứng từ không thuộc quyển sổ này e");
                        result = false;
                        break;
                    case "so_ct":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_qs:H=Quyển sổ;so_ct:H=Số chứng từ", "Danh sách số chứng từ trùng số");
                        else
                            BrowseError(tbl, "ma_qs:H=Quyển sổ e;so_ct:H=Số chứng từ e", "Danh sách số chứng từ trùng số e");
                        result = false;
                        break;


                    case "ma_gd":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_gd:H=Mã giao dịch", "Danh sách mã giao dịch không hợp lệ");
                        else
                            BrowseError(tbl, "ma_gd:H=Mã giao dịch e", "Danh sách mã giao dịch không hợp lệ e");

                        result = false;
                        break;
                    case "tk_no":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "tk_no:H=Tài khoản nợ;ten_tk:H=Tên tài khoản", "Danh sách tài khoản nợ không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");
                        else
                            BrowseError(tbl, "tk_no:H=Tài khoản nợ;ten_tk:H=Tên tài khoản", "Danh sách tài khoản nợ không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");

                        result = false;
                        break;
                    case "tk_co":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "tk_co:H=Tài khoản có;ten_tk:H=Tên tài khoản", "Danh sách tài khoản có không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");
                        else
                            BrowseError(tbl, "tk_co:H=Tài khoản có e;ten_tk2:H=Tên tài khoản e", "Danh sách tài khoản có không có trong danh mục tài khoản hoặc là tài khoản tổng hợp e");

                        result = false;
                        break;

                    case "ma_td_i":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_td_i:H=Mã tự do 1", "Danh sách mã tự do 1 không có trong danh mục tự do 1");
                        else
                            BrowseError(tbl, "ma_td_i:H=Mã tự do 1", "Danh sách mã tự do 1 không có trong danh mục tự do 1 e");

                        result = false;
                        break;
                    case "ma_td2_i":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_td2_i:H=Mã tự do 2", "Danh sách mã tự do 2 không có trong danh mục tự do 2");
                        else
                            BrowseError(tbl, "ma_td2_i:H=Mã tự do 2", "Danh sách mã tự do 2 không có trong danh mục tự do 2 e");

                        result = false;
                        break;
                    case "ma_td3_i":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_td3_i:H=Mã tự do 3", "Danh sách mã tự do 3 không có trong danh mục tự do 3");
                        else
                            BrowseError(tbl, "ma_td3_i:H=Mã tự do 3", "Danh sách mã tự do 3 không có trong danh mục tự do 3 e");

                        result = false;
                        break;
                    case "ma_dvcs":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_dvcs:H=Mã ĐVCS", "Danh sách mã ĐVCS không có trong danh mục ĐVCS");
                        else
                            BrowseError(tbl, "ma_dvcs:H=Mã ĐVCS", "Danh sách mã ĐVCS không có trong danh mục ĐVCS e");

                        result = false;
                        break;

                    case "ngay_ct":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ngay_ct:H=Ngày chứng từ:D", "Danh sách ngày chứng từ nhỏ hơn ngày mở sổ");
                        else
                            BrowseError(tbl, "ngay_ct:H=Ngày chứng từ:D", "Danh sách ngày chứng từ nhỏ hơn ngày mở sổ");

                        result = false;
                        break;

                    case "ma_vv_i":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_vv_i:H=Mã dự án", "Danh sách mã dự án không có trong danh mục dự án");
                        else
                            BrowseError(tbl, "ma_vv_i:H=Mã dự án", "Danh sách mã dự án không có trong danh mục dự án");

                        result = false;
                        break;
                    case "so_ct_khac_ngay":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "so_ct_khac_ngay:H=Số c.từ;ma_qs:H=Mã quyển sổ", "Danh sách c.từ có ngày c.từ khác nhau");
                        else
                            BrowseError(tbl, "so_ct_khac_ngay:H=Số c.từ;ma_qs:H=Mã quyển sổ", "Danh sách c.từ có ngày c.từ khác nhau");

                        result = false;
                        break;
                    case "ma_px_i":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_px_i:H=Mã phân xưởng", "Danh sách mã phân xưởng không có trong danh mục phân xưởng");
                        else
                            BrowseError(tbl, "ma_px_i:H=Mã phân xưởng", "Danh sách mã phân xưởng không có trong danh mục phân xưởng");

                        result = false;
                        break;
                    case "ma_sp":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_sp:H=Mã sản phẩm", "Danh sách mã sản phẩm không có trong danh mục sản phẩm");
                        else
                            BrowseError(tbl, "ma_sp:H=Mã sản phẩm", "Danh sách mã sản phẩm không có trong danh mục sản phẩm");

                        result = false;
                        break;
                    case "ma_bpht_i":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_bpht_i:H=Mã bộ phận hạch toán", "Danh sách mã bộ phận hạch toán không có trong danh mục bộ phận hạch toán");
                        else
                            BrowseError(tbl, "ma_bpht_i:H=Mã bộ phận hạch toán", "Danh sách mã bộ phận hạch toán không có trong danh mục bộ phận hạch toán");

                        result = false;
                        break;
                    case "ma_phi_i":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_phi_i:H=Mã phí", "Danh sách mã phí không có trong danh mục phí");
                        else
                            BrowseError(tbl, "ma_phi_i:H=Mã phí", "Danh sách mã phí không có trong danh mục phí");

                        result = false;
                        break;
                }
            }
            return result;
        }

        private static bool ShowError_PXE(DataSet dsError)
        {
            bool result = true;
            foreach (DataTable tbl in dsError.Tables)
            {
                if (tbl.Rows.Count == 0)
                    continue;
                switch (tbl.Columns[0].ColumnName)
                {
                    case "ma_kh":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_kh:H=Mã khách hàng", "Danh sách mã khách hàng không có trong danh mục khách hàng");
                        else
                            BrowseError(tbl, "ma_kh:H=Mã khách hàng", "Danh sách mã khách hàng không có trong danh mục khách hàng e");

                        result = false;
                        break;
                    case "ma_kho":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_kho:H=Mã kho xuất;ma_dvcs:H=Mã ĐVCS", "Danh sách mã kho xuất không có trong danh mục kho hoặc không thuộc mã ĐVCS của chứng từ");
                        else
                            BrowseError(tbl, "ma_kho:H=Mã kho xuất;ma_dvcs:H=Mã ĐVCS", "Danh sách mã kho xuất không có trong danh mục kho hoặc không thuộc mã ĐVCS của chứng từ");

                        result = false;
                        break;
                    case "ma_khon":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_khon:H=Mã kho nhập;ma_dvcs:H=Mã ĐVCS", "Danh sách mã kho nhập không có trong danh mục kho hoặc không thuộc mã ĐVCS của chứng từ");
                        else
                            BrowseError(tbl, "ma_khon:H=Mã kho nhập;ma_dvcs:H=Mã ĐVCS", "Danh sách mã kho nhập không có trong danh mục kho hoặc không thuộc mã ĐVCS của chứng từ");

                        result = false;
                        break;
                    case "ma_vt":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_vt:H=Mã vật tư", "Danh sách mã vật tư không có trong danh mục vật tư");
                        else
                            BrowseError(tbl, "ma_vt:H=Mã vật tư e", "Danh sách mã vật tư không có trong danh mục vật tư e");

                        result = false;
                        break;

                    case "ma_qs":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_qs:H=Quyển sổ", "Danh sách quyển sổ không có trong danh mục quyển sổ hoặc chứng từ không thuộc quyển sổ này");
                        else
                            BrowseError(tbl, "ma_qs:H=Quyển sổ e", "Danh sách quyển sổ không có trong danh mục quyển sổ hoặc chứng từ không thuộc quyển sổ này e");
                        result = false;
                        break;
                    case "so_ct":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_qs:H=Quyển sổ;so_ct:H=Số chứng từ", "Danh sách số chứng từ trùng số");
                        else
                            BrowseError(tbl, "ma_qs:H=Quyển sổ e;so_ct:H=Số chứng từ e", "Danh sách số chứng từ trùng số e");
                        result = false;
                        break;
                    case "ma_gd":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_gd:H=Mã giao dịch", "Danh sách mã giao dịch không hợp lệ");
                        else
                            BrowseError(tbl, "ma_gd:H=Mã giao dịch e", "Danh sách mã giao dịch không hợp lệ e");

                        result = false;
                        break;
                    case "tk_no":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "tk_no:H=Tài khoản nợ;ten_tk:H=Tên tài khoản", "Danh sách tài khoản nợ không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");
                        else
                            BrowseError(tbl, "tk_no:H=Tài khoản nợ;ten_tk:H=Tên tài khoản", "Danh sách tài khoản nợ không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");

                        result = false;
                        break;
                    case "tk_co":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "tk_co:H=Tài khoản có;ten_tk:H=Tên tài khoản", "Danh sách tài khoản có không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");
                        else
                            BrowseError(tbl, "tk_co:H=Tài khoản có e;ten_tk2:H=Tên tài khoản e", "Danh sách tài khoản có không có trong danh mục tài khoản hoặc là tài khoản tổng hợp e");

                        result = false;
                        break;

                    case "ma_td_i":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_td_i:H=Mã tự do 1", "Danh sách mã tự do 1 không có trong danh mục tự do 1");
                        else
                            BrowseError(tbl, "ma_td_i:H=Mã tự do 1", "Danh sách mã tự do 1 không có trong danh mục tự do 1 e");

                        result = false;
                        break;
                    case "ma_td2_i":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_td2_i:H=Mã tự do 2", "Danh sách mã tự do 2 không có trong danh mục tự do 2");
                        else
                            BrowseError(tbl, "ma_td2_i:H=Mã tự do 2", "Danh sách mã tự do 2 không có trong danh mục tự do 2 e");

                        result = false;
                        break;
                    case "ma_td3_i":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_td3_i:H=Mã tự do 3", "Danh sách mã tự do 3 không có trong danh mục tự do 3");
                        else
                            BrowseError(tbl, "ma_td3_i:H=Mã tự do 3", "Danh sách mã tự do 3 không có trong danh mục tự do 3 e");

                        result = false;
                        break;
                    case "ma_dvcs":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_dvcs:H=Mã ĐVCS", "Danh sách mã ĐVCS không có trong danh mục ĐVCS");
                        else
                            BrowseError(tbl, "ma_dvcs:H=Mã ĐVCS", "Danh sách mã ĐVCS không có trong danh mục ĐVCS e");

                        result = false;
                        break;

                    case "ngay_ct":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ngay_ct:H=Ngày chứng từ:D", "Danh sách ngày chứng từ nhỏ hơn ngày mở sổ");
                        else
                            BrowseError(tbl, "ngay_ct:H=Ngày chứng từ:D", "Danh sách ngày chứng từ nhỏ hơn ngày mở sổ");

                        result = false;
                        break;

                    case "ma_vv_i":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_vv_i:H=Mã dự án", "Danh sách mã dự án không có trong danh mục dự án");
                        else
                            BrowseError(tbl, "ma_vv_i:H=Mã dự án", "Danh sách mã dự án không có trong danh mục dự án");

                        result = false;
                        break;
                    case "so_ct_khac_ngay":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "so_ct_khac_ngay:H=Số c.từ;ma_qs:H=Mã quyển sổ", "Danh sách c.từ có ngày c.từ khác nhau");
                        else
                            BrowseError(tbl, "so_ct_khac_ngay:H=Số c.từ;ma_qs:H=Mã quyển sổ", "Danh sách c.từ có ngày c.từ khác nhau");

                        result = false;
                        break;
                    case "ma_px_i":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_px_i:H=Mã phân xưởng", "Danh sách mã phân xưởng không có trong danh mục phân xưởng");
                        else
                            BrowseError(tbl, "ma_px_i:H=Mã phân xưởng", "Danh sách mã phân xưởng không có trong danh mục phân xưởng");

                        result = false;
                        break;
                    case "ma_sp":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_sp:H=Mã sản phẩm", "Danh sách mã sản phẩm không có trong danh mục sản phẩm");
                        else
                            BrowseError(tbl, "ma_sp:H=Mã sản phẩm", "Danh sách mã sản phẩm không có trong danh mục sản phẩm");

                        result = false;
                        break;
                    case "ma_bpht_i":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_bpht_i:H=Mã bộ phận hạch toán", "Danh sách mã bộ phận hạch toán không có trong danh mục bộ phận hạch toán");
                        else
                            BrowseError(tbl, "ma_bpht_i:H=Mã bộ phận hạch toán", "Danh sách mã bộ phận hạch toán không có trong danh mục bộ phận hạch toán");

                        result = false;
                        break;
                }
            }
            return result;
        }

        private static bool ShowError_PNG(DataSet dsError)
        {
            bool result = true;
            foreach (DataTable tbl in dsError.Tables)
            {
                if (tbl.Rows.Count == 0)
                    continue;
                switch (tbl.Columns[0].ColumnName)
                {
                    case "ma_kh":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_kh:H=Mã khách hàng", "Danh sách mã khách hàng không có trong danh mục khách hàng");
                        else
                            BrowseError(tbl, "ma_kh:H=Mã khách hàng", "Danh sách mã khách hàng không có trong danh mục khách hàng e");

                        result = false;
                        break;
                    case "ma_kho":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_kho:H=Mã kho;ma_dvcs:H=Mã ĐVCS", "Danh sách mã kho không có trong danh mục kho hoặc không thuộc mã ĐVCS của chứng từ");
                        else
                            BrowseError(tbl, "ma_kho:H=Mã kho;ma_dvcs:H=Mã ĐVCS", "Danh sách mã kho không có trong danh mục kho hoặc không thuộc mã ĐVCS của chứng từ");

                        result = false;
                        break;
                    case "ma_vt":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_vt:H=Mã vật tư", "Danh sách mã vật tư không có trong danh mục vật tư");
                        else
                            BrowseError(tbl, "ma_vt:H=Mã vật tư e", "Danh sách mã vật tư không có trong danh mục vật tư e");

                        result = false;
                        break;
                    case "ma_nx":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_nx:H=Mã nhập xuất (Tk nợ);ten_tk:H=Tên nhập xuất", "Danh sách mã nhập xuất (Tk nợ) không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");
                        else
                            BrowseError(tbl, "ma_nx:H=Mã nhập xuất (Tk nợ);ten_tk:H=Tên nhập xuất", "Danh sách mã nhập xuất (Tk nợ) không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");

                        result = false;
                        break;
                    case "ma_qs":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_qs:H=Quyển sổ", "Danh sách quyển sổ không có trong danh mục quyển sổ hoặc chứng từ không thuộc quyển sổ này");
                        else
                            BrowseError(tbl, "ma_qs:H=Quyển sổ e", "Danh sách quyển sổ không có trong danh mục quyển sổ hoặc chứng từ không thuộc quyển sổ này e");
                        result = false;
                        break;
                    case "so_ct":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_qs:H=Quyển sổ;so_ct:H=Số chứng từ", "Danh sách số chứng từ trùng số");
                        else
                            BrowseError(tbl, "ma_qs:H=Quyển sổ e;so_ct:H=Số chứng từ e", "Danh sách số chứng từ trùng số e");
                        result = false;
                        break;
                    case "tk_nvl":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "tk_nvl:H=Tk doanh thu;ten_tk:H=Tên tài khoản", "Danh sách tài khoản doanh thu không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");
                        else
                            BrowseError(tbl, "tk_nvl:H=Tk doanh thu;ten_tk:H=Tên tài khoản", "Danh sách tài khoản doanh thu không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");

                        result = false;
                        break;
                    case "tk_vt":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "tk_vt:H=Tk vật tư;ten_tk:H=Tên tài khoản", "Danh sách tài khoản vật tư không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");
                        else
                            BrowseError(tbl, "tk_vt:H=Tk vật tư;ten_tk:H=Tên tài khoản", "Danh sách tài khoản vật tư không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");

                        result = false;
                        break;
                    case "tk_gv":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "tk_gv:H=Tk giá vốn;ten_tk:H=Tên tài khoản", "Danh sách tài khoản giá vốn không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");
                        else
                            BrowseError(tbl, "tk_gv:H=Tk giá vốn;ten_tk:H=Tên tài khoản", "Danh sách tài khoản giá vốn không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");

                        result = false;
                        break;
                    case "tk_ck":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "tk_ck:H=Tk chiết khấu;ten_tk:H=Tên tài khoản", "Danh sách tài khoản chiết khấu không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");
                        else
                            BrowseError(tbl, "tk_ck:H=Tk chiết khấu;ten_tk:H=Tên tài khoản", "Danh sách tài khoản chiết khấu không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");

                        result = false;
                        break;

                    

                    case "ma_dvcs":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_dvcs:H=Mã ĐVCS", "Danh sách mã ĐVCS không có trong danh mục ĐVCS");
                        else
                            BrowseError(tbl, "ma_dvcs:H=Mã ĐVCS", "Danh sách mã ĐVCS không có trong danh mục ĐVCS e");

                        result = false;
                        break;
                    case "ngay_ct":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ngay_ct:H=Ngày chứng từ:D", "Danh sách ngày chứng từ nhỏ hơn ngày mở sổ");
                        else
                            BrowseError(tbl, "ngay_ct:H=Ngày chứng từ:D", "Danh sách ngày chứng từ nhỏ hơn ngày mở sổ");

                        result = false;
                        break;

                    case "tk_thue_co":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "tk_thue_co:H=Tài khoản thuế;ten_tk:H=Tên tài khoản", "Danh sách tài khoản thuế không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");
                        else
                            BrowseError(tbl, "tk_thue_co:H=Tài khoản thuế;ten_tk:H=Tên tài khoản", "Danh sách tài khoản thuế không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");

                        result = false;
                        break;
                    case "tk_km_i":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "tk_km_i:H=Tài khoản cp km;ten_tk:H=Tên tài khoản", "Danh sách tài khoản cp km không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");
                        else
                            BrowseError(tbl, "tk_km_i:H=Tài khoản cp km;ten_tk:H=Tên tài khoản", "Danh sách tài khoản cp km không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");

                        result = false;
                        break;

                    case "ma_vv_i":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_vv_i:H=Mã dự án", "Danh sách mã dự án không có trong danh mục dự án");
                        else
                            BrowseError(tbl, "ma_vv_i:H=Mã dự án", "Danh sách mã dự án không có trong danh mục dự án");

                        result = false;
                        break;
                    
                    case "so_ct_khac_ngay":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "so_ct_khac_ngay:H=Số c.từ;ma_qs:H=Mã quyển sổ", "Danh sách c.từ có ngày c.từ khác nhau");
                        else
                            BrowseError(tbl, "so_ct_khac_ngay:H=Số c.từ;ma_qs:H=Mã quyển sổ", "Danh sách c.từ có ngày c.từ khác nhau");

                        result = false;
                        break;

                }
            }
            return result;
        }

        private static bool ShowError_PT1(DataSet dsError)
        {
            bool result = true;
            foreach (DataTable tbl in dsError.Tables)
            {
                if (tbl.Rows.Count == 0)
                    continue;
                switch (tbl.Columns[0].ColumnName)
                {
                    case "ma_kh":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_kh:H=Mã khách hàng", "Danh sách mã khách hàng không có trong danh mục khách hàng");
                        else
                            BrowseError(tbl, "ma_kh:H=Mã khách hàng", "Danh sách mã khách hàng không có trong danh mục khách hàng e");

                        result = false;
                        break;

                    case "ma_kh_i":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_kh_i:H=Mã khách", "Danh sách mã khách chi tiết không có trong danh mục khách hàng");
                        else
                            BrowseError(tbl, "ma_td_i:H=Mã khách", "Danh sách mã khách chi tiết không có trong danh mục khách hàng");

                        result = false;
                        break;

                    case "so_ct_i":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "so_ct_i:H=Số chứng từ", "Danh sách chứng từ mã giao dịch loại 3 không có mã khách");
                        else
                            BrowseError(tbl, "so_ct_i:H=Số chứng từ", "Danh sách chứng từ mã giao dịch loại 3 không có mã khách");

                        result = false;
                        break;

                    case "ma_td_i":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_td_i:H=Mã tự do", "Danh sách mã tự do không có trong danh mục tự do");
                        else
                            BrowseError(tbl, "ma_td_i:H=Mã tự do", "Danh sách mã tự do không có trong danh mục tự do");

                        result = false;
                        break;

                    case "ma_qs":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_qs:H=Quyển sổ", "Danh sách quyển sổ không có trong danh mục quyển sổ hoặc chứng từ không thuộc quyển sổ này");
                        else
                            BrowseError(tbl, "ma_qs:H=Quyển sổ e", "Danh sách quyển sổ không có trong danh mục quyển sổ hoặc chứng từ không thuộc quyển sổ này e");
                        result = false;
                        break;
                    case "so_ct":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_qs:H=Quyển sổ;so_ct:H=Số chứng từ", "Danh sách số chứng từ trùng số");
                        else
                            BrowseError(tbl, "ma_qs:H=Quyển sổ e;so_ct:H=Số chứng từ e", "Danh sách số chứng từ trùng số e");
                        result = false;
                        break;

                    case "ma_gd":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_gd:H=Mã giao dịch", "Danh sách mã giao dịch không hợp lệ");
                        else
                            BrowseError(tbl, "ma_gd:H=Mã giao dịch e", "Danh sách mã giao dịch không hợp lệ e");

                        result = false;
                        break;

                    case "tk":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "tk:H=Tài khoản nợ;ten_tk:H=Tên tài khoản", "Danh sách tài khoản nợ không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");
                        else
                            BrowseError(tbl, "tk:H=Tài khoản nợ;ten_tk:H=Tên tài khoản", "Danh sách tài khoản nợ không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");

                        result = false;
                        break;

                    case "tk_i":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "tk_i:H=Tài khoản có;ten_tk:H=Tên tài khoản", "Danh sách tài khoản có không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");
                        else
                            BrowseError(tbl, "tk_i:H=Tài khoản có;ten_tk:H=Tên tài khoản", "Danh sách tài khoản có không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");

                        result = false;
                        break;

                    case "ma_dvcs":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_dvcs:H=Mã ĐVCS", "Danh sách mã ĐVCS không có trong danh mục ĐVCS");
                        else
                            BrowseError(tbl, "ma_dvcs:H=Mã ĐVCS", "Danh sách mã ĐVCS không có trong danh mục ĐVCS e");

                        result = false;
                        break;

                    case "ngay_ct":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ngay_ct:H=Ngày chứng từ:D", "Danh sách ngày chứng từ nhỏ hơn ngày mở sổ");
                        else
                            BrowseError(tbl, "ngay_ct:H=Ngày chứng từ:D", "Danh sách ngày chứng từ nhỏ hơn ngày mở sổ");

                        result = false;
                        break;

                    case "ma_vv_i":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_vv_i:H=Mã dự án", "Danh sách mã dự án không có trong danh mục dự án");
                        else
                            BrowseError(tbl, "ma_vv_i:H=Mã dự án", "Danh sách mã dự án không có trong danh mục dự án");

                        result = false;
                        break;

                    case "ma_phi_i":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_phi_i:H=Mã phí", "Danh sách mã phí không có trong danh mục phí");
                        else
                            BrowseError(tbl, "ma_phi_i:H=Mã phí", "Danh sách mã phí không có trong danh mục phí");

                        result = false;
                        break;

                    case "so_ct_khac_ngay":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "so_ct_khac_ngay:H=Số c.từ;ma_qs:H=Mã quyển sổ", "Danh sách c.từ có ngày c.từ khác nhau");
                        else
                            BrowseError(tbl, "so_ct_khac_ngay:H=Số c.từ;ma_qs:H=Mã quyển sổ", "Danh sách c.từ có ngày c.từ khác nhau");

                        result = false;
                        break;
                }
            }
            return result;
        }

        private static bool ShowError_PC1(DataSet dsError)
        {
            bool result = true;
            foreach (DataTable tbl in dsError.Tables)
            {
                if (tbl.Rows.Count == 0)
                    continue;
                switch (tbl.Columns[0].ColumnName)
                {
                    case "ma_kh":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_kh:H=Mã khách hàng", "Danh sách mã khách hàng không có trong danh mục khách hàng");
                        else
                            BrowseError(tbl, "ma_kh:H=Mã khách hàng", "Danh sách mã khách hàng không có trong danh mục khách hàng e");

                        result = false;
                        break;

                    case "ma_kh_i":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_kh_i:H=Mã khách", "Danh sách mã khách chi tiết không có trong danh mục khách hàng");
                        else
                            BrowseError(tbl, "ma_td_i:H=Mã khách", "Danh sách mã khách chi tiết không có trong danh mục khách hàng");

                        result = false;
                        break;

                    case "so_ct_i":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "so_ct_i:H=Số chứng từ", "Danh sách chứng từ mã giao dịch loại 3 không có mã khách");
                        else
                            BrowseError(tbl, "so_ct_i:H=Số chứng từ", "Danh sách chứng từ mã giao dịch loại 3 không có mã khách");

                        result = false;
                        break;

                    case "ma_td_i":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_td_i:H=Mã tự do", "Danh sách mã tự do không có trong danh mục tự do");
                        else
                            BrowseError(tbl, "ma_td_i:H=Mã tự do", "Danh sách mã tự do không có trong danh mục tự do");

                        result = false;
                        break;

                    case "ma_phi_i":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_phi_i:H=Mã phí", "Danh sách mã phí không có trong danh mục phí");
                        else
                            BrowseError(tbl, "ma_phi_i:H=Mã phí", "Danh sách mã phí không có trong danh mục phí");

                        result = false;
                        break;

                    case "ma_qs":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_qs:H=Quyển sổ", "Danh sách quyển sổ không có trong danh mục quyển sổ hoặc chứng từ không thuộc quyển sổ này");
                        else
                            BrowseError(tbl, "ma_qs:H=Quyển sổ e", "Danh sách quyển sổ không có trong danh mục quyển sổ hoặc chứng từ không thuộc quyển sổ này e");
                        result = false;
                        break;
                    case "so_ct":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_qs:H=Quyển sổ;so_ct:H=Số chứng từ", "Danh sách số chứng từ trùng số");
                        else
                            BrowseError(tbl, "ma_qs:H=Quyển sổ e;so_ct:H=Số chứng từ e", "Danh sách số chứng từ trùng số e");
                        result = false;
                        break;

                    case "ma_gd":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_gd:H=Mã giao dịch", "Danh sách mã giao dịch không hợp lệ");
                        else
                            BrowseError(tbl, "ma_gd:H=Mã giao dịch e", "Danh sách mã giao dịch không hợp lệ e");

                        result = false;
                        break;
                    case "tk":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "tk:H=Tài khoản có;ten_tk:H=Tên tài khoản", "Danh sách tài khoản nợ không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");
                        else
                            BrowseError(tbl, "tk:H=Tài khoản có;ten_tk:H=Tên tài khoản", "Danh sách tài khoản nợ không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");

                        result = false;
                        break;
                    case "tk_i":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "tk_i:H=Tài khoản nợ;ten_tk:H=Tên tài khoản", "Danh sách tài khoản có không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");
                        else
                            BrowseError(tbl, "tk_i:H=Tài khoản nợ;ten_tk:H=Tên tài khoản", "Danh sách tài khoản có không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");

                        result = false;
                        break;

                    case "ma_dvcs":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_dvcs:H=Mã ĐVCS", "Danh sách mã ĐVCS không có trong danh mục ĐVCS");
                        else
                            BrowseError(tbl, "ma_dvcs:H=Mã ĐVCS", "Danh sách mã ĐVCS không có trong danh mục ĐVCS e");

                        result = false;
                        break;

                    case "ngay_ct":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ngay_ct:H=Ngày chứng từ:D", "Danh sách ngày chứng từ nhỏ hơn ngày mở sổ");
                        else
                            BrowseError(tbl, "ngay_ct:H=Ngày chứng từ:D", "Danh sách ngày chứng từ nhỏ hơn ngày mở sổ");

                        result = false;
                        break;

                    case "ma_vv_i":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_vv_i:H=Mã dự án", "Danh sách mã dự án không có trong danh mục dự án");
                        else
                            BrowseError(tbl, "ma_vv_i:H=Mã dự án", "Danh sách mã dự án không có trong danh mục dự án");

                        result = false;
                        break;

                    case "so_ct_khac_ngay":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "so_ct_khac_ngay:H=Số c.từ;ma_qs:H=Mã quyển sổ", "Danh sách c.từ có ngày c.từ khác nhau");
                        else
                            BrowseError(tbl, "so_ct_khac_ngay:H=Số c.từ;ma_qs:H=Mã quyển sổ", "Danh sách c.từ có ngày c.từ khác nhau");

                        result = false;
                        break;
                }
            }
            return result;
        }

        private static bool ShowError_BC1(DataSet dsError)
        {
            bool result = true;
            foreach (DataTable tbl in dsError.Tables)
            {
                if (tbl.Rows.Count == 0)
                    continue;
                switch (tbl.Columns[0].ColumnName)
                {
                    case "ma_kh":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_kh:H=Mã khách hàng", "Danh sách mã khách hàng không có trong danh mục khách hàng");
                        else
                            BrowseError(tbl, "ma_kh:H=Mã khách hàng", "Danh sách mã khách hàng không có trong danh mục khách hàng e");

                        result = false;
                        break;
                    case "ma_qs":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_qs:H=Quyển sổ", "Danh sách quyển sổ không có trong danh mục quyển sổ hoặc chứng từ không thuộc quyển sổ này");
                        else
                            BrowseError(tbl, "ma_qs:H=Quyển sổ e", "Danh sách quyển sổ không có trong danh mục quyển sổ hoặc chứng từ không thuộc quyển sổ này e");
                        result = false;
                        break;
                    case "so_ct":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_qs:H=Quyển sổ;so_ct:H=Số chứng từ", "Danh sách số chứng từ trùng số");
                        else
                            BrowseError(tbl, "ma_qs:H=Quyển sổ e;so_ct:H=Số chứng từ e", "Danh sách số chứng từ trùng số e");
                        result = false;
                        break;

                    case "ma_gd":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_gd:H=Mã giao dịch", "Danh sách mã giao dịch không hợp lệ");
                        else
                            BrowseError(tbl, "ma_gd:H=Mã giao dịch e", "Danh sách mã giao dịch không hợp lệ e");

                        result = false;
                        break;

                    case "tk":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "tk:H=Tài khoản nợ;ten_tk:H=Tên tài khoản", "Danh sách tài khoản nợ không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");
                        else
                            BrowseError(tbl, "tk:H=Tài khoản nợ;ten_tk:H=Tên tài khoản", "Danh sách tài khoản nợ không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");

                        result = false;
                        break;

                    case "tk_i":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "tk_i:H=Tài khoản có;ten_tk:H=Tên tài khoản", "Danh sách tài khoản có không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");
                        else
                            BrowseError(tbl, "tk_i:H=Tài khoản có;ten_tk:H=Tên tài khoản", "Danh sách tài khoản có không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");

                        result = false;
                        break;

                    case "ma_dvcs":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_dvcs:H=Mã ĐVCS", "Danh sách mã ĐVCS không có trong danh mục ĐVCS");
                        else
                            BrowseError(tbl, "ma_dvcs:H=Mã ĐVCS", "Danh sách mã ĐVCS không có trong danh mục ĐVCS e");

                        result = false;
                        break;

                    case "ngay_ct":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ngay_ct:H=Ngày chứng từ:D", "Danh sách ngày chứng từ nhỏ hơn ngày mở sổ");
                        else
                            BrowseError(tbl, "ngay_ct:H=Ngày chứng từ:D", "Danh sách ngày chứng từ nhỏ hơn ngày mở sổ");

                        result = false;
                        break;

                    case "ma_vv_i":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_vv_i:H=Mã dự án", "Danh sách mã dự án không có trong danh mục dự án");
                        else
                            BrowseError(tbl, "ma_vv_i:H=Mã dự án", "Danh sách mã dự án không có trong danh mục dự án");

                        result = false;
                        break;

                    case "so_ct_khac_ngay":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "so_ct_khac_ngay:H=Số c.từ;ma_qs:H=Mã quyển sổ", "Danh sách c.từ có ngày c.từ khác nhau");
                        else
                            BrowseError(tbl, "so_ct_khac_ngay:H=Số c.từ;ma_qs:H=Mã quyển sổ", "Danh sách c.từ có ngày c.từ khác nhau");

                        result = false;
                        break;
                }
            }
            return result;
        }

        private static bool ShowError_BN1(DataSet dsError)
        {
            bool result = true;
            foreach (DataTable tbl in dsError.Tables)
            {
                if (tbl.Rows.Count == 0)
                    continue;
                switch (tbl.Columns[0].ColumnName)
                {
                    case "ma_kh":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_kh:H=Mã khách hàng", "Danh sách mã khách hàng không có trong danh mục khách hàng");
                        else
                            BrowseError(tbl, "ma_kh:H=Mã khách hàng", "Danh sách mã khách hàng không có trong danh mục khách hàng e");

                        result = false;
                        break;
                    case "ma_qs":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_qs:H=Quyển sổ", "Danh sách quyển sổ không có trong danh mục quyển sổ hoặc chứng từ không thuộc quyển sổ này");
                        else
                            BrowseError(tbl, "ma_qs:H=Quyển sổ e", "Danh sách quyển sổ không có trong danh mục quyển sổ hoặc chứng từ không thuộc quyển sổ này e");
                        result = false;
                        break;
                    case "so_ct":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_qs:H=Quyển sổ;so_ct:H=Số chứng từ", "Danh sách số chứng từ trùng số");
                        else
                            BrowseError(tbl, "ma_qs:H=Quyển sổ e;so_ct:H=Số chứng từ e", "Danh sách số chứng từ trùng số e");
                        result = false;
                        break;

                    case "ma_gd":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_gd:H=Mã giao dịch", "Danh sách mã giao dịch không hợp lệ");
                        else
                            BrowseError(tbl, "ma_gd:H=Mã giao dịch e", "Danh sách mã giao dịch không hợp lệ e");

                        result = false;
                        break;
                    case "tk":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "tk:H=Tài khoản có;ten_tk:H=Tên tài khoản", "Danh sách tài khoản nợ không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");
                        else
                            BrowseError(tbl, "tk:H=Tài khoản có;ten_tk:H=Tên tài khoản", "Danh sách tài khoản nợ không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");

                        result = false;
                        break;
                    case "tk_i":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "tk_i:H=Tài khoản nợ;ten_tk:H=Tên tài khoản", "Danh sách tài khoản có không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");
                        else
                            BrowseError(tbl, "tk_i:H=Tài khoản nợ;ten_tk:H=Tên tài khoản", "Danh sách tài khoản có không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");

                        result = false;
                        break;

                    case "ma_dvcs":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_dvcs:H=Mã ĐVCS", "Danh sách mã ĐVCS không có trong danh mục ĐVCS");
                        else
                            BrowseError(tbl, "ma_dvcs:H=Mã ĐVCS", "Danh sách mã ĐVCS không có trong danh mục ĐVCS e");

                        result = false;
                        break;

                    case "ngay_ct":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ngay_ct:H=Ngày chứng từ:D", "Danh sách ngày chứng từ nhỏ hơn ngày mở sổ");
                        else
                            BrowseError(tbl, "ngay_ct:H=Ngày chứng từ:D", "Danh sách ngày chứng từ nhỏ hơn ngày mở sổ");

                        result = false;
                        break;

                    case "ma_vv_i":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_vv_i:H=Mã dự án", "Danh sách mã dự án không có trong danh mục dự án");
                        else
                            BrowseError(tbl, "ma_vv_i:H=Mã dự án", "Danh sách mã dự án không có trong danh mục dự án");

                        result = false;
                        break;

                    case "so_ct_khac_ngay":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "so_ct_khac_ngay:H=Số c.từ;ma_qs:H=Mã quyển sổ", "Danh sách c.từ có ngày c.từ khác nhau");
                        else
                            BrowseError(tbl, "so_ct_khac_ngay:H=Số c.từ;ma_qs:H=Mã quyển sổ", "Danh sách c.từ có ngày c.từ khác nhau");

                        result = false;
                        break;
                }
            }
            return result;
        }

        private static bool ShowError_HD1(DataSet dsError)
        {
            bool result = true;
            foreach (DataTable tbl in dsError.Tables)
            {
                if (tbl.Rows.Count == 0)
                    continue;
                switch (tbl.Columns[0].ColumnName)
                {
                    case "ma_kh":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_kh:H=Mã khách hàng", "Danh sách mã khách hàng không có trong danh mục khách hàng");
                        else
                            BrowseError(tbl, "ma_kh:H=Mã khách hàng", "Danh sách mã khách hàng không có trong danh mục khách hàng e");

                        result = false;
                        break;
                    
                   
                    case "ma_nx":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_nx:H=Mã nhập xuất (Tk nợ);ten_tk:H=Tên nhập xuất", "Danh sách mã nhập xuất (Tk nợ) không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");
                        else
                            BrowseError(tbl, "ma_nx:H=Mã nhập xuất (Tk nợ);ten_tk:H=Tên nhập xuất", "Danh sách mã nhập xuất (Tk nợ) không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");

                        result = false;
                        break;
                    case "ma_qs":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_qs:H=Quyển sổ", "Danh sách quyển sổ không có trong danh mục quyển sổ hoặc chứng từ không thuộc quyển sổ này");
                        else
                            BrowseError(tbl, "ma_qs:H=Quyển sổ e", "Danh sách quyển sổ không có trong danh mục quyển sổ hoặc chứng từ không thuộc quyển sổ này e");
                        result = false;
                        break;
                    case "so_ct":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_qs:H=Quyển sổ;so_ct:H=Số chứng từ", "Danh sách số chứng từ trùng số");
                        else
                            BrowseError(tbl, "ma_qs:H=Quyển sổ e;so_ct:H=Số chứng từ e", "Danh sách số chứng từ trùng số e");
                        result = false;
                        break;
                    case "tk_dt":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "tk_dt:H=Tk doanh thu;ten_tk:H=Tên tài khoản", "Danh sách tài khoản doanh thu không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");
                        else
                            BrowseError(tbl, "tk_dt:H=Tk doanh thu;ten_tk:H=Tên tài khoản", "Danh sách tài khoản doanh thu không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");

                        result = false;
                        break;
                    case "tk_ck":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "tk_ck:H=Tk chiết khấu;ten_tk:H=Tên tài khoản", "Danh sách tài khoản chiết khấu không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");
                        else
                            BrowseError(tbl, "tk_ck:H=Tk chiết khấu;ten_tk:H=Tên tài khoản", "Danh sách tài khoản chiết khấu không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");

                        result = false;
                        break;
                    case "tk_thue_i":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "tk_thue_i:H=Tài khoản thuế;ten_tk:H=Tên tài khoản", "Danh sách tài khoản thuế không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");
                        else
                            BrowseError(tbl, "tk_thue_i:H=Tài khoản thuế;ten_tk:H=Tên tài khoản", "Danh sách tài khoản thuế không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");

                        result = false;
                        break;
                    case "ma_dvcs":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_dvcs:H=Mã ĐVCS", "Danh sách mã ĐVCS không có trong danh mục ĐVCS");
                        else
                            BrowseError(tbl, "ma_dvcs:H=Mã ĐVCS", "Danh sách mã ĐVCS không có trong danh mục ĐVCS e");

                        result = false;
                        break;
                    case "ngay_ct":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ngay_ct:H=Ngày chứng từ:D", "Danh sách ngày chứng từ nhỏ hơn ngày mở sổ");
                        else
                            BrowseError(tbl, "ngay_ct:H=Ngày chứng từ:D", "Danh sách ngày chứng từ nhỏ hơn ngày mở sổ");

                        result = false;
                        break;
                    case "ma_vv_i":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_vv_i:H=Mã dự án", "Danh sách mã dự án không có trong danh mục dự án");
                        else
                            BrowseError(tbl, "ma_vv_i:H=Mã dự án", "Danh sách mã dự án không có trong danh mục dự án");

                        result = false;
                        break;
                    case "so_ct_khac_ngay":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "so_ct_khac_ngay:H=Số c.từ;ma_qs:H=Mã quyển sổ", "Danh sách c.từ có ngày c.từ khác nhau");
                        else
                            BrowseError(tbl, "so_ct_khac_ngay:H=Số c.từ;ma_qs:H=Mã quyển sổ", "Danh sách c.từ có ngày c.từ khác nhau");

                        result = false;
                        break;

                }
            }
            return result;
        }

        private static bool ShowError_HD4(DataSet dsError)
        {
            bool result = true;
            foreach (DataTable tbl in dsError.Tables)
            {
                if (tbl.Rows.Count == 0)
                    continue;
                switch (tbl.Columns[0].ColumnName)
                {
                    case "ma_kh":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_kh:H=Mã khách hàng", "Danh sách mã khách hàng không có trong danh mục khách hàng");
                        else
                            BrowseError(tbl, "ma_kh:H=Mã khách hàng", "Danh sách mã khách hàng không có trong danh mục khách hàng e");

                        result = false;
                        break;


                    case "ma_nx":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_nx:H=Mã nhập xuất (Tk nợ);ten_tk:H=Tên nhập xuất", "Danh sách mã nhập xuất (Tk nợ) không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");
                        else
                            BrowseError(tbl, "ma_nx:H=Mã nhập xuất (Tk nợ);ten_tk:H=Tên nhập xuất", "Danh sách mã nhập xuất (Tk nợ) không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");

                        result = false;
                        break;
                    case "ma_qs":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_qs:H=Quyển sổ", "Danh sách quyển sổ không có trong danh mục quyển sổ hoặc chứng từ không thuộc quyển sổ này");
                        else
                            BrowseError(tbl, "ma_qs:H=Quyển sổ e", "Danh sách quyển sổ không có trong danh mục quyển sổ hoặc chứng từ không thuộc quyển sổ này e");
                        result = false;
                        break;
                    case "so_ct":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_qs:H=Quyển sổ;so_ct:H=Số chứng từ", "Danh sách số chứng từ trùng số");
                        else
                            BrowseError(tbl, "ma_qs:H=Quyển sổ e;so_ct:H=Số chứng từ e", "Danh sách số chứng từ trùng số e");
                        result = false;
                        break;
                    case "tk_dt":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "tk_dt:H=Tk doanh thu;ten_tk:H=Tên tài khoản", "Danh sách tài khoản doanh thu không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");
                        else
                            BrowseError(tbl, "tk_dt:H=Tk doanh thu;ten_tk:H=Tên tài khoản", "Danh sách tài khoản doanh thu không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");

                        result = false;
                        break;
                  
                    case "tk_thue_i":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "tk_thue_i:H=Tài khoản thuế;ten_tk:H=Tên tài khoản", "Danh sách tài khoản thuế không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");
                        else
                            BrowseError(tbl, "tk_thue_i:H=Tài khoản thuế;ten_tk:H=Tên tài khoản", "Danh sách tài khoản thuế không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");

                        result = false;
                        break;
                    case "ma_dvcs":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_dvcs:H=Mã ĐVCS", "Danh sách mã ĐVCS không có trong danh mục ĐVCS");
                        else
                            BrowseError(tbl, "ma_dvcs:H=Mã ĐVCS", "Danh sách mã ĐVCS không có trong danh mục ĐVCS e");

                        result = false;
                        break;
                    case "ngay_ct":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ngay_ct:H=Ngày chứng từ:D", "Danh sách ngày chứng từ nhỏ hơn ngày mở sổ");
                        else
                            BrowseError(tbl, "ngay_ct:H=Ngày chứng từ:D", "Danh sách ngày chứng từ nhỏ hơn ngày mở sổ");

                        result = false;
                        break;
                    case "ma_vv_i":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_vv_i:H=Mã dự án", "Danh sách mã dự án không có trong danh mục dự án");
                        else
                            BrowseError(tbl, "ma_vv_i:H=Mã dự án", "Danh sách mã dự án không có trong danh mục dự án");

                        result = false;
                        break;
                    case "ma_bp":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_bp:H=Mã bộ phận", "Danh sách mã NVBH không có trong danh mục NV bán hàng");
                        else
                            BrowseError(tbl, "ma_bp:H=Mã bộ phận", "Danh sách mã NVBH không có trong danh mục NV bán hàng");

                        result = false;
                        break;
                    case "so_ct_khac_ngay":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "so_ct_khac_ngay:H=Số c.từ;ma_qs:H=Mã quyển sổ", "Danh sách c.từ có ngày c.từ khác nhau");
                        else
                            BrowseError(tbl, "so_ct_khac_ngay:H=Số c.từ;ma_qs:H=Mã quyển sổ", "Danh sách c.từ có ngày c.từ khác nhau");

                        result = false;
                        break;

                }
            }
            return result;
        }


        private static bool ShowError_PN1(DataSet dsError)
        {
            bool result = true;
            foreach (DataTable tbl in dsError.Tables)
            {
                if (tbl.Rows.Count == 0)
                    continue;
                switch (tbl.Columns[0].ColumnName)
                {
                    case "ma_kh":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_kh:H=Mã khách hàng", "Danh sách mã khách hàng không có trong danh mục khách hàng");
                        else
                            BrowseError(tbl, "ma_kh:H=Mã khách hàng", "Danh sách mã khách hàng không có trong danh mục khách hàng e");

                        result = false;
                        break;


                    case "ma_nx":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_nx:H=Mã nhập xuất (Tk có);ten_tk:H=Tên nhập xuất", "Danh sách mã nhập xuất (Tk có) không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");
                        else
                            BrowseError(tbl, "ma_nx:H=Mã nhập xuất (Tk có);ten_tk:H=Tên nhập xuất", "Danh sách mã nhập xuất (Tk có) không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");

                        result = false;
                        break;
                    case "ma_qs":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_qs:H=Quyển sổ", "Danh sách quyển sổ không có trong danh mục quyển sổ hoặc chứng từ không thuộc quyển sổ này");
                        else
                            BrowseError(tbl, "ma_qs:H=Quyển sổ e", "Danh sách quyển sổ không có trong danh mục quyển sổ hoặc chứng từ không thuộc quyển sổ này e");
                        result = false;
                        break;
                    case "so_ct":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_qs:H=Quyển sổ;so_ct:H=Số chứng từ", "Danh sách số chứng từ trùng số");
                        else
                            BrowseError(tbl, "ma_qs:H=Quyển sổ e;so_ct:H=Số chứng từ e", "Danh sách số chứng từ trùng số e");
                        result = false;
                        break;
                    case "tk_vt":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "tk_vt:H=Tk doanh thu;ten_tk:H=Tên tài khoản", "Danh sách tài khoản nợ không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");
                        else
                            BrowseError(tbl, "tk_vt:H=Tk doanh thu;ten_tk:H=Tên tài khoản", "Danh sách tài khoản nợ không có trong danh mục tài khoản hoặc là tài khoản tổng hợp");

                        result = false;
                        break;
                   
                    case "ma_dvcs":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_dvcs:H=Mã ĐVCS", "Danh sách mã ĐVCS không có trong danh mục ĐVCS");
                        else
                            BrowseError(tbl, "ma_dvcs:H=Mã ĐVCS", "Danh sách mã ĐVCS không có trong danh mục ĐVCS e");

                        result = false;
                        break;
                    case "ngay_ct":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ngay_ct:H=Ngày chứng từ:D", "Danh sách ngày chứng từ nhỏ hơn ngày mở sổ");
                        else
                            BrowseError(tbl, "ngay_ct:H=Ngày chứng từ:D", "Danh sách ngày chứng từ nhỏ hơn ngày mở sổ");

                        result = false;
                        break;
                    case "ma_vv_i":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "ma_vv_i:H=Mã dự án", "Danh sách mã dự án không có trong danh mục dự án");
                        else
                            BrowseError(tbl, "ma_vv_i:H=Mã dự án", "Danh sách mã dự án không có trong danh mục dự án");

                        result = false;
                        break;
                    case "so_ct_khac_ngay":
                        if (StartUp.M_LAN == "V")
                            BrowseError(tbl, "so_ct_khac_ngay:H=Số c.từ;ma_qs:H=Mã quyển sổ", "Danh sách c.từ có ngày c.từ khác nhau");
                        else
                            BrowseError(tbl, "so_ct_khac_ngay:H=Số c.từ;ma_qs:H=Mã quyển sổ", "Danh sách c.từ có ngày c.từ khác nhau");

                        result = false;
                        break;

                }
            }
            return result;
        }

        public static void BrowseError(DataTable data, string fields, string title)
        {
            if (StartUp.waiting != null)
                StartUp.waiting.Close();
            //SendMessage(StartUp.SysObj.HandleWaiting, RF_PROCESSWAITING, IntPtr.Zero, new IntPtr((int)'1'));
            FormBrowse oBrowse = new FormBrowse(StartUp.SysObj, data.DefaultView, fields);
            oBrowse.frmBrw.Title = SmLib.SysFunc.Cat_Dau(title);
            oBrowse.frmBrw.LanguageID = "AA_SMIMEXCT_4";
            oBrowse.ShowDialog();
        }


    }


}
