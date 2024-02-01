using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
//using System.Windows.Input;
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

 
namespace Arkh
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
            DataTable TableFields = ListFunc.GetSqlTableFieldList(StartUp.SysObj, StartUp.sqlTableView);
            //txthan_tt.Mask = "{double:" + ListFunc.GetLengthColumn(TableFields, "han_tt") + ".0}";
            txtma_kh.MaxLength = ListFunc.GetLengthColumn(TableFields, "ma_kh");
            txtten_kh.MaxLength = ListFunc.GetLengthColumn(TableFields, "ten_kh");
            txtten_kh2.MaxLength = ListFunc.GetLengthColumn(TableFields, "ten_kh2");
            txtma_tra_cuu.MaxLength = ListFunc.GetLengthColumn(TableFields, "ma_tra_cuu");
            txtdia_chi.MaxLength = ListFunc.GetLengthColumn(TableFields, "dia_chi");
            txtdoi_tac.MaxLength = ListFunc.GetLengthColumn(TableFields, "doi_tac");
            txtma_so_thue.MaxLength = 14;// ListFunc.GetLengthColumn(TableFields, "ma_so_thue");
            txtdien_thoai.MaxLength = ListFunc.GetLengthColumn(TableFields, "dien_thoai");
            txtfax.MaxLength = ListFunc.GetLengthColumn(TableFields, "fax");
            txtemail.MaxLength = ListFunc.GetLengthColumn(TableFields, "e_mail");
            txttk_nh.MaxLength = ListFunc.GetLengthColumn(TableFields, "tk_nh");
            txtten_nh.MaxLength = ListFunc.GetLengthColumn(TableFields, "ten_nh");
            txttinh_thanh.MaxLength = ListFunc.GetLengthColumn(TableFields, "tinh_thanh");
            txtghi_chu.MaxLength = ListFunc.GetLengthColumn(TableFields, "ghi_chu");
            this.Title = SysFunc.Cat_Dau(StartUp.titleWindow);

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
                            int a = StartUp.SysObj.DmdmInfo.Select("ma_dm like '" + StartUp.sqlTableName + "' and  increase_type = 2").Length;
                            DataTable dt = StartUp.SysObj.DmdmInfo;
                            newDataTable = StartUp.GetRow(StartUp.sqlTableView);
                            DataRow newRow = newDataTable.NewRow();
                            if (StartUp.SysObj.GetOption("M_AUTO_LIST_NUM").ToString().Equals("1"))
                            {
                                if (/*!string.IsNullOrEmpty(StartUp.currSqlTableKey) &&*/ StartUp.SysObj.DmdmInfo.Select("ma_dm like '" + StartUp.sqlTableName + "' and  increase_type = 2").Length > 0)
                                {
                                    string _value = SysFunc.IncreaseCode(StartUp.SysObj, StartUp.currSqlTableKey, StartUp.SqlTableKey, StartUp.sqlTableUpdateName);
                                    if (!string.IsNullOrEmpty(_value) && _value.Length <= txtma_kh.MaxLength)
                                        newRow[StartUp.SqlTableKey] = _value;
                                }

                                if (string.IsNullOrEmpty(newRow[StartUp.SqlTableKey].ToString().Trim()))
                                    newRow[StartUp.SqlTableKey] = SysFunc.GetNewMadm(StartUp.SysObj, StartUp.sqlTableName);
                            }
                            newRow["status"] = "1";
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
                                    string _value = SysFunc.IncreaseCode(StartUp.SysObj, StartUp.currSqlTableKey, StartUp.SqlTableKey, StartUp.sqlTableUpdateName);
                                    if (!string.IsNullOrEmpty(_value) && _value.Length <= txtma_kh.MaxLength)
                                        newDataTable.Rows[0][StartUp.SqlTableKey] = _value;
                                }
                                if (string.IsNullOrEmpty(newDataTable.Rows[0][StartUp.SqlTableKey].ToString().Trim()))
                                    newDataTable.Rows[0][StartUp.SqlTableKey] = SysFunc.GetNewMadm(StartUp.SysObj, StartUp.sqlTableName);
                            }
                            newDataTable.Rows[0]["status"] = "1";
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

        //protected string GetNewMadm(SysLib.SysObject SysObj, string ma_dm)
        //{
        //    try
        //    {
        //        string cmd = "UPDATE dmdm SET stt13 = stt13 + 1 WHERE ma_dm='" + ma_dm.Trim() + "';";
        //        cmd += "SELECT transform, stt13 FROM dmdm WHERE ma_dm = '" + ma_dm.Trim() + "'";
        //        DataRow row = SysObj.ExcuteReader(new SqlCommand(cmd)).Tables[0].Rows[0];
        //        if (row[1] != null && row[1] != DBNull.Value)
        //            return string.Format(row[0].ToString(), Convert.ToDouble(row[1]));
        //    }
        //    catch (Exception ex)
        //    {
        //        SmErrorLib.ErrorLog.CatchMessage(ex);
        //    }
        //    return "";
        //}

        #region txtstatus_LostFocus
        private void txtstatus_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtstatus.Text == "")
                newDataTable.Rows[0]["status"] = 0;
        }
        #endregion

        #region saveCustomer()
        void saveCustomer()
        {
            StartUp.LastEditTable = null;
            if (StartUp.currActionTask == ActionTask.View)
                return;

           // newDataTable.Rows[0]["ten_kh"] = SysFunc.SetName(new string[] { newDataTable.Rows[0]["ten_kh"].ToString(), newDataTable.Rows[0]["ten_kh2"].ToString() });
            newDataTable.AcceptChanges();
            int M_User_Id = int.Parse(StartUp.SysObj.UserInfo.Rows[0]["user_id"].ToString());
            string M_User_Name = StartUp.SysObj.UserInfo.Rows[0]["user_name"].ToString().Trim();

            if (newDataTable.Columns.Contains("search"))
                
                
                SmLib.SysFunc.SetStrSearch(StartUp.SysObj, "dmkh", ref newDataTable);

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
                    ListFunc.updateRowInDatabaseByKey(StartUp.sqlTableUpdateName, StartUp.SqlTableKey, OldRow.Rows[0], newDataTable.Rows[0], StartUp.SysObj);
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
                ListFunc.inserRowInDataBase(StartUp.sqlTableUpdateName, newDataTable.Rows[0], StartUp.SysObj);
            }
            StartUp.LastEditTable = newDataTable;
        }
        #endregion

        #region CheckValid
        private bool CheckValid()
        {
            bool result = true;
            #region ma_kh
            if (txtma_kh.Text.Trim() == string.Empty && result == true)
            {
                TabInfor.SelectedIndex = 0;
                ExMessageBox.Show( 1745,StartUp.SysObj, "Chưa vào mã " + "[" + StartUp.TableName + "]" + "!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);

                txtma_kh.Focus();
                result = false;
            }
            if (txtma_kh.Text.Trim() != "" && result == true)
            {
                string sb = SmLib.SysFunc.CheckInValidCode(StartUp.SysObj, txtma_kh.Text.Trim());
                if (sb != "" && result == true)
                {
                    TabInfor.SelectedIndex = 0;
                    ExMessageBox.Show( 1750,StartUp.SysObj, "Mã không được chứa các ký tự " + "[" + sb + "]" + "!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtma_kh.SelectAll();
                    txtma_kh.Focus();
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
                        cmdGet.Parameters.Add("@ma_dm", SqlDbType.Char).Value = StartUp.sqlTableUpdateName;
                        cmdGet.Parameters.Add("@" + StartUp.SqlTableKey, SqlDbType.Char).Value = txtma_kh.Text.Trim();
                        int ListExist = (int)StartUp.SysObj.ExcuteScalar(cmdGet);
                        if (ListExist > 0 && ischeck == true)
                        {
                            TabInfor.SelectedIndex = 0;
                            ExMessageBox.Show( 1755,StartUp.SysObj, "Mã đã có hoặc mã lồng nhau!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                            txtma_kh.SelectAll();
                            txtma_kh.Focus();
                            result = false;
                        }
                        if (result == true && ischeck == true)
                        {
                            string ma_cu = "";
                            if(StartUp.currActionTask == ActionTask.Edit)
                                ma_cu = OldRow.Rows[0][StartUp.SqlTableKey].ToString().Trim();
                            if (SmLib.SysFunc.CheckStringContain(StartUp.SysObj, StartUp.sqlTableUpdateName, StartUp.SqlTableKey, txtma_kh.Text.Trim(), ma_cu))
                            {
                                TabInfor.SelectedIndex = 0;
                                ExMessageBox.Show( 1760,StartUp.SysObj, "Mã đã có hoặc mã lồng nhau!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                                txtma_kh.SelectAll();
                                txtma_kh.Focus();
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
            #endregion

            #region ten_kh
            if (result && txtten_kh.Text.Trim() == "")
            {
                TabInfor.SelectedIndex = 0;
                ExMessageBox.Show( 1765,StartUp.SysObj, "Chưa vào tên " + "[" + StartUp.TableName + "]" + "!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                txtten_kh.Focus();
                result = false;
            }

          
            #endregion

            #region ma_so_thue
            if (txtma_so_thue.Text.Trim() != "" && result == true)
            {
                string M_MST_CHECK = StartUp.SysObj.GetOption("M_MST_CHECK").ToString().Trim();
                if (!M_MST_CHECK.Equals("0"))
                {
                    if (SmLib.SysFunc.CheckSumMaSoThue(txtma_so_thue.Text.Trim()) == false)
                    {
                        if (M_MST_CHECK.Equals("1"))
                            ExMessageBox.Show( 1770,StartUp.SysObj, "Mã số thuế không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);

                        else
                        {
                            TabInfor.SelectedIndex = 0;
                            ExMessageBox.Show( 1775,StartUp.SysObj, "Mã số thuế không hợp lệ, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                            txtma_so_thue.SelectAll();
                            txtma_so_thue.Focus();
                            result = false;
                        }
                    }

                    if (result == true)
                    {
                        //nếu là add và copy thì ischeck = true
                       // bool ischeck = true;
                        //Lỗi 1109007 yêu cầu kiểm tra không cần sửa
                        
                        //if (StartUp.currActionTask == ActionTask.Edit && result == true)
                        //{
                        //    //nếu edit thì ischeck = false
                        //    //nếu khi edit mã được đổi, và mã cũ khác mã mới thì ischeck = true
                        //    ischeck = false;
                        //    if (OldRow.Rows[0]["ma_so_thue"].ToString().Trim() != newDataTable.Rows[0]["ma_so_thue"].ToString().Trim())
                        //        ischeck = true;
                        //}
                        SqlCommand cmdGet = new SqlCommand("select * from " + StartUp.sqlTableUpdateName + " where ma_so_thue = @ma_so_thue");
                        cmdGet.Parameters.Add("@ma_so_thue", SqlDbType.Char).Value = txtma_so_thue.Text.Trim();
                        DataTable dt = StartUp.SysObj.ExcuteReader(cmdGet).Tables[0];
                        
                        //if (dt.Rows.Count > 0 && ischeck == true)
                        if ((StartUp.currActionTask != ActionTask.Edit && dt.Rows.Count > 0)|| (StartUp.currActionTask == ActionTask.Edit && dt.Rows.Count > 1))
                        {
                            if (M_MST_CHECK.Equals("1"))
                                ExMessageBox.Show( 1780,StartUp.SysObj, "Mã số thuế này đã có!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                            else
                            {
                                TabInfor.SelectedIndex = 0;
                                ExMessageBox.Show( 1785,StartUp.SysObj, "Mã số thuế này đã có, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                                txtma_so_thue.SelectAll();
                                txtma_so_thue.Focus();
                                result = false;
                            }
                        }
                    }
                }
            }
            #endregion

            #region tk
            if (result && !txttk.CheckLostFocus())
            {
                TabInfor.SelectedIndex = 0;
                ExMessageBox.Show( 1790,StartUp.SysObj, "Tài khoản ngầm định không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                txttk.IsFocus = true;
                result = false;
            }
            #endregion

            #region nh_kh1
            if (result && !txtnh_kh1.CheckLostFocus())
            {
                TabInfor.SelectedIndex = 0;
                ExMessageBox.Show( 1795,StartUp.SysObj, "Nhóm khách 1 không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                txtnh_kh1.IsFocus = true;
                result = false;
            }
            #endregion

            #region nh_kh2
            if (result && !txtnh_kh2.CheckLostFocus())
            {
                TabInfor.SelectedIndex = 0;
                ExMessageBox.Show( 1800,StartUp.SysObj, "Nhóm khách 2 không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                txtnh_kh2.IsFocus = true;
                result = false;
            }
            #endregion

            #region nh_kh3
            if (result && !txtnh_kh3.CheckLostFocus())
            {
                TabInfor.SelectedIndex = 0;
                ExMessageBox.Show( 1805,StartUp.SysObj, "Nhóm khách 3 không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                txtnh_kh3.IsFocus = true;
                result = false;
            }
            #endregion

            return result;
        }
        #endregion

        #region txttk_LostFocus
        private void txttk_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txttk.RowResult == null)
                txtten_tk.Text = "";
            else
                txtten_tk.Text = StartUp.M_LAN.Equals("V") ? txttk.RowResult["ten_tk"].ToString() : txttk.RowResult["ten_tk2"].ToString();
        }
        #endregion

        #region EnableEditMode
        public void EnableEditMode(bool isEditMode)
       {
           FormInEditMode.IsEditMode = isEditMode;
       }
       #endregion

        #region txtnh_kh1_LostFocus
        private void txtnh_kh1_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtnh_kh1.RowResult == null)
                txtten_nh_kh1.Text = "";
            else
                txtten_nh_kh1.Text = StartUp.M_LAN.Equals("V") ? txtnh_kh1.RowResult["ten_nh"].ToString() : txtnh_kh1.RowResult["ten_nh2"].ToString();
        }
        #endregion

        #region txtnh_kh2_LostFocus
        private void txtnh_kh2_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtnh_kh2.RowResult == null)
                txtten_nh_kh2.Text = "";
            else
                txtten_nh_kh2.Text = StartUp.M_LAN.Equals("V") ? txtnh_kh2.RowResult["ten_nh"].ToString() : txtnh_kh2.RowResult["ten_nh2"].ToString();
        }
        #endregion

        #region txtnh_kh3_LostFocus
        private void txtnh_kh3_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtnh_kh3.RowResult == null)
                txtten_nh_kh3.Text = "";
            else
                txtten_nh_kh3.Text = StartUp.M_LAN.Equals("V") ? txtnh_kh3.RowResult["ten_nh"].ToString() : txtnh_kh3.RowResult["ten_nh2"].ToString();
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

        #region txtghi_chu_PreviewKeyDown
        private void txtghi_chu_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Enter) && Keyboard.Modifiers == ModifierKeys.Alt)
            {
                TextBox txt = sender as TextBox;
                txt.SelectedText = Environment.NewLine;
                txt.SelectionStart = txt.SelectionStart + 1;
                txt.SelectionLength = 0;
            }
        }

        protected override bool IsEnterToPassObject(object sender)
        {
            if (sender is TextBox && (sender as TextBox).Name == "txtghi_chu" && Keyboard.Modifiers == ModifierKeys.Alt)
            {
                return false;
            }
            else
            {
                return base.IsEnterToPassObject(sender);
            }
        }
        #endregion

        #region txtma_kh_GotFocus
        private void txtma_kh_GotFocus(object sender, RoutedEventArgs e)
        {
            txtma_kh.Text = txtma_kh.Text.Trim();
            txtma_kh.SelectAll();
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
            if (StartUp.currActionTask != ActionTask.None)
                SysFunc.RollbackMadm(BindingSysObj, StartUp.sqlTableName);
            StartUp.currActionTask = ActionTask.None;
        } 
        #endregion

        #region ConfirmGV_OnCancel
        private void ConfirmGV_OnCancel(object sender, RoutedEventArgs e)
        {
            SysFunc.RollbackMadm(BindingSysObj, StartUp.sqlTableName);
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

        private void txtHan_ck_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (txtHan_ck.RowResult == null)
            {
                newDataTable.Rows[0]["han_tt"] = 0;
                txtthck.Text = "";
            }
            else
            {
                newDataTable.Rows[0]["han_tt"] = string.IsNullOrEmpty(txtHan_ck.RowResult["han_tt"].ToString()) ? 0 : txtHan_ck.RowResult["han_tt"];
                txtthck.Text = StartUp.M_LAN.Equals("V") ? txtHan_ck.RowResult["ten_thck"].ToString() : txtHan_ck.RowResult["ten_thck2"].ToString();
            }
        }

    }
    
}
