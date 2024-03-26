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
using Infragistics.Windows.DataPresenter;
using Sm.Windows.Controls;
using SmVoucherLib;

namespace INCTPNG
{
    /// <summary>
    /// Interaction logic for FrmViewInctpng.xaml
    /// </summary>
    public partial class FrmViewInctpng :Sm.Windows.Controls.Form
    {
        public DataView vPh, vCt;
        public FrmViewInctpng(SysLib.SysObject SysObj, string tableNameSearch)
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(FrmViewInctpng_Loaded);
            KeyUp += new KeyEventHandler(FrmViewInctpng_KeyUp);
            GridSearch.SysObj = SysObj;
            GridSearch.tableName = tableNameSearch;
            GridSearch.RemoveBtnHuyAndKiemtra();
        }

       

        #region LoadData
        void LoadData()
        {
            this.BindingSysObj = StartUp.SysObj;
            vPh = new DataView(StartUp.DsTrans.Tables[0]);
            vPh.RowFilter = "stt_rec <> ''";
            vCt = new DataView(StartUp.DsTrans.Tables[1]);

            if (vPh.Count > 0)
            {
                vCt.RowFilter = "stt_rec='" + vPh[0]["stt_rec"] + "'";
                GrdPh.DataSource = vPh;
                GrdCt.DataSource = vCt;
                GrdPh.ActiveRecord = (GrdPh.Records[0] as DataRecord);
            }

        }
        #endregion

        #region FrmViewInctpng_Loaded
        void FrmViewInctpng_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
            txtSo_ct1.Focus();
        }
        #endregion 

        #region btnNhan_Click
        private void btnNhan_Click(object sender, RoutedEventArgs e)
        {
            if (CheckValid())
            {
                StartUp.TransFilterCmd.Parameters["@PhFilter"].Value = GetPhFilterExpr(); // "ngay_ct between '20100101' and '20100131'";

                DataSet newDs = DataProvider.FillCommand(StartUp.SysObj, StartUp.TransFilterCmd);
                //dịch focus về row 0 
                StartUp.DataFilter(StartUp.DsTrans.Tables[0].Rows[0]["stt_rec"].ToString());

                int Count1 = StartUp.DsTrans.Tables[0].Rows.Count;
                int Count2 = StartUp.DsTrans.Tables[1].Rows.Count;
                int Count3 = StartUp.DsTrans.Tables[2].Rows.Count;
                //xóa Table[0] từ row 1--> hết
                for (int i = Count1 - 1; i >= 1; i--)
                    StartUp.DsTrans.Tables[0].Rows.RemoveAt(i);
                //xóa Table[1] từ row 0--> hết
                for (int i = 0; i < Count2; i++)
                    StartUp.DsTrans.Tables[1].Rows.RemoveAt(0);
                //xóa Table[2] từ row 0--> hết
                for (int i = 0; i < Count3; i++)
                    StartUp.DsTrans.Tables[2].Rows.RemoveAt(0);

                //Add lại các row vào Table[0]
                int Count = 0;
                Count = newDs.Tables[0].Rows.Count;
                for (int i = 0; i < Count; i++)
                {
                    StartUp.DsTrans.Tables[0].Rows.Add(newDs.Tables[0].Rows[i].ItemArray);
                }
                //Add lại các row vào Table[1]
                Count = newDs.Tables[1].Rows.Count;
                for (int i = 0; i < Count; i++)
                {
                    StartUp.DsTrans.Tables[1].Rows.Add(newDs.Tables[1].Rows[i].ItemArray);
                }
                //Add lại các row vào Table[2]
                Count = newDs.Tables[2].Rows.Count;
                for (int i = 0; i < Count; i++)
                {
                    StartUp.DsTrans.Tables[2].Rows.Add(newDs.Tables[2].Rows[i].ItemArray);
                }

                // ko xoá dòng thứ 0 của table[0] vì dòng đó là dòng tạm.

                //filter lại các Table[0], Table[1], Table[2]
                if (newDs.Tables[0].Rows.Count > 0)
                {
                    if (FrmInctpng.iRow >= newDs.Tables[0].Rows.Count)
                        FrmInctpng.iRow = newDs.Tables[0].Rows.Count - 1;
                    StartUp.DataFilter(StartUp.DsTrans.Tables[0].Rows[FrmInctpng.iRow]["stt_rec"].ToString());
                }
                LoadData();
            }
        }
        #endregion

        #region CheckValid
        bool CheckValid()
        {
            bool result = true;
            if ((DateTime)txtNgay_ct1.Value > (DateTime)txtNgay_ct2.Value)
            {
                result = false;
                ExMessageBox.Show( 835,StartUp.M_LAN, "Thưa ngài, chứng từ từ ngày phải nhỏ hơn đến ngày", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Question);
                txtNgay_ct1.SelectAll();
                txtNgay_ct1.Focus();
            }
            return result;
        }
        #endregion

        #region btnHuy_Click
        private void btnHuy_Click(object sender, RoutedEventArgs e)
        {
           // this.Hide();
            this.Close();
        }
        #endregion

        #region FrmViewInctpng_KeyUp
        void FrmViewInctpng_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }
        #endregion

        #region FormView_Closed
        private void FormView_Closed(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region GrdPh_SelectedItemsChanged
        private void GrdPh_SelectedItemsChanged(object sender, Infragistics.Windows.DataPresenter.Events.SelectedItemsChangedEventArgs e)
        {
            vCt.RowFilter = "stt_rec='" + vPh[(GrdPh.ActiveRecord as DataRecord).Index]["stt_rec"] + "'";
        }
        #endregion

            #region GetPhFilterExpr
            private string GetPhFilterExpr()
            {
                string sPhFilter = "1=1 ";
                if (!string.IsNullOrEmpty(txtNgay_ct1.Text))
                {
                    sPhFilter += " and ngay_ct >= " + ConvertDataToSql(txtNgay_ct1.Value, typeof(DateTime));
                }
                if (!string.IsNullOrEmpty(txtNgay_ct2.Text))
                {
                    sPhFilter += " and ngay_ct <= " + ConvertDataToSql(txtNgay_ct2.Value, typeof(DateTime));
                }
                if (!string.IsNullOrEmpty(txtSo_ct1.Text))
                {
                    sPhFilter += " and so_ct like " + ConvertDataToSql("%" + txtSo_ct1.Text.Trim() + "%", typeof(string));
                }
                if (!string.IsNullOrEmpty(txtMa_kh.Text))
                {
                    sPhFilter += " and ma_kh like " + ConvertDataToSql(txtMa_kh.Text.Trim() + "%", typeof(string));
                }
                if (!string.IsNullOrEmpty(txtTk_co.Text))
                {
                    sPhFilter += " and tk_co like " + ConvertDataToSql(txtTk_co.Text.Trim() + "%", typeof(string));
                }
                return sPhFilter;
            }
            #endregion

        #region ConvertDataToSql
        public string ConvertDataToSql(object value, Type ValueType)
        {
            string sResult = "";
            switch (ValueType.ToString())
            {
                case "System.String":
                    sResult = string.Format("'{0}'", (value as string).Replace("'", "''"));
                    break;
                case "System.DateTime":
                    sResult = string.Format("'{0}'", ((DateTime)value).ToString("yyyyMMdd"));
                    break;
                default:
                    sResult = string.Format("'{0}'", value);
                    break;
            }

            return sResult;
        }
        #endregion
    }
}
