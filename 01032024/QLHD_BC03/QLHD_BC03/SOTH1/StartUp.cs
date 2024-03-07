using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using SmLib.SM.FormBrowse;
using System.Windows.Input;
using SmErrorLib;
using SmLib;
using Sm.Windows.Controls;
using Infragistics.Windows.DataPresenter;
using System.Windows;
using System.Diagnostics;
using GdtLib;

namespace QLHD_BC03
{
    public class StartUp : StartupBase
    {
        static public DataRow CommandInfo;

        static public DateTime M_ngay_ct0;
        static public string M_IP_TIEN;
        static public string M_IP_TIEN_NT;
        static public string M_IP_SL;
        static public string M_IP_GIA;
        static public string M_IP_GIA_NT;

        static public string M_ma_nt0;

        static public DataSet dsQLHD_BC03 = new DataSet();
        //0 tất cả, 1 VND, 2 Ngoại tệ
        static private int KindStyleReport = -1;

        static public string Title = "";
        static public string TableList = "v_QLHD_BC03";
        static private FrmFilter _Form;
        static private FormBrowse oBrowse;
        static private SqlCommand cmd = new SqlCommand();

        #region DataOption
        static public DataTable DtGroupInfo = null;
        public static DataTable GroupSelected = null;
        static string SumFields = "so_luong;tien;tien_nt;tien2;tien_nt2;thue;thue_nt;ck;ck_nt;pt;pt_nt;lai;lai_nt";
        #endregion


        #region Run
        public override void Run()
        {
           Namespace = "QLHD_BC03";
            M_LAN = SysObj.GetOption("M_LAN").ToString();
            M_ma_nt0 = SysObj.GetOption("M_MA_NT0").ToString();
            M_ngay_ct0 = (DateTime)SysObj.GetSysVar("M_NGAY_KY1");

            M_IP_TIEN = SysObj.GetOption("M_IP_TIEN").ToString();
            M_IP_TIEN_NT = SysObj.GetOption("M_IP_TIEN_NT").ToString();
            M_IP_SL = SysObj.GetOption("M_IP_SL").ToString();
            M_IP_GIA = SysObj.GetOption("M_IP_GIA").ToString();
            M_IP_GIA_NT = SysObj.GetOption("M_IP_GIA_NT").ToString();


            CommandInfo = SmLib.SysFunc.GetCommandInfo(SysObj, Menu_Id);
            Title = SmLib.SysFunc.Cat_Dau(M_LAN.Equals("V") ? CommandInfo["bar"].ToString() : CommandInfo["bar2"].ToString());
            if (CommandInfo != null)
            {
                _Form = new FrmFilter();
                SmLib.SysFunc.LoadIcon(_Form);
                _Form.Title = Title;

                _Form.ShowDialog();
            }
        } 
        #endregion

        #region CreateTableInfo
        public static DataTable CreateTableInfo(object tu_ngay, object den_ngay)
        {
            DataTable dtInfo = new DataTable();
            dtInfo.TableName = "TableInfo";

            DataColumn newcolumn = new DataColumn("M_ma_nt0", typeof(string));
            newcolumn.DefaultValue = M_ma_nt0;
            dtInfo.Columns.Add(newcolumn);

            //newcolumn = new DataColumn("M_IP_TIEN", typeof(string));
            //newcolumn.DefaultValue = M_IP_TIEN;
            //dtInfo.Columns.Add(newcolumn);

            //newcolumn = new DataColumn("M_IP_TIEN_NT", typeof(string));
            //newcolumn.DefaultValue = M_IP_TIEN_NT;
            //dtInfo.Columns.Add(newcolumn);

            newcolumn = new DataColumn("ngay_ct1", typeof(DateTime));
            newcolumn.DefaultValue = tu_ngay;
            dtInfo.Columns.Add(newcolumn);

            newcolumn = new DataColumn("ngay_ct2", typeof(DateTime));
            newcolumn.DefaultValue = den_ngay;
            dtInfo.Columns.Add(newcolumn);

            DataRow newrow = dtInfo.NewRow();
            dtInfo.Rows.Add(newrow);
            return dtInfo;
        }
        #endregion

