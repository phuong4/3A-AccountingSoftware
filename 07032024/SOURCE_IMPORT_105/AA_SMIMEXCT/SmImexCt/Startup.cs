using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SmLib.SM.FormBrowse;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;
using Sm.Languages;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.DataPresenter.Events;
using Sm.Windows.Controls;
using System.Threading;
using System.Globalization;
using System.Diagnostics;
using Infragistics.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO;
using System.Data.OleDb;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Windows.Threading;


namespace AA_SMIMEXCT
{
    public class StartUp : StartupBase
    {
        public override void Run() { Namespace = "AA_SMIMEXCT"; Show(Menu_Id); }

        //private const int RF_PROCESSMESSAGE = 0xA123;
        //private const int RF_PROCESSWAITINGSHOW = 0xA126;
        //private const int RF_PROCESSWAITING = 0xA125;

        //[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        //public static extern int SendMessage(IntPtr hwnd, [MarshalAs(UnmanagedType.U4)] int Msg, IntPtr wParam, IntPtr lParam);

        private DataSet dsDmimexct;
        private DataRow drCommandInfo;
        private FormBrowse oBrowse;
        SqlCommand cmd_gia2 = new SqlCommand();
        public static string M_UPDATE_GIA2 = "";
        public static string Ma_nt0 = "VND";
        public static string Ws_Id { get; set; }
        public static int _User_id = 1;
        public static string _paths = "";
        public static string ma_imex_truoc = "";
        public static FrmWaiting waiting;
        public static DataSet DataImport;
        void Show(string id)
        {

            //id = StartUp.SysObj.GetSysVar("M_ID_AA_SMIMEXCT").ToString();
            drCommandInfo = SmLib.SysFunc.GetCommandInfo(SysObj, id);
            Ma_nt0 = (string)AA_SMIMEXCT.StartUp.SysObj.GetOption("M_MA_NT0");
            M_UPDATE_GIA2 = SysObj.GetOption("M_UPDATE_GIA2").ToString().Trim();
            _User_id = (int)SysObj.UserInfo.Rows[0][0];

            if (drCommandInfo == null || drCommandInfo.ItemArray.Length == 0)
            {
                if (!Process.GetCurrentProcess().ProcessName.Equals("SmProcess"))
                    App.Current.Shutdown();
                return;
            }

            SqlCommand cmd = new SqlCommand(drCommandInfo["store_proc"].ToString());
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@ma_phan_he", SqlDbType.Char).Value = string.IsNullOrEmpty(drCommandInfo["parameter"].ToString()) ? "%%" : drCommandInfo["parameter"].ToString();

            dsDmimexct = SysObj.ExcuteReader(cmd);
            if (dsDmimexct == null || dsDmimexct.Tables.Count == 0)
            {
                if (!Process.GetCurrentProcess().ProcessName.Equals("SmProcess"))
                    App.Current.Shutdown();
                return;
            }
            string[] sFieldArrays;
            if (M_LAN == "V")
                    sFieldArrays = drCommandInfo["Vbrowse1"].ToString().Split("|".ToCharArray());
            else
                    sFieldArrays = drCommandInfo["Ebrowse1"].ToString().Split("|".ToCharArray());

            string fields = sFieldArrays[0];
           
            oBrowse = new FormBrowse(SysObj, dsDmimexct.Tables[0].DefaultView, fields);
            oBrowse.CTRL_R += new FormBrowse.GridKeyUp_CTRL_R(oBrowse_CTRL_R);
            oBrowse.F4 += new FormBrowse.GridKeyUp_F4(oBrowse_F4);
            oBrowse.F5 += new FormBrowse.GridKeyUp_F5(oBrowse_F5);
            oBrowse.frmBrw.Closed += new EventHandler(frmBrw_Closed);
            SmLib.SysFunc.LoadIcon(oBrowse.frmBrw);

            object objTb = oBrowse.frmBrw.ToolBar.FindName("tbReport");
            if (objTb != null)
            {
                ToolBar tb = objTb as ToolBar;
                (tb.Items[2] as ToolBarButton).Text = "Lấy dữ liệu";
                (tb.Items[2] as ToolBarButton).ToolTip = "F4";
                (tb.Items[2] as ToolBarButton).IsEnabled = true;
                (tb.Items[2] as ToolBarButton).Click += new RoutedEventHandler(StartUp_Click);
            }

            oBrowse.frmBrw.DisplayLanguage = M_LAN;
            oBrowse.frmBrw.Title = SmLib.SysFunc.Cat_Dau(M_LAN.Equals("V") ? drCommandInfo["bar"].ToString() : drCommandInfo["bar2"].ToString());
            oBrowse.frmBrw.LanguageID  = "AA_SMIMEXCT_3";
            oBrowse.ShowDialog();

            if (!Process.GetCurrentProcess().ProcessName.Equals("SmProcess"))
                App.Current.Shutdown();
        }

        void frmBrw_Closed(object sender, EventArgs e)
        {
            cmd_gia2.CommandText = "update options set value = '" + M_UPDATE_GIA2.Trim() + "' where name = 'M_UPDATE_GIA2'";
            StartUp.SysObj.ExcuteNonQuery(cmd_gia2);

            Environment.Exit(Environment.ExitCode);
        }


        void StartUp_Click(object sender, RoutedEventArgs e)
        {
            oBrowse_F4(null, null);
        }

        void oBrowse_F5(object sender, EventArgs e)
        {
            
        }

