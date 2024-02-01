using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using SmDataLib;
using SmLib.SM.FormBrowse;
using System.Data;
using Infragistics.Windows.DataPresenter;
using System.Windows;
using SmDefine;
using SmErrorLib;
using System.Windows.Input;
using Sm.Windows.Controls;
using SmLib;
using System.Diagnostics;
using SmLib.SM.FrameBrowse;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Media;

namespace Arkh
{
    public class StartUp:StartupBase
    {

        static public string sqlTableUpdateName = "dmkh";
        static public string sqlTableName = "dmkh0";
        static public string sqlTableView = "v_dmkh";
        static public DataRow CommandInfo;
     
        static public string SqlTableKey = "ma_kh";
        static public string SqlTableObjectName = "ten_kh";

        static public string TableName = "khách hàng";
        static public string TableName_CatDau = "khach hang";
        static public string TableName_CatDau2 = "customer";
        static public SmLib.SM.FrameBrowse.FrameBrowse oBrowse;
        

        static public ActionTask currActionTask = ActionTask.None;
        static public string currSqlTableKey = string.Empty;
        static public string titleWindow = string.Empty;
        static public bool IsLoadFromLookUp = false;

        static public DataTable LastEditTable = null;
       

        #region Run
        public override void Run()
        {
           Namespace = "Arkh";
            try
            {
               
                M_LAN = SysObj.GetOption("M_LAN").ToString();
             
                CommandInfo =SysFunc.GetCommandInfo(SysObj, Menu_Id);

                oBrowse = new SmLib.SM.FrameBrowse.FrameBrowse(SysObj, sqlTableName);
                oBrowse.frmBrw.Title = SmLib.SysFunc.Cat_Dau(M_LAN.Equals("V") ? CommandInfo["bar"].ToString() : CommandInfo["bar2"].ToString());
                TableName = M_LAN.Equals("V") ? "khách hàng" : "customer";
                SmLib.SysFunc.LoadIcon(oBrowse.frmBrw);
                
                oBrowse.F2 +=new SmLib.SM.FrameBrowse.FrameBrowse.GridKeyUp_F2(oBrowse_F2);
                oBrowse.F3 +=new SmLib.SM.FrameBrowse.FrameBrowse.GridKeyUp_F3(oBrowse_F3);
                oBrowse.F4 +=new SmLib.SM.FrameBrowse.FrameBrowse.GridKeyUp_F4(oBrowse_F4);
                oBrowse.Ctrl_F4+=new SmLib.SM.FrameBrowse.FrameBrowse.GridKeyUp_Ctrl_F4(oBrowse_Ctrl_F4);
                oBrowse.F6 += new SmLib.SM.FrameBrowse.FrameBrowse.GridKeyUp_F6(oBrowse_F6);
                oBrowse.F7 += new SmLib.SM.FrameBrowse.FrameBrowse.GridKeyUp_F7(oBrowse_F7);
                oBrowse.F8 +=new SmLib.SM.FrameBrowse.FrameBrowse.GridKeyUp_F8(oBrowse_F8);
                //oBrowse.F11 += new FrameBrowse.GridKeyUp_F11(oBrowse_F11);
                oBrowse.frmBrw.ToolBar.Cm_Moi += new ListToolBar.Execute(ToolBar_Cm_Moi);
                oBrowse.frmBrw.ToolBar.Cm_Sua += new ListToolBar.Execute(ToolBar_Cm_Sua);
                oBrowse.frmBrw.ToolBar.Cm_Xoa += new ListToolBar.Execute(ToolBar_Cm_Xoa);
                oBrowse.frmBrw.ToolBar.Cm_In += new ListToolBar.Execute(ToolBar_Cm_In);
                oBrowse.frmBrw.ToolBar.Cm_Copy += new ListToolBar.Execute(ToolBar_Cm_Copy);
                oBrowse.frmBrw.ToolBar.Cm_DoiMa += new ListToolBar.Execute(ToolBar_Cm_DoiMa);
                oBrowse.frmBrw.ShowInTaskbar = true;
              
                object objTb = oBrowse.frmBrw.ToolBar.FindName("tbList");
                if (objTb != null)
                {
                    ToolBar tb = objTb as ToolBar;
                    ToolBarButton btSapXep = new ToolBarButton();
                    btSapXep.ImagePath = "Images\\Preview.png";
                    btSapXep.Text = M_LAN.Equals("V") ? "Sắp xếp" : "Sort";
                    btSapXep.Click += new RoutedEventHandler(btSapXep_Click);

                    //tb.Items.Insert(8, btSapXep);
                }
                //GetStrSearch();
                oBrowse.frmBrw.LanguageID  = "Arkh_1";
                oBrowse.ShowDialog();
            }
            catch (Exception ex)
            {
                ErrorLog.CatchMessage(ex);
            }
        }

