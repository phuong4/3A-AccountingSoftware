using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data;
using System.Diagnostics;
using Sm.Windows.Controls;
using Infragistics.Windows.DataPresenter;
using System.Globalization;
using System.Data.SqlClient;
using System.Windows.Interop;

namespace HTQ_CNHTHANH
{
    /// <summary>
    /// Interaction logic for FrmPrintGlctpk1.xaml
    /// </summary>
    public partial class Arso1t2In : Sm.Windows.Controls.Form
    {
        public Arso1t2In()
        {
            InitializeComponent();
            SmLib.SysFunc.LoadIcon(this);
            GridSearch.LocalSysObj = StartUp.SysObj;
            GridSearch.ReportGroupName = StartUp.CommandInfo["rep_file"].ToString();
            GridSearch.DSource = StartUp.DataSourceReport;

            //Kieu_in = "2";
            Kieu_in = StartUp.Kieu_in;
            if (BindingSysObj.GetOption("M_LAN").ToString().Equals("V"))
            {
                BtnExport.Content = BindingSysObj.GetSysVar("M_EXPORT_SIGN").ToString();
            }
            else
            {
                BtnExport.Content = BindingSysObj.GetSysVar2("M_EXPORT_SIGN").ToString();
            }
        }

        #region Form_Loaded
        private void Form_Loaded(object sender, RoutedEventArgs e)
        {
            //DataSet ds = new DataSet("ds");
            //DataTable tbl = new DataTable("TableDetail");
            //string[] fields = "stt,so_khsx,so_dh,ten_kh_i,ten_vt,loai_giay,k_thuoc1,k_thuoc2,tong_giay,tong_giay_can,sl_g_can,so_kem,qc_in".Split(new char[] { ',' });
            //foreach (string s in fields)
            //    tbl.Columns.Add(s.Trim());
            //tbl.Rows.Add(new object[] { });
            //ds.Tables.Add(tbl);
            ////ds.Tables.Add(Info.Copy());
            ////ds.Tables.Add(new Total().ToTable());
            //GridSearch.DSource = ds;
            ////txtKieuIn.Value = Kieu_In;
            ////Kieu_in = "2";
        }
        #endregion

