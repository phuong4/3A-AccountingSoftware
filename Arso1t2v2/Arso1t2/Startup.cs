//Dev by Phương and ChatGPT with love <3
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SmLib.SM.FormBrowse;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;
using Sm.Languages;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.DataPresenter.Events;
using Sm.Windows.Controls;
using System.Threading;
using System.Globalization;
using System.Diagnostics;
using Infragistics.Windows;
using Infragistics.Windows.Themes;
using System.Windows.Markup;
using System.IO;
using System.Xml;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Arso1t2_LSX
{
    public class StartUp : StartupBase
    {
        public override void Run() { Namespace = "Arso1t2"; (new StartUp()).Show(Menu_Id); }

        private DataSet dsArso1t2;
        public static DataRow drCommandInfo;
        //private DataTable dtInfo;
        private FormBrowse oBrowse;
        private string[] sFieldArrays;
        private SqlCommand cmdQuery;

        //0 tất cả, 1 VND, 2 Ngoại tệ
        static private int KindStyleReport = -1;

        //private string sIp_gia, sIp_gia_nt, sIp_tien, sIp_tien_nt, sIp_sl, sNum_separator;

        public static string Ma_nt0 = "VND";
        //public static string Language = Languages.VIETNAMESE;
        public static string ReportID = "";
        //public static string ReportGroup = "";
        //static string M_IP_TIEN, M_IP_TIEN_NT;

        //string FieldDetails = "";
        //string Tk = "", AdvanceFilter = "";
        DateTime ngay_ct1, ngay_ct2;

        public static string Kieu_in = "2";


        void Show(string id)
        {
           
            drCommandInfo = SmLib.SysFunc.GetCommandInfo(SysObj, id);
            ReportID = drCommandInfo["rep_file"].ToString();

            //M_IP_TIEN = SysObj.GetOption("M_IP_TIEN").ToString();
            //M_IP_TIEN_NT = SysObj.GetOption("M_IP_TIEN_NT").ToString();
            Ma_nt0 = (string)Arso1t2_LSX.StartUp.SysObj.GetOption("M_MA_NT0");
            //Language = (string)Arso1t2.StartUp.SysObj.GetOption("M_LAN").ToString();

            if (drCommandInfo == null || drCommandInfo.ItemArray.Length == 0)
            {
                if (!Process.GetCurrentProcess().ProcessName.Equals("SmProcess"))
                    App.Current.Shutdown();
                return;
            }

            //dtInfo = new DataTable("klq");
            //dtInfo.Columns.Add(new DataColumn("so_lsx", typeof(string)));
            //dtInfo.Columns.Add(new DataColumn("ngay_kh1_ct", typeof(DateTime)));
            //dtInfo.Columns.Add(new DataColumn("ngay_kh2_ct", typeof(DateTime)));
            //dtInfo.Columns.Add(new DataColumn("ma_sp", typeof(string)));
            //dtInfo.Columns.Add(new DataColumn("sl_kh", typeof(decimal)));
            //dtInfo.Columns.Add(new DataColumn("sl_sx", typeof(decimal)));
            //dtInfo.Columns.Add(new DataColumn("sl_kt", typeof(decimal)));
            //dtInfo.Columns.Add(new DataColumn("sl_nhap", typeof(decimal)));
            //dtInfo.Columns.Add(new DataColumn("sl_hong", typeof(decimal)));
            //dtInfo.Columns.Add(new DataColumn("sl_ll", typeof(decimal)));
            //dtInfo.Columns.Add(new DataColumn("tk_dd", typeof(string)));
            //dtInfo.Columns.Add(new DataColumn("ngay_lkh", typeof(DateTime)));
            //dtInfo.Columns.Add(new DataColumn("ma_px", typeof(string)));
            //dtInfo.Columns.Add(new DataColumn("dien_giai", typeof(string)));
            //dtInfo.Columns.Add(new DataColumn("ngay_kh1_ph", typeof(DateTime)));
            //dtInfo.Columns.Add(new DataColumn("ngay_kh2_ph", typeof(DateTime)));
            //dtInfo.Columns.Add(new DataColumn("ngay_tt1", typeof(DateTime)));
            //dtInfo.Columns.Add(new DataColumn("ngay_tt2", typeof(DateTime)));
            //dtInfo.Columns.Add(new DataColumn("ma_bpht", typeof(string)));
            //dtInfo.Columns.Add(new DataColumn("ma_dvcs", typeof(string)));
            //dtInfo.Columns.Add(new DataColumn("ma_tra_cuu", typeof(string)));
            //dtInfo.Columns.Add(new DataColumn("ten_tk", typeof(string)));
            //dtInfo.Columns.Add(new DataColumn("ten_tk2", typeof(string)));
            //dtInfo.Columns.Add(new DataColumn("id", typeof(string)));
            //dtInfo.Columns.Add(new DataColumn("ma_hd", typeof(string)));
            //dtInfo.Columns.Add(new DataColumn("ten_vt", typeof(string)));
            //dtInfo.Columns.Add(new DataColumn("ten_vt2", typeof(string)));
             

            //try
            //{
            //    sNum_separator = ((string)SysObj.GetOption("M_NUM_SEPARATOR"));
            //    sIp_gia = (string)SysObj.GetOption("M_IP_GIA");
            //    sIp_gia_nt = (string)SysObj.GetOption("M_IP_GIA_NT");
            //    sIp_tien = (string)SysObj.GetOption("M_IP_TIEN");
            //    sIp_tien_nt = (string)SysObj.GetOption("M_IP_TIEN_NT");
            //    sIp_sl = (string)SysObj.GetOption("M_IP_SL");
            //}
            //catch (Exception ex1) { }

            Arso1t2Loc loc = new Arso1t2Loc();
            //loc.DisplayLanguage = Language;
            loc.BindingSysObj = SysObj;
            loc.Title = SmLib.SysFunc.Cat_Dau(M_LAN.Equals("V") ? drCommandInfo["bar"].ToString() : drCommandInfo["bar2"].ToString());
            SmLib.SysFunc.LoadIcon(loc);
            if (!loc.ShowDialog())
            {
                if (!Process.GetCurrentProcess().ProcessName.Equals("SmProcess"))
                    App.Current.Shutdown();

                return;
            }

            //string _advance = "1=1 and ma_kh like '" + loc.txtMaKhach.Text.Trim().Replace("'", "''") + "%' ";

            //if (!(loc.AdvanceFilter == null || loc.AdvanceFilter.Length == 0))
            //    _advance = loc.AdvanceFilter;

            //Debug.WriteLine(_advance);
            ngay_ct1 = (DateTime)loc.M_NGAY_CT1;
            ngay_ct2 = (DateTime)loc.M_NGAY_CT2;
            //string ma_dvcs = (string)loc.M_MA_DVCS;
            //Tk = loc.M_TK.ToString();

            //if (!string.IsNullOrEmpty(ma_dvcs))
            //    _advance = _advance + " and ma_dvcs like '" + ma_dvcs.Trim().Replace("'", "''") + "%'";

            //AdvanceFilter = _advance;

            cmdQuery = new SqlCommand(drCommandInfo["store_proc"].ToString());
            cmdQuery.CommandType = CommandType.StoredProcedure;
            cmdQuery.Parameters.Add("@Malsx", SqlDbType.VarChar).Value = loc.M_TK.ToString().Trim();
            cmdQuery.Parameters.Add("@Ngay_ct1", SqlDbType.SmallDateTime).Value = ngay_ct1;//dtFilter.Rows[0]["ngay_ct1"];
            cmdQuery.Parameters.Add("@Ngay_ct2", SqlDbType.SmallDateTime).Value = ngay_ct2;//dtFilter.Rows[0]["ngay_ct2"];
            cmdQuery.Parameters.Add("@Mapx", SqlDbType.VarChar).Value = loc.M_MA_KH.ToString().Trim();
            cmdQuery.Parameters.Add("@Tong_cong", SqlDbType.TinyInt).Value = 0;
            dsArso1t2 = SysObj.ExcuteReader(cmdQuery);
            DataTable tbSum = dsArso1t2.Tables[0];
            //List<string> distinctSoLsx = new List<string>();

            //// Duyệt qua từng bản ghi trong bảng tbSum
            //for (int i = tbSum.Rows.Count - 1; i >= 0; i--)
            //{
            //    DataRow row = tbSum.Rows[i];
            //    string soLsx = row["so_lsx"].ToString();

            //    // Kiểm tra xem giá trị so_lsx đã tồn tại trong danh sách chưa
            //    if (!distinctSoLsx.Contains(soLsx))
            //    {
            //        // Nếu chưa tồn tại, thêm giá trị so_lsx vào danh sách và giữ lại bản ghi
            //        distinctSoLsx.Add(soLsx);
            //    }
            //    else
            //    {
            //        // Nếu đã tồn tại, loại bỏ bản ghi đó khỏi bảng tbSum
            //        tbSum.Rows.RemoveAt(i);
            //    }
            //}

            //tbSum.Columns.Add(new DataColumn("ngay_ct1", typeof(DateTime)));
            //tbSum.Columns.Add(new DataColumn("ngay_ct2", typeof(DateTime)));
            //// Lặp qua từng bản ghi trong bảng tbSum, phải thêm giá trị ngay_ct1 và ngay_ct2 vào trong bảng này để sau đó chạy script trong mẫu in, nếu ko sẽ báo lỗi hoặc phải sửa lại toàn bộ mẫu in, phải thêm vào tất cả các bản ghi chứ ko phải chỉ bản ghi đầu tiên vì ko thể biết sau đó sẽ in bản ghi nào
            //foreach (DataRow row in tbSum.Rows)
            //{
            //    // Gán giá trị ngay_cta vào cột ngay_ct1 của bản ghi hiện tại
            //    row["ngay_ct1"] = (DateTime)loc.M_NGAY_CT1;

            //    // Gán giá trị ngay_ctb vào cột ngay_ct2 của bản ghi hiện tại
            //    row["ngay_ct2"] = (DateTime)loc.M_NGAY_CT2;
            //}

            //if (dsArso1t2 == null || dsArso1t2.Tables.Count < 2)
            //{
            //    if (!Process.GetCurrentProcess().ProcessName.Equals("SmProcess"))
            //        App.Current.Shutdown();
            //    return;
            //}

            //string tk = "", ten_tk = "", ten_tk2 = "";
            //if (dsArso1t2.Tables[1].Rows.Count > 0)
            //{
            //    DataRow rTk = dsArso1t2.Tables[1].Rows[0];
            //    tk = rTk[0].ToString();
            //    ten_tk = rTk[1].ToString();
            //    ten_tk2 = rTk[2].ToString();
            //}
            //dtInfo.Rows.Add(new object[] { ngay_ct1, ngay_ct2, tk, ten_tk, ten_tk2, "", "", "", _advance});


            KindStyleReport = Convert.ToInt16(loc.txtMau_bc.Value);
            if (M_LAN.Equals("V"))
                if (KindStyleReport == 1)
                    sFieldArrays = drCommandInfo["Vbrowse1"].ToString().Split("|".ToCharArray());
                else
                    sFieldArrays = drCommandInfo["Vbrowse2"].ToString().Split("|".ToCharArray());
            else
                if (KindStyleReport == 1)
                    sFieldArrays = drCommandInfo["Ebrowse1"].ToString().Split("|".ToCharArray());
                else
                    sFieldArrays = drCommandInfo["Ebrowse2"].ToString().Split("|".ToCharArray());

            string fields = sFieldArrays[0];
            //FieldDetails = sFieldArrays[1];
      
            //Verify(dsArso1t2.Tables[0], fields);
            //for (int j = 0; j < dsArso1t2.Tables[0].Columns.Count; j++)
            //    Debug.Write(dsArso1t2.Tables[0].Columns[j].ColumnName + ",");

            oBrowse = new FormBrowse(SysObj, dsArso1t2.Tables[0].DefaultView, fields);
            oBrowse.F5 += new FormBrowse.GridKeyUp_F5(_browser_F5); //chi tiết
            oBrowse.F7 += new FormBrowse.GridKeyUp_F7(_browser_F7);
            oBrowse.CTRL_R += new FormBrowse.GridKeyUp_CTRL_R(oBrowse_CTRL_R); //làm mới
            oBrowse.frmBrw.PreviewKeyUp += new KeyEventHandler(frmBrw_PreviewKeyUp);
            oBrowse.DataGrid.FieldLayoutInitialized += new EventHandler<FieldLayoutInitializedEventArgs>(DataGrid_FieldLayoutInitialized);

            SmLib.SysFunc.LoadIcon(oBrowse.frmBrw);
            var et = (drCommandInfo["ma_phan_he"].ToString().Trim().Equals("SO") ? "Customer" : "Supplier")
                + " statement: Account ";
            string title = M_LAN.Equals("V") ? "Báo cáo chi tiết lệnh sản xuất" : et;
            //oBrowse.frmBrw.DisplayLanguage = Language;
            oBrowse.frmBrw.Title = title ;
            //if (KindStyleReport == 1)
            //{
            //    oBrowse.AddValueSummary(new string[] { M_LAN.Equals("V") ? "ten_kh" : "ten_kh2", "ps_no", "ps_co", "dk","ck", "du_dau", "du_cuoi"},
            //        new string[] { M_LAN.Equals("V") ? "Tổng cộng:" : "Total:",
            //                                        DecimalTryParse(tbSum.Rows[0]["t_ps_no"].ToString()).ToString(M_IP_TIEN),
            //                                        DecimalTryParse(tbSum.Rows[0]["t_ps_co"].ToString()).ToString(M_IP_TIEN),
            //                                        DecimalTryParse(tbSum.Rows[0]["t_dk"].ToString()).ToString(M_IP_TIEN),
                                                    
            //                                        DecimalTryParse(tbSum.Rows[0]["t_ck"].ToString()).ToString(M_IP_TIEN),
            //                                        tbSum.Rows[0]["du_dau_no_co"].ToString(),
            //                                        tbSum.Rows[0]["du_cuoi_no_co"].ToString()
            //                            });
            //}
            //else
            //{
            //    oBrowse.AddValueSummary(new string[] { M_LAN.Equals("V") ? "ten_kh" : "ten_kh2", "ps_no", "ps_co", "dk", "ck", "ps_no_nt", "ps_co_nt", "dk_nt", "ck_nt", "du_dau_nt", "du_cuoi_nt"},
            //        new string[] {M_LAN.Equals("V") ? "Tổng cộng:" : "Total:",
            //                                            DecimalTryParse(tbSum.Rows[0]["t_ps_no"].ToString()).ToString(M_IP_TIEN),
            //                                            DecimalTryParse(tbSum.Rows[0]["t_ps_co"].ToString()).ToString(M_IP_TIEN),
            //                                            DecimalTryParse(tbSum.Rows[0]["t_dk"].ToString()).ToString(M_IP_TIEN),
            //                                            DecimalTryParse(tbSum.Rows[0]["t_ck"].ToString()).ToString(M_IP_TIEN),

            //                                            DecimalTryParse(tbSum.Rows[0]["t_ps_no_nt"].ToString()).ToString(M_IP_TIEN_NT),
            //                                            DecimalTryParse(tbSum.Rows[0]["t_ps_co_nt"].ToString()).ToString(M_IP_TIEN_NT),
            //                                            DecimalTryParse(tbSum.Rows[0]["t_dk_nt"].ToString()).ToString(M_IP_TIEN_NT),
            //                                            DecimalTryParse(tbSum.Rows[0]["t_ck_nt"].ToString()).ToString(M_IP_TIEN_NT),
            //                                            tbSum.Rows[0]["du_dau_no_co"].ToString(),
            //                                            tbSum.Rows[0]["du_cuoi_no_co"].ToString(),
            //                                            tbSum.Rows[0]["du_dau_no_co_nt"].ToString(),
            //                                            tbSum.Rows[0]["du_cuoi_no_co_nt"].ToString()
            //                                });
            //}
            


            oBrowse.frmBrw.LanguageID  = "Arso1t2_2";
            oBrowse.ShowDialog();
            if (!Process.GetCurrentProcess().ProcessName.Equals("SmProcess"))
                App.Current.Shutdown();

        }

        #region DecimalTryParse
        static decimal DecimalTryParse(object value)
        {
            decimal result = 0;
            if (value != null)
                decimal.TryParse(value.ToString(), out result);
            return result;
        }
        #endregion

        void DataGrid_FieldLayoutInitialized(object sender, FieldLayoutInitializedEventArgs e)
        {
            try
            {
                XamDataGrid grid = sender as XamDataGrid;

                //string stFormat = "{" + string.Format("double:-16.{0}", SmLib.SysFunc.GetFormatDecimal((string)SysObj.GetOption("M_IP_TY_GIA"))) + "}";
                //Style st = new Style(typeof(Infragistics.Windows.Editors.XamNumericEditor));
                //Setter setterFormat = new Setter(Infragistics.Windows.Editors.XamNumericEditor.MaskProperty, stFormat);//
                //Setter setterFormarProvider = new Setter(Infragistics.Windows.Editors.XamNumericEditor.FormatProviderProperty, System.Threading.Thread.CurrentThread.CurrentCulture);
                //Setter setterPromtChar = new Setter(Infragistics.Windows.Editors.XamNumericEditor.PromptCharProperty, ' ');
                //st.Setters.Add(setterFormat);
                //st.Setters.Add(setterFormarProvider);
                //st.Setters.Add(setterPromtChar);

                //grid.FieldLayouts[0].Fields["ty_gia"].Settings.EditorStyle = st;
                //grid.FieldLayouts[0].Fields["ty_gia"].Settings.AllowEdit = true;
                grid.FieldLayouts[0].Fields["tag"].Settings.AllowEdit = true;
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }

        void frmBrw_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.None)
            {
                switch (e.Key)
                {
                  
                    case Key.Space:
                        SelectEntry();
                        break;
                }
            } 
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                switch (e.Key)
                {
                    case Key.A:
                        SelectAll(true);
                        break;
                    case Key.U:
                        SelectAll(false);
                        break;
                }
            }
        }


        private void SelectEntry()
        {
            DataRecord rec = oBrowse.DataGrid.ActiveRecord as DataRecord;
            if (rec == null || rec.RecordType != RecordType.DataRecord)
                return;
            if (oBrowse.DataGrid.ActiveCell != null)
                oBrowse.DataGrid.ActiveCell = null;

            Cell cell = rec.Cells["tag"];
            if (cell.Value == null || cell.Value is DBNull)
                cell.Value = false;
            else
                cell.Value = !((bool)cell.Value);
        }

        private void SelectAll(bool tag)
        {
            if (oBrowse.DataGrid.ActiveCell != null)
                oBrowse.DataGrid.ActiveCell = null;
            DataRecord rec;
            for (int i = 0; i < oBrowse.DataGrid.Records.Count; i++)
            {
                rec = oBrowse.DataGrid.Records[i] as DataRecord;
                rec.Cells["tag"].Value = tag;
            }
        }

        private string ToS(object value, string format)
        {
            if (value == null || value is DBNull)
                return "";

            return ((decimal)value).ToString(format);
        }

        private string ToS(object value)
        {
            if (value == null || value is DBNull)
                return "0";

            return ((decimal)value).ToString(new CultureInfo("en-US"));
        }


        private decimal ToDec(object value)
        {
            if (value == null || value is DBNull)
                return 0;

            return (decimal)value;
        }


        public static void Verify(DataTable data, string fieldList)
        {
            if (data == null || data.Columns.Count == 0)
                return;
            if (fieldList == null || fieldList.Length == 0)
                return;

            bool isFound = false;
            DataColumnCollection colums = data.Columns;
            string[] fields = fieldList.Split(";".ToCharArray());
            string[] properties;

            foreach (string field in fields)
            {
                properties = field.Split(":".ToCharArray());
                isFound = false;
                for (int i = 0; i < colums.Count; i++)
                {
                    if (colums[i].ColumnName == properties[0])
                    {
                        isFound = true;
                        break;
                    }
                }
                if (!isFound)
                    Debug.WriteLine(string.Format("Column {0} is invalid.", properties[0]));
            }
        }

        #region ParseDecimal
        public decimal ParseDecimal(object obj, decimal defaultvalue)
        {
            decimal ketqua = defaultvalue;
            decimal.TryParse(obj != null ? obj.ToString() : defaultvalue.ToString(), out ketqua);
            return ketqua;
        }
        #endregion

        void oBrowse_CTRL_R(object sender, EventArgs e)
        {
            DataSet ds = SysObj.ExcuteReader(cmdQuery);
            if (dsArso1t2 == null || ds == null)
                return;
            DataTable tbl;
            string filter = dsArso1t2.Tables[0].DefaultView.RowFilter;
            DataRecord rec = oBrowse.ActiveRecord;
            int recNo = 0;

            if (rec != null)
                recNo = rec.Index;
            for (int i = ds.Tables.Count; i < dsArso1t2.Tables.Count; i++)
            {
                tbl = dsArso1t2.Tables[i];
                dsArso1t2.Tables.RemoveAt(i);

                ds.Tables.Add(tbl);
            }

            dsArso1t2 = ds;
            oBrowse.DataGrid.DataSource = dsArso1t2.Tables[0].DefaultView;

            oBrowse.frmBrw.oBrowse.FieldLayouts[0].SummaryDefinitions.Clear();
            DataTable tbSum = dsArso1t2.Tables[0];
            //if (KindStyleReport == 1)
            //{
            //    oBrowse.AddValueSummary(new string[] {  M_LAN.Equals("V") ? "ten_kh" : "ten_kh2", "ps_no", "ps_co", "dk", "ck", "du_dau", "du_cuoi"},
            //        new string[] { M_LAN.Equals("V") ? "Tổng cộng:" :  "Total:",
            //                                        //DecimalTryParse(tbSum.Rows[0]["t_ps_no"].ToString()).ToString(M_IP_TIEN),
            //                                        //DecimalTryParse(tbSum.Rows[0]["t_ps_co"].ToString()).ToString(M_IP_TIEN),
            //                                        //DecimalTryParse(tbSum.Rows[0]["t_dk"].ToString()).ToString(M_IP_TIEN),
            //                                        //DecimalTryParse(tbSum.Rows[0]["t_ck"].ToString()).ToString(M_IP_TIEN),
            //                                        tbSum.Rows[0]["du_dau_no_co"].ToString(),
            //                                        tbSum.Rows[0]["du_cuoi_no_co"].ToString()
            //                            });
            //}
            //else
            //{
            //    oBrowse.AddValueSummary(new string[] { M_LAN.Equals("V") ? "ten_kh" : "ten_kh2", "ps_no", "ps_co", "dk", "du_dau", "ck", "du_cuoi", "ps_no_nt", "ps_co_nt", "dk_nt", "du_dau_nt", "ck_nt", "du_cuoi_nt" },
            //        new string[] { M_LAN.Equals("V") ? "Tổng cộng:" : "Total:",
            //                                            //DecimalTryParse(tbSum.Rows[0]["t_ps_no"].ToString()).ToString(M_IP_TIEN),
            //                                            //DecimalTryParse(tbSum.Rows[0]["t_ps_co"].ToString()).ToString(M_IP_TIEN),
            //                                            //DecimalTryParse(tbSum.Rows[0]["t_dk"].ToString()).ToString(M_IP_TIEN),
            //                                            //tbSum.Rows[0]["du_dau_no_co"].ToString(),
            //                                            //DecimalTryParse(tbSum.Rows[0]["t_ck"].ToString()).ToString(M_IP_TIEN),
            //                                            //tbSum.Rows[0]["du_cuoi_no_co"].ToString(),

            //                                            //DecimalTryParse(tbSum.Rows[0]["t_ps_no_nt"].ToString()).ToString(M_IP_TIEN),
            //                                            //DecimalTryParse(tbSum.Rows[0]["t_ps_co_nt"].ToString()).ToString(M_IP_TIEN),
            //                                            //DecimalTryParse(tbSum.Rows[0]["t_dk_nt"].ToString()).ToString(M_IP_TIEN),
            //                                            //tbSum.Rows[0]["du_dau_no_co_nt"].ToString(),
            //                                            //DecimalTryParse(tbSum.Rows[0]["t_ck_nt"].ToString()).ToString(M_IP_TIEN),
            //                                            tbSum.Rows[0]["du_cuoi_no_co_nt"].ToString()
            //                                });
            //}
            oBrowse.UpdateSumaryFields();
            if (recNo < oBrowse.DataGrid.Records.Count && oBrowse.DataGrid.Records.Count > 0)
                oBrowse.DataGrid.ActiveRecord = oBrowse.DataGrid.Records[recNo];

        }


        void _browser_F5(object sender, EventArgs e)
        {
            if (oBrowse.ActiveRecord == null)
                return;
            CellCollection cells = (oBrowse.ActiveRecord as DataRecord).Cells;
            DataRowView currentRow = (oBrowse.DataGrid.ActiveRecord as DataRecord).DataItem as DataRowView;
            string _so_lsx = cells["so_lsx"].Value.ToString().Trim();
            //string _ten_kh = M_LAN.Equals("V") ? cells["dien_giai"].Value.ToString().Trim() : cells["dien_giai"].Value.ToString().Trim();
            string _dien_giai = cells["dien_giai"].Value.ToString().Trim();

            if (_so_lsx.Length == 0)
                return;

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "LSX";
            cmd.Parameters.Add("@Mapx", SqlDbType.VarChar).Value = "";
            cmd.Parameters.Add("@Malsx", SqlDbType.VarChar).Value = _so_lsx;
            cmd.Parameters.Add("@Ngay_ct1", SqlDbType.SmallDateTime).Value = ngay_ct1;
            cmd.Parameters.Add("@Ngay_ct2", SqlDbType.SmallDateTime).Value = ngay_ct2;
            cmd.Parameters.Add("@Tong_cong", SqlDbType.TinyInt).Value = 1;
            //lsxct = StartUp.SysObj.ExcuteReader(cmdQuery).Tables[0];
            //cmd.CommandText = "SELECT *" +
            //    " FROM v_arso1t2" +
            //    //" WHERE ngay_ct BETWEEN '" + ngay_ct1.ToString("yyyyMMdd") + "'" + " AND '" + ngay_ct2.ToString("yyyyMMdd") + "'" +
            //    //" AND tk LIKE '" + Tk.Trim() + "%'" + 
            //    " AND ma_kh LIKE '" + _ma_kh + "%'" +
            //    //" AND " + AdvanceFilter + 
            //    " ORDER BY ngay_ct, stt_ct_nkc, so_ct, stt_rec0, id";

            //Debug.WriteLine(cmd.CommandText);
            DataSet dsDetail = SysObj.ExcuteReader(cmd);

            string strFields = sFieldArrays[1];

            object _tong_cong = dsDetail.Tables[1].Rows[0]["tong_cong"];
            //decimal _co_dk = (decimal)currentRow["co_dk"];
            //decimal _ps_no = (decimal)currentRow["ps_no"];
            //decimal _ps_co = (decimal)currentRow["ps_co"];
            //decimal _no_ck = (decimal)currentRow["no_ck"];
            //decimal _co_ck = (decimal)currentRow["co_ck"];

            //decimal _no_dk_nt = (decimal)currentRow["no_dk_nt"];
            //decimal _co_dk_nt = (decimal)currentRow["co_dk_nt"];
            //decimal _ps_no_nt = (decimal)currentRow["ps_no_nt"];
            //decimal _ps_co_nt = (decimal)currentRow["ps_co_nt"];
            //decimal _no_ck_nt = (decimal)currentRow["no_ck_nt"];
            //decimal _co_ck_nt = (decimal)currentRow["co_ck_nt"];


            FormBrowse detail = new FormBrowse(SysObj, dsDetail.Tables[0].DefaultView, strFields);
            detail.frmBrw.ShowInTaskbar = false;
            detail.frmBrw.EscToClose = true;
            SmLib.SysFunc.LoadIcon(detail.frmBrw);
            detail.frmBrw.Title = SmLib.SysFunc.Cat_Dau((StartUp.M_LAN.Equals("V") ? "Chi tiết lệnh sản xuất: " : "Detail of customer statement: ") + _so_lsx + " - " + _dien_giai);

            //string dk_caption, ck_caption;
            //if (StartUp.SysObj.GetOption("M_LAN").ToString() == "V")
            //{
            //    dk_caption = _no_dk > 0 ? "Số dư nợ đầu kỳ:" : _co_dk > 0 ?
            //        "Số dư có đầu kỳ:" : "Số dư đầu kỳ:";

            //    ck_caption = _no_ck > 0 ? "Số dư nợ cuối kỳ:" : _co_ck > 0 ?
            //        "Số dư có cuối kỳ:" : "Số dư cuối kỳ:";
            //}
            //else
            //{
            //    dk_caption = _no_dk > 0 ? "Opening debit balance:" : _co_dk > 0 ?
            //        "Opening credit balance:" : "Opening balance:";

            //    ck_caption = _no_ck > 0 ? "Closing debit balance:" : _co_ck > 0 ?
            //        "Closing credit balance:" : "Closing balance:";
            //}
            //detail.AddValueSummary(new string[] { "dien_giai", "ps_no", "ps_no_nt", "ps_co", "ps_co_nt" },
            //                       new string[] { //dk_caption,
                                               /*_no_dk.ToString(M_IP_TIEN),
                                               _no_dk_nt.ToString(M_IP_TIEN_NT),
                                               _co_dk.ToString(M_IP_TIEN),                                               
                                               _co_dk_nt.ToString(M_IP_TIEN_NT)*///});
            detail.AddValueSummary(new string[] { "ten_vt", "sl_kh"/*, "ps_no_nt", "ps_co", "ps_co_nt" */},
                new string[] { M_LAN.Equals("V") ? "Tổng cộng:" : "Total:",
                                               _tong_cong.ToString()
                                               /*_ps_no_nt.ToString(M_IP_TIEN_NT),
                                               _ps_co.ToString(M_IP_TIEN),
                                               _ps_co_nt.ToString(M_IP_TIEN_NT)*/});
            //detail.AddValueSummary(new string[] { "dien_giai", "ps_no", "ps_no_nt", "ps_co", "ps_co_nt" },
            //                       new string[] { //ck_caption,
                                               /*_no_ck.ToString(M_IP_TIEN),
                                               _no_ck_nt.ToString(M_IP_TIEN_NT),
                                               _co_ck.ToString(M_IP_TIEN),                                               
                                               _co_ck_nt.ToString(M_IP_TIEN_NT)*///});



            detail.frmBrw.LanguageID  = "Arso1t2_3";
            detail.ShowDialog();
        }

        #region SumFunction
        decimal SumFunction(DataTable datatable, string columnname, string strKey, string valueKey)
        {
            decimal result = 0;
            string[] key = strKey.Split(';');
            string[] value = valueKey.Split(';');
            var SumTotal = datatable.AsEnumerable()
                        .Where(b => b.Field<string>(key[0]) == value[0] && b.Field<string>(key[1]) == value[1])
                        .Sum(x => x.Field<decimal?>(columnname));
            if (SumTotal != null)
                result = ParseDecimal(SumTotal, 0);
            return result;
        }
        #endregion

        void _browser_F7(object sender, EventArgs e)
        {
            if (oBrowse.ActiveRecord != null)
            {
                if (oBrowse.DataGrid.ActiveCell != null && oBrowse.DataGrid.ActiveCell.IsInEditMode)
                    oBrowse.DataGrid.ActiveCell.EndEditMode();
                (oBrowse.ActiveRecord as DataRecord).Update();
            }
            if (!oBrowse.DataGrid.Records.Any(x => (x as DataRecord).Cells["tag"].Value.ToString() == "True"))
            {
                ExMessageBox.Show( 2140,SysObj, "Phải đánh dấu trước khi in!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                return ;
            }

            Arso1t2In oReport = new Arso1t2In();
            oReport.Records = oBrowse.DataGrid.Records;

            oReport.Info = dsArso1t2.Tables[0].Copy();
            oReport.ShowDialog();
        }

        //public static string FcCaption
        //{
        //    get
        //    {
        //        if (M_LAN.Equals("V"))
        //            return "Ngoại tệ";
        //        return "Fc";
        //    }
        //}

        //public static string ReportTypeCaption
        //{
        //    get
        //    {
        //        if (M_LAN.Equals("V"))
        //            return "Mẫu " + Ma_nt0 + "/Ngoại tệ";
        //        return "Report form " + Ma_nt0 + "/Fc";
        //    }
        //}
    }

}