        void  btSapXep_Click(object sender, RoutedEventArgs e)
        {
 	         SapXep(null, null);
        }
        #endregion

        void oBrowse_F11(object sender, EventArgs e)
        {
            SapXep(null, null);
        }

        void SapXep(object sender, System.Windows.Input.KeyEventArgs e)
        {

            ArkhF10 win = new ArkhF10();

            SmLib.SysFunc.LoadIcon(win);
            win.Title = SmLib.SysFunc.Cat_Dau(win.Title);

            if (!win.ShowDialog())
                return;

            XamDataGrid grd = FindDataGrid(oBrowse.frmBrw);
            if (grd == null)
                return;
            grd.FieldLayouts[0].SortedFields.Clear();
            switch (win.txtSapXep.Text.Trim())
            {
                case "1":
                    grd.FieldLayouts[0].SortedFields.Add(new FieldSortDescription("ma_kh", System.ComponentModel.ListSortDirection.Ascending, false));
                    break;
                case "2":
                    grd.FieldLayouts[0].SortedFields.Add(new FieldSortDescription("ten_kh", System.ComponentModel.ListSortDirection.Ascending, false));
                    break;
                case "3":
                    grd.FieldLayouts[0].SortedFields.Add(new FieldSortDescription("nh_kh1", System.ComponentModel.ListSortDirection.Ascending, false));
                    break;
                case "4":
                    grd.FieldLayouts[0].SortedFields.Add(new FieldSortDescription("nh_kh2", System.ComponentModel.ListSortDirection.Ascending, false));
                    break;
                case "5":
                    grd.FieldLayouts[0].SortedFields.Add(new FieldSortDescription("nh_kh3", System.ComponentModel.ListSortDirection.Ascending, false));
                    break;
            }

        }

        private XamDataGrid FindDataGrid(DependencyObject parent)
        {
            if(parent == null)
                return null;
            if(parent is XamDataGrid)
                return parent as XamDataGrid;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = FindDataGrid(VisualTreeHelper.GetChild(parent, i));
                if (child != null && child is XamDataGrid)
                    return child as XamDataGrid;
            }
            return null;
            
        }
        #region Ctrl_F4
        void oBrowse_Ctrl_F4(object sender, EventArgs e)
        {
            Copy();
        } 
        #endregion
        
        #region ToolBar_Cm_DoiMa
        void ToolBar_Cm_DoiMa()
        {
            DoiMa();
        }
        #endregion

        #region ToolBar_Cm_Copy
        void ToolBar_Cm_Copy()
        {
            Copy();
        }
        #endregion

        #region ToolBar_Cm_In
        void ToolBar_Cm_In()
        {
            In();
        }
        #endregion

        #region ToolBar_Cm_Xoa
        void ToolBar_Cm_Xoa()
        {
            Xoa();
        }
        #endregion

        #region ToolBar_Cm_Sua
        void ToolBar_Cm_Sua()
        {
            Sua();
        }
        #endregion

        #region ToolBar_Cm_Moi
        void ToolBar_Cm_Moi()
        {
            Them();
        }
        #endregion

        #region F2
        void oBrowse_F2(object sender, EventArgs e)
        {
            if (currActionTask != ActionTask.None || oBrowse.ActiveRecord == null)
                return;

            currActionTask = ActionTask.View;
            currSqlTableKey = oBrowse.ActiveRecord.Cells[SqlTableKey].Value.ToString();
            titleWindow = M_LAN.Equals("V") ? "Xem thông tin " + TableName_CatDau : "View " + TableName_CatDau2 + " information";
            showWindow();
        }
        #endregion

        #region F3
        void oBrowse_F3(object sender, EventArgs e)
        {
            Sua();
        }
        void Sua()
        {
            if (currActionTask != ActionTask.None || oBrowse.ActiveRecord == null)
                return;
            currActionTask = ActionTask.Edit;
            currSqlTableKey = oBrowse.ActiveRecord.Cells[SqlTableKey].Value.ToString();
            titleWindow = M_LAN.Equals("V") ? "Sửa thông tin " + TableName_CatDau : "Edit " + TableName_CatDau2 + " information";
            showWindow();
        }
        #endregion

        #region F4
        void oBrowse_F4(object sender, EventArgs e)
        {
            Them();
        }
        void Them()
        {
            if (currActionTask != ActionTask.None)
                return;
            currActionTask = ActionTask.Add;
            if (oBrowse.ActiveRecord != null)
            {
                currSqlTableKey = oBrowse.ActiveRecord.Cells[SqlTableKey].Value.ToString().Trim();
            }
            titleWindow = M_LAN.Equals("V") ? "Thêm " + TableName : "Add accumulative amount";
            showWindow();
            
        }
        #endregion 

