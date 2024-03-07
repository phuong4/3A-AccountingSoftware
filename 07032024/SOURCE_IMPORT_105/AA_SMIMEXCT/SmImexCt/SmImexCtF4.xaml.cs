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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Sm.Windows.Controls;
using SysLib;
using Sm.Languages;
using Infragistics.Windows.Editors;
using System.Diagnostics;
using SmReport;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Win32;
using SmLib.SM.FormBrowse;
using System.IO;
using Infragistics.Windows.DataPresenter;

namespace AA_SMIMEXCT
{
    /// <summary>
    /// Interaction logic for Window1.xaml 
    /// </summary>
    public partial class AA_SMIMEXCTF4 : FormFilter
    {
       
        bool bResult = false;
        public static string Ten_file = "";
        public AA_SMIMEXCTF4() 
        { 
            InitializeComponent();
            txtFileName.Focus();
            
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == FormFilter.DataContextProperty)
            {
                DataRowView item = DataContext as DataRowView;
                string _ma_imex_info = item["ma_imex"].ToString().Trim();
                txtMa_qs.Filter = " ma_cts like '%" + _ma_imex_info.Trim() + "%'";

                if (_ma_imex_info == StartUp.ma_imex_truoc)
                    txtFileName.Text = StartUp._paths;
                else
                    txtFileName.Text = "";
            }
        }

