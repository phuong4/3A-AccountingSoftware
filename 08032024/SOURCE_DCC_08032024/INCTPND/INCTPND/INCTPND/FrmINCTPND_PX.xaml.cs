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
namespace INCTPND
{
    /// <summary>
    /// Interaction logic for FrmINCTPND_PX.xaml
    /// </summary>
    public partial class FrmINCTPND_PX : Form
    {
        CodeValueBindingObject Voucher_Ma_nt0;
        public DataRowView drvFrmINCTPND_PX;

        public FrmINCTPND_PX(DataTable tbSource, string ten_vt)
        {
            InitializeComponent();
            SmLib.SysFunc.LoadIcon(this);
            Loaded += new RoutedEventHandler(FrmINCTPND_PX_Loaded);
            frmINCTPND_PX.Title = SmLib.SysFunc.Cat_Dau(ten_vt.ToString());
            GrdINCTPND_PX.DataSource = tbSource.DefaultView;
        }

        private void FrmINCTPND_PX_Loaded(object sender, RoutedEventArgs e)
        {
            Voucher_Ma_nt0 = (CodeValueBindingObject)frmINCTPND_PX.FindResource("Voucher_Ma_nt0");
            Voucher_Ma_nt0.Text = StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString();
            Voucher_Ma_nt0.Value = (StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString().Equals(StartUp.M_ma_nt0));

            if (GrdINCTPND_PX.Records.Count > 0)
            {
                GrdINCTPND_PX.ActiveRecord = GrdINCTPND_PX.Records[0];
            }
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
            if (GrdINCTPND_PX.ActiveRecord != null)
            {
                drvFrmINCTPND_PX = (GrdINCTPND_PX.ActiveRecord as DataRecord).DataItem as DataRowView;
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
                GrdINCTPND_PX.FieldLayouts[0].Fields["gia_nt"].Visibility = Visibility.Hidden;
                GrdINCTPND_PX.FieldLayouts[0].Fields["gia_nt"].Settings.CellMaxWidth = 0;
            }
            else
            {
                GrdINCTPND_PX.FieldLayouts[0].Fields["gia_nt"].Visibility = Visibility.Visible;
                GrdINCTPND_PX.FieldLayouts[0].Fields["gia_nt"].Settings.CellMaxWidth = GrdINCTPND_PX.FieldLayouts[0].Fields["gia_nt"].Width.Value.Value;
            }

        }
    }
}
