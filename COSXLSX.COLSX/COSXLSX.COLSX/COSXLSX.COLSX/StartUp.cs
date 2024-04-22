using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sm.Windows.Controls;
using System.Data;
using System.Data.SqlClient;
using SmLib.SM.FormBrowse;
using SmLib.SM.FormBrowse2;
using System.Diagnostics;
using SmVoucherLib;
using Infragistics.Windows.DataPresenter;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using SmLib;
using System.Windows.Input;
using SmErrorLib;


namespace COSXLSX.COLSX
{
    class StartUp : StartUpTrans
    {
        static public string sqlTableName = "lsxph";
        static public string SqlTableKey = "so_lsx"; //index was outside the bound of array
        static public DataSet DataSourceReport = new DataSet();
        static private FormBrowse2 oBrowse;
       // static private DataRow CommandInfo;
      
        static public string M_USE_ID;
        static public FrmLoc _frmLoc;
        static public DateTime M_NGAY_KY1;

        static public string stringBrowse3 = "";
        static public string stringBrowse4 = "";

        static SqlCommand cmd = new SqlCommand();
        static public bool isNew = true;
        static public string so_lsx_new;
        static public string so_lsx_old;
        static public string Title = "";


        public override void Run()
        {
           Namespace = "COSXLSX.COLSX";
            try
            {
             
                CommandInfo = SmLib.SysFunc.GetCommandInfo(SysObj, Menu_Id);
                M_NGAY_KY1 = (DateTime)SysObj.GetSysVar("M_NGAY_KY1");

                Title = SysFunc.Cat_Dau(M_LAN.Equals("V") ? CommandInfo["bar"].ToString() : CommandInfo["bar2"].ToString());
                if (CommandInfo != null)
                {
                    _frmLoc = new FrmLoc();
                    SmLib.SysFunc.LoadIcon(_frmLoc);
                    _frmLoc.Title = Title;
                    _frmLoc.ShowDialog();

                }
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }

        static public string sMalsx, sMapx;
        static public object sStartDate, sEndDate;
        //static public int g_isCopy = 0;
        public static void CallGridVouchers(bool isFirstLoad,   object StartDate, object EndDate, string malsx, string mapx)
        {
            try
            {
                sStartDate=StartDate;
                sEndDate = EndDate;
                sMalsx = malsx;
                sMapx = mapx;
                //g_isCopy = isCopy;
                if (isFirstLoad)
                {
                    string strBrowse = "";
                    string strBrowse2 = "";
                    string[] listStored = CommandInfo["store_proc"].ToString().Split('|');

                    cmd = new SqlCommand();
                    cmd.CommandText = "Exec " + CommandInfo["store_proc"].ToString() + " @StartDate, @EndDate, @Malsx, @Mapx";

                    cmd.Parameters.Add("@StartDate", SqlDbType.VarChar).Value = string.IsNullOrEmpty(StartDate.ToString()) ? "" : String.Format("{0:yyyyMMdd}", (DateTime)StartDate);
                    cmd.Parameters.Add("@EndDate", SqlDbType.VarChar).Value = string.IsNullOrEmpty(EndDate.ToString()) ? "" : String.Format("{0:yyyyMMdd}", (DateTime)EndDate);
                    cmd.Parameters.Add("@Malsx", SqlDbType.VarChar).Value = malsx;
                    cmd.Parameters.Add("@Mapx", SqlDbType.VarChar).Value = mapx;

                    DataTable tbMain = StartUp.SysObj.ExcuteReader(cmd).Tables[0].Copy();
                    DataTable tbDetail = StartUp.SysObj.ExcuteReader(cmd).Tables[1].Copy();
                    //DataTable tbDetail2 = StartUp.SysObj.ExcuteReader(cmd).Tables[2].Copy();
                    //DataTable tbDetail3 = StartUp.SysObj.ExcuteReader(cmd).Tables[3].Copy();

                    tbMain.TableName = "tbMain";
                    tbDetail.TableName = "tbDetail";
                    //tbDetail2.TableName = "tbDetail2";
                    //tbDetail3.TableName = "tbDetail3";

                    DataSourceReport.Tables.Add(tbMain);
                    DataSourceReport.Tables.Add(tbDetail);
                    //DataSourceReport.Tables.Add(tbDetail2);
                    //DataSourceReport.Tables.Add(tbDetail3);

                    // tạo relation cho 2 table Header va Detail trong dataset DataSourceReport
                    DataColumn ParentCol = tbMain.Columns["so_lsx"];
                    DataColumn ChildrenCol = tbDetail.Columns["so_lsx"];

                    string RelationName = "so_lsx_Relation";
                    DataRelation dRelation = new DataRelation(RelationName, ParentCol, ChildrenCol, false);
                    DataSourceReport.Relations.Add(dRelation);

                    string[] arrStrBrowse1 = CommandInfo["VBrowse1"].ToString().Trim().Split('|');
                    if(M_LAN == "E")
                        arrStrBrowse1 = CommandInfo["EBrowse1"].ToString().Trim().Split('|');
                    strBrowse = arrStrBrowse1[0];
                    strBrowse2 = arrStrBrowse1[1];

                    //if (arrStrBrowse1 != null)
                    //{
 
                    //    stringBrowse3 = arrStrBrowse1[2];
                    //    stringBrowse4 = arrStrBrowse1[3];
                    //}

                    //tbMain.DefaultView.Sort = "ngay_ct , so_ct ASC";
                    //tbDetail.DefaultView.Sort = "ma_vt ASC";



                    oBrowse = new FormBrowse2(SysObj, tbMain.DefaultView, tbDetail.DefaultView, strBrowse, strBrowse2, "so_lsx");
                    oBrowse.Esc += new FormBrowse2.GridKeyUp_Esc(oBrowse_Esc);
                    oBrowse.CTRL_R += new FormBrowse2.GridKeyUp_CTRL_R(oBrowse_CTRL_R);
                  //  oBrowse.F3 += new FormBrowse2.GridKeyUp_F3(oBrowse_F3);
                  //  oBrowse.F4 += new FormBrowse2.GridKeyUp_F4(oBrowse_F4);
                  //  oBrowse.F8 += new FormBrowse2.GridKeyUp_F8(oBrowse_F8);
                    oBrowse.frmBrw.PreviewKeyDown += new System.Windows.Input.KeyEventHandler(frmBrw_PreviewKeyDown);
                    SmVoucherLib.ToolBarButton _new = new SmVoucherLib.ToolBarButton();
                    Sm.Windows.Controls.ToolBarButton btnMoi = new Sm.Windows.Controls.ToolBarButton();
                    btnMoi.Text = M_LAN.Equals("V") ? "Mới" : "New";
                    btnMoi.Name = "btnMoi";
                    btnMoi.ToolTip = "F4";
                    btnMoi.ImagePath = "Images\\AddNew.png";
                    btnMoi.BorderBrush = null;
                    btnMoi.Click += new RoutedEventHandler(btnMoi_Click);

                    Sm.Windows.Controls.ToolBarButton btnSua = new Sm.Windows.Controls.ToolBarButton();
                    btnSua.Text = M_LAN.Equals("V") ? "Sửa" : "Edit";
                    btnSua.Name = "btnSua";
                    btnSua.ToolTip = "F3";
                    btnSua.ImagePath = "Images\\Edit.png";
                    btnSua.BorderBrush = null;
                    
                    btnSua.Click += new RoutedEventHandler(btnSua_Click);


                    Sm.Windows.Controls.ToolBarButton btnXoa = new Sm.Windows.Controls.ToolBarButton();
                    btnXoa.Text =M_LAN.Equals("V") ? "Xóa" : "Delete";
                    btnXoa.Name = "btnXoa";
                    btnXoa.ToolTip = "F8";
                    btnXoa.ImagePath = "Images\\Delete.png";
                    btnXoa.BorderBrush = null;
                    btnXoa.Click += new RoutedEventHandler(btnXoa_Click);

                    Sm.Windows.Controls.ToolBarButton btnPrint = new Sm.Windows.Controls.ToolBarButton();
                    btnPrint.Text = M_LAN.Equals("V") ? "In" : "Edit";
                    btnPrint.Name = "btnIn";
                    btnPrint.ToolTip = "F";
                    btnPrint.ImagePath = "Images\\Edit.png";
                    btnPrint.BorderBrush = null;

                    btnPrint.Click += new RoutedEventHandler(btnIn_Click);


                    System.Windows.Controls.ToolBar ToolBar = (oBrowse.frmBrw.ToolBar.FindName("tbReport") as System.Windows.Controls.ToolBar);
                    if (ToolBar != null)
                    {
                        ToolBar.Items.Add(btnMoi);
                        ToolBar.Items.Add(btnSua);
                        ToolBar.Items.Add(btnXoa);
                        ToolBar.Items.Add(btnPrint);
                    }


                    //_new.Name = "tbNew";
                    //_new.Text = "Mới";
                    //_new.ToolTip = "F4, Ctrl+N";
                    //_new.ImagePath = @"Images\AddNew.png";
                    //_new.Click += new RoutedEventHandler(oBrowse_F4);
                    //_new.BorderBrush = null;

                    //SmVoucherLib.ToolBarButton _delete = new SmVoucherLib.ToolBarButton();
                    //_delete.Name = "btnDelete";
                    //_delete.Text = "Xóa";
                    //_delete.ToolTip = "F8";
                    //_delete.ImagePath = @"Images\Delete.png";
                    //_delete.Click += new RoutedEventHandler(oBrowse_F8);
                    //_delete.BorderBrush = null;

                    //SmVoucherLib.ToolBarButton _edit = new SmVoucherLib.ToolBarButton();
                    //_edit.Name = "btEdit";
                    //_edit.Text = "Sửa";
                    //_edit.ToolTip = "F3";
                    //_edit.ImagePath = @"Images\Edit.png";
                    //_edit.Click += new RoutedEventHandler(oBrowse_F3);
                    //_edit.BorderBrush = null;
                    //System.Windows.Controls.ToolBar ToolBar = (oBrowse.frmBrw.ToolBar.FindName("tbReport") as System.Windows.Controls.ToolBar);
                    //if (ToolBar != null)
                    //{
                    //    ToolBar.Items.Add(_new);
                    //    ToolBar.Items.Add(_edit);
                    //    ToolBar.Items.Add(_delete);
                    //}

                    oBrowse.frmBrw.oBrowse.FieldSettings.AllowEdit = false;
                    oBrowse.frmBrw.Title = SysFunc.Cat_Dau(M_LAN.Equals("V") ? CommandInfo["bar"].ToString() : CommandInfo["bar2"].ToString());
                    (oBrowse.frmBrw.ToolBar.FindName("btnEdit") as Button).Visibility = Visibility.Collapsed;
                    (oBrowse.frmBrw.ToolBar.FindName("btnPrint") as Button).Visibility = Visibility.Collapsed;
                    (oBrowse.frmBrw.ToolBar.FindName("btnDetail") as Button).Visibility = Visibility.Collapsed;
                    (oBrowse.frmBrw.ToolBar.FindName("btnOption") as Button).Visibility = Visibility.Collapsed;
                    (oBrowse.frmBrw.ToolBar.FindName("btnExport") as Button).Visibility = Visibility.Collapsed;
                    (oBrowse.frmBrw.ToolBar.FindName("btnRefresh") as Button).Visibility = Visibility.Collapsed;
                    if (oBrowse.frmBrw.oBrowse.Records.Count() == 0)
                        oBrowse.frmBrw.oBrowse.Focus();

                }
                else
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@StartDate", SqlDbType.VarChar).Value = string.IsNullOrEmpty(StartDate.ToString()) ? "" : String.Format("{0:yyyyMMdd}", (DateTime)StartDate);
                    cmd.Parameters.Add("@EndDate", SqlDbType.VarChar).Value = string.IsNullOrEmpty(EndDate.ToString()) ? "" : String.Format("{0:yyyyMMdd}", (DateTime)EndDate);
                    cmd.Parameters.Add("@Malsx", SqlDbType.VarChar).Value = malsx;
                    cmd.Parameters.Add("@Mapx", SqlDbType.VarChar).Value = mapx;

                    DataSourceReport.Relations.Remove("so_lsx_Relation");

                    DataSourceReport.Tables.Remove("tbMain");
                    DataSourceReport.Tables.Remove("tbDetail");

                    DataTable tbMain = SysObj.ExcuteReader(cmd).Tables[0].Copy();
                    tbMain.TableName = "tbMain";
                    oBrowse.frmBrw.oBrowse.DataSource = tbMain.DefaultView;
                    oBrowse.ObrowseView = tbMain.DefaultView;
                    DataSourceReport.Tables.Add(tbMain);

                    DataTable tbDetail = SysObj.ExcuteReader(cmd).Tables[1].Copy();
                    tbDetail.TableName = "tbDetail";
                    oBrowse.frmBrw.oBrowseCt.DataSource = tbDetail.DefaultView;
                    oBrowse.ObrowseViewCt = tbDetail.DefaultView;
                    DataSourceReport.Tables.Add(tbDetail);

                    DataColumn ParentCol = tbMain.Columns["so_lsx"];
                    DataColumn ChildrenCol = tbDetail.Columns["so_lsx"];
 

                    string RelationName = "so_lsx_Relation";
                    DataRelation dRelation = new DataRelation(RelationName, ParentCol, ChildrenCol, false);
                    DataSourceReport.Relations.Add(dRelation);


                    int _vitri = setFocusNew();
                    oBrowse.frmBrw.oBrowse.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        if (oBrowse.frmBrw.oBrowse.Records.Count() > 0)
                            oBrowse.frmBrw.oBrowse.ActiveRecord = oBrowse.frmBrw.oBrowse.Records[_vitri] as DataRecord;
                        else
                        {
                            oBrowse.frmBrw.oBrowse.Focus();
                        }

                    }));

                }

