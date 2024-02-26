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

namespace Socthda
{
    /// <summary>
    /// Interaction logic for FrmSOCTPNF_PN.xaml
    /// </summary>
    public partial class FrmSocthda_Pn : Form
    {
        CodeValueBindingObject Voucher_Ma_nt0;
        public DataRowView drvFrmSOCTPNF_PN;
        public FrmSocthda_Pn(DataTable tbSource, string ten_vt)
        {
            InitializeComponent();
            SmLib.SysFunc.LoadIcon(this);
            this.EscToClose = true;
            this.Title = SmLib.SysFunc.Cat_Dau(ten_vt.ToString());
            GrdSOCTHDA_PN.DataSource = tbSource.DefaultView;
        }

        private void FrmPoctpxf_PN_Loaded(object sender, RoutedEventArgs e)
        {
            Voucher_Ma_nt0 = (CodeValueBindingObject)this.FindResource("Voucher_Ma_nt0");
            Voucher_Ma_nt0.Text = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString();
            Voucher_Ma_nt0.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0));

            if (GrdSOCTHDA_PN.Records.Count > 0)
            {
                GrdSOCTHDA_PN.Focus();
                GrdSOCTHDA_PN.ActiveRecord = GrdSOCTHDA_PN.Records[0];
            }
            isVisibleField();
        }

       

        private void ConfirmGridView_OnOk(object sender, RoutedEventArgs e)
        {
            if (GrdSOCTHDA_PN.ActiveRecord != null)
            {
                drvFrmSOCTPNF_PN = (GrdSOCTHDA_PN.ActiveRecord as DataRecord).DataItem as DataRowView;
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
                
                GrdSOCTHDA_PN.FieldLayouts[0].Fields["gia_nt"].Visibility = Visibility.Hidden;
                GrdSOCTHDA_PN.FieldLayouts[0].Fields["gia_nt"].Settings.CellMaxWidth = 0;
            }
            else
            {
                GrdSOCTHDA_PN.FieldLayouts[0].Fields["gia_nt"].Visibility = Visibility.Visible;
                GrdSOCTHDA_PN.FieldLayouts[0].Fields["gia_nt"].Settings.CellMaxWidth = GrdSOCTHDA_PN.FieldLayouts[0].Fields["gia_nt"].Width.Value.Value;
            }

        }

    }
}