        #region Form_KeyDown
        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.Close();
            }
        }
        #endregion

        #region btnthoat_Click
        private void btnthoat_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        #endregion

        private void btnin_Click(object sender, RoutedEventArgs e)
        {
            //Print();
            //Kieu_in = "2";
            if (Kieu_in == "1")
                InTungTrang(true);
            else
                InLienTuc(true);

            this.Close();
        }
        public DataTable Info { get; set; }

        void Print()
        {
            //Kieu_in = "2";
            if (Kieu_in == "1")
                InTungTrang(true);
            else
                InLienTuc(true);
        }

        private void InTungTrang(bool flag)
        {
            DataSet DataSourceReport = StartUp.DataSourceReport;
            SmLib.SysFunc.DSCopyWithFilter(StartUp.oBrowse.frmBrw.oBrowse, ref DataSourceReport, "tbDetail");

            DataSourceReport.AcceptChanges();
            DataSourceReport.Tables["tbDetail"].DefaultView.RowFilter = "tag=True";
            var tnFilted = new DataView(DataSourceReport.Tables["tbDetail"], "tag=True", "", DataViewRowState.CurrentRows).ToTable();
            if (tnFilted.Rows.Count > 0)
            {
              
                foreach (DataRow r in tnFilted.Rows)
                {
                    DataSourceReport.Tables["tbDetail"].Clear();
                    DataSourceReport.Tables["tbDetail"].ImportRow(r);
                    GridSearch.DSource = DataSourceReport;
                    if (!flag)
                        GridSearch.V_Xem(false);
                    else
                        GridSearch.V_In(1);
                }
            }


            SmLib.SysFunc.ResetFilter(ref DataSourceReport, "tbDetail");



            this.Close();
        }

        private void InLienTuc(bool flag)
        {
            /*   */


            DataSet DataSourceReport = StartUp.DataSourceReport;
            SmLib.SysFunc.DSCopyWithFilter(StartUp.oBrowse.frmBrw.oBrowse, ref DataSourceReport, "tbDetail");

            DataSourceReport.AcceptChanges();
            DataSourceReport.Tables["tbDetail"].DefaultView.RowFilter = "tag=True";
            var tnFilted = new DataView(DataSourceReport.Tables["tbDetail"], "tag=True", "", DataViewRowState.CurrentRows).ToTable();
            if (tnFilted.Rows.Count > 0)
            {
                DataSourceReport.Tables["tbDetail"].Clear();
                DataSourceReport.Tables["tbDetail"].Merge(tnFilted);
                GridSearch.DSource = DataSourceReport;
                if (!flag)
                    GridSearch.V_Xem(false);
                else
                    GridSearch.V_In(1);
            }


            SmLib.SysFunc.ResetFilter(ref DataSourceReport, "tbDetail");



            this.Close();
        }
        void GridSearch_ReportPreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            btXem_Click(null, null);
        }

        string GetCommand(DateTime ngay_ct1, DateTime ngay_ct2, string tk, string ma_kh, string advance)
        {
            //ko dung like vi se len ca ma_kh long nhau
            ma_kh = ma_kh.Replace(" ", "");
            ma_kh = ma_kh.Replace(",", "','");
            ma_kh = "('" + ma_kh + "')";

            string sql = "select ngay_ct, ma_ct0, stt_ct_nkc, ltrim(rtrim(so_ct)) as so_ct, RTRIM(ma_ct0) + so_ct as ct," +
                "RTRIM(dien_giai) + ' ' + CASE WHEN ma_vv IS NULL OR RTRIM(ma_vv) = '' THEN '' ELSE '(' + RTRIM(ma_vv) + ')' END as dien_giai," +
                "RTRIM(dien_giai) + ' ' + CASE WHEN ma_vv IS NULL OR RTRIM(ma_vv) = '' THEN '' ELSE '(' + RTRIM(ma_vv) + ')' END as dien_giai2," +
                "ma_kh,ten_kh,ten_kh2," +
                "tk_du, ps_no, ps_no_nt, CASE WHEN ma_nt LIKE '" + StartUp.M_MA_NT0 + "' THEN NULL ELSE ma_nt END AS ma_nt,CASE WHEN ma_nt LIKE '" + StartUp.M_MA_NT0 + "' THEN NULL ELSE ty_gia END ty_gia, ps_co, ps_co_nt" +
            " FROM v_arso1t2" +
            " WHERE ngay_ct BETWEEN '" + ngay_ct1.ToString("MMM dd yyyy", new CultureInfo("en-US")) +
                "' AND '" + ngay_ct2.ToString("MMM dd yyyy", new CultureInfo("en-US")) + "'" +
                " AND tk LIKE '" + tk + "%'" +
                " AND " + advance + " AND rtrim(ma_kh) in " + ma_kh +
            " ORDER BY ngay_ct, stt_ct_nkc, so_ct";

            //string sql = "select ngay_ct, ma_ct0, stt_ct_nkc, so_ct, RTRIM(ma_ct0) + so_ct as ct," +
            //    "RTRIM(dien_giai) + ' ' + CASE WHEN ma_vv IS NULL OR RTRIM(ma_vv) = '' THEN '' ELSE '(' + RTRIM(ma_vv) + ')' END as dien_giai," +
            //    "ma_kh,ten_kh,ten_kh2," +
            //    "tk_du, ps_no, ps_no_nt, ty_gia, ps_co, ps_co_nt" +
            //" FROM v_arso1t2" +
            //" WHERE ngay_ct BETWEEN '" + ngay_ct1.ToString("MMM dd yyyy", new CultureInfo("en-US")) +
            //    "' AND '" + ngay_ct2.ToString("MMM dd yyyy", new CultureInfo("en-US")) + "'" +
            //    " AND tk LIKE '" + tk + "%' AND dbo.InList(RTRIM(ma_kh), '" + ma_kh + "', ',') = 1" +
            //    " AND " + advance + 
            //" ORDER BY ngay_ct, stt_ct_nkc, so_ct";




            return sql;
        }

        void CopyInfo(DataRecord rec, DataRow r)
        {

            r["ma_kh"] = rec.Cells["ma_kh"].Value.ToString().Trim();
            r["ten_kh"] = rec.Cells["ten_kh"].Value.ToString().Trim();
            r["ten_kh2"] = rec.Cells["ten_kh2"].Value.ToString().Trim();

            string du = rec.Cells["du_dau"].Value.ToString().Trim();
            string ten = "SỐ DƯ ĐẦU KỲ:";
            string ten2 = "OPENING BALANCE:";
            if (du == "N")
            {
                ten = "SỐ DƯ NỢ ĐẦU KỲ:";
                ten2 = "OPENING DEBIT BALANCE:";
            }
            else
                if (du == "C")
            {
                ten = "SỐ DƯ CÓ ĐẦU KỲ:";
                ten2 = "OPENING CREDIT BALANCE:";
            }
            r["ten_dk"] = ten;
            r["ten_dk2"] = ten2;
            r["dk"] = rec.Cells["dk"].Value;
            r["dk_nt"] = rec.Cells["dk_nt"].Value;
            r["no_co_dk"] = rec.Cells["du_dau"].Value;


            du = rec.Cells["du_cuoi"].Value.ToString().Trim();
            ten = "SỐ DƯ CUỐI KỲ:";
            ten2 = "CLOSING BALANCE:";
            if (du == "N")
            {
                ten = "SỐ DƯ NỢ CUỐI KỲ:";
                ten2 = "CLOSING DEBIT BALANCE:";
            }
            else
                if (du == "C")
            {
                ten = "SỐ DƯ CÓ CUỐI KỲ:";
                ten2 = "CLOSING CREDIT BALANCE:";
            }
            r["ten_ck"] = ten;
            r["ten_ck2"] = ten2;
            r["ck"] = rec.Cells["ck"].Value;
            r["ck_nt"] = rec.Cells["ck_nt"].Value;
            r["no_co_ck"] = rec.Cells["du_cuoi"].Value;
        }

        void SubTotal(DataRecord rec, DataTable data)
        {
            string ma_kh, ten_kh, ten_kh2;

            DataRow r = data.NewRow();
            r["ma_kh"] = ma_kh = rec.Cells["ma_kh"].Value.ToString();
            r["ten_kh"] = ten_kh = rec.Cells["ten_kh"].Value.ToString().Trim();
            r["ten_kh2"] = ten_kh2 = rec.Cells["ten_kh2"].Value.ToString().Trim();

            r["dien_giai"] = "Dư đầu:";
            r["dien_giai2"] = "Opening balance:";
            r["ps_no"] = rec.Cells["no_dk"].Value;
            r["ps_co"] = rec.Cells["co_dk"].Value;
            r["ps_no_nt"] = rec.Cells["no_dk_nt"].Value;
            r["ps_co_nt"] = rec.Cells["co_dk_nt"].Value;
            data.Rows.Add(r);

            r = data.NewRow();
            r["ma_kh"] = ma_kh;
            r["ten_kh"] = ten_kh;
            r["ten_kh2"] = ten_kh2;
            r["dien_giai"] = "Tổng phát sinh:";
            r["dien_giai2"] = "Total arising:";
            r["ps_no"] = rec.Cells["ps_no"].Value;
            r["ps_co"] = rec.Cells["ps_co"].Value;
            r["ps_no_nt"] = rec.Cells["ps_no_nt"].Value;
            r["ps_co_nt"] = rec.Cells["ps_co_nt"].Value;
            data.Rows.Add(r);

            r = data.NewRow();
            r["ma_kh"] = ma_kh;
            r["ten_kh"] = ten_kh;
            r["dien_giai"] = "Dư cuối:";
            r["dien_giai2"] = "Closing balance:";
            r["ps_no"] = rec.Cells["no_ck"].Value;
            r["ps_co"] = rec.Cells["co_ck"].Value;
            r["ps_no_nt"] = rec.Cells["no_ck_nt"].Value;
            r["ps_co_nt"] = rec.Cells["co_ck_nt"].Value;
            data.Rows.Add(r);
        }

        public RecordCollectionBase Records { get; set; }

        public string Kieu_in
        {
            get { return (string)GetValue(Kieu_inProperty); }
            set { SetValue(Kieu_inProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Kieu_in.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty Kieu_inProperty =
            DependencyProperty.Register("Kieu_in", typeof(string), typeof(Arso1t2In), new UIPropertyMetadata("2"));



        private void txtKieuIn_LostFocus(object sender, RoutedEventArgs e)
        {
            if (txtKieuIn.Text.Trim().Length == 0)
                txtKieuIn.Value = "1";

        }

        private void btXem_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(Kieu_in);
            // Kieu_in = "2";
            if (Kieu_in == "1")
                InTungTrang(false);
            else
                InLienTuc(false);
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            IntPtr windowHandle = new WindowInteropHelper(this).Handle;
            GridSearch.V_XuatPdf(string.Empty, windowHandle);
        }
    }

    class Total
    {
        decimal no_dk;
        decimal no_dk_nt;
        decimal co_dk;
        decimal co_dk_nt;

        decimal ps_no;
        decimal ps_no_nt;
        decimal ps_co;
        decimal ps_co_nt;

        decimal no_ck;
        decimal no_ck_nt;
        decimal co_ck;
        decimal co_ck_nt;

        public Total()
        {
            Empty();
        }

        public void Empty()
        {
            no_dk = no_dk_nt = co_dk = co_dk_nt = 0;
            ps_no = ps_no_nt = ps_co = ps_co_nt = 0;
            no_ck = no_ck_nt = co_ck = co_ck_nt = 0;
        }

        public void Sum(DataRecord rec)
        {
            no_dk += ToDec(rec.Cells["no_dk"].Value);
            no_dk_nt += ToDec(rec.Cells["no_dk_nt"].Value);
            co_dk += ToDec(rec.Cells["co_dk"].Value);
            co_dk_nt += ToDec(rec.Cells["co_dk_nt"].Value);

            ps_no += ToDec(rec.Cells["ps_no"].Value);
            ps_no_nt += ToDec(rec.Cells["ps_no_nt"].Value);
            ps_co += ToDec(rec.Cells["ps_co"].Value);
            ps_co_nt += ToDec(rec.Cells["ps_co_nt"].Value);

            no_ck += ToDec(rec.Cells["no_ck"].Value);
            no_ck_nt += ToDec(rec.Cells["no_ck_nt"].Value);
            co_ck += ToDec(rec.Cells["co_ck"].Value);
            co_ck_nt += ToDec(rec.Cells["co_ck_nt"].Value);
        }

        public DataTable ToTable()
        {
            DataTable tbl = new DataTable("tblTotal");
            tbl.Columns.Add("no_dk", typeof(decimal));
            tbl.Columns.Add("no_dk_nt", typeof(decimal));
            tbl.Columns.Add("co_dk", typeof(decimal));
            tbl.Columns.Add("co_dk_nt", typeof(decimal));

            tbl.Columns.Add("ps_no", typeof(decimal));
            tbl.Columns.Add("ps_no_nt", typeof(decimal));
            tbl.Columns.Add("ps_co", typeof(decimal));
            tbl.Columns.Add("ps_co_nt", typeof(decimal));

            tbl.Columns.Add("no_ck", typeof(decimal));
            tbl.Columns.Add("no_ck_nt", typeof(decimal));
            tbl.Columns.Add("co_ck", typeof(decimal));
            tbl.Columns.Add("co_ck_nt", typeof(decimal));

            tbl.Rows.Add(new object[] { no_dk, no_dk_nt, co_dk, co_dk_nt, ps_no, ps_no_nt, ps_co, ps_co_nt, no_ck, no_ck_nt, co_ck, co_ck_nt });
            return tbl;
        }

        private decimal ToDec(object value)
        {
            if (value == null || value is DBNull)
                return 0;

            return (decimal)value;
        }
    }
}
