using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using SmLib.SM.FormBrowse;
using System.Data;
using Infragistics.Windows.DataPresenter;
using SmDefine;
using SmErrorLib;
using Sm.Windows.Controls;
using SmLib.SM.FrameBrowse;
using System.Windows;
using System.Windows.Input;
using SmLib;

namespace Invt
{
    public class StartUp:StartupBase
    {

        static public string sqlTableListName = "dmvt0";
        static public string sqlTableName = "dmvt";
        static public string sqlTableView = "v_dmvt";
        static public string SqlTableKey = "ma_vt";
        static public string SqlTableObjectName = "ma_vt";

        static public string TableName;
        static public DataRow CommandInfo;

        static public FrameBrowse oBrowse;
        

        static public ActionTask currActionTask = ActionTask.None;
        static public string currSqlTableKey = string.Empty;
        static public string titleWindow = string.Empty;
        static public bool IsLoadFromLookUp = false;
         
        static public DataRow LastEditRow = null;

        #region Run
        public override void Run()
        {
           Namespace = "Invt";
            try
            {
                CommandInfo = SmLib.SysFunc.GetCommandInfo(SysObj, Menu_Id);
                titleWindow =M_LAN.Equals("V") ? SmLib.SysFunc.Cat_Dau(CommandInfo["bar"].ToString()) : SmLib.SysFunc.Cat_Dau(CommandInfo["bar2"].ToString());
                TableName = M_LAN.Equals("V") ? "vật tư" : "item";
                oBrowse = new SmLib.SM.FrameBrowse.FrameBrowse(SysObj, sqlTableListName);
                oBrowse.frmBrw.Title =titleWindow;

                SmLib.SysFunc.LoadIcon(oBrowse.frmBrw);

                oBrowse.F2 += new SmLib.SM.FrameBrowse.FrameBrowse.GridKeyUp_F2(oBrowse_F2);
                oBrowse.F3 += new SmLib.SM.FrameBrowse.FrameBrowse.GridKeyUp_F3(oBrowse_F3);
                oBrowse.F4 += new SmLib.SM.FrameBrowse.FrameBrowse.GridKeyUp_F4(oBrowse_F4);
                oBrowse.Ctrl_F4 +=new SmLib.SM.FrameBrowse.FrameBrowse.GridKeyUp_Ctrl_F4(oBrowse_Ctrl_F4);
                oBrowse.F6 += new SmLib.SM.FrameBrowse.FrameBrowse.GridKeyUp_F6(oBrowse_F6);
                oBrowse.F7 += new SmLib.SM.FrameBrowse.FrameBrowse.GridKeyUp_F7(oBrowse_F7);
                oBrowse.F8 += new SmLib.SM.FrameBrowse.FrameBrowse.GridKeyUp_F8(oBrowse_F8);
                oBrowse.frmBrw.ToolBar.Cm_Moi += new ListToolBar.Execute(ToolBar_Cm_Moi);
                oBrowse.frmBrw.ToolBar.Cm_Sua += new ListToolBar.Execute(ToolBar_Cm_Sua);
                oBrowse.frmBrw.ToolBar.Cm_Xoa += new ListToolBar.Execute(ToolBar_Cm_Xoa);
                oBrowse.frmBrw.ToolBar.Cm_In += new ListToolBar.Execute(ToolBar_Cm_In);
                oBrowse.frmBrw.ToolBar.Cm_Copy += new ListToolBar.Execute(ToolBar_Cm_Copy);
                oBrowse.frmBrw.ToolBar.Cm_DoiMa += new ListToolBar.Execute(ToolBar_Cm_DoiMa);
                

                oBrowse.frmBrw.LanguageID  = "Invt_1";
                oBrowse.ShowDialog();
                
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
            titleWindow = M_LAN.Equals("V") ? ("Xem thông tin " + TableName) : ("View " + TableName + " information");
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
            titleWindow = M_LAN.Equals("V") ? ("Sửa thông tin " + TableName) : ("Edit " + TableName + " information"); 
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
                currSqlTableKey = oBrowse.ActiveRecord.Cells[SqlTableKey].Value.ToString().Trim();
            else
                currSqlTableKey = "";
            titleWindow = M_LAN.Equals("V") ? ("Thêm " + TableName) : ("Add new " + TableName); 
            showWindow();
           
        }
        #endregion

        #region Copy
        void Copy()
        {
            if (currActionTask != ActionTask.None || oBrowse.ActiveRecord == null)
                return;
            currActionTask = ActionTask.Copy;
            if (oBrowse.ActiveRecord != null)
                currSqlTableKey = oBrowse.ActiveRecord.Cells[SqlTableKey].Value.ToString().Trim();
            else
                currSqlTableKey = "";
            titleWindow = M_LAN.Equals("V") ? ("Thêm " + TableName) : ("Add new " + TableName); 
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
            DataRecord rec = (oBrowse.ActiveRecord as DataRecord);
            rec.Cells[SqlTableKey].Value = rec.Cells[SqlTableKey].Value.ToString().Trim();
            var title = M_LAN.Equals("V") ? ("Doi ma " + TableName) : ("Change " + TableName + " code");
            var tableAndName = new string[] { title, M_LAN.Equals("V") ? "ten_vt" : "ten_vt2" };
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
                    SqlCommand cmdGet = new SqlCommand("exec dbo.CheckDeleteListId @ma_dm, @" + SqlTableKey);
                    cmdGet.Parameters.Add("@ma_dm", SqlDbType.Char).Value = sqlTableName;
                    cmdGet.Parameters.Add("@" + SqlTableKey, SqlDbType.Char).Value = currSqlTableKey;
                    int ListDelete = (int)SysObj.ExcuteScalar(cmdGet);
                    if (ListDelete > 0)
                    {
                        if (ExMessageBox.Show( 2335,SysObj, "Có chắc chắn xóa không?", "", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
                        {
                            deleteRowByKey(sqlTableName);
                        }
                    }
                    else
                    {
                        ExMessageBox.Show( 2340,SysObj, "Đã có phát sinh không được xóa!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (SqlException ex)
                {
                    ErrorLog.CatchMessage(ex);
                }
            }
            else
            {
                ExMessageBox.Show( 2345,SysObj, "Không có quyền xóa " + "[" + TableName + "]" + "!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Question);
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
            {
                if (SmDataLib.ListFunc.deleteRowInDatabaseByKey(tableName, SqlTableKey, dt.Rows[0], SysObj) > 0)
                {
                    SqlCommand cmdDeleteGia = new SqlCommand("DELETE FROM dmgia WHERE ma_vt LIKE @ma_vt; DELETE FROM dmgia2 WHERE ma_vt LIKE @ma_vt2");
                    cmdDeleteGia.Parameters.Add("@ma_vt", SqlDbType.VarChar).Value = dt.Rows[0][SqlTableKey].ToString().Trim();
                    cmdDeleteGia.Parameters.Add("@ma_vt2", SqlDbType.VarChar).Value = dt.Rows[0][SqlTableKey].ToString().Trim();
                    SysObj.ExcuteNonQuery(cmdDeleteGia);
                }
            }
        }
        #endregion

        #region Extend_oBrowse_Command
        public DataTable Extend_oBrowse_Command(SysLib.SysObject SysObj, ActionTask Mode, string Current_Ma_vt)
        {
            TableName = M_LAN.Equals("V") ? "vật tư" : "item";
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
                                currSqlTableKey = Current_Ma_vt;
                                titleWindow = M_LAN.Equals("V") ? ("Thêm " + TableName) : ("Add new " + TableName); 
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
                                currSqlTableKey = Current_Ma_vt;
                                titleWindow = M_LAN.Equals("V") ? ("Thêm " + TableName) : ("Add new " + TableName);
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
                                currSqlTableKey = Current_Ma_vt;

                                titleWindow = M_LAN.Equals("V") ? ("Sửa thông tin " + TableName) : ("Edit " + TableName + " information"); 
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
                                currSqlTableKey = Current_Ma_vt;

                                titleWindow = M_LAN.Equals("V") ? ("Xem thông tin " + TableName) : ("View " + TableName + " information");
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
                ExMessageBox.Show( 2350,SysObj, "Không có quyền " + "[" + titleWindow.ToLower() + "]" + "!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                currActionTask = ActionTask.None;
            }
            if (LastEditRow == null)
                return null;
            return LastEditRow.Table;
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

                if (LastEditRow != null && oBrowse != null)
                    oBrowse.frmBrw.ReloadData(LastEditRow);
            }
            else
            {
                ExMessageBox.Show( 2355,SysObj, "Không có quyền " + "[" + titleWindow.ToLower() + "]" + "!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
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
                    DataTable TableFields = SmDataLib.ListFunc.GetSqlTableFieldList(SysObj, sqlTableName);
                    string[] arrKeyValue = currSqlTableKey.Split(';');
                    string[] arrSqlTableKey = SqlTableKey.Split(';');
                    for (int i = 0; i < arrSqlTableKey.Count(); i++)
                    {
                        if (i > 0)
                            strSelect += " and";
                        DataRow[] FieldInfo = TableFields.Select("name='" + arrSqlTableKey[i] + "'");
                        strSelect += " " + arrSqlTableKey[i] + "=@" + arrSqlTableKey[i];
                        cmdGet.Parameters.Add("@" + arrSqlTableKey[i], SmDataLib.ListFunc.GetSqlDBType(FieldInfo[0]["datatype"].ToString())).Value = arrKeyValue[i];
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