        #region Copy
        void Copy()
        {
            if (currActionTask != ActionTask.None || oBrowse.ActiveRecord == null)
                return;
            currActionTask = ActionTask.Copy;
            currSqlTableKey = oBrowse.ActiveRecord.Cells[SqlTableKey].Value.ToString().Trim();
            titleWindow = M_LAN.Equals("V") ? "Thêm " + TableName : "Add accumulative amount";
            showWindow();
            
        }
        #endregion

        #region F6
        void oBrowse_F6(object sender, EventArgs e)
        {
            DoiMa();
        }
        void DoiMa()
        {
            
            if (currActionTask != ActionTask.None || oBrowse.ActiveRecord == null)
                return;

            string[] TitleAndName = new string[2];
            TitleAndName[0] = M_LAN.Equals("V") ? "Đổi mã khách hàng" : "Change customer ID";
            TitleAndName[1] = M_LAN.Equals("V") ? "ten_kh" : "ten_kh2";

            DataRecord rec = (oBrowse.ActiveRecord as DataRecord);
            rec.Cells[SqlTableKey].Value = rec.Cells[SqlTableKey].Value.ToString().Trim();
            SmLib.SysFunc.ChangeListID(SysObj, sqlTableUpdateName, oBrowse, SqlTableKey, TitleAndName);
           
        }
        #endregion

        #region F7
        void oBrowse_F7(object sender, EventArgs e)
        {
            In();
        }
        void In()
        {
            SqlCommand cmdPrint = new SqlCommand("select " + SmLib.SysFunc.GetFieldFromStrBrowse(oBrowse.Listinfo["full_field"].ToString())
                + " from " + sqlTableView);
            DataSet dsPrint = SysObj.ExcuteReader(cmdPrint);
            
            SmReport.ReportManager oReport = new SmReport.ReportManager(SysObj, CommandInfo["rep_file"].ToString());
            SmLib.SysFunc.DSCopyWithFilter(oBrowse.frmBrw.GetAllData(), ref dsPrint, 0);
            oReport.Preview(dsPrint);
        }
       
        #endregion