        #region CallReport
        static string strstore;
        public static void CallReport(bool isFirstLoad, object tu_ngay, object den_ngay, object giam_tru, string advance, int mau_bc)
        {
            strstore = CommandInfo["store_proc"].ToString();
            string strBrowse = GetTableShow(mau_bc, false);
            string sdate = string.IsNullOrEmpty(tu_ngay.ToString()) ? "" : String.Format("{0:yyyyMMdd}", (DateTime)tu_ngay);
            string edate = string.IsNullOrEmpty(den_ngay.ToString()) ? "" : String.Format("{0:yyyyMMdd}", (DateTime)den_ngay);
            KindStyleReport = mau_bc;
            
            try
            {
                if (isFirstLoad)
                {


                    //excute store proc
                    cmd.CommandText = "EXEC @store @ngay_ct1, @ngay_ct2, @advance";
                    cmd.Parameters.Add("@store", SqlDbType.VarChar).Value = strstore;
                    cmd.Parameters.Add("@ngay_ct1", SqlDbType.Char).Value = sdate;
                    cmd.Parameters.Add("@ngay_ct2", SqlDbType.Char).Value = edate;
                    //cmd.Parameters.Add("@giam_tru", SqlDbType.TinyInt).Value = giam_tru;
                    cmd.Parameters.Add("@advance", SqlDbType.NVarChar).Value = advance;

                    dsQLHD_BC03 = SysObj.ExcuteReader(cmd);

                    GdtLib.Gdt.frmTaskDialog frmTask = new GdtLib.Gdt.frmTaskDialog();
                    int count = dsQLHD_BC03.Tables[0].DefaultView.Count;
                    frmTask.SetProgressBarMax(count);
                    frmTask.SetTitle("KIỂM TRA MST");
                    frmTask.SetIsIndeterminate(false);
                    frmTask.SetProgressBarValue(0);
                    frmTask.RunTask = (f) => {
                        try
                        {
                            for (int i = 0; i < count; i++)
                            {
                                var row2 = dsQLHD_BC03.Tables[0].Rows[i];
                                string zma_so_thue = row2["ma_so_thue"].ToString();
                                frmTask.SetMessage("Kiểm tra MST:" + zma_so_thue);
                                frmTask.IncreateProgressBarValue(1);

                                string Zten_tt = "";
                                int ztag = 1;
                                var m = SyncUtils.GetMstInfo(zma_so_thue);
                                //MessageBox.Show(m.tThai);
                                switch (m.tThai)
                                {
                                    case "00":
                                        Zten_tt = "NNT đã được cấp MST";
                                        break;
                                    case "01":
                                        Zten_tt = "NNT ngừng hoạt động và đã hoàn thành thủ tục chấm dứt hiệu lực MST";
                                        ztag = 0;
                                        break;
                                    case "02":
                                        Zten_tt = "NNT đã chuyển cơ quan thuế quản lý";
                                        break;
                                    case "03":
                                        Zten_tt = "NNT ngừng hoạt động nhưng chưa hoàn thành thủ tục chấm dứt hiệu lực MST";
                                        ztag = 0;
                                        break;
                                    case "04":
                                        Zten_tt = "NNT đang hoạt động (áp dụng cho hộ kinh doanh, cá nhân kinh doanh chưa đủ thông tin đăng ký thuế)";
                                        break;
                                    case "05":
                                        Zten_tt = "NNT tạm ngừng hoạt động, kinh doanh";
                                        ztag = 0;
                                        break;
                                    case "06":
                                        Zten_tt = "NNT không hoạt động tại địa chỉ đã đăng";
                                        ztag = 0;
                                        break;
                                    case "07":
                                        Zten_tt = "NNT chờ làm thủ tục phá sản";
                                        ztag = 0;
                                        break;
                                }

                                dsQLHD_BC03.Tables[0].Rows[i]["status_mst"] = m.tThai;
                                dsQLHD_BC03.Tables[0].Rows[i]["ten_status_mst"] = Zten_tt;
                                dsQLHD_BC03.Tables[0].Rows[i]["ftag"] = ztag;
                            }
                        }
                        catch (Exception ex)
                        {

                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            frmTask.InvokeClose();
                        }

                        
                    };

                    frmTask.ShowDialog();


                    if (dsQLHD_BC03 != null && dsQLHD_BC03.Tables.Count > 0)
                    {
                        //dsQLHD_BC03.Tables[0].TableName = "TableTongCong";
                        dsQLHD_BC03.Tables[0].TableName = "TableCT";
                       

                        //show form browse
                        oBrowse = new FormBrowse(SysObj, dsQLHD_BC03.Tables["TableCT"].DefaultView, strBrowse);
                        oBrowse.F7 += new FormBrowse.GridKeyUp_F7(oBrowse_F7);
                        oBrowse.F5 += new FormBrowse.GridKeyUp_F5(oBrowse_F5);
                        oBrowse.F11 +=new FormBrowse.GridKeyUp_F11(oBrowse_F11);
                        oBrowse.CTRL_R += new FormBrowse.GridKeyUp_CTRL_R(oBrowse_CTRL_R);
                        oBrowse.frmBrw.PreviewKeyDown += new KeyEventHandler(frmBrw_PreviewKeyDown);
                        oBrowse.frmBrw.oBrowse.FieldSettings.AllowEdit = false;

                        //SmLib.SysFunc.InsertListGroup(dsQLHD_BC03.Tables[1], DtGroupInfo, "nh_vt", "", "ten_vt;ten_vt2", "ma_vt", SumFields);
                        //oBrowse.SetRowColorByTag("ftag", "1", System.Windows.Media.Color.FromRgb(0, 0, 0), true);
                        oBrowse.SetRowColorByTag("ftag", "0", System.Windows.Media.Colors.Red, true);

                        #region load style
                        //ResourceDictionary rd = Application.LoadComponent(
                        //              new Uri("/SmStyle;component/DataGridViewStyles.xaml",
                        //              UriKind.RelativeOrAbsolute)) as ResourceDictionary;
                        //oBrowse.frmBrw.Resources.MergedDictionaries.Add(rd);


                        //object StyleHeader = oBrowse.frmBrw.FindResource("StyleHeaderPrefixArea");
                        //object StyleRecord = oBrowse.frmBrw.FindResource("StyleRecordSelector");

                        //if (StyleHeader != null && StyleHeader is Style)
                        //{
                        //    oBrowse.frmBrw.oBrowse.FieldLayoutSettings.HeaderPrefixAreaStyle = StyleHeader as Style;
                        //}
                        //else
                        //    Debug.WriteLine("Cannot load style.");

                        //if (StyleRecord != null && StyleRecord is Style)
                        //{
                        //    oBrowse.frmBrw.oBrowse.FieldLayoutSettings.RecordSelectorStyle = StyleRecord as Style;
                        //}
                        //else
                        //    Debug.WriteLine("Cannot load style.");

                        #endregion

                        oBrowse.frmBrw.Title = Title;

                    }
                }
                else
                {
                    if (dsQLHD_BC03 == null)
                        return;

                    dsQLHD_BC03.Tables[0].Rows.Clear();
                    //dsQLHD_BC03.Tables[1].Rows.Clear();
                    //dsQLHD_BC03.Tables[2].Rows.Clear();

                     //excute store proc
                    cmd.Parameters["@store"].Value = strstore;
                    cmd.Parameters["@advance"].Value = advance;
                    dsQLHD_BC03 = SysObj.ExcuteReader(cmd);

                    //dsQLHD_BC03.Tables[0].TableName = "TableTongCong";
                    dsQLHD_BC03.Tables[0].TableName = "TableCT";
                    if (dsQLHD_BC03 != null && dsQLHD_BC03.Tables.Count > 0)
                    {
                        oBrowse.frmBrw.oBrowse.DataSource = dsQLHD_BC03.Tables["TableCT"].DefaultView;
                        //oBrowse.frmBrw.oBrowse.FieldLayouts[0].SummaryDefinitions.Clear();

                        //SmLib.SysFunc.InsertListGroup(dsQLHD_BC03.Tables[1], DtGroupInfo, "nh_vt", "", "ten_vt;ten_vt2", "ma_vt", SumFields);
                        oBrowse.UpdateSumaryFields();
                    }
                }

                #region Summary
                //oBrowse.AddValueSummary(new string[] {  (StartUp.M_LAN.Equals("V") ? "ten_vt" : "ten_vt2"), "so_luong", 
                //                                       "tien", "tien_nt",
                //                                       "tien2", "tien_nt2", 
                //                                       "thue", "thue_nt", "ck", "ck_nt",
                //                                       "pt", "pt_nt", "lai", "lai_nt"},
                //                                       new string[] { StartUp.M_LAN.Equals("V") ? "Tổng cộng:" : "Total:",
                //                       DecimalTryParse(dsQLHD_BC03.Tables["TableTongCong"].Rows[0]["t_sl"]).ToString(M_IP_SL),
                //                       DecimalTryParse(dsQLHD_BC03.Tables["TableTongCong"].Rows[0]["t_tien"]).ToString(M_IP_TIEN),
                //                       DecimalTryParse(dsQLHD_BC03.Tables["TableTongCong"].Rows[0]["t_tien_nt"]).ToString(M_IP_TIEN_NT),
                //                       DecimalTryParse(dsQLHD_BC03.Tables["TableTongCong"].Rows[0]["t_tien2"]).ToString(M_IP_TIEN),
                //                       DecimalTryParse(dsQLHD_BC03.Tables["TableTongCong"].Rows[0]["t_tien_nt2"]).ToString(M_IP_TIEN_NT),
                //                       DecimalTryParse(dsQLHD_BC03.Tables["TableTongCong"].Rows[0]["t_thue"]).ToString(M_IP_TIEN),
                //                       DecimalTryParse(dsQLHD_BC03.Tables["TableTongCong"].Rows[0]["t_thue_nt"]).ToString(M_IP_TIEN_NT),
                //                       DecimalTryParse(dsQLHD_BC03.Tables["TableTongCong"].Rows[0]["t_ck"]).ToString(M_IP_TIEN),
                //                       DecimalTryParse(dsQLHD_BC03.Tables["TableTongCong"].Rows[0]["t_ck_nt"]).ToString(M_IP_TIEN_NT),
                //                       DecimalTryParse(dsQLHD_BC03.Tables["TableTongCong"].Rows[0]["t_pt"]).ToString(M_IP_TIEN),
                //                       DecimalTryParse(dsQLHD_BC03.Tables["TableTongCong"].Rows[0]["t_pt_nt"]).ToString(M_IP_TIEN_NT),
                //                       DecimalTryParse(dsQLHD_BC03.Tables["TableTongCong"].Rows[0]["t_lai"]).ToString(M_IP_TIEN),
                //                       DecimalTryParse(dsQLHD_BC03.Tables["TableTongCong"].Rows[0]["t_lai_nt"]).ToString(M_IP_TIEN_NT)});
                #endregion

                dsQLHD_BC03.Tables.Add(CreateTableInfo(tu_ngay, den_ngay).Copy());

                if (isFirstLoad)
                {



                    oBrowse.frmBrw.LanguageID  = "QLHD_BC03_1";
                    oBrowse.ShowDialog();
                    _Form.Close();
                }
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }

           
        }
        #endregion

