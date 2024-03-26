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

namespace SOGIA2_F3
{
    public class StartUp : StartupBase
    {

        static public string sqlTableName = "dmgia2";
        static public string sqlTableView = "v_dmgia2";
        static public DataRow CommandInfo;
        static public string SqlTableKey = "ma_vt;nh_kh3;ngay_ban";
        static public string SqlTableObjectName = "ten_vt";

        static public string TableName = "giá bán";
        static public string TableName_CatDau = "gia ban";
        static public string TableName_CatDau2 = "sale price";
        static public SmLib.SM.FrameBrowse.FrameBrowse oBrowse;


        static public ActionTask currActionTask = ActionTask.None;
        static public string currSqlTableKey = string.Empty;
        static public string titleWindow = string.Empty;
        static public bool IsLoadFromLookUp = false;

        static public DataTable LastEditTable = null;


        #region Run
        public override void Run()
        {
           Namespace = "SOGIA2_F3";
            try
            {

                M_LAN = SysObj.GetOption("M_LAN").ToString();

                CommandInfo = SysFunc.GetCommandInfo(SysObj, Menu_Id);

                oBrowse = new SmLib.SM.FrameBrowse.FrameBrowse(SysObj, sqlTableName);
                oBrowse.frmBrw.Title = SmLib.SysFunc.Cat_Dau(M_LAN.Equals("V") ? CommandInfo["bar"].ToString() : CommandInfo["bar2"].ToString());

                SmLib.SysFunc.LoadIcon(oBrowse.frmBrw);

                oBrowse.F2 += new SmLib.SM.FrameBrowse.FrameBrowse.GridKeyUp_F2(oBrowse_F2);
                oBrowse.F3 += new SmLib.SM.FrameBrowse.FrameBrowse.GridKeyUp_F3(oBrowse_F3);
                oBrowse.F4 += new SmLib.SM.FrameBrowse.FrameBrowse.GridKeyUp_F4(oBrowse_F4);
                oBrowse.Ctrl_F4 += new SmLib.SM.FrameBrowse.FrameBrowse.GridKeyUp_Ctrl_F4(oBrowse_Ctrl_F4);
               // oBrowse.F6 += new SmLib.SM.FrameBrowse.FrameBrowse.GridKeyUp_F6(oBrowse_F6);
                oBrowse.F7 += new SmLib.SM.FrameBrowse.FrameBrowse.GridKeyUp_F7(oBrowse_F7);
                oBrowse.F8 += new SmLib.SM.FrameBrowse.FrameBrowse.GridKeyUp_F8(oBrowse_F8);
                oBrowse.frmBrw.ToolBar.Cm_Moi += new ListToolBar.Execute(ToolBar_Cm_Moi);
                oBrowse.frmBrw.ToolBar.Cm_Sua += new ListToolBar.Execute(ToolBar_Cm_Sua);
                oBrowse.frmBrw.ToolBar.Cm_Xoa += new ListToolBar.Execute(ToolBar_Cm_Xoa);
                oBrowse.frmBrw.ToolBar.Cm_In += new ListToolBar.Execute(ToolBar_Cm_In);
                oBrowse.frmBrw.ToolBar.Cm_Copy += new ListToolBar.Execute(ToolBar_Cm_Copy);
                //oBrowse.frmBrw.ToolBar.Cm_DoiMa += new ListToolBar.Execute(ToolBar_Cm_DoiMa);
                oBrowse.frmBrw.ShowInTaskbar = true;


                UpdateFieldSearch();
                oBrowse.frmBrw.LanguageID  = "SOGIA2_F3_1";
                oBrowse.ShowDialog();
            }
            catch (Exception ex)
            {
                ErrorLog.CatchMessage(ex);
            }
        }
        #endregion

        #region Update truong search

        public void UpdateFieldSearch()
        {
            try
            {
                //khong can truyen tham so do da gan cung o store
                SqlCommand cmd = new SqlCommand("Exec [dbo].[SMDM_UPDATE_SEARCH_DMGIA2] @tableName, @tableView, @listKey, @strKey, @value");
                cmd.Parameters.Add("@tableName", SqlDbType.VarChar).Value = "";
                cmd.Parameters.Add("@tableView", SqlDbType.VarChar).Value = "";
                cmd.Parameters.Add("@listKey", SqlDbType.VarChar).Value = "";
                cmd.Parameters.Add("@strKey", SqlDbType.VarChar).Value = "";
                cmd.Parameters.Add("@value", SqlDbType.NVarChar).Value = "";
                SysObj.ExcuteNonQuery(cmd);
            }
            catch (Exception ex)
            {

            }
        }

