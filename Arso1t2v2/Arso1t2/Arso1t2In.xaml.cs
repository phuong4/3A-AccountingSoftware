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

namespace Arso1t2_LSX
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
            GridSearch.ReportGroupName = StartUp.drCommandInfo["rep_file"].ToString();
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
            DataSet dsPrint = new DataSet("ds");
            //DataTable tbl = new DataTable();
            //string[] fields = "stt,so_lsx,ngay_kh1_ct,ngay_kh2_ct,ma_sp,sl_kh,sl_sx,sl_kt,sl_nhap,sl_hong,sl_ll,id,ma_hd,ten_vt,ten_vt2,dien_giai,tong_cong".Split(new char[] { ',' });
            //foreach (string s in fields)
            //    tbl.Columns.Add(s.Trim());

            //dsPrint.Tables.Add(tbl);
            //Info.Columns.Add(new DataColumn("stt", typeof(ushort)));
            //Chỗ này sẽ là vấn đề khi chỉnh sửa mẫu in, tức là khi hiện ra danh sách lệnh sản xuất bên ngoài mình query 1 lần, đến lúc in từng trang/in liên tục mình query thêm lần nữa, vấn đề là kết quả câu truy vấn trả về sẽ gồm các cột khác nhau, ví dụ như query bên ngoài kia mình ko trả về cột stt nhưng query lúc in mới trả về cột stt, nhưng khi chỉnh sửa mẫu in thì mẫu in chỉ nhận những cột trong kết quả mình query ban đầu thôi, mình muốn hiện những cột trong kết quả query lúc in thì phải thêm ở đây bằng cách Info.Columns.Add(...) thì lúc chỉnh sửa mẫu in sẽ có những cột đó để thêm vào mẫu in, mẫu in thêm xong rồi thì xóa mấy dòng Info.Columns.Add ở đây cũng được
            dsPrint.Tables.Add(Info.Copy());
            //ds.Tables[0].TableName = "tblInfo"; //Ở trong phần In từng trang để tên table là tblInfo thì ở đây cũng để tên giống vậy, tóm lại phải để tên table giống nhau, nếu ko khi sửa mẫu in sẽ bị hiển thị sai dữ liệu, ví dụ bảng có 2 bản ghi thì mẫu in hiển thị 4, có 3 thì hiển thị 9, có 4 hiển thị 16...
            //ds.Tables.Add(new Total().ToTable());
            GridSearch.DSource = dsPrint;
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
            Print();

        	this.Close();
		}
        public DataTable Info { get; set; }

        void Print()
        {
            StartUp.Kieu_in = Kieu_in;
            if (Kieu_in == "1")
                InTungTrang(true);
            else
                InLienTuc(true);
        }

        private void InTungTrang(bool flag)
        {
            //DataRow r = Info.Rows[0];
            //DateTime ngay_ct1 = (DateTime) r["ngay_ct1"];
            //DateTime ngay_ct2 = (DateTime) r["ngay_ct2"];
            //string tk = r["tk"].ToString().Trim();
            //string advance = r["advance"].ToString().Trim();
            //string ma_kh = "";
            //string sql = "";

            //DataRecord rec;
            //for(int i = 0; i < Records.Count; i++)
            //{
            //    rec = Records[i] as DataRecord;
            //    if (rec == null || rec.Cells["tag"].Value.ToString() == "False")
            //        continue;
            //    ma_kh = rec.Cells["ma_kh"].Value.ToString().Trim();
            //    CopyInfo(rec, r);
            //    sql = GetCommand(ngay_ct1, ngay_ct2, tk, ma_kh, advance);

            //    Debug.WriteLine(sql);
            //    SqlCommand cmd = new SqlCommand(sql);
            //    DataTable dt = StartUp.SysObj.ExcuteReader(cmd).Tables[0];
            //    DataSet ds = new DataSet("ds");

            //    //ta chieu de tranh bi double so lieu khi preview
            //    DataTable a = new DataTable();
            //    ds.Tables.Add(a.Copy());
            //    ds.Tables.Add(dt.Copy());
            //    ds.Tables.Add(Info.Copy());
            //    ds.Tables[1].TableName = "TableDetail";

            //    GridSearch.DSource = ds;
            //    if (!flag)
            //        GridSearch.V_Xem(i < Records.Count - 1);
            //    else
            //        GridSearch.V_In(1);

            //    rec.Cells["tag"].Value = false;
            //}
            //this.Close();
            DataSet dsPrint = new DataSet("ds");
            //dsPrint.Tables.Add(Info);
            //dsPrint.Tables[0].TableName = "tblInfo";
            DataTable lsxct = new DataTable();
            for (int i = 0; i < Info.Rows.Count; i++)
            {
                DataRow row = Info.Rows[i];
                bool tagValue = Convert.ToBoolean(row["tag"]);
                //decimal tong_cong = 0;
                if (tagValue) //Nếu bản ghi được đánh dấu -> tag = true
                {
                    SqlCommand cmdQuery = new SqlCommand();
                    cmdQuery.CommandType = CommandType.StoredProcedure;
                    cmdQuery.CommandText = "LSX";
                    cmdQuery.Parameters.Add("@Mapx", SqlDbType.VarChar).Value = "";
                    cmdQuery.Parameters.Add("@Malsx", SqlDbType.VarChar).Value = row["so_lsx"].ToString().Trim();
                    cmdQuery.Parameters.Add("@Ngay_ct1", SqlDbType.SmallDateTime).Value = new DateTime(1900, 1, 1);
                    cmdQuery.Parameters.Add("@Ngay_ct2", SqlDbType.SmallDateTime).Value = new DateTime(2079, 6, 6);
                    cmdQuery.Parameters.Add("@Tong_cong", SqlDbType.TinyInt).Value = 1;
                    lsxct = StartUp.SysObj.ExcuteReader(cmdQuery).Tables[0];
                    //lsxct.Columns.Add(new DataColumn("tong_cong", typeof(decimal)));
                    //foreach (DataRow row0 in lsxct.Rows)
                    //{
                    //    tong_cong += DecimalTryParse(row0["sl_kh"].ToString());
                    //}
                    //lsxct.Rows[0]["tong_cong"] = tong_cong;
                    dsPrint.Tables.Add(lsxct.Copy());
                    dsPrint.Tables[0].TableName = "tblInfo";
                    GridSearch.DSource = dsPrint;
                    if (!flag)
                      GridSearch.V_Xem(true);
                  else
                      GridSearch.V_In(1);
                    //tong_cong = 0;
                    dsPrint.Tables.Remove("tblInfo");
                }
                else continue;
            }
            this.Close();
        }

        private void InLienTuc(bool flag)
        {
            //DataRow r = Info.Rows[0];
            //DateTime ngay_ct1 = (DateTime)r["ngay_ct1"];
            //DateTime ngay_ct2 = (DateTime)r["ngay_ct2"];
            //string tk = r["tk"].ToString().Trim();
            //string advance = r["advance"].ToString().Trim();
            //string ma_kh = "";
            //string sql = "";

            //Total t = new Total();

            //DataRecord rec;
            //for (int i = 0; i < Records.Count; i++)
            //{
            //    rec = Records[i] as DataRecord;
            //    if (rec == null || rec.Cells["tag"].Value.ToString() == "False")
            //        continue;
            //    if (ma_kh != "")
            //        ma_kh += ",";
            //    ma_kh += rec.Cells["ma_kh"].Value.ToString().Trim();
            //    t.Sum(rec);
            //}

            //sql = GetCommand(ngay_ct1, ngay_ct2, tk, ma_kh, advance);

            DataSet dsPrint = new DataSet("ds");

            //Debug.WriteLine(sql);
            //SqlCommand cmd = new SqlCommand(sql);
            //DataSet ds = StartUp.SysObj.ExcuteReader(cmd);
            //if (ds == null && ds.Tables.Count == 0)
            //    return;

            //DataTable data = ds.Tables[0];
            //data.TableName = "TableDetail";
            //ds.Tables.Clear();
            //dsPrint.Tables.Add(data);
            dsPrint.Tables.Add(Info);
            for (int i = 0; i < dsPrint.Tables[0].Rows.Count; i++)
            {
                DataRow row = dsPrint.Tables[0].Rows[i];
                bool tagValue = Convert.ToBoolean(row["tag"]);

                if (!tagValue)
                {
                    // Xóa bản ghi khỏi DataSet
                    dsPrint.Tables[0].Rows.RemoveAt(i);
                    i--; // Giảm biến i để không bị bỏ qua bản ghi sau khi xóa
                }
            }
            dsPrint.Tables[0].TableName = "tblInfo";
            //for (int i = 0; i < Records.Count; i++)
            //{
            //    rec = Records[i] as DataRecord;
            //    if (rec == null || rec.Cells["tag"].Value.ToString() == "False")
            //        continue;
            //    SubTotal(rec, data);
            //    CopyInfo(rec, r);
            //    rec.Cells["tag"].Value = false;
            //}
            //data.DefaultView.Sort = "ma_kh,ngay_ct,stt_ct_nkc,so_ct";
            //dsPrint.Tables.Add(t.ToTable());
            GridSearch.DSource = dsPrint;
            if (!flag)
                GridSearch.V_Xem(false);
            else
                GridSearch.V_In(1);

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
            StartUp.Kieu_in = Kieu_in;
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