        #region F8
        void oBrowse_F8(object sender, EventArgs e)
        {
            Xoa();
        }
        void Xoa()
        {
            if (currActionTask != ActionTask.None || oBrowse.ActiveRecord == null)
                return;
            currSqlTableKey = oBrowse.ActiveRecord.Cells[SqlTableKey].Value.ToString();
            currActionTask = ActionTask.Delete;
            if (SmLib.SysFunc.CheckPermission(SysObj, currActionTask, Menu_Id))
            {
                try
                {
                    System.Data.SqlClient.SqlCommand cmdGet = new System.Data.SqlClient.SqlCommand("exec dbo.CheckDeleteListId @ma_dm, @" + SqlTableKey);
                    cmdGet.Parameters.Add("@ma_dm", SqlDbType.Char).Value = sqlTableUpdateName;
                    cmdGet.Parameters.Add("@" + SqlTableKey, SqlDbType.Char).Value = currSqlTableKey;
                    int ListDelete = (int)SysObj.ExcuteScalar(cmdGet);
                    if (ListDelete > 0)
                    {
                        if (ExMessageBox.Show( 1810,SysObj, "Có chắc chắn xóa không?", "Xac nhan nhap lieu", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                        {
                            deleteRowByKey(sqlTableUpdateName);
                        }
                    }
                    else
                    {
                         
                        ExMessageBox.Show( 1815,SysObj, "Đã có phát sinh không được xóa!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (SqlException ex)
                {
                    ErrorLog.CatchMessage(ex);
                }
            }
            else
            {
                ExMessageBox.Show( 1820,SysObj, "Không có quyền xóa " + "[" + TableName + "]" + "!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            currActionTask = ActionTask.None;
            oBrowse.frmBrw.ReloadData();
        }
        #endregion

        #region deleteRowByKey
        void deleteRowByKey(string tableName)
        {
            DataTable dt = GetRow(tableName);
            if (dt.Rows.Count > 0)
                SmDataLib.ListFunc.deleteRowInDatabaseByKey(tableName, SqlTableKey, dt.Rows[0], SysObj);
            
        }
        #endregion

        #region Extend_oBrowse_Command
        public DataTable Extend_oBrowse_Command(SysLib.SysObject SysObj, ActionTask Mode, string Current_Ma_kh)
        {
            currActionTask = Mode;
            if (SmLib.SysFunc.CheckPermission(SysObj, currActionTask, Menu_Id))
            {
                IsLoadFromLookUp = true;
                switch (Mode)
                {
                    case ActionTask.Add:
                        {
                            try
                            {
                                StartUp.SysObj = SysObj;
                                currSqlTableKey = Current_Ma_kh;
                                titleWindow = M_LAN.Equals("V") ? "Thêm " + TableName : "Add " + TableName_CatDau2;
                                showWindow();
                               
                            }
                            catch (Exception ex)
                            {
                                ErrorLog.CatchMessage(ex);
                            }
                        }
                        break;
                    case ActionTask.Copy:
                        {
                            try
                            {
                                StartUp.SysObj = SysObj;
                                currSqlTableKey = Current_Ma_kh;
                                titleWindow = M_LAN.Equals("V") ? "Thêm " + TableName : "Add " + TableName_CatDau2;
                                showWindow();

                            }
                            catch (Exception ex)
                            {
                                ErrorLog.CatchMessage(ex);
                            }
                        }
                        break;
                    case ActionTask.Edit:
                        {
                            try
                            {
                                StartUp.SysObj = SysObj;
                                currSqlTableKey = Current_Ma_kh;

                                titleWindow = M_LAN.Equals("V") ? "Sửa thông tin " + TableName : "Edit " + TableName_CatDau2 + " information";
                                showWindow();

                            }
                            catch (Exception ex)
                            {
                                ErrorLog.CatchMessage(ex);
                            }
                        }
                        break;
                    case ActionTask.View:
                        {
                            try
                            {
                                StartUp.SysObj = SysObj;
                                currSqlTableKey = Current_Ma_kh;

                                titleWindow = M_LAN.Equals("V") ? "Xem thông tin " + TableName : "View " + TableName_CatDau2 + " information";
                                showWindow();

                            }
                            catch (Exception ex)
                            {
                                ErrorLog.CatchMessage(ex);
                            }
                        }
                        break;
                }
            }
            else
            {
                ExMessageBox.Show( 1825,SysObj, "Không có quyền " + "[" + titleWindow.ToLower() + "]" + "!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                currActionTask = ActionTask.None;
            }

            return LastEditTable;
        }
        #endregion

        #region showWindow
        void showWindow()
        {
            if (SmLib.SysFunc.CheckPermission(SysObj, currActionTask, Menu_Id))
            {
                newForm window = new newForm();
                switch (currActionTask)
                {
                    case ActionTask.View:
                        {
                            window.EnableEditMode(false);
                        }
                        break;
                    case ActionTask.Edit:
                        {
                            window.EnableEditMode(true);
                        }
                        break;
                    case ActionTask.Copy:
                        {
                            window.EnableEditMode(true);
                        }
                        break;
                    case ActionTask.Add:
                        {
                            window.EnableEditMode(true);
                        }
                        break;
                }

                window.ShowDialog();
                if (currActionTask == ActionTask.View)
                    return;

                if (LastEditTable != null && oBrowse != null)
                    oBrowse.frmBrw.ReloadData(LastEditTable.Rows[0]);
                
            }
            else
            {
                ExMessageBox.Show( 1830,SysObj, "Không có quyền " + "[" + titleWindow.ToLower() + "]" + "!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                currActionTask = ActionTask.None;
            }
        }

        #endregion

        #region GetRow
        public static DataTable GetRow(string sqlTableName)
        {
            DataTable dt = null;
            SqlCommand cmdGet = new SqlCommand();
            try
            {
                string strSelect = "select * from " + sqlTableName + " where ";
                if (currActionTask == ActionTask.Add)
                {
                    strSelect += "1=0";
                }
                else
                {
                    DataTable TableFields = ListFunc.GetSqlTableFieldList(SysObj, sqlTableName);
                    string[] arrKeyValue = currSqlTableKey.Split(';');
                    string[] arrSqlTableKey = SqlTableKey.Split(';');
                    for (int i = 0; i < arrSqlTableKey.Count(); i++)
                    {
                        if (i > 0)
                            strSelect += " and";
                        DataRow[] FieldInfo = TableFields.Select("name='" + arrSqlTableKey[i] + "'");
                        strSelect += " " + arrSqlTableKey[i] + "=@" + arrSqlTableKey[i];
                        cmdGet.Parameters.Add("@" + arrSqlTableKey[i], ListFunc.GetSqlDBType(FieldInfo[0]["datatype"].ToString())).Value = arrKeyValue[i];
                    }

                }
                cmdGet.CommandText = strSelect;
                dt = SysObj.ExcuteReader(cmdGet).Tables[0];
                
            }
            catch (SqlException sqlex)
            {
                ErrorLog.CatchMessage(sqlex);
            }
            return dt;
        }
        #endregion\

       

        public static string CatChuoi(string lstString, string M_LAN)
        {
            string result = string.Empty;
            string[] stringSeparators = new string[] { "*|" };
            if (string.IsNullOrEmpty(lstString.Trim()))
                if (M_LAN.ToUpper().Trim().Equals("V"))
                {
                    result = lstString.Split(stringSeparators, StringSplitOptions.None)[0];
                }
                else
                {
                    result = lstString.Split(stringSeparators, StringSplitOptions.None)[1];
                }
            return result;
        }

    }
}

