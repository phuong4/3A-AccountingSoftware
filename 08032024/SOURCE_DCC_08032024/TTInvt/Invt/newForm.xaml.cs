using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data;
using System.Diagnostics;
using SmDefine;
using SmErrorLib;
using Sm.Windows.Controls;
using System.Windows.Threading;
using System.Threading;
using SmDataLib;
using System.Data.SqlClient;
using SmLib;

namespace TT_Invt
{
    /// <summary>
    /// Interaction logic for newForm.xaml
    /// </summary>
    public partial class newForm : FormList
    {
        // tao datatable tam
        DataTable newDataTable = new DataTable();
        DataTable OldRow = null;
        bool isError = false;
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

        void txtNum_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as NumericTextBox).SelectAll();  
        }

        #region LoadForm
        void LoadForm()
        {
            int format = SmLib.SysFunc.GetFormatDecimal(StartUp.SysObj.GetOption("M_IP_SL").ToString());
            DataTable TableFields = ListFunc.GetSqlTableFieldList(StartUp.SysObj, StartUp.sqlTableName);
            txtsl_min.Mask = "{double:" + (ListFunc.GetLengthColumn(TableFields, "sl_min") - format) + "." + format.ToString() + "}";
            txtsl_max.Mask = "{double:" + (ListFunc.GetLengthColumn(TableFields, "sl_max") - format) + "." + format.ToString() + "}";
            txtma_vt.MaxLength = ListFunc.GetLengthColumn(TableFields, "ma_vt");
            txtma_tra_cuu.MaxLength = ListFunc.GetLengthColumn(TableFields, "ma_tra_cuu");

            txtten_vt.MaxLength = ListFunc.GetLengthColumn(TableFields, "ten_vt");
            txtten_vt2.MaxLength = ListFunc.GetLengthColumn(TableFields, "ten_vt2");
         
            txtdvt.MaxLength = ListFunc.GetLengthColumn(TableFields, "dvt");
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
                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    KeyTextBox.SelectAll();
                    KeyTextBox.Focus();
                }));
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
                            newRow["vt_ton_kho"] = 1;
                            newRow["gia_ton"] = 1;
                            newRow["sua_tk_vt"] = 0;
                            newRow["sl_min"] = 0;
                            newRow["sl_max"] = 0;
                            newRow["status"] = "1";                            
                            if (StartUp.SysObj.GetOption("M_AUTO_LIST_NUM").ToString().Equals("1"))
                            {
                                #region
                                if (!string.IsNullOrEmpty(StartUp.currSqlTableKey) && StartUp.SysObj.DmdmInfo.Select("ma_dm like '" + StartUp.sqlTableName + "' and  increase_type = 2").Length > 0)
                                {
                                    string _value = SysFunc.IncreaseCode(StartUp.SysObj, StartUp.currSqlTableKey, StartUp.SqlTableKey, StartUp.sqlTableName);
                                    if (!string.IsNullOrEmpty(_value) && _value.Length <= txtma_vt.MaxLength)
                                        newRow[StartUp.SqlTableKey] = _value;
                                }
                                #endregion

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
                            newDataTable = StartUp.GetRow(StartUp.sqlTableName);


                            if (newDataTable.Rows.Count > 0)
                            {
                                newDataTable.Rows[0][StartUp.SqlTableKey] = newDataTable.Rows[0][StartUp.SqlTableKey].ToString().Trim();
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
                                isError = true;
                                if (Convert.ToInt16(OldRow.Rows[0]["vt_ton_kho"]) == 1)
                                {
                                    //txtvt_ton_kho.IsEnabled = false;
                                }
                            }
                            TextBox NameTextBox = SmLib.SysFunc.FindChild<TextBox>(this, "txtten_vt"); //
                            if ((KeyTextBox == null || KeyTextBox.IsReadOnly) && NameTextBox != null)
                            {
                                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                                {
                                    NameTextBox.SelectAll();
                                    NameTextBox.Focus();
                                }));
                            }
                            else
                            {
                                if (KeyTextBox != null)
                                    KeyTextBox.Focus();
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
                            StartUp.LastEditRow = null;
                            newDataTable = StartUp.GetRow(StartUp.sqlTableName);
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
                            newDataTable = StartUp.GetRow(StartUp.sqlTableName);
                            newDataTable.Rows[0]["status"] = "1";

                            if (StartUp.SysObj.GetOption("M_AUTO_LIST_NUM").ToString().Equals("1"))
                            {
                                #region
                                if (!string.IsNullOrEmpty(StartUp.currSqlTableKey) && StartUp.SysObj.DmdmInfo.Select("ma_dm like '" + StartUp.sqlTableName + "' and  increase_type = 2").Length > 0)
                                {
                                    string _value = SysFunc.IncreaseCode(StartUp.SysObj, StartUp.currSqlTableKey, StartUp.SqlTableKey, StartUp.sqlTableName);
                                    if (!string.IsNullOrEmpty(_value) && _value.Length <= txtma_vt.MaxLength)
                                        newDataTable.Rows[0][StartUp.SqlTableKey] = _value;
                                }
                                #endregion

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

            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                if (txtgia_ton.Text == "0")
                    txtgia_ton.Text = "";
                if (!string.IsNullOrEmpty(txtgia_ton.Text.Trim()))
                {
                    txtgia_ton.SearchInit();
                    if (txtgia_ton.RowResult != null)
                    {
                        txtgia_ton_PreviewLostFocus(null, null);
                        //txtten_gia_ton.Text = StartUp.M_LAN == "V" ? txtgia_ton.RowResult["ten_loai"].ToString() : txtgia_ton.RowResult["ten_loai2"].ToString();
                        // txtten_gia_ton.Text = txtgia_ton.RowResult["ten_loai"].ToString();
                    }
                }
            }));
        }
        #endregion

        #region txtstatus_LostFocus
        private void txtstatus_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtstatus.Text == "")
                newDataTable.Rows[0]["status"] = 0;
        }
        #endregion
        #region saveCustomer
        void saveCustomer()
        {
            StartUp.LastEditRow = null;
            if (StartUp.currActionTask == ActionTask.View)
                return;
            int M_User_Id = int.Parse(StartUp.SysObj.UserInfo.Rows[0]["user_id"].ToString());
            string M_User_Name = StartUp.SysObj.UserInfo.Rows[0]["user_name"].ToString();

            newDataTable.AcceptChanges();

            if (newDataTable.Columns.Contains("search"))
                SmLib.SysFunc.SetStrSearch(StartUp.SysObj, StartUp.sqlTableName, ref newDataTable);

            if( newDataTable.Rows[0]["vt_ton_kho"].ToString().Equals("0"))
                newDataTable.Rows[0]["gia_ton"] = 0;
            if (StartUp.currActionTask == ActionTask.Edit)
            {
                newDataTable.Rows[0]["date"] = DateTime.Now;
                newDataTable.Rows[0]["time"] = DateTime.Now.ToString("HH:mm:ss");
                newDataTable.Rows[0]["user_id"] = M_User_Id;
                newDataTable.Rows[0]["user_name"] = M_User_Name;

                //update xuong database
                //neu update database thanh cong thi update datatable tren grid
                if (OldRow != null)
                    ListFunc.updateRowInDatabaseByKey(StartUp.sqlTableName, StartUp.SqlTableKey, OldRow.Rows[0], newDataTable.Rows[0], StartUp.SysObj);
                
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
            StartUp.LastEditRow = newDataTable.Select()[0]; 
        }
        #endregion

        #region CheckValid
        private bool CheckValid()
        {
            bool result = true;

            #region ma_vt
            if (result && txtma_vt.Text.Trim() == string.Empty)
            {
                TabInfor.SelectedIndex = 0;
                ExMessageBox.Show( 2215,StartUp.SysObj, "Chưa vào mã " + "[" + StartUp.TableName + "]" + "!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                txtma_vt.Focus();
                result = false;
            }
            if (result && txtma_vt.Text.Trim() != string.Empty)
            {
                string sb = SmLib.SysFunc.CheckInValidCode(StartUp.SysObj, txtma_vt.Text.Trim());
                if (sb != "" && result == true)
                {
                    TabInfor.SelectedIndex = 0;
                    ExMessageBox.Show( 2220,StartUp.SysObj, "Mã không được chứa các ký tự " + "[" + sb + "]" + " !", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtma_vt.SelectAll();
                    txtma_vt.Focus();
                    result = false;
                }
                if (result)
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
                        cmdGet.Parameters.Add("@" + StartUp.SqlTableKey, SqlDbType.Char).Value = txtma_vt.Text.Trim();
                        int ListExist = (int)StartUp.SysObj.ExcuteScalar(cmdGet);
                        if (ListExist > 0 && ischeck == true)
                        {
                            TabInfor.SelectedIndex = 0;
                            ExMessageBox.Show( 2225,StartUp.SysObj, "Mã đã có hoặc mã lồng nhau!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);

                            txtma_vt.SelectAll();
                            txtma_vt.Focus();
                            result = false;
                        }
                        if (result && ischeck)
                        {
                            string ma_cu = "";
                            if (StartUp.currActionTask == ActionTask.Edit)
                                ma_cu = OldRow.Rows[0][StartUp.SqlTableKey].ToString().Trim();
                            if (SmLib.SysFunc.CheckStringContain(StartUp.SysObj, StartUp.sqlTableName, StartUp.SqlTableKey, txtma_vt.Text.Trim(), ma_cu))
                            {
                                TabInfor.SelectedIndex = 0;
                                ExMessageBox.Show( 2230,StartUp.SysObj, "Mã đã có hoặc mã lồng nhau!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                                txtma_vt.SelectAll();
                                txtma_vt.Focus();
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

            #region ten_vt
            if (result && txtten_vt.Text.Trim() == string.Empty)
            {
                TabInfor.SelectedIndex = 0;
                ExMessageBox.Show( 2235,StartUp.SysObj, "Chưa vào tên " + "[" + StartUp.TableName + "]" + "!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                txtten_vt.Focus();
                result = false;
            } 
            #endregion

            #region dvt
            if (result && isError)
            {
                if (OldRow.Rows[0]["dvt"].ToString().Trim() != "" && txtdvt.Text.Trim() == "")
                {
                    TabInfor.SelectedIndex = 0;
                    ExMessageBox.Show( 2240,BindingSysObj, "Đã có phát sinh, đơn vị tính không được để trống!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtdvt.Focus();
                    result = false;
                }
            } 
            #endregion

            #region gia_ton
            if (result && txtvt_ton_kho.Value.ToString() == "1" && txtgia_ton.Text.Trim() == "")
            {
                TabInfor.SelectedIndex = 0;
                ExMessageBox.Show( 2245,BindingSysObj, "Chưa chọn cách tính giá tồn kho!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                txtgia_ton.IsFocus = true;
                result = false;
            }
            #endregion

            #region loai_vt
            if (result && !txtloai_vt.CheckLostFocus())
            {
                TabInfor.SelectedIndex = 0;
                ExMessageBox.Show( 2250,StartUp.SysObj, "Loại vật tư không hợp lệ!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                txtloai_vt.IsFocus = true;
                result = false;
            }
            #endregion

            #region tk_vt
            if (result && txttk_vt.Text.Trim() == string.Empty)
            {
                if (txtvt_ton_kho.Text.Trim() == "1")
                {
                    TabInfor.SelectedIndex = 0;
                    ExMessageBox.Show( 2255,StartUp.SysObj, "Chưa vào tài khoản kho!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                    txttk_vt.IsFocus = true;
                    result = false;
                }
            }
            //http://forum.fast.com.vn/showthread.php?t=8601 sua theo ma loi 137496753
            //if (result && txttk_vt.Text.Trim() == string.Empty)
            //{
            //    if (txtsua_tk_kho.Text.Trim() == "0")
            //    {
            //        TabInfor.SelectedIndex = 0;
            //        ExMessageBox.Show(2256, StartUp.SysObj, "Chưa vào tài khoản kho!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
            //        txttk_vt.IsFocus = true;
            //        result = false;
            //    }
            //}
            if (result && !txttk_vt.CheckLostFocus())
            {
                TabInfor.SelectedIndex = 0;
                ExMessageBox.Show( 2260,StartUp.SysObj, "Tài khoản kho không hợp lệ!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                txttk_vt.IsFocus = true;
                result = false;
            }
            if (result && StartUp.currActionTask == ActionTask.Edit)
            {

                if (OldRow.Rows[0]["tk_vt"].ToString().Trim() != newDataTable.Rows[0]["tk_vt"].ToString().Trim())
                {
                    try
                    {
                        System.Data.SqlClient.SqlCommand cmdGet = new System.Data.SqlClient.SqlCommand("select Top(1)* from ct70 where ma_vt=@ma_vt and tk_vt=@tk_vt");
                        cmdGet.Parameters.Add("@" + StartUp.SqlTableKey, SqlDbType.Char).Value = StartUp.currSqlTableKey;
                        cmdGet.Parameters.Add("@tk_vt", SqlDbType.Char).Value = OldRow.Rows[0]["tk_vt"].ToString();
                        DataTable dt = StartUp.SysObj.ExcuteReader(cmdGet).Tables[0];
                        if (dt.Rows.Count > 0)
                        {
                            TabInfor.SelectedIndex = 0;
                            if (ExMessageBox.Show( 2265,StartUp.SysObj, "Tài khoản kho cũ đã có phát sinh, có tiếp tục không?", "Xac nhan nhap lieu", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                            {
                                txttk_vt.IsFocus = true;
                                result = false;
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        ErrorLog.CatchMessage(ex);
                    }
                }

            } 
            #endregion

            #region tk_dt
            if (result && !txttk_dt.CheckLostFocus())
            {
                TabInfor.SelectedIndex = 0;
                ExMessageBox.Show( 2270,StartUp.SysObj, "Tài khoản doanh thu không hợp lệ!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                txttk_dt.IsFocus = true;
                result = false;
            }
            #endregion

            #region tk_tl
            if (result && !txttk_tl.CheckLostFocus())
            {
                TabInfor.SelectedIndex = 0;
                ExMessageBox.Show( 2275,StartUp.SysObj, "Tài khoản hàng bán bị trả lại không hợp lệ!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                txttk_tl.IsFocus = true;
                result = false;
            }
            #endregion

            #region tk_dtnb
            if (result && !txttk_dtnb.CheckLostFocus())
            {
                TabInfor.SelectedIndex = 0;
                ExMessageBox.Show( 2280,StartUp.SysObj, "Tài khoản doanh thu nội bộ không hợp lệ!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                txttk_dtnb.IsFocus = true;
                result = false;
            }
            #endregion

            #region tk_cl_vt
            if (result && !txttk_cl_vt.CheckLostFocus())
            {
                TabInfor.SelectedIndex = 0;
                ExMessageBox.Show( 2285,StartUp.SysObj, "Tài khoản chênh lệch giá vật tư không hợp lệ!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                txttk_cl_vt.IsFocus = true;
                result = false;
            }
            #endregion

            #region tk_ck
            if (result && !txttk_ck.CheckLostFocus())
            {
                TabInfor.SelectedIndex = 0;
                ExMessageBox.Show( 2290,StartUp.SysObj, "Tài khoản chiết khấu không hợp lệ!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                txttk_ck.IsFocus = true;
                result = false;
            }
            #endregion

            #region tk_nvl
            if (result && !txttk_nvl.CheckLostFocus())
            {
                TabInfor.SelectedIndex = 0;
                ExMessageBox.Show( 2295,StartUp.SysObj, "Tài khoản nguyên vật liệu không hợp lệ!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                txttk_nvl.IsFocus = true;
                result = false;
            }
            #endregion

            #region tk_gv
            if (result && !txttk_gv.CheckLostFocus())
            {
                TabInfor.SelectedIndex = 0;
                ExMessageBox.Show( 2300,StartUp.SysObj, "Tài khoản giá vốn không hợp lệ!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                txttk_gv.IsFocus = true;
                result = false;
            }
            #endregion

            #region tk_spdd
            if (result && !txttk_spdd.CheckLostFocus())
            {
                TabInfor.SelectedIndex = 0;
                ExMessageBox.Show( 2305,StartUp.SysObj, "Tài khoản sản phẩm dở dang không hợp lệ!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                txttk_spdd.IsFocus = true;
                result = false;
            }
            #endregion

            #region tk_km
            if (result && !txttk_km.CheckLostFocus())
            {
                TabInfor.SelectedIndex = 0;
                ExMessageBox.Show( 2310,StartUp.SysObj, "Tài khoản cp khuyến mãi không hợp lệ!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                txttk_km.IsFocus = true;
                result = false;
            }
            #endregion

            #region nh_vt1
            if (result && !txtnh_vt1.CheckLostFocus())
            {
                TabInfor.SelectedIndex = 0;
                ExMessageBox.Show( 2315,StartUp.SysObj, "Nhóm vật tư 1 không hợp lệ!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                txtnh_vt1.IsFocus = true;
                result = false;
            }
            #endregion

            #region nh_vt2
            if (result && !txtnh_vt1.CheckLostFocus())
            {
                TabInfor.SelectedIndex = 0;
                ExMessageBox.Show( 2320,StartUp.SysObj, "Nhóm vật tư 2 không hợp lệ!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                txtnh_vt2.IsFocus = true;
                result = false;
            }
            #endregion

            #region nh_vt3
            if (result && !txtnh_vt1.CheckLostFocus())
            {
                TabInfor.SelectedIndex = 0;
                ExMessageBox.Show( 2325,StartUp.SysObj, "Nhóm vật tư 3 không hợp lệ!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Information);
                txtnh_vt3.IsFocus = true;
                result = false;
            }
            #endregion
            return result;
        }
        #endregion

        #region EnableEditMode
        public void EnableEditMode(bool isEditMode)
        {
            FormInEditMode.IsEditMode = isEditMode;
        }
        #endregion

        #region txtloai_vt_PreviewLostFocus
         private void txtloai_vt_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (txtloai_vt.RowResult == null)
                txtten_loai_vt.Text = "";
            else
                txtten_loai_vt.Text = StartUp.M_LAN.Equals("V") ? txtloai_vt.RowResult["ten"].ToString() : txtloai_vt.RowResult["ten2"].ToString();
        }
        #endregion

        #region txttk_vt_PreviewLostFocus
           private void txttk_vt_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (txttk_vt.RowResult == null)
                txtten_tk_vt.Text = "";
            else
                txtten_tk_vt.Text = StartUp.M_LAN.Equals("V") ? txttk_vt.RowResult["ten_tk"].ToString() : txttk_vt.RowResult["ten_tk2"].ToString();
        }
        #endregion
           #region txtma_kho_PreviewLostFocus
           private void txtma_kho_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
           {
               if (txtma_kho.RowResult == null)
                   txtten_kho.Text = "";
               else
                   txtten_kho.Text = StartUp.M_LAN.Equals("V") ? txtma_kho.RowResult["ten_kho"].ToString() : txtma_kho.RowResult["ten_kho2"].ToString();
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

        #region txtvt_ton_kho_LostFocus
        private void txtvt_ton_kho_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtvt_ton_kho.Text.Trim() == "")
                newDataTable.Rows[0]["vt_ton_kho"] = 0;
                
        }
        #endregion

        #region txtsua_tk_kho_LostFocus
        private void txtsua_tk_kho_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!txtsua_tk_kho.IsFocusWithin && txtsua_tk_kho.Text.Trim() == "")
                newDataTable.Rows[0]["sua_tk_vt"] = 0;
        }
        #endregion

        #region txtsl_min_LostFocus
        private void txtsl_min_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtsl_min.Text.Trim() == "")
                txtsl_min.Value = 0;
        }
        #endregion

        #region txtsl_max_LostFocus
        private void txtsl_max_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtsl_max.Text.Trim() == "")
                txtsl_max.Value = 0;
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
                txt.SelectionLength = 1;
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

        #region txtma_vt_GotFocus
        private void txtma_vt_GotFocus(object sender, RoutedEventArgs e)
        {
            txtma_vt.Text = txtma_vt.Text.Trim();
            txtma_vt.SelectAll();
        }
        #endregion

        #region txtma_tra_cuu_GotFocus
        private void txtma_tra_cuu_GotFocus(object sender, RoutedEventArgs e)
        {
            txtma_tra_cuu.Text = txtma_tra_cuu.Text.Trim();
            txtma_tra_cuu.SelectAll();
        }
        #endregion

        #region txtdvt_LostFocus
        private void txtdvt_LostFocus(object sender, RoutedEventArgs e)
        {
            if (isError)
            {
                if (OldRow.Rows[0]["dvt"].ToString().Trim() != "" && txtdvt.Text.Trim() == "")
                {
                    ExMessageBox.Show( 2330,BindingSysObj, "Đã có phát sinh, đơn vị tính không được để trống!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else if (txtdvt.Text.Trim() == "")
            {
                newDataTable.Rows[0]["vt_ton_kho"] = 0;
                //newDataTable.Rows[0]["gia_ton"] = DBNull.Value;
                //txtten_gia_ton.Text = "";
            }
            
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

        #region txtgia_ton_PreviewLostFocus
        private void txtgia_ton_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (txtgia_ton.RowResult == null)
                txtten_gia_ton.Text = "";
            else
            {
                if(StartUp.M_LAN == "V")
                    txtten_gia_ton.Text = txtgia_ton.RowResult["ten_loai"].ToString();
                else
                    txtten_gia_ton.Text = txtgia_ton.RowResult["ten_loai2"].ToString();

            }
        } 
        #endregion

        #region txtkieu_xe_PreviewLostFocus
        private void txtkieu_xe_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (txtma_kx.RowResult == null)
                txtten_kx.Text = "";
            else
            {
                if (StartUp.M_LAN == "V")
                    txtten_kx.Text = txtma_kx.RowResult["ten_kx"].ToString();
                else
                    txtten_kx.Text = txtma_kx.RowResult["ten_kx2"].ToString();

            }
        }
        #endregion

        #region txtma_mau_PreviewLostFocus
        private void txtma_mau_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (txtma_mau.RowResult == null)
                txtten_mau.Text = "";
            else
            {
                if (StartUp.M_LAN == "V")
                    txtten_mau.Text = txtma_mau.RowResult["ten_mau"].ToString();
                else
                    txtten_mau.Text = txtma_mau.RowResult["ten_mau2"].ToString();

            }
        }
        #endregion

        #region ConfirmGV_OnCancel
        private void ConfirmGV_OnCancel(object sender, RoutedEventArgs e)
        {
            SysFunc.RollbackMadm(BindingSysObj, StartUp.sqlTableName);
            StartUp.LastEditRow = null;
            this.Close();
        }
        #endregion

        #region FormList_PreviewKeyUp
        private void FormList_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.None && e.Key == Key.Escape)
            {
                StartUp.LastEditRow = null;
                this.Close();
            }

        }
        #endregion

        private void txtvt_ton_kho_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            txtgia_ton.Text = Convert.ToInt32(e.NewValue) == 0 ? "" : "1";
            txtgia_ton.SearchInit();
            txtgia_ton_PreviewLostFocus(null, null);
        }

    }
}
