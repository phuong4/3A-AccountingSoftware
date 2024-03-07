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
using SmLib.SM.FrameBrowse;
using System.Diagnostics;

namespace AA_SmImexCt0
{
    public class StartUp : StartupBase
    {

        static public string sqlTableName = "dmbp";
        static public string sqlTableView = "v_dmbp";
        static public DataRow CommandInfo;
        static public string SqlTableKey = "ma_bp";
        static public string SqlTableObjectName = "ten_bp";

        static public string TableName = "nhân viên";
        static public string TableName_CatDau = "nhan vien";
        static public string TableName_CatDau2 = "staff";
        static public SmLib.SM.FrameBrowse.FrameBrowse oBrowse;


        static public ActionTask currActionTask = ActionTask.None;
        static public string currSqlTableKey = string.Empty;
        static public string titleWindow = string.Empty;
        static public string titleWindow2 = string.Empty;
        static public bool IsLoadFromLookUp = false;

        static public DataTable LastEditTable = null;
        static public string M_IP_TIEN;
        static public string M_IP_TIEN_NT;

        #region Run
        public override void Run()
        {
            Namespace = "AA_SmImexCt0";
            try
            {

                M_LAN = SysObj.GetOption("M_LAN").ToString();
                SysObj.SynchroFile(".", "AA_SMIMEXCT.exe");

                Process.Start("AA_SMIMEXCT.exe", string.Format("\"{0}\"", Menu_Id));

                if (Process.GetCurrentProcess().ProcessName != "SmProcess.exe")
                {
                    App.Current.Shutdown();
                }
            }
            catch (Exception ex)
            {
                ErrorLog.CatchMessage(ex);
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
            currSqlTableKey = oBrowse.ActiveRecord.Cells[SqlTableKey].Value.ToString();
            titleWindow = M_LAN.Equals("V") ? "Xem thong tin " + TableName_CatDau : "View " + TableName_CatDau2 + " information";
            titleWindow2 = M_LAN.Equals("V") ? "Xem thông tin " + TableName : "View " + TableName_CatDau2 + " information";
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
            titleWindow = M_LAN.Equals("V") ? "Sua thong tin " + TableName_CatDau : "Edit " + TableName_CatDau2 + " information";
            titleWindow2 = M_LAN.Equals("V") ? "Sửa thông tin " + TableName : "Edit " + TableName_CatDau2 + " information";
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
            titleWindow = M_LAN.Equals("V") ? "Them " + TableName_CatDau : "Add " + TableName_CatDau2;
            titleWindow2 = M_LAN.Equals("V") ? "Thêm " + TableName : "Add " + TableName_CatDau2;
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
            titleWindow = M_LAN.Equals("V") ? "Them " + TableName_CatDau : "Add " + TableName_CatDau2;
            titleWindow2 = M_LAN.Equals("V") ? "Thêm " + TableName : "Add " + TableName_CatDau2;
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
            var title = M_LAN.Equals("V") ? ("Doi ma " + TableName) : ("Change " + TableName_CatDau2 + " code");
            var tableAndName = new string[] { title, M_LAN.Equals("V") ? "ten_bp" : "ten_bp2" };
            SmLib.SysFunc.ChangeListID(SysObj, sqlTableName, oBrowse, SqlTableKey, tableAndName);
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
                    cmdGet.Parameters.Add("@ma_dm", SqlDbType.Char).Value = sqlTableName;
                    cmdGet.Parameters.Add("@" + SqlTableKey, SqlDbType.Char).Value = currSqlTableKey;
                    int ListDelete = (int)SysObj.ExcuteScalar(cmdGet);
                    if (ListDelete > 0)
                    {
                        if (ExMessageBox.Show(40, SysObj, "Có chắc chắn xóa không?", "Xac nhan nhap lieu", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                        {
                            deleteRowByKey(sqlTableName);
                        }
                    }
                    else
                    {

                        ExMessageBox.Show(45, SysObj, "Đã có phát sinh không được xóa!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (SqlException ex)
                {
                    ErrorLog.CatchMessage(ex);
                }
            }
            else
            {
                ExMessageBox.Show(50, SysObj, "Không có quyền xóa " + "[" + TableName + "]" + "!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
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
                                titleWindow = M_LAN.Equals("V") ? "Them " + TableName_CatDau : "Add " + TableName_CatDau2;
                                titleWindow2 = M_LAN.Equals("V") ? "Thêm " + TableName : "Add " + TableName_CatDau2;
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
                                titleWindow = M_LAN.Equals("V") ? "Them " + TableName_CatDau : "Add " + TableName_CatDau2;
                                titleWindow2 = M_LAN.Equals("V") ? "Thêm " + TableName : "Add " + TableName_CatDau2;
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

                                titleWindow = M_LAN.Equals("V") ? "Sua thong tin " + TableName_CatDau : "Edit " + TableName_CatDau2 + " information";
                                titleWindow2 = M_LAN.Equals("V") ? "Sửa thông tin " + TableName : "Edit " + TableName_CatDau2 + " information";
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

                                titleWindow = M_LAN.Equals("V") ? "Xem thong tin " + TableName_CatDau : "View " + TableName_CatDau2 + " information";
                                titleWindow2 = M_LAN.Equals("V") ? "Xem thông tin " + TableName : "View " + TableName_CatDau2 + " information";
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
                ExMessageBox.Show(55, SysObj, "Không có quyền " + "[" + titleWindow2.ToLower() + "]" + "!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
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
                ExMessageBox.Show(60, SysObj, "Không có quyền " + "[" + titleWindow2.ToLower() + "]" + "!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
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
        #endregion
    }
}
