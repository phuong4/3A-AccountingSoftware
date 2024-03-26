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


namespace SOGIA2_F3
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class newForm : FormList
    {

        // tao datatable tam
        DataTable newDataTable = new DataTable();
        DataTable OldRow = null;
        string[] OldKeyValue = new string[2];
        string[] OldKeyCol = new string[2];
        string strOldNgay_ban = string.Empty;
        string strOldMa_vt = string.Empty;
        Sm.Windows.Controls.EditModeBindingObject FormInEditMode;
        CodeValueBindingObject M_IP_GIA;
        CodeValueBindingObject M_IP_GIA_NT;
        public newForm()
        {

            InitializeComponent();
            BindingSysObj = StartUp.SysObj;
            DisplayLanguage = StartUp.M_LAN;
            SmLib.SysFunc.LoadIcon(this);

            FormInEditMode = (Sm.Windows.Controls.EditModeBindingObject)this.FindResource("IsInEditMode");
            M_IP_GIA = (CodeValueBindingObject)this.FindResource("M_IP_GIA");
            M_IP_GIA_NT = (CodeValueBindingObject)this.FindResource("M_IP_GIA_NT");

            if (StartUp.currActionTask == ActionTask.View)
                this.ConfirmGV.ButtonType = 1;
        }


        #region LoadForm
        void LoadForm()
        {
            DataTable TableFields = ListFunc.GetSqlTableFieldList(StartUp.SysObj, StartUp.sqlTableName);
            //txtGia_VND.Mask = "{double:" + ListFunc.GetLengthColumn(TableFields, "gia2") + ".0}";
            //txtGia_NT.Mask = "{double:" + ListFunc.GetLengthColumn(TableFields, "gia_nt2") + ".0}";
            OldKeyCol = StartUp.SqlTableKey.Split(';');
            OldKeyValue = StartUp.currSqlTableKey.Split(';');
            this.Title = SysFunc.Cat_Dau(StartUp.titleWindow);

        }
        #endregion

        #region newForm_Loaded
        void newForm_Loaded(object sender, RoutedEventArgs e)
        {
            M_IP_GIA.Text = BindingSysObj.GetOption("M_IP_GIA").ToString();
            M_IP_GIA_NT.Text = BindingSysObj.GetOption("M_IP_GIA_NT").ToString();

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
                                //newRow[StartUp.SqlTableKey] = SysFunc.GetNewMadm(StartUp.SysObj, StartUp.sqlTableName);
                                newRow["ngay_ban"] = DateTime.Today;
                                newRow["gia2"] = 0;
                                newRow["gia_nt2"] = 0;
                            }
                            newDataTable.Rows.Add(newRow);
                        }
                        catch (Exception ex)
                        {
                            ErrorLog.CatchMessage(ex);
                        }
                        txtMa_vt.IsFocus = true;
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

                            strOldMa_vt = OldKeyValue[0];
                            strOldNgay_ban = OldKeyValue[1];
                        }
                        catch (Exception ex)
                        {
                            ErrorLog.CatchMessage(ex);
                        }
                        txtMa_vt.IsFocus = true;
                        break;
                    }
                case ActionTask.View:
                    {
                        try
                        {
                            StartUp.LastEditTable = null;
                            newDataTable = StartUp.GetRow(StartUp.sqlTableView);
                        }
                        catch (Exception ex)
                        {
                            ErrorLog.CatchMessage(ex);
                        }
                        txtNgayHL.Focusable = false;
                        txtGia_VND.Focusable = false;
                        txtGia_NT.Focusable = false;
                        ConfirmGV.pnlButton.btnCancel.Focus();
                    }
                    break;
                case ActionTask.Copy:
                    {
                        try
                        {
                            newDataTable = StartUp.GetRow(StartUp.sqlTableView);
                            strOldMa_vt = OldKeyValue[0];
                            strOldNgay_ban = OldKeyValue[1];
                        }
                        catch (Exception ex)
                        {
                            ErrorLog.CatchMessage(ex);
                        }
                        txtMa_vt.IsFocus = true;
                        break;
                    }
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

 
        private bool CheckValid()
        {
            bool result = true;
            #region ma_vt
            if (txtMa_vt.Text.Trim() == string.Empty && result == true)
            {
                TabInfor.SelectedIndex = 0;
                ExMessageBox.Show( 2370,StartUp.SysObj, "Chưa vào mã vật tư!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);

                txtMa_vt.Focus();
                result = false;
            }
            if (txtNgayHL.dValue == new DateTime() && result == true)
            {
                TabInfor.SelectedIndex = 0;
                ExMessageBox.Show(2371, StartUp.SysObj, "Chưa vào ngày hiệu lực!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);

                txtNgayHL.Focus();
                result = false;
            }
            if (txtMa_vt.Text.Trim() != "" && result == true)
            {
                //string sb = SmLib.SysFunc.CheckInValidCode(StartUp.SysObj, txtMa_vt.Text.Trim());
                //if (sb != "" && result == true)
                //{
                //    TabInfor.SelectedIndex = 0;
                //    ExMessageBox.Show( 2375,StartUp.SysObj, "Mã không được chứa các ký tự " + "[" + sb + "]" + "!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                //    txtMa_vt.SelectAll();
                //    txtMa_vt.Focus();
                //    result = false;
             



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
                            if (OldRow.Rows[0]["nh_kh3"].ToString().Trim() + "-" + OldRow.Rows[0]["ma_vt"].ToString().Trim() + OldRow.Rows[0]["ngay_ban"].ToString().Trim() != newDataTable.Rows[0]["nh_kh3"].ToString().Trim() + "-" + newDataTable.Rows[0]["ma_vt"].ToString().Trim() + newDataTable.Rows[0]["ngay_ban"].ToString().Trim())
                                ischeck = true;
                        }
                        System.Data.SqlClient.SqlCommand cmdGet = new System.Data.SqlClient.SqlCommand("select count(1)	from " + StartUp.sqlTableName + " where nh_kh3 = @Nh_kh3 AND ma_vt = @ma_vt and ngay_ban = @ngay_ban");
                        cmdGet.Parameters.Add("@Nh_kh3", SqlDbType.Char).Value = txtNh_kh3.Text.Trim();
                        cmdGet.Parameters.Add("@ma_vt", SqlDbType.Char).Value = txtMa_vt.Text.Trim();
                        cmdGet.Parameters.Add("@ngay_ban", SqlDbType.Char).Value = string.Format("{0:yyyyMMdd}", txtNgayHL.dValue);
                        int ListExist = (int)StartUp.SysObj.ExcuteScalar(cmdGet);
                        if (ListExist > 0 && ischeck == true)
                        {
                            TabInfor.SelectedIndex = 0;
                            ExMessageBox.Show( 2380,StartUp.SysObj, "Mã đã có khai báo!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);

                            txtMa_vt.IsFocus = true;
                            result = false;
                        }
                        //if (result == true && ischeck == true)
                        //{
                        //    if (SmLib.SysFunc.CheckStringContain(StartUp.SysObj, StartUp.sqlTableName, StartUp.currSqlTableKey))
                        //    {
                        //        TabInfor.SelectedIndex = 0;
                        //        ExMessageBox.Show( 2385,StartUp.SysObj, "Mã đã có hoặc mã lồng nhau!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);

                        //        txtMa_vt.IsFocus = true;
                        //        result = false;
                        //    }
                        //}
                    }
                    catch (Exception ex)
                    {
                        ErrorLog.CatchMessage(ex);
                    }
                }
            }
           

            //#region ten_kh
            //if (result && txtten_kh.Text.Trim() == "")
            //{
            //    TabInfor.SelectedIndex = 0;
            //    ExMessageBox.Show( 2390,StartUp.SysObj, "Chưa vào tên " + "[" + StartUp.TableName + "]" + "!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            //    txtten_kh.Focus();
            //    result = false;
            //}
            //#endregion

            //#region ma_so_thue
            //if (txtma_so_thue.Text.Trim() != "" && result == true)
            //{
            //    string M_MST_CHECK = StartUp.SysObj.GetOption("M_MST_CHECK").ToString().Trim();
            //    if (!M_MST_CHECK.Equals("0"))
            //    {
            //        if (SmLib.SysFunc.CheckSumMaSoThue(txtma_so_thue.Text.Trim()) == false)
            //        {
            //            if (M_MST_CHECK.Equals("1"))
            //                ExMessageBox.Show( 2395,StartUp.SysObj, "Mã số thuế không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);

            //            else
            //            {
            //                TabInfor.SelectedIndex = 0;
            //                ExMessageBox.Show( 2400,StartUp.SysObj, "Mã số thuế không hợp lệ, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            //                txtma_so_thue.SelectAll();
            //                txtma_so_thue.Focus();
            //                result = false;
            //            }
            //        }

            //        if (result == true)
            //        {
            //            //nếu là add và copy thì ischeck = true
            //            // bool ischeck = true;
            //            //Lỗi 1109007 yêu cầu kiểm tra không cần sửa

            //            //if (StartUp.currActionTask == ActionTask.Edit && result == true)
            //            //{
            //            //    //nếu edit thì ischeck = false
            //            //    //nếu khi edit mã được đổi, và mã cũ khác mã mới thì ischeck = true
            //            //    ischeck = false;
            //            //    if (OldRow.Rows[0]["ma_so_thue"].ToString().Trim() != newDataTable.Rows[0]["ma_so_thue"].ToString().Trim())
            //            //        ischeck = true;
            //            //}
            //            SqlCommand cmdGet = new SqlCommand("select * from " + StartUp.sqlTableName + " where ma_so_thue = @ma_so_thue");
            //            cmdGet.Parameters.Add("@ma_so_thue", SqlDbType.Char).Value = txtma_so_thue.Text.Trim();
            //            DataTable dt = StartUp.SysObj.ExcuteReader(cmdGet).Tables[0];

            //            //if (dt.Rows.Count > 0 && ischeck == true)
            //            if ((StartUp.currActionTask != ActionTask.Edit && dt.Rows.Count > 0) || (StartUp.currActionTask == ActionTask.Edit && dt.Rows.Count > 1))
            //            {
            //                if (M_MST_CHECK.Equals("1"))
            //                    ExMessageBox.Show( 2405,StartUp.SysObj, "Mã số thuế này đã có!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            //                else
            //                {
            //                    TabInfor.SelectedIndex = 0;
            //                    ExMessageBox.Show( 2410,StartUp.SysObj, "Mã số thuế này đã có, không lưu được!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            //                    txtma_so_thue.SelectAll();
            //                    txtma_so_thue.Focus();
            //                    result = false;
            //                }
            //            }
            //        }
            //    }
            //}
            //#endregion

            //#region tk
            //if (result && !txttk.CheckLostFocus())
            //{
            //    TabInfor.SelectedIndex = 0;
            //    ExMessageBox.Show( 2415,StartUp.SysObj, "Tài khoản ngầm định không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            //    txttk.IsFocus = true;
            //    result = false;
            //}
            //#endregion

            //#region nh_kh1
            //if (result && !txtnh_kh1.CheckLostFocus())
            //{
            //    TabInfor.SelectedIndex = 0;
            //    ExMessageBox.Show( 2420,StartUp.SysObj, "Nhóm khách 1 không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            //    txtnh_kh1.IsFocus = true;
            //    result = false;
            //}
            //#endregion

            //#region nh_kh2
            //if (result && !txtnh_kh2.CheckLostFocus())
            //{
            //    TabInfor.SelectedIndex = 0;
            //    ExMessageBox.Show( 2425,StartUp.SysObj, "Nhóm khách 2 không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            //    txtnh_kh2.IsFocus = true;
            //    result = false;
            //}
            //#endregion

            //#region nh_kh3
            //if (result && !txtnh_kh3.CheckLostFocus())
            //{
            //    TabInfor.SelectedIndex = 0;
            //    ExMessageBox.Show( 2430,StartUp.SysObj, "Nhóm khách 3 không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Information);
            //    txtnh_kh3.IsFocus = true;
            //    result = false;
            //}
           #endregion

            return result;
        }
       

        #region EnableEditMode
        public void EnableEditMode(bool isEditMode)
        {
            FormInEditMode.IsEditMode = isEditMode;
        }
        #endregion

        #region txtnh_kh1_LostFocus
        private void txtnh_kh1_LostFocus(object sender, RoutedEventArgs e)
        {
            //if (txtnh_kh1.RowResult == null)
            //    txtten_nh_kh1.Text = "";
            //else
            //    txtten_nh_kh1.Text = (txtnh_kh1.RowResult["ten_nh"]).ToString();
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

        private void txtMa_vt_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtMa_vt.RowResult == null)
                txtten_vt.Text = "";
            else
                txtten_vt.Text = StartUp.M_LAN.Equals("V") ? txtMa_vt.RowResult["ten_vt"].ToString() : txtMa_vt.RowResult["ten_vt2"].ToString();
        }

        private void txtNh_kh3_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtNh_kh3.RowResult == null)
                txtten_kh3.Text = "";
            else
                txtten_kh3.Text = StartUp.M_LAN.Equals("V") ? txtNh_kh3.RowResult["ten_nh"].ToString() : txtNh_kh3.RowResult["ten_nh2"].ToString();
        }

    }

}