        #region oBrowse_CTRL_R
        static void oBrowse_CTRL_R(object sender, EventArgs e)
        {
            CallReport(false, _Form.txtngay_ct1.Value, _Form.txtngay_ct2.Value, _Form.cbgiam_tru.Value, _Form.GetStringAdvance(), Convert.ToInt16(_Form.cbmau_bc.Value));
        }
        #endregion

        #region oBrowse_F5
        static void oBrowse_F5(object sender, EventArgs e)
        {
            Detail();
        }
        static void Detail()
        {
            string strBrowse = GetTableShow(KindStyleReport, true);
            string advance =" ";
            string _ma_so_thue = "";
            string _ten_kh = "";
            if (oBrowse.ActiveRecord == null)
                return;
            if (oBrowse.ActiveRecord.Cells["ma_so_thue"].Value != null)
            {
                _ma_so_thue = oBrowse.ActiveRecord.Cells["ma_so_thue"].Value.ToString().Trim();
                _ten_kh = oBrowse.ActiveRecord.Cells["ten_kh"].Value.ToString().Trim();
            }
            if (_ma_so_thue != "")
                advance =  _ma_so_thue ;
            try
            {

                //excute store proc
                cmd.Parameters["@store"].Value = strstore + "_Detail";
                cmd.Parameters["@advance"].Value = advance;

                DataSet dsQLHD_BC03_Ct = SysObj.ExcuteReader(cmd);
                if (dsQLHD_BC03_Ct != null && dsQLHD_BC03_Ct.Tables.Count > 0)
                {
                    //show form browse
                    FormBrowse oBrowseDetail = new FormBrowse(SysObj, dsQLHD_BC03_Ct.Tables[0].DefaultView, strBrowse);
                    oBrowseDetail.frmBrw.EscToClose = true;
                    oBrowseDetail.frmBrw.oBrowse.FieldSettings.AllowEdit = false;

                    oBrowseDetail.frmBrw.Title = SysFunc.Cat_Dau((StartUp.M_LAN.Equals("V") ? "Chi tiết ps của " : "Detail of ") + _ten_kh);


                    oBrowseDetail.frmBrw.LanguageID = "QLHD_BC03_2";
                    oBrowseDetail.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }
        #endregion

        #region frmBrw_PreviewKeyDown
        static void frmBrw_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //if (Keyboard.IsKeyDown(Key.P) && Keyboard.Modifiers == ModifierKeys.Control)
            //{
            //    Print();
            //}
            if (Keyboard.IsKeyDown(Key.F5) && Keyboard.Modifiers == ModifierKeys.None)
            {
                Detail();
            }
            if (Keyboard.IsKeyDown(Key.A) && Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (dsQLHD_BC03.Tables["TableTongCong"].Rows.Count > 0)
                {
                    for (int i = 0; i < dsQLHD_BC03.Tables["TableTongCong"].Rows.Count; i++)
                    {
                        dsQLHD_BC03.Tables["TableTongCong"].Rows[i]["tag"] = 1;
                    }
                }
            }
            if (Keyboard.IsKeyDown(Key.U) && Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (dsQLHD_BC03.Tables["TableTongCong"].Rows.Count > 0)
                {
                    for (int i = 0; i < dsQLHD_BC03.Tables["TableTongCong"].Rows.Count; i++)
                    {
                        dsQLHD_BC03.Tables["TableTongCong"].Rows[i]["tag"] = 0;
                    }
                }
            }
        }
        #endregion

        #region oBrowse_F7
        static void oBrowse_F7(object sender, EventArgs e)
        {
            Print();
        }

        static void Print()
        {
            SmReport.ReportManager oReport = new SmReport.ReportManager(SysObj, CommandInfo["rep_file"].ToString(), KindStyleReport);
            oReport.Preview(dsQLHD_BC03);
        }

       
        #endregion

        static void oBrowse_F11(object sender, EventArgs e)
        {
            //Them phan nhom 1,2,3
            QLHD_BC03F10 win = new QLHD_BC03F10(GroupSelected);
            win.DisplayLanguage = M_LAN;

            SmLib.SysFunc.LoadIcon(win);
            win.Title = SmLib.SysFunc.Cat_Dau(win.Title);

            try
            {

                if (!win.ShowDialog())
                    return;
                GroupSelected = win.DataOption.Copy();
                //FormFilter/ConfirmGridView
                if (DtGroupInfo == null)
                {
                    SqlCommand cmdGetGroup = new SqlCommand("Select loai_nh, ma_nh, ten_nh, ten_nh2 from dmnhvt");
                    DtGroupInfo = SysObj.ExcuteReader(cmdGetGroup).Tables[0];
                }
                string sGroup = "", strSort = "";
                int g1, g2, g3, nSort = 0;
                int.TryParse(win.DataOption.Rows[0]["group1"].ToString(), out g1);
                int.TryParse(win.DataOption.Rows[0]["group2"].ToString(), out g2);
                int.TryParse(win.DataOption.Rows[0]["group3"].ToString(), out g3);
                int.TryParse(win.DataOption.Rows[0]["sortby"].ToString(), out nSort);

                if (g1 != 0)
                    sGroup += (sGroup == "" ? "" : ";") + win.DataOption.Rows[0]["group1"].ToString();
                if (g2 != 0)
                    sGroup += (sGroup == "" ? "" : ";") + win.DataOption.Rows[0]["group2"].ToString();
                if (g3 != 0)
                    sGroup += (sGroup == "" ? "" : ";") + win.DataOption.Rows[0]["group3"].ToString();

                #region Switch nSort

                switch (nSort)
                {
                    case 0:
                        strSort = M_LAN == "V" ? "ten_vt" : "ten_vt2";
                        break;
                    case 1:
                        strSort = "ma_vt";
                        break;
                    default:
                        break;
                }
                #endregion
                //Stopwatch sw = new Stopwatch();
                //sw.Start();
                //SmLib.SysFunc.InsertListGroup(dsQLHD_BC03.Tables[1], DtGroupInfo, "nh_vt", sGroup, "ten_vt;ten_vt2", strSort, SumFields);
                //sw.Stop();
                //DataSourceReport.Tables["tbDetail"].AcceptChanges();
                //App.Current.Dispatcher.BeginInvoke(new Action(() =>
                //    {
                //        oBrowse.SetRowColorByTag("ftag", "1");
                //    }
                //    ), System.Windows.Threading.DispatcherPriority.Background);

            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }

        }

        #region GetTableShow
        static string GetTableShow(int mau_bc, bool isDetail)
        {
            string strBrowse = string.Empty;
            string[] strBrowses = null;
            switch (M_LAN)
            {
                case "V":
                    {
                        switch (mau_bc)
                        {
                            case 0:
                                strBrowses = CommandInfo["Vbrowse1"].ToString().Split('|');
                                break;
                            case 1:
                                strBrowses = CommandInfo["Vbrowse2"].ToString().Split('|');
                                break;

                        }
                    } break;
                default:
                    {
                        switch (mau_bc)
                        {
                            case 0:
                                strBrowses = CommandInfo["Ebrowse1"].ToString().Split('|');
                                break;
                            case 1:
                                strBrowses = CommandInfo["Ebrowse2"].ToString().Split('|');
                                break;

                        }
                    } break;
            }
            if (!isDetail)
                strBrowse = strBrowses[0];
            else
                strBrowse = strBrowses[1];
            return strBrowse;
        }
        #endregion

        #region DecimalTryParse
        static decimal DecimalTryParse(object value)
        {
            decimal result = 0;
            if (value != null)
                decimal.TryParse(value.ToString(), out result);
            return result;
        }
        #endregion
    }
}
