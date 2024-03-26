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
using Sm.Windows.Controls;
using System.Data;
using Infragistics.Windows.DataPresenter;

namespace Inctpxd
{
    /// <summary>
    /// Interaction logic for FrmSOCTPNF_PN.xaml
    /// </summary>
    public partial class FrmInctpxd_Pn : Form
    {
        CodeValueBindingObject Voucher_Ma_nt0;
        public DataRowView drvFrmINCTPXD_PN;
        public FrmInctpxd_Pn(DataTable tbSource, string ten_vt)
        {
            InitializeComponent();
            SmLib.SysFunc.LoadIcon(this);
            Loaded += new RoutedEventHandler(FrmPoctpxf_PN_Loaded);
            this.Title = SmLib.SysFunc.Cat_Dau(ten_vt.ToString());
            this.EscToClose = true;
            GrdINCTPXD_PN.DataSource = tbSource.DefaultView;
            if (StartUp.M_LAN.Equals("V"))
            {
                GrdINCTPXD_PN.FieldLayouts[0].Fields["ten_ct2"].Visibility = Visibility.Collapsed;
            }
            else
            {
                GrdINCTPXD_PN.FieldLayouts[0].Fields["ten_ct"].Visibility = Visibility.Collapsed;
            }
        }

        private void FrmPoctpxf_PN_Loaded(object sender, RoutedEventArgs e)
        {
            Voucher_Ma_nt0 = (CodeValueBindingObject)this.FindResource("Voucher_Ma_nt0");
            Voucher_Ma_nt0.Text = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString();
            Voucher_Ma_nt0.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0));

            if (GrdINCTPXD_PN.Records.Count > 0)
            {
                GrdINCTPXD_PN.Focus();
                GrdINCTPXD_PN.ActiveRecord = GrdINCTPXD_PN.Records[0];
            }
            isVisibleField();
        }

       

        private void ConfirmGridView_OnOk(object sender, RoutedEventArgs e)
        {
            if (GrdINCTPXD_PN.ActiveRecord != null)
            {
                drvFrmINCTPXD_PN = (GrdINCTPXD_PN.ActiveRecord as DataRecord).DataItem as DataRowView;
                this.Close();
            }
        }

        private void ConfirmGridView_OnCancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public void isVisibleField()
        {
            if (Voucher_Ma_nt0.Text.Trim().Equals(StartUp.M_ma_nt0))
            {

                GrdINCTPXD_PN.FieldLayouts[0].Fields["gia_nt"].Visibility = Visibility.Hidden;
                GrdINCTPXD_PN.FieldLayouts[0].Fields["gia_nt"].Settings.CellMaxWidth = 0;
            }
            else
            {
                GrdINCTPXD_PN.FieldLayouts[0].Fields["gia_nt"].Visibility = Visibility.Visible;
                GrdINCTPXD_PN.FieldLayouts[0].Fields["gia_nt"].Settings.CellMaxWidth = GrdINCTPXD_PN.FieldLayouts[0].Fields["gia_nt"].Width.Value.Value;
            }

        }

    }
}
