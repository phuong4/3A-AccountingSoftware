using System.Windows;
using SmReport;
using Sm.Windows.Controls;

namespace Arkh
{
    /// <summary>
    /// Interaction logic for ArkhF10.xaml
    /// </summary>
    partial class ArkhF10 : FormFilter
    {
        private bool isOK = false;
        public ArkhF10()
        {
            InitializeComponent();
            txtSapXep.Focus();
        }

 
        private void ConfirmGridView_OnOk(object sender, RoutedEventArgs e)
        {
            isOK = true;
            Close();
        }

        public new bool ShowDialog()
        {
            base.ShowDialog();
            return isOK;
        }

        void txt_GotFocus(object sender, RoutedEventArgs e)
        {
            NumericTextBox txt = sender as NumericTextBox;
            txt.SelectAll();
        }

        private void FormFilter_Loaded(object sender, RoutedEventArgs e)
        {
            txtSapXep.Value = "1";
            txtSapXep.Focus();
        }

    }
}