        void oBrowse_F4(object sender, EventArgs e)
        {
      
            if (oBrowse.DataGrid.ActiveRecord == null || oBrowse.DataGrid.ActiveRecord.RecordType != RecordType.DataRecord)
                return;
            AA_SMIMEXCTF4 f4 = new AA_SMIMEXCTF4();

            f4.DataContext = (oBrowse.DataGrid.ActiveRecord as DataRecord).DataItem;

            f4.Title = SmLib.SysFunc.Cat_Dau(f4.Title);
            f4.BindingSysObj = SysObj;
            SmLib.SysFunc.LoadIcon(f4);
            
            if (!f4.ShowDialog())
                return;

           
         //   SendMessage(StartUp.SysObj.HandleWaiting, RF_PROCESSWAITINGSHOW, IntPtr.Zero, new IntPtr((int)'1'));
            waiting = new FrmWaiting(2);
            waiting.Show();
            if (ThucThi(f4))
            {
                if (waiting != null)
                    waiting.Close();
              //  SendMessage(StartUp.SysObj.HandleWaiting, RF_PROCESSWAITING, IntPtr.Zero, new IntPtr((int)'1'));
                ExMessageBox.Show(170, StartUp.SysObj, "Chương trình đã thực hiện xong!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                if (waiting != null)
                    waiting.Close();
              //  SendMessage(StartUp.SysObj.HandleWaiting, RF_PROCESSWAITING, IntPtr.Zero, new IntPtr((int)'1'));
                ExMessageBox.Show(175, StartUp.SysObj, "Chương trình huỷ bỏ số liệu đưa vào!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
           
        }

        private  bool ThucThi(AA_SMIMEXCTF4 imexCtF4)
        {
            try
            {
                DataImport = C_GetDataExcel.GetData(imexCtF4.Info, imexCtF4.txtBangMa.Text);
                if (DataImport == null)
                    return false;
                //Kiem tra các trường bắt buộc phải nhập
                if (!CheckKeyNull(DataImport.Tables["DataExcel"], imexCtF4.Info.FieldNotNull))
                    return false;
                //2.Dua du lieu xuong table template
                bool Check_Upload = C_ImportVoucher.UploadTable(imexCtF4.Info.TableTemplate, DataImport, C_GetDataExcel.StrBrowse);
                if (!Check_Upload)
                    return false;

                //3.Kiểm tra tính hợp lệ của dữ liệu
                bool Check_Post = C_ImportVoucher.Check_Data(imexCtF4.Info);
                if (!Check_Post)
                    return false;

                cmd_gia2.CommandText = "update options set value = '0' where name = 'M_UPDATE_GIA2'";
                StartUp.SysObj.ExcuteNonQuery(cmd_gia2);

                //4.Post
              //  bool Post = C_ImportVoucher.Post_All(imexCtF4.Info);
                bool Post = C_ImportVoucher.Post(imexCtF4.Info);
                if (!Post)
                    return false;
            }
            catch
            {
                return false;
            }
            finally
            {
                cmd_gia2.CommandText = "update options set value = '" + M_UPDATE_GIA2.Trim() + "' where name = 'M_UPDATE_GIA2'";
                StartUp.SysObj.ExcuteNonQuery(cmd_gia2);
            }
            return true;
            
        }

        //các cột không được phép bỏ trống (đc khai báo trong column khoa của table dmimex)
        private static bool CheckKeyNull(DataTable tb_excel, string list_key)
        {
            if (list_key.Trim() == "")
                return true;
            string[] key = list_key.Split(';');
            DataTable tb_Key_Null = tb_excel.Clone();

            for (int i = 0; i < tb_excel.Rows.Count; i++)
            {
                for (int j = 0; j < key.Length; j++)
                {
                    if (tb_excel.Columns.Contains(key[j].Trim()))
                    {
                        if (tb_excel.Rows[i][key[j].Trim()].ToString().Trim() == "")
                        {
                            tb_Key_Null.ImportRow(tb_excel.Rows[i]);
                            break;
                        }
                    }
                }
            }

            if (tb_Key_Null.Rows.Count > 0)
            {
                
                FormBrowse oBrowse = new FormBrowse(StartUp.SysObj, tb_Key_Null.DefaultView, C_GetDataExcel.StrBrowseFieldNull);
                oBrowse.frmBrw.Title = SmLib.SysFunc.Cat_Dau("Danh sách các cột bị bỏ trống");
                oBrowse.frmBrw.LanguageID = "AA_SMIMEXCT_41";
                oBrowse.ShowDialog();
               
                return false;
            }
            else
                return true;
        }


        void oBrowse_CTRL_R(object sender, EventArgs e)
        {

            SqlCommand cmd = new SqlCommand(drCommandInfo["store_proc"].ToString());
            cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.Add("@Type", SqlDbType.VarChar).Value = "DM";
            cmd.Parameters.Add("@ma_phan_he", SqlDbType.Char).Value = string.IsNullOrEmpty(drCommandInfo["parameter"].ToString()) ? "%%" : drCommandInfo["parameter"].ToString();

            dsDmimexct = SysObj.ExcuteReader(cmd);
            oBrowse.DataGrid.DataSource = dsDmimexct.Tables[0].DefaultView;
        }

    }

    internal struct ImportInfo
    {
        public string Ma_Imex { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
        public string TableTemplate { get; set; }
        public string ExcelTemplate { get; set; }
        public string PostProc { get; set; }
        public string Ma_ct { get; set; }
        public string Ma_qs { get; set; }
        public string Xy_ly { get; set; }
        public string FieldNotNull { get; set; }
        public string StrBrowseV { get; set; }
        public string StrBrowseE { get; set; }
    }
 
}