        #endregion

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
            currSqlTableKey = strKeyValue();
            titleWindow = StartUp.M_LAN.Equals("V") ? ("Xem thông tin " + TableName) : ("View " + TableName_CatDau2 + " information");
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
            currSqlTableKey = strKeyValue();
            titleWindow = StartUp.M_LAN.Equals("V") ? ("Sửa thông tin " + TableName) : ("Edit " + TableName_CatDau2 + " information");
            showWindow();
        }
        #endregion
        string strKeyValue()
        {
            string[] arrSqlTableKey = SqlTableKey.Split(';');
            string keyvalue = string.Empty;
            string columnname = string.Empty;
            for (int i = 0; i < arrSqlTableKey.Count(); i++)
            {
                columnname = arrSqlTableKey[i];
                DataRowView _drv = oBrowse.ActiveRecord.DataItem as DataRowView;
                if (i == arrSqlTableKey.Count() - 1)
                    keyvalue += String.Format("{0:yyyyMMdd}", (DateTime)_drv[columnname]);
                else
                    keyvalue += _drv[columnname].ToString() + ";";
            }
            return keyvalue;
        }
        #region F4
        void oBrowse_F4(object sender, EventArgs e)
        {
            Them();
        }
        void Them()
        {
            if (currActionTask != ActionTask.None)
            {
                return;
            }
            currActionTask = ActionTask.Add;
            if(oBrowse.ActiveRecord != null)
                currSqlTableKey = strKeyValue();
            titleWindow = StartUp.M_LAN.Equals("V") ? ("Thêm " + TableName) : ("Add " + TableName_CatDau2);
            showWindow();

        }
        #endregion

        #region Copy
        void Copy()
        {
            if (currActionTask != ActionTask.None || oBrowse.ActiveRecord == null)
                return;
            currActionTask = ActionTask.Copy;
            currSqlTableKey = strKeyValue();
            titleWindow = StartUp.M_LAN.Equals("V") ? ("Thêm " + TableName) : ("Add " + TableName_CatDau2);
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
            SmLib.SysFunc.ChangeListID(SysObj, sqlTableName, oBrowse, SqlTableKey);

        }
        #endregion

        #region F7
        void oBrowse_F7(object sender, EventArgs e)
        {
            In();
        }
        void In()
        {
            SqlCommand cmdPrint = new SqlCommand("select * from " + sqlTableView);
            DataSet dsPrint = SysObj.ExcuteReader(cmdPrint);

            SmReport.ReportManager oReport = new SmReport.ReportManager(SysObj, CommandInfo["rep_file"].ToString());
           // SmLib.SysFunc.DSCopyWithFilter(oBrowse.frmBrw.GetAllData(), ref dsPrint, 0);
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
            currSqlTableKey = strKeyValue();
            currActionTask = ActionTask.Delete;
            if (SmLib.SysFunc.CheckPermission(SysObj, currActionTask, Menu_Id))
            {
                try
                {
                    //System.Data.SqlClient.SqlCommand cmdGet = new System.Data.SqlClient.SqlCommand("exec dbo.CheckDeleteListId @ma_dm, @" + SqlTableKey);
                    //cmdGet.Parameters.Add("@ma_dm", SqlDbType.Char).Value = sqlTableName;
                    //cmdGet.Parameters.Add("@" + SqlTableKey, SqlDbType.Char).Value = currSqlTableKey;
                    //int ListDelete = (int)SysObj.ExcuteScalar(cmdGet);
                    //if (ListDelete > 0)
                    //{
                        if (ExMessageBox.Show( 2435,SysObj, "Có chắc chắn xóa không?", "Xac nhan nhap lieu", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                        {
                            deleteRowByKey(sqlTableName);
                        }
                    //}
                    //else
                    //{

                    //    ExMessageBox.Show( 2440,SysObj, "Đã có phát sinh không được xóa!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                    //}
                }
                catch (SqlException ex)
                {
                    ErrorLog.CatchMessage(ex);
                }
            }
            else
            {
                ExMessageBox.Show( 2445,SysObj, "Không có quyền xóa " + "[" + TableName + "]" + "!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
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
                                titleWindow = StartUp.M_LAN.Equals("V") ? ("Thêm " + TableName) : ("Add " + TableName_CatDau2);
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

                                titleWindow = StartUp.M_LAN.Equals("V") ? ("Sửa thông tin " + TableName) : ("Edit " + TableName_CatDau2 + " information");
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

                                titleWindow = StartUp.M_LAN.Equals("V") ? ("Xem thông tin " + TableName) : ("View " + TableName_CatDau2 + " information");
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
                ExMessageBox.Show( 2450,SysObj, "Không có quyền " + "[" + titleWindow.ToLower() + "]" + "!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
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
                ExMessageBox.Show( 2455,SysObj, "Không có quyền " + "[" + titleWindow.ToLower() + "]" + "!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
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
                        if(i == 0)
                            cmdGet.Parameters.Add("@" + arrSqlTableKey[i], ListFunc.GetSqlDBType(FieldInfo[0]["datatype"].ToString())).Value = arrKeyValue[i];
                        else
                            cmdGet.Parameters.Add("@" + arrSqlTableKey[i], SqlDbType.VarChar).Value = arrKeyValue[i];

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
        #endregion
    }
}