        private void ConfirmGridView_OnOk(object sender, RoutedEventArgs e)
        {
            txtMa_qs.SearchInit();
          
            if (txtFileName.Text.Trim() == "")
            {
                ExMessageBox.Show(105, StartUp.SysObj, string.Format("Chưa chọn file excel!", txtFileName.Text), "", MessageBoxButton.OK, MessageBoxImage.Information);
                txtFileName.Focus();
                return;
            }
            if (!File.Exists(txtFileName.Text))
            {
                ExMessageBox.Show(110, StartUp.SysObj, string.Format("Tập tin [{0}] không tồn tại!", txtFileName.Text), "", MessageBoxButton.OK, MessageBoxImage.Information);
                txtFileName.Focus();
                return ;
            }
            if (!txtMa_qs.CheckLostFocus())
            {
                ExMessageBox.Show(130, BindingSysObj, "Chưa nhập quyển số!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                txtMa_qs.IsFocus = true;
                return;
            }
            //copy file excel to temp
            Directory.CreateDirectory(Environment.GetEnvironmentVariable("Temp") + "\\ExcelImport\\");
            string _tempPath = Environment.GetEnvironmentVariable("Temp") + "\\ExcelImport\\" + Ten_file.Trim();
            string _currentPath = txtFileName.Text;
            if (File.Exists(_tempPath))
            {
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(_tempPath);
                if (fileInfo.IsReadOnly == true)
                    fileInfo.IsReadOnly = false;
            }
            File.Copy(_currentPath, _tempPath, true);
            ////

            DataRowView item = DataContext as DataRowView;
            ImportInfo info = new ImportInfo();
            info.Name = item["ten"].ToString();
            info.FileName = _tempPath;
            info.TableTemplate =  item["dbf_mau"].ToString();
            info.ExcelTemplate = item["ex_mau"].ToString();
            info.Ma_Imex = item["ma_imex"].ToString();
            info.PostProc = item["postproc"].ToString();
            info.Ma_ct = item["ma_ct"].ToString();
            info.Ma_qs = txtMa_qs.Text.Trim();
            info.Xy_ly = txtXyLy.Text.Trim();
            info.StrBrowseV = item["vbrowse1"].ToString().Trim();
            info.StrBrowseE = item["ebrowse2"].ToString().Trim();
            info.FieldNotNull = item["check_null"].ToString().Trim();
            Info = info;

            bResult = true;
            Close();
        }

        internal ImportInfo Info
        {
            get;
            private set;
        }

        public new bool ShowDialog()
        {
            base.ShowDialog();
            return bResult;
        }


        private void btnDbf_Click(object sender, RoutedEventArgs e)
        {
            if (txtFileName.Text.Trim() == "")
            {
                ExMessageBox.Show(215, StartUp.SysObj, string.Format("Chưa chọn file excel!", txtFileName.Text), "", MessageBoxButton.OK, MessageBoxImage.Information);
                txtFileName.Focus();
                return;
            }
            if (!File.Exists(txtFileName.Text))
            {
                ExMessageBox.Show(220, StartUp.SysObj, string.Format("Tập tin [{0}] không tồn tại!", txtFileName.Text), "", MessageBoxButton.OK, MessageBoxImage.Information);
                txtFileName.Focus();
                return;
            }

            //copy file excel to temp
            Directory.CreateDirectory(Environment.GetEnvironmentVariable("Temp") + "\\ExcelImport\\");
            string _tempPath = Environment.GetEnvironmentVariable("Temp") + "\\ExcelImport\\" + Ten_file.Trim();
            string _currentPath = txtFileName.Text;
            if (File.Exists(_tempPath))
            {
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(_tempPath);
                if (fileInfo.IsReadOnly == true)
                    fileInfo.IsReadOnly = false;
            }
            File.Copy(_currentPath, _tempPath, true);
            ////
            DataSet output = new DataSet();
            DataTable tbExcel = C_GetDataExcel.GetDataFromExcel(_tempPath, "");

            if (tbExcel == null)
                return;

            output.Tables.Add(tbExcel.Copy());
            
           
            if (txtBangMa.Text.Trim() == "1")
            {
               
                DataTable tbStrucImex = C_GetDataExcel.GetStructTable(this.TableTemplate);
                output.Tables.Add(tbStrucImex.Copy());
                C_GetDataExcel.ConverFont(ref output);
                
            }

            
            FormBrowse oBrowse = new FormBrowse(StartUp.SysObj, output.Tables[0].DefaultView, C_GetDataExcel.StrBrowse);
            oBrowse.frmBrw.Title = SmLib.SysFunc.Cat_Dau(TemplateTitle);
            oBrowse.frmBrw.LanguageID = "AA_SMIMEXCT_2";
            
                 
            oBrowse.ShowDialog();
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            string curPath = Environment.CurrentDirectory;

            string path = txtFileName.Text;
            string fileName = "";
            if(path.Trim() != "")
            {
                path = System.IO.Path.GetDirectoryName(txtFileName.Text);

                if (path.Trim() != "" && System.IO.Directory.Exists(path))
                    Environment.CurrentDirectory = path;
                fileName = System.IO.Path.GetFileName(txtFileName.Text);
            }

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.FileName = fileName;
            dlg.Filter = "Microsoft excel (*.Xls)|*.xls";

            if (dlg.ShowDialog() == true)
            {
                txtFileName.Text = dlg.FileName;
                Ten_file = dlg.SafeFileName;
            }


            Environment.CurrentDirectory = curPath;
        }

        private string Fields
        {
            get
            {
                DataRowView item = DataContext as DataRowView;
                object fields = StartUp.M_LAN.Equals("V") ? item["vbrowse1"] : item["ebrowse2"];
                if (fields is DBNull || fields.ToString().Trim() == "")
                    return "";
                return fields.ToString().Trim();
                
            }
           
        }

        private string TableTemplate
        {
            get
            {
                DataRowView item = DataContext as DataRowView;

                return item["dbf_mau"].ToString().Trim();

            }

        }

        private string TemplateTitle
        {
            get
            {
                DataRowView item = DataContext as DataRowView;
                return item[StartUp.M_LAN.Equals("V") ? "ten" : "ten2"].ToString().Trim();
            }
        }

        private void btnMau_Click(object sender, RoutedEventArgs e)
        {
            StartUp.SysObj.SynchroFile(@".\Excel-Mau", txtdbf_mau.Text.Trim() + ".Xls");

            string fileName = StartUp.SysObj.M_StartUp_Path + "Excel-Mau\\" + txtdbf_mau.Text.Trim() + ".Xls";
            if (System.IO.File.Exists(fileName))
                Process.Start(fileName);
            else
                Debug.WriteLine(string.Format("File {0} does not exist.", fileName));
        }

        private void btnXuatMau_Click(object sender, RoutedEventArgs e)
        {
            StartUp.SysObj.SynchroFile(@".\Excel-Mau", txtdbf_mau.Text.Trim() + ".Xls");

            string path_mau = StartUp.SysObj.M_StartUp_Path + "Excel-Mau\\" + txtdbf_mau.Text.Trim() + ".Xls";
            SaveFileDialog savefileDialog = new SaveFileDialog();
            savefileDialog.Filter = "Excel 2003 (.xls)|*.xls";
            string _path = "";
            if (savefileDialog.ShowDialog() == true)
            {
                _path = savefileDialog.FileName;
                try
                {
                    File.Copy(path_mau, _path, true);

                }
                catch (Exception ex)
                {
                    ExMessageBox.Show(125, StartUp.SysObj, "[" + ex.Message + "]", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
            }
            if (System.IO.File.Exists(_path))
                Process.Start(_path);
        }

        private void txtMa_qs_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            AutoCompleteTextBox txt = sender as AutoCompleteTextBox;

            if (txt.RowResult == null)
            {
                lblTen_qs.Text = "";
                return;
            }

            try
            {
                lblTen_qs.Text = StartUp.M_LAN.Equals("V") ? txt.RowResult["ten_qs"].ToString() : txt.RowResult["ten_qs2"].ToString();
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }  
        }

        private void FrmAA_SMIMEXCTF4_Closed(object sender, EventArgs e)
        {
            DataRowView item = DataContext as DataRowView;
            
            StartUp._paths = txtFileName.Text.Trim();
            StartUp.ma_imex_truoc = item["ma_imex"].ToString().Trim();
        }

        private void txtXyLy_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtXyLy.Text.Trim()))
                txtXyLy.Text = "0";
        }

        private void txtBangMa_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtBangMa.Text.Trim()))
                txtBangMa.Text = "0";
        }

    }
}
