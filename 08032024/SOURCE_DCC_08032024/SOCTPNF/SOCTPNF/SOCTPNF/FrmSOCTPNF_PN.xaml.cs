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
using System.Windows.Threading;

namespace SOCTPNF
{
    /// <summary>
    /// Interaction logic for FrmSOCTPNF_PN.xaml
    /// </summary>
    public partial class FrmSOCTPNF_PN : Form
    {
        CodeValueBindingObject Voucher_Ma_nt0;
        public DataRowView drvFrmSOCTPNF_PN;
        public FrmSOCTPNF_PN(DataTable tbSource, string ten_vt)
        {
           
            InitializeComponent();
            SmLib.SysFunc.LoadIcon(this);
            Loaded += new RoutedEventHandler(FrmSOCTPNF_PN_Loaded);
            frmSOCTPNF_PN.Title = SmLib.SysFunc.Cat_Dau(ten_vt.ToString());
            GrdSOCTPNF_PN.DataSource = tbSource.DefaultView;
            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => {
                if (GrdSOCTPNF_PN.Records.Count > 0)
                    GrdSOCTPNF_PN.ActiveRecord = GrdSOCTPNF_PN.Records[0];
            }));
            if (StartUp.M_LAN.Equals("V"))
            {
                GrdSOCTPNF_PN.FieldLayouts[0].Fields["ten_ct2"].Visibility = Visibility.Collapsed;
            }
            else
            {
                GrdSOCTPNF_PN.FieldLayouts[0].Fields["ten_ct"].Visibility = Visibility.Collapsed;
            }
        }

        private void FrmSOCTPNF_PN_Loaded(object sender, RoutedEventArgs e)
        {
            Voucher_Ma_nt0 = (CodeValueBindingObject)frmSOCTPNF_PN.FindResource("Voucher_Ma_nt0");
            Voucher_Ma_nt0.Text = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString();
            Voucher_Ma_nt0.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0));

            if (GrdSOCTPNF_PN.Records.Count > 0)
            {
                GrdSOCTPNF_PN.ActiveRecord = GrdSOCTPNF_PN.Records[0];
            }
            GrdSOCTPNF_PN.Focus();
            isVisibleField();
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }

        private void ConfirmGridView_OnOk(object sender, RoutedEventArgs e)
        {
            if (GrdSOCTPNF_PN.ActiveRecord != null)
            {
                drvFrmSOCTPNF_PN = (GrdSOCTPNF_PN.ActiveRecord as DataRecord).DataItem as DataRowView;
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
                GrdSOCTPNF_PN.FieldLayouts[0].Fields["gia_nt2"].Visibility = Visibility.Hidden;
                GrdSOCTPNF_PN.FieldLayouts[0].Fields["gia_nt"].Visibility = Visibility.Hidden;
                GrdSOCTPNF_PN.FieldLayouts[0].Fields["ck_nt"].Visibility = Visibility.Hidden;

                GrdSOCTPNF_PN.FieldLayouts[0].Fields["gia_nt2"].Settings.CellMaxWidth = 0;
                GrdSOCTPNF_PN.FieldLayouts[0].Fields["gia_nt"].Settings.CellMaxWidth = 0;
                GrdSOCTPNF_PN.FieldLayouts[0].Fields["ck_nt"].Settings.CellMaxWidth = 0;

            }
            else
            {
                GrdSOCTPNF_PN.FieldLayouts[0].Fields["gia_nt2"].Visibility = Visibility.Visible;
                GrdSOCTPNF_PN.FieldLayouts[0].Fields["gia_nt"].Visibility = Visibility.Visible;
                GrdSOCTPNF_PN.FieldLayouts[0].Fields["ck_nt"].Visibility = Visibility.Visible;

                GrdSOCTPNF_PN.FieldLayouts[0].Fields["gia_nt2"].Settings.CellMaxWidth = GrdSOCTPNF_PN.FieldLayouts[0].Fields["gia_nt2"].Width.Value.Value;
                GrdSOCTPNF_PN.FieldLayouts[0].Fields["gia_nt"].Settings.CellMaxWidth = GrdSOCTPNF_PN.FieldLayouts[0].Fields["gia_nt2"].Width.Value.Value;
                GrdSOCTPNF_PN.FieldLayouts[0].Fields["ck_nt"].Settings.CellMaxWidth = GrdSOCTPNF_PN.FieldLayouts[0].Fields["ck_nt"].Width.Value.Value;

            }

        }

    }
}