                if (isFirstLoad)
                {



                    oBrowse.frmBrw.LanguageID  = "COSXLSX.COLSX_3";
                    oBrowse.ShowDialog();
                    if (!Process.GetCurrentProcess().ProcessName.Equals("SmProcess"))
                        App.Current.Shutdown();

                }
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }

        }


        #region GetHdm
        public static DataSet GetHdb(string filter)
        {
            SqlCommand cmd = new SqlCommand("exec LoadVoucher @ma_ct, @PhFilter, @CtFilter, @GtFilter, @sl_ct");
            cmd.Parameters.Add("@ma_ct", SqlDbType.Char).Value = "HDB";
            cmd.Parameters.Add("@PhFilter", SqlDbType.NVarChar, 4000).Value = filter;
            cmd.Parameters.Add("@CtFilter", SqlDbType.NVarChar, 4000).Value = "1=1";
            cmd.Parameters.Add("@GtFilter", SqlDbType.NVarChar, 4000).Value = "";
            cmd.Parameters.Add("@sl_ct", SqlDbType.Int).Value = 0;

            DataSet ds = SmVoucherLib.DataProvider.FillCommand(SysObj, cmd);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                if (ds.Tables[0].Rows[i]["ma_nt"].ToString().Trim().ToUpper().Equals(StartUp.M_ma_nt0))
                {
                    DataRow[] ldr = ds.Tables[1].Select("stt_rec LIKE '" + ds.Tables[0].Rows[i]["stt_rec"].ToString() + "'");
                    foreach (DataRow dr in ldr)
                    {
                        dr["gia_nt2"] = 0;
                        dr["tien_nt2"] = 0;
                    }
                }
            }

            DataColumn dc = new DataColumn("tag", typeof(bool));
            dc.DefaultValue = false;
            ds.Tables[0].Columns.Add(dc);

            return ds;
        }
        #endregion

        static void frmBrw_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            
                if (Keyboard.IsKeyDown(Key.F3))
                    V_Sua();
                if (Keyboard.IsKeyDown(Key.F4))
                    V_Moi();
                if (Keyboard.IsKeyDown(Key.F8))
                    V_Xoa();
            
        }

        static int setFocusNew()
        {
            int n = oBrowse.frmBrw.oBrowse.Records.Count();

            for (int i = 0; i < n; i++)
            {
                if ((oBrowse.frmBrw.oBrowse.Records[i] as DataRecord).Cells["so_lsx"].Value.ToString().Trim() == so_lsx_new)
                    return i;
            }


            return 0;
        }


        static void btnSua_Click(object sender, RoutedEventArgs e)
        {
            
            V_Sua();
        }

        static void btnIn_Click(object sender, RoutedEventArgs e)
        {

            V_In();
        }
        static void btnMoi_Click(object sender, RoutedEventArgs e)
        {
            V_Moi();
        }
        static void btnXoa_Click(object sender, RoutedEventArgs e)
        {
 
            V_Xoa();
        }


        public static void V_Moi()
        {           
            isNew = true;
            so_lsx_new = "";
            FrmCapNhat fCN = new FrmCapNhat("");
            fCN.Title = M_LAN.Equals("V") ? "Them lenh san xuat" : "Add";
            fCN.ShowDialog();
        }

        public static void V_Sua()
        {
            isNew = false;
            if (oBrowse.ActiveRecord == null)  return;
            
            DataRecord record = oBrowse.frmBrw.oBrowse.ActiveRecord as DataRecord;

            if (record != null && record.RecordType == RecordType.DataRecord)
            {
                int index = oBrowse.frmBrw.oBrowse.ActiveRecord.Index; 
                string so_lsx = so_lsx_old =  record.Cells["so_lsx"].Value.ToString();                
                FrmCapNhat fCN = new FrmCapNhat(so_lsx);
                fCN.Title = M_LAN.Equals("V") ? "Sua thong tin lenh san xuat" : "Edit";
                fCN.ShowDialog();

                //index = 0;

                //if (DataSourceReport.Tables[0].DefaultView.Count > 0)
                //{
                //    for (int i = 0; i < DataSourceReport.Tables[0].DefaultView.Count; i++)
                //    {
                //        if (fCN.txtSo_lsx.Text.Trim().ToString().Equals(DataSourceReport.Tables[0].DefaultView[i]["so_lsx"].ToString().Trim()))
                //        {
                //            index = i;
                //            break;
                //        }
                //    }
                    oBrowse.frmBrw.oBrowse.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                    {
                        oBrowse.frmBrw.oBrowse.ActiveRecord = oBrowse.frmBrw.oBrowse.Records[index] as DataRecord;
                    }));
                //}

            }
        }

        public static void V_In()
        {
            isNew = false;
            if (oBrowse.ActiveRecord == null) return;

            DataRecord record = oBrowse.frmBrw.oBrowse.ActiveRecord as DataRecord;

            if (record != null && record.RecordType == RecordType.DataRecord)
            {
                int index = oBrowse.frmBrw.oBrowse.ActiveRecord.Index;
                string so_lsx = so_lsx_old = record.Cells["so_lsx"].Value.ToString();

                //FrmCapNhat fCN = new FrmCapNhat(so_lsx);

                FrmIn oReport = new FrmIn(so_lsx);
                oReport.ShowDialog();

                oReport.Title = M_LAN.Equals("V") ? "In lenh san xuat" : "Edit";

                oBrowse.frmBrw.oBrowse.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    oBrowse.frmBrw.oBrowse.ActiveRecord = oBrowse.frmBrw.oBrowse.Records[index] as DataRecord;
                }));

            }
        }

        public static void V_Xoa()
        {
            string so_lsx = string.Empty;
            if (oBrowse.ActiveRecord == null)
                return;
            DataRecord record = oBrowse.frmBrw.oBrowse.ActiveRecord as DataRecord;

            so_lsx = record.Cells["so_lsx"].Value.ToString();
            //Kiem tra da phat sinh chưa
            DataSet _ds = new DataSet();
            bool flag = false;
            //kiem tra co phat sinh trong [cosxlsx-cdvtsx] hay ko
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "select Count(*) as flag from [cosxlsx-cdvtsx] where rtrim(ltrim(so_lsx)) = '" + so_lsx + "'";
            _ds = StartUp.SysObj.ExcuteReader(cmd);
            
            if(_ds.Tables[0].Rows[0][0].ToString().Trim() != "0")
                flag = true;

            //kiem tra co phat sinh trong Cdspytcp hay ko
            if (flag == false)
            {                       
                SqlCommand cmd1 = new SqlCommand();
                _ds.Tables.Clear();
                cmd1.CommandText = "select Count(*) as flag from [cosxlsx-cdspytcp] where rtrim(ltrim(so_lsx)) = '" + so_lsx + "'";
                _ds = StartUp.SysObj.ExcuteReader(cmd1);
                if (_ds.Tables[0].Rows[0][0].ToString().Trim() != "0")
                    flag = true;
                
            }

            if (flag == false)
            {
                SqlCommand cmd1 = new SqlCommand();
                _ds.Tables.Clear();
                cmd1.CommandText = "select Count(*) as flag from [CT63] where rtrim(ltrim(so_lsx_i)) = '" + so_lsx + "'";
                _ds = StartUp.SysObj.ExcuteReader(cmd1);
                if (_ds.Tables[0].Rows[0][0].ToString().Trim() != "0")
                    flag = true;

            }

            if (flag == false)
            {
                if (ExMessageBox.Show(1935, StartUp.SysObj, "Có chắc chắn xóa không?", "Fast Accounting 11 .NET", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                {

                    SqlCommand cmd1 = new SqlCommand();
                    //Xoa Ph
                    cmd1.CommandText = "Delete lsxph Where so_lsx = @so_lsx";
                    cmd1.Parameters.Add("@so_lsx", SqlDbType.Char, 16).Value = so_lsx;
                    StartUp.SysObj.ExcuteNonQuery(cmd1);

                    //Xoa Ct
                    SqlCommand cmd2 = new SqlCommand();
                    cmd2.CommandText = "Delete lsxct Where so_lsx = @so_lsx";
                    cmd2.Parameters.Add("@so_lsx", SqlDbType.Char, 16).Value = so_lsx;
                    StartUp.SysObj.ExcuteNonQuery(cmd2);

                    //Xoa dmbplsx
                    SqlCommand cmd3 = new SqlCommand();
                    cmd3.CommandText = "Delete dmbplsx Where so_lsx = @so_lsx";
                    cmd3.Parameters.Add("@so_lsx", SqlDbType.Char, 16).Value = so_lsx;
                    StartUp.SysObj.ExcuteNonQuery(cmd3);
                    StartUp.CallGridVouchers(false, sStartDate, sEndDate, sMalsx, sMapx);
                }
            }
            else
            {

                ExMessageBox.Show(1275, SysObj, "Đã có phát sinh không được xóa!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        



        static void oBrowse_F8(object sender, EventArgs e)
        {
            V_Xoa();
        }

        static void oBrowse_F3(object sender, EventArgs e)
        {
            V_Sua();
        }

        static void oBrowse_F4(object sender, EventArgs e)
        {
            V_Moi();
        }

        static void oBrowse_CTRL_R(object sender, EventArgs e)
        {
            StartUp.CallGridVouchers(false, sStartDate, sEndDate, sMalsx, sMapx);
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
        public static void oBrowse_Esc(object sender, EventArgs e)
        {

        }
    }
}
