using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data;
using System.Diagnostics;
using Sm.Windows.Controls;
using SmDefine;
using SmErrorLib;
using SmDataLib;
using System.Data.SqlClient;
using System.Windows.Input;
using System.Text.RegularExpressions;
using SmLib;


namespace Dmmodel
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class newForm : FormList
    {

        // tao datatable tam
        DataTable newDataTable = new DataTable();
        DataTable OldRow = null;

        Sm.Windows.Controls.EditModeBindingObject FormInEditMode;

        public newForm()
        {

            InitializeComponent();
            BindingSysObj = StartUp.SysObj;
            DisplayLanguage = StartUp.M_LAN;
            SmLib.SysFunc.LoadIcon(this);
            
            FormInEditMode = (Sm.Windows.Controls.EditModeBindingObject)this.FindResource("IsInEditMode");
            if (StartUp.currActionTask == ActionTask.View)
                this.ConfirmGV.ButtonType = 1;
        }


        #region LoadForm
        void LoadForm()
        {
            DataTable TableFields = ListFunc.GetSqlTableFieldList(StartUp.SysObj, StartUp.sqlTableName);

            txtma_model.MaxLength = ListFunc.GetLengthColumn(TableFields, "ma_model");
            txtten_model.MaxLength = ListFunc.GetLengthColumn(TableFields, "ten_model");
            txtten_model2.MaxLength = ListFunc.GetLengthColumn(TableFields, "ten_model");
            txtma_tra_cuu.MaxLength = ListFunc.GetLengthColumn(TableFields, "ma_tra_cuu");
           
            this.Title = StartUp.titleWindow;

        }
        #endregion

        #region newForm_Loaded
        void newForm_Loaded(object sender, RoutedEventArgs e)
        {
            LoadForm();
            // set focus cho ma_kh
            TextBox KeyTextBox = SmLib.SysFunc.FindChild<TextBox>(this, "txt" + StartUp.SqlTableKey); //
            if (KeyTextBox != null)
            {
                KeyTextBox.SelectAll();
                KeyTextBox.Focus();

            }
            else
            {
                Debug.Write("Findchild not found");
            }

            switch (StartUp.currActionTask)
            {
                case ActionTask.Add:
                    {
                        try
                        {
                            newDataTable = StartUp.GetRow(StartUp.sqlTableView);
                            DataRow newRow = newDataTable.NewRow();
                            if (StartUp.SysObj.GetOption("M_AUTO_LIST_NUM").ToString().Equals("1"))
                            {
                                if (!string.IsNullOrEmpty(StartUp.currSqlTableKey) && StartUp.SysObj.DmdmInfo.Select("ma_dm like '" + StartUp.sqlTableName + "' and  increase_type = 2").Length > 0)
                                {
                                    string _value = SysFunc.IncreaseCode(StartUp.SysObj, StartUp.currSqlTableKey, StartUp.SqlTableKey, StartUp.sqlTableName);
                                    if (!string.IsNullOrEmpty(_value) && _value.Length <= txtma_model.MaxLength)
                                        newRow[StartUp.SqlTableKey] = _value;
                                }
                                if (string.IsNullOrEmpty(newRow[StartUp.SqlTableKey].ToString().Trim()))
                                    newRow[StartUp.SqlTableKey] = SysFunc.GetNewMadm(StartUp.SysObj, StartUp.sqlTableName);
                            }
                            newDataTable.Rows.Add(newRow);
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
                            newDataTable = StartUp.GetRow(StartUp.sqlTableView);
                            if (newDataTable.Rows.Count > 0)
                            {
                                OldRow = newDataTable.Copy();
                            }

                            System.Data.SqlClient.SqlCommand cmdGet = new System.Data.SqlClient.SqlCommand("exec dbo.CheckDeleteListId @ma_dm, @" + StartUp.SqlTableKey);
                            cmdGet.Parameters.Add("@ma_dm", SqlDbType.Char).Value = StartUp.sqlTableName;
                            cmdGet.Parameters.Add("@" + StartUp.SqlTableKey, SqlDbType.Char).Value = StartUp.currSqlTableKey;
                            int ListDelete = (int)StartUp.SysObj.ExcuteScalar(cmdGet);
                            if (ListDelete <= 0)
                            {
                                if (KeyTextBox != null)
                                    KeyTextBox.IsReadOnly = true;

                                TextBox NameTextBox = SmLib.SysFunc.FindChild<TextBox>(this, "txt" + StartUp.SqlTableObjectName); //
                                if (NameTextBox != null)
                                {
                                    NameTextBox.SelectAll();
                                    NameTextBox.Focus();
                                }
                            }
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
                            StartUp.LastEditTable = null;
                            newDataTable = StartUp.GetRow(StartUp.sqlTableView);
                            
                            this.ConfirmGV.ButtonType = 1;
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
                            newDataTable = StartUp.GetRow(StartUp.sqlTableView);
                            if (StartUp.SysObj.GetOption("M_AUTO_LIST_NUM").ToString().Equals("1"))
                            {
                                if (!string.IsNullOrEmpty(StartUp.currSqlTableKey) && StartUp.SysObj.DmdmInfo.Select("ma_dm like '" + StartUp.sqlTableName + "' and  increase_type = 2").Length > 0)
                                {
                                    string _value = SysFunc.IncreaseCode(StartUp.SysObj, StartUp.currSqlTableKey, StartUp.SqlTableKey, StartUp.sqlTableName);
                                    if (!string.IsNullOrEmpty(_value) && _value.Length <= txtma_model.MaxLength)
                                        newDataTable.Rows[0][StartUp.SqlTableKey] = _value;
                                }
                                if (string.IsNullOrEmpty(newDataTable.Rows[0][StartUp.SqlTableKey].ToString().Trim()))
                                    newDataTable.Rows[0][StartUp.SqlTableKey] = SysFunc.GetNewMadm(StartUp.SysObj, StartUp.sqlTableName);
                            }
                        }
                        catch (Exception ex)
                        {
                            ErrorLog.CatchMessage(ex);
                        }
                    }
                    break;
            }


            gridMain.DataContext = newDataTable;

        }
        #endregion

        #region saveCustomer()
        void saveCustomer()
        {
            StartUp.LastEditTable = null;
            if (StartUp.currActionTask == ActionTask.View)
                return;

            newDataTable.AcceptChanges();
            int M_User_Id = int.Parse(StartUp.SysObj.UserInfo.Rows[0]["user_id"].ToString());
            string M_User_Name = StartUp.SysObj.UserInfo.Rows[0]["user_name"].ToString().Trim();
            if (newDataTable.Columns.Contains("search"))
                SmLib.SysFunc.SetStrSearch(StartUp.SysObj, StartUp.sqlTableName, ref newDataTable);

            if (StartUp.currActionTask == ActionTask.Edit)
            {
                newDataTable.Rows[0]["date"] = DateTime.Now;
                newDataTable.Rows[0]["time"] = DateTime.Now.ToString("HH:mm:ss");
                newDataTable.Rows[0]["user_id"] = M_User_Id;
                newDataTable.Rows[0]["user_name"] = M_User_Name;

                //update xuong database
                //neu update database thanh cong thi update datatable tren grid
                if (OldRow != null)
                {
                    ListFunc.updateRowInDatabaseByKey(StartUp.sqlTableName, StartUp.SqlTableKey, OldRow.Rows[0], newDataTable.Rows[0], StartUp.SysObj);
                }
            }
            else
            {
                newDataTable.Rows[0]["date"] = DateTime.Now;
                newDataTable.Rows[0]["time"] = DateTime.Now.ToString("HH:mm:ss");
                newDataTable.Rows[0]["user_id"] = M_User_Id;
                newDataTable.Rows[0]["user_name"] = M_User_Name;


                newDataTable.Rows[0]["date0"] = DateTime.Now;
                newDataTable.Rows[0]["time0"] = DateTime.Now.ToString("HH:mm:ss");
                newDataTable.Rows[0]["user_id0"] = M_User_Id;
                newDataTable.Rows[0]["user_name0"] = M_User_Name;

                //insert xuong database
                //neu insert database thanh cong thi insert row datatable tren grid
                ListFunc.inserRowInDataBase(StartUp.sqlTableName, newDataTable.Rows[0], StartUp.SysObj);

            }
            StartUp.LastEditTable = newDataTable;
        }
        #endregion

        #region CheckValid
        private bool CheckValid()
        {
            bool result = true;
            if (txtma_model.Text.Trim() == string.Empty && result == true)
            {
                TabInfor.SelectedIndex = 0;
                ExMessageBox.Show( 15,StartUp.SysObj, "Chưa vào mã " + "[" + StartUp.TableName + "]" + "!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);

                txtma_model.Focus();
                result = false;
            }
            if (txtma_model.Text.Trim() != "" && result == true)
            {
                string sb = SmLib.SysFunc.CheckInValidCode(StartUp.SysObj, txtma_model.Text.Trim());
                if (sb != "" && result == true)
                {
                    TabInfor.SelectedIndex = 0;
                    ExMessageBox.Show( 20,StartUp.SysObj, "Mã không được chứa các ký tự " + "[" + sb + "]" + "!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtma_model.SelectAll();
                    txtma_model.Focus();
                    result = false;
                }
                if (result == true)
                {
                    try
                    {
                        //nếu là add và copy thì ischeck = true
                        bool ischeck = true;
                        if (StartUp.currActionTask == ActionTask.Edit && result == true)
                        {
                            //nếu edit thì ischeck = false
                            //nếu khi edit mã được đổi, và mã cũ khác mã mới thì ischeck = true
                            ischeck = false;
                            if (OldRow.Rows[0][StartUp.SqlTableKey].ToString().Trim() != newDataTable.Rows[0][StartUp.SqlTableKey].ToString().Trim())
                                ischeck = true;
                        }
                        System.Data.SqlClient.SqlCommand cmdGet = new System.Data.SqlClient.SqlCommand("exec dbo.CheckExistListId @ma_dm, @" + StartUp.SqlTableKey);
                        cmdGet.Parameters.Add("@ma_dm", SqlDbType.Char).Value = StartUp.sqlTableName;
                        cmdGet.Parameters.Add("@" + StartUp.SqlTableKey, SqlDbType.Char).Value = txtma_model.Text.Trim();
                        int ListExist = (int)StartUp.SysObj.ExcuteScalar(cmdGet);
                        if (ListExist > 0 && ischeck == true)
                        {
                            TabInfor.SelectedIndex = 0;
                            ExMessageBox.Show( 25,StartUp.SysObj, "Mã đã có hoặc mã lồng nhau!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                            txtma_model.SelectAll();
                            txtma_model.Focus();
                            result = false;
                        }
                        if (result == true && ischeck == true)
                        {
                            string ma_cu = "";
                            if (StartUp.currActionTask == ActionTask.Edit)
                                ma_cu = OldRow.Rows[0][StartUp.SqlTableKey].ToString().Trim();
                            if (SmLib.SysFunc.CheckStringContain(StartUp.SysObj, StartUp.sqlTableName, StartUp.SqlTableKey, txtma_model.Text.Trim(), ma_cu))
                            {
                                TabInfor.SelectedIndex = 0;
                                ExMessageBox.Show( 30,StartUp.SysObj, "Mã đã có hoặc mã lồng nhau!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                                txtma_model.SelectAll();
                                txtma_model.Focus();
                                result = false;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.CatchMessage(ex);
                    }
                }
            }
            if (txtten_model.Text.Trim() == string.Empty && result == true)
            {
                TabInfor.SelectedIndex = 0;
                ExMessageBox.Show( 35,StartUp.SysObj, "Chưa vào tên " + "[" + StartUp.TableName + "]" + "!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);

                txtten_model.Focus();
                result = false;
            }
           
            return result;
        }
        #endregion

        #region EnableEditMode
        public void EnableEditMode(bool isEditMode)
        {
            FormInEditMode.IsEditMode = isEditMode;
        }
        #endregion

        #region ConfirmGridView_OnOk
        private void ConfirmGridView_OnOk(object sender, RoutedEventArgs e)
        {
            if (StartUp.currActionTask == ActionTask.View)
            {
                this.Close();
                return;
            }
            if (CheckValid())
            {
                saveCustomer();
                StartUp.currActionTask = ActionTask.None;
                this.Close();
            }
        }
        #endregion

        #region txtma_model_GotFocus
        private void txtma_model_GotFocus(object sender, RoutedEventArgs e)
        {
            txtma_model.Text = txtma_model.Text.Trim();
            txtma_model.SelectAll();
        }
        #endregion

        #region txtma_tra_cuu_GotFocus
        private void txtma_tra_cuu_GotFocus(object sender, RoutedEventArgs e)
        {
            txtma_tra_cuu.Text = txtma_tra_cuu.Text.Trim();
            txtma_tra_cuu.SelectAll();
        }
        #endregion

        #region FormList_Closed
        private void FormList_Closed(object sender, EventArgs e)
        {
            StartUp.currActionTask = ActionTask.None;
        } 
        #endregion

        #region ConfirmGV_OnCancel
        private void ConfirmGV_OnCancel(object sender, RoutedEventArgs e)
        {
            StartUp.LastEditTable = null;
            this.Close();
        }
        #endregion

        #region FormList_PreviewKeyUp
        private void FormList_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.None && e.Key == Key.Escape)
            {
                StartUp.LastEditTable = null;
                this.Close();
            }

        }
        #endregion
    }

}
