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

namespace SmImexCt
{
    /// <summary>
    /// Interaction logic for Window1.xaml 
    /// </summary>
    public partial class Soth3Loc : FormFilter
    {
        bool bResult = false;

        public Soth3Loc() { InitializeComponent(); }

        public string AdvanceFilter
        {
            get
            {
                GridSearch._GenerateSQLString();
                return GridSearch.arrStrFilter[0];
            }
        }

        private void ConfirmGridView_OnOk(object sender, RoutedEventArgs e)
        {
            bResult = true;
            Hide();
        }

        public new bool ShowDialog()
        {
            base.ShowDialog();
            return bResult;
        }

        private void FrmSoth3Loc_Loaded(object sender, RoutedEventArgs e)
        {
            txtNgay_ct1.Focus();
            GridSearch.SysObj = BindingSysObj;
            GridSearch.filterID = "SmImexCt";
            GridSearch.tableList = "v_Soth3";
        }

        protected override bool IsEnterToPassObject(object sender)
        {
            if (sender is XamComboEditor)
                return true;

            return base.IsEnterToPassObject(sender);
        }

        private void txtMa_kh_LostFocus(object sender, RoutedEventArgs e)
        {
            AutoCompleteTextBox txt = sender as AutoCompleteTextBox;

            if (txt.RowResult == null)
            {
                txtTen_kh.Text = "";
                return;
            }

            try
            {
                txtTen_kh.Text = txt.RowResult["ten_kh"].ToString();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void txtMa_vt_LostFocus(object sender, RoutedEventArgs e)
        {
            AutoCompleteTextBox txt = sender as AutoCompleteTextBox;

            if (txt.RowResult == null)
            {
                txtTen_vt.Text = "";
                return;
            }

            try
            {
                txtTen_vt.Text = txt.RowResult["ten_vt"].ToString();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

        }
    }
}
