using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SmLib.SM.FormBrowse;
using System.Data.SqlClient;
using System.Windows;
using SmLib.SM.FormBrowse2;
using Infragistics.Windows.DataPresenter;
using Microsoft.Win32;

namespace NN_BCLIN
{
    class StartUp : Sm.Windows.Controls.StartupBase
    {
       
        static public DataSet DataSourceReport = new DataSet();
       
        static public DataTable dtInfo;
        static public DataTable tbMain = null;
        static public DataTable tbDetail = null;
        static private FormBrowse2 oBrowse;
       
        static private DataRow CommandInfo;
       
        static public string sqlTableView = "v_ct65";
        static public string SqlTableKey = "stt_rec";//
        static public string SqlTableObjectName = "ten_kh";
        //static public string pListVoucher = "HDA;HDX";

        static public DateTime M_ngay_ct0;
        static public string M_ma_nt0;

        static SqlCommand cmd1 = new SqlCommand();
        static SqlCommand cmd = new SqlCommand();
        static FormLoc _frmLoc;
        static public string strLan = string.Empty;
       
        static public string TableName = "ct70";
        public override void Run()
        {
           Namespace = "NN_BCLIN";
            try
            {
                DateTime t1 = DateTime.Now;
                M_ma_nt0 = StartUp.SysObj.GetOption("M_MA_NT0").ToString();
                CommandInfo = SmLib.SysFunc.GetCommandInfo(SysObj, Menu_Id);
                M_ngay_ct0 = (DateTime)SysObj.GetSysVar("M_NGAY_KY1");
                strLan = SysObj.GetOption("M_LAN").ToString().ToUpper();

                dtInfo = new DataTable();
                dtInfo.TableName = "TbInfo";
                dtInfo.Columns.Add("StartDate",typeof(DateTime));
                dtInfo.Columns.Add("EndDate", typeof(DateTime));
                dtInfo.Rows.Add(DateTime.Now.Date, DateTime.Now.Date);
                if (CommandInfo != null)
                {
                    //_frmSearch = new TransactionListFrm();
                    //_frmSearch.ShowDialog();
                    _frmLoc = new FormLoc();
                    _frmLoc.Title = SmLib.SysFunc.Cat_Dau(M_LAN.Equals("V") ? CommandInfo["bar"].ToString() : CommandInfo["bar2"].ToString());
                    DateTime t2 = DateTime.Now;
                   // MessageBox.Show((t2 - t1).TotalMilliseconds.ToString());
                    _frmLoc.ShowDialog();
                   
                }
              
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public static void CallGridVouchers(object StartDate, object EndDate, string filter,  int KindReport)
        
        {
            try
            {
                string strBrowse = "";
                string strBrowse2 = "";
                string strBrowse_nt = "";
                string strBrowse2_nt = "";
                string[] listStored = CommandInfo["store_proc"].ToString().Split('|');
                cmd = new SqlCommand();
                cmd.CommandText = "Exec " + listStored[0] + " @StartDate , @EndDate , @Condition";
                cmd.Parameters.Add("@StartDate", SqlDbType.VarChar).Value = string.IsNullOrEmpty(StartDate.ToString()) ? "" : String.Format("{0:yyyyMMdd}", (DateTime)StartDate);
                cmd.Parameters.Add("@EndDate", SqlDbType.VarChar).Value = string.IsNullOrEmpty(EndDate.ToString()) ? "" : String.Format("{0:yyyyMMdd}", (DateTime)EndDate);
                cmd.Parameters.Add("@Condition", SqlDbType.NVarChar).Value =filter;
                DataSet ds = SysObj.ExcuteReader(cmd);
                DataTable tbDetail = ds.Tables[1].Copy();
                DataTable tbMain = ds.Tables[0].Copy();
                //tbMain.DefaultView.Sort = "ngay_ct,ma_ct_in,so_ct";
                tbMain.TableName = "tbMain";

                DataTable reportHeaderTable = tbMain.Copy();
                DataTable reportDetailTable = tbDetail.Copy();
                reportHeaderTable.TableName = "tbPh";
                reportHeaderTable.Columns.Add("KHInfo", typeof(String), "TRIM(ma_kh)+' - '+ TRIM(ten_kh)");
                reportDetailTable.TableName = "tbCt";
                // Thêm cột Vật tư Info (khỏi phải xài CalculatedField)
                reportDetailTable.Columns.Add("VTInfo", typeof(String), "TRIM(ma_vt)+' - '+ TRIM(ten_vt)");
                DataSourceReport = new DataSet();
                DataSourceReport.Tables.Add(reportHeaderTable);
                DataSourceReport.Tables.Add(reportDetailTable);
                DataSourceReport.Tables.Add(dtInfo.Copy());
                // tạo relation cho 2 table Header va Detail trong dataset DataSourceReport
                DataColumn ParentCol = reportHeaderTable.Columns["stt_rec"];
                DataColumn ChildrenCol = reportDetailTable.Columns["stt_rec"];

                String RelationName ="Stt_rec_Relation";
                DataRelation dRelation = new DataRelation(RelationName, ParentCol, ChildrenCol,false);
                DataSourceReport.Relations.Add(dRelation);
                String[] arrStrBrowse1, arrStrBrowse2;
                if (strLan.Equals("V"))
                {
                    arrStrBrowse1 = CommandInfo["VBrowse1"].ToString().Trim().Split('|');
                    arrStrBrowse2 = CommandInfo["VBrowse2"].ToString().Trim().Split('|');
                }
                else
                {
                    arrStrBrowse1 = CommandInfo["EBrowse1"].ToString().Trim().Split('|');
                    arrStrBrowse2 = CommandInfo["EBrowse2"].ToString().Trim().Split('|');
                }

                //if (loai != "2")
                //{
                //    ;
                //    
                //}
                //else
                //{
                //    strBrowse = arrStrBrowse1[2];
                //    strBrowse2 = arrStrBrowse1[3];
                //    strBrowse_nt = arrStrBrowse2[2];
                //    strBrowse2_nt = arrStrBrowse2[3];
                //}



                strBrowse = arrStrBrowse1[0];
                strBrowse2 = arrStrBrowse1[1];
                strBrowse_nt = arrStrBrowse2[0];
                strBrowse2_nt = arrStrBrowse2[1];
                oBrowse = new FormBrowse2(SysObj, tbMain.DefaultView, tbDetail.DefaultView, strBrowse, strBrowse2, "stt_rec");

                oBrowse.F5 += new FormBrowse2.GridKeyUp_F5(oBrowse_F5);
                oBrowse.F7 += new FormBrowse2.GridKeyUp_F7(oBrowse_F7);
                oBrowse.CTRL_R += new FormBrowse2.GridKeyUp_CTRL_R(oBrowse_CTRL_R);
                oBrowse.frmBrw.oBrowse.FieldSettings.AllowEdit = true;
                oBrowse.frmBrw.Title = SmLib.SysFunc.Cat_Dau(M_LAN.Equals("V") ? CommandInfo["bar"].ToString() : CommandInfo["bar2"].ToString());
                //oBrowse.Esc += new FormBrowse2.GridKeyUp_Esc(oBrowse_Esc);


                oBrowse.frmBrw.LanguageID  = "SOBK1_1";
                oBrowse.ShowDialog();
                App.Current.Shutdown();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        #region QueryData
        public static void QueryData(bool isFirstLoad, object StartDate, object EndDate, string filter,  string MaVT, int KindReport)
        {
            try
            {
                if (isFirstLoad)
                {
                    CallGridVouchers(StartDate, EndDate, filter,  KindReport);
                }
                else
                {

                    DataTable tbInfo = DataSourceReport.Tables["tbInfo"].Copy();
                    DataSourceReport = new DataSet();
                    DataSet ds = SysObj.ExcuteReader(cmd);
                    DataTable tbDetail = ds.Tables[1].Copy();
                    DataTable tbMain = ds.Tables[0].Copy();
                    tbDetail.TableName = "tbDetail";
                    tbMain.TableName = "tbMain";
                    DataTable reportHeaderTable = tbMain.Copy();
                    DataTable reportDetailTable = tbDetail.Copy();
                    reportHeaderTable.TableName = "tbPh";
                    reportHeaderTable.Columns.Add("KHInfo", typeof(String), "TRIM(ma_kh)+' - '+ TRIM(ten_kh)");
                    reportDetailTable.TableName = "tbCt";
                    // Thêm cột Vật tư Info (khỏi phải xài CalculatedField)
                    reportDetailTable.Columns.Add("VTInfo", typeof(String), "TRIM(ma_vt)+' - '+ TRIM(ten_vt)");
                    DataSourceReport = new DataSet();
                    DataSourceReport.Tables.Add(reportHeaderTable);
                    DataSourceReport.Tables.Add(reportDetailTable);
                    DataSourceReport.Tables.Add(tbInfo.Copy());
                    // tạo relation cho 2 table Header va Detail trong dataset DataSourceReport
                    DataColumn ParentCol = reportHeaderTable.Columns["stt_rec"];
                    DataColumn ChildrenCol = reportDetailTable.Columns["stt_rec"];

                    String RelationName = "Stt_rec_Relation";
                    DataRelation dRelation = new DataRelation(RelationName, ParentCol, ChildrenCol, false);
                    DataSourceReport.Relations.Add(dRelation);

                    //oBrowse.frmBrw.oBrowse.DataSource = null;

                    oBrowse.frmBrw.oBrowse.DataSource = tbMain.DefaultView;
                    oBrowse.frmBrw.oBrowseCt.DataSource = tbDetail.DefaultView;
                    oBrowse.ObrowseViewCt = tbDetail.DefaultView;
                    DataSourceReport.Tables.Add(tbDetail);
                    oBrowse.frmBrw.oBrowse.FieldLayouts[0].SummaryDefinitions.Clear();
                    //if (KieuLoc == 1)
                    //{
                    //    //oBrowse.ObrowseViewCt.RowFilter = " ma_vt ='" + MaVT + "'";
                    //    if (!String.IsNullOrEmpty(MaVT))
                    //        oBrowse.constCtFilter = " ma_vt ='" + MaVT + "'";
                    //}
                    oBrowse.UpdateSumaryFields();
                }
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }
        }
        static void oBrowse_CTRL_R(object sender, EventArgs e)
        {
            string filter = _frmLoc.GetFilter();
            StartUp.QueryData(false, (object)_frmLoc.TxtStartDateTime.Value, (object)_frmLoc.TxtEndDateTime.Value, filter,  _frmLoc.txtMaVT.Text.Trim(), int.Parse(_frmLoc.cbMauBaoCao2.Value.ToString()));
        }
        #endregion

        static void oBrowse_F5(object sender, EventArgs e)
        {
            int dem = 0;
            foreach (DataRow row in tbMain.Rows)
            {
                if (row["tag"].ToString().Trim().Equals("True"))
                    dem += 1;
            }

            if (dem == 0)
            {
                MessageBox.Show( "Chưa chọn hóa đơn!");
                return;
            }



            DataTable dt = new DataTable();
            dt.Columns.Add("ma_td2", typeof(string));
            dt.Columns.Add("ma_s1", typeof(string));
            dt.Columns.Add("ma_s2", typeof(string));
            dt.Columns.Add("ma_s3", typeof(string));

            DataRow dr = dt.NewRow();

            dt.Rows.Add(dr);

            FrmTaoHDA win = new FrmTaoHDA();
            win.DisplayLanguage = M_LAN;

            SmLib.SysFunc.LoadIcon(win);
            win.Title = SmLib.SysFunc.Cat_Dau(win.Title);

            win.tbInfoPX = dt;
            win.DataContext = dt.DefaultView;

            try
            {
                win.ShowDialog();

                if (win.isOk)
                {
                    dt.Rows[0]["ma_td2"] = win.txtMa_td2.Text.Trim();
                    dt.Rows[0]["ma_s1"] = win.txtMa_s1.Text.Trim();
                    dt.Rows[0]["ma_s2"] = win.txtMa_s2.Text.Trim();
                    dt.Rows[0]["ma_s3"] = win.txtMa_s3.Text.Trim();

                    //TbInfo.Rows[0]["Ma_td2"] = win.txtMa_td2.Text.Trim();
                    //TbInfo.Rows[0]["Ten_td2"] = win.txtTen_td2.Text.Trim();
                    //TbInfo.Rows[0]["Ma_s1"] = win.txtMa_s1.Text.Trim();
                    //TbInfo.Rows[0]["Ten_s1"] = win.txtTen_s1.Text.Trim();
                    //TbInfo.Rows[0]["Ma_s2"] = win.txtMa_s2.Text.Trim();
                    //TbInfo.Rows[0]["Ten_s2"] = win.txtTen_s2.Text.Trim();
                    //TbInfo.Rows[0]["Ma_s3"] = win.txtMa_s3.Text.Trim();
                    //TbInfo.Rows[0]["Ten_s3"] = win.txtTen_s3.Text.Trim();
                    //TbInfo.Rows[0]["Ngay_ct"] = win.txtNgay_ct.dValue;

                    string list_stt_rec = "";
                    string list_ct = "";

                    foreach (DataRow row in tbMain.Rows)
                    {
                        if (row["tag"].ToString().Trim().Equals("True"))
                        {
                            string stt_rec = row["stt_rec"].ToString().Trim();
                            string so_ct = row["so_ct"].ToString().Trim();

                            list_stt_rec += (string.IsNullOrEmpty(list_stt_rec) ? "" : ";") + stt_rec;
                            list_ct += (string.IsNullOrEmpty(list_ct) ? "" : ";") + so_ct;

                            SqlCommand cmd = new SqlCommand("UPDATE ph81 SET ma_td2 = @ma_td2, ma_s1 = @ma_s1, ma_s2 = @ma_s2, ma_s3 = @ma_s3 WHERE stt_rec = @stt_rec");

                            cmd.Parameters.Add("@ma_td2", SqlDbType.VarChar).Value = dt.Rows[0]["ma_td2"];
                            cmd.Parameters.Add("@ma_s1", SqlDbType.VarChar).Value = dt.Rows[0]["ma_s1"];
                            cmd.Parameters.Add("@ma_s2", SqlDbType.VarChar).Value = dt.Rows[0]["ma_s2"];
                            cmd.Parameters.Add("@ma_s3", SqlDbType.VarChar).Value = dt.Rows[0]["ma_s3"];
                            cmd.Parameters.Add("@Stt_rec", SqlDbType.VarChar).Value = stt_rec;

                            StartUp.SysObj.ExcuteNonQuery(cmd);
                        }
                    }

                    //TbInfo.Rows[0]["List_ct"] = list_ct;
                    //DataSourceReport.Tables.Remove("TbInfo");
                    //DataSourceReport.Tables.Add(TbInfo.Copy());

                    //SqlCommand sqlCmd = new SqlCommand("Exec " + CommandInfo["store_proc"].ToString().Split('|')[0].ToString().Trim() + ";10 @List_stt_rec");
                    //sqlCmd.Parameters.Add("@List_stt_rec", SqlDbType.NVarChar).Value = list_stt_rec;

                    //TbTotal = SysObj.ExcuteReader(sqlCmd).Tables[0].Copy();
                    //TbTotal.TableName = "TbTotal";

                    //DataSourceReport.Tables.Remove("TbTotal");
                    //DataSourceReport.Tables.Add(TbTotal.Copy());

                    //oBrowse_F7(null, null);

                    MessageBox.Show("Chương trình đã thực hiện xong!", "Thong bao");

                    //Cap nhat lai dstrans
                    //string filter = _frmLoc.GetFilter();
                    //StartUp.QueryData(false, (object)_frmLoc.TxtStartDateTime.Value, (object)_frmLoc.TxtEndDateTime.Value, filter, int.Parse(_frmLoc.cbMauBaoCao2.Value.ToString()));

                    if (tbMain.Rows.Count > 0)
                        oBrowse.frmBrw.oBrowse.ActiveRecord = oBrowse.frmBrw.oBrowse.Records[0];

                }
            }
            catch (Exception ex)
            {
                SmErrorLib.ErrorLog.CatchMessage(ex);
            }

        }


        //static void oBrowse_Esc(object sender, EventArgs e)
        //{
        //    oBrowse.frmBrw.Close();
        //    App.Current.Shutdown();
        //}



        static void oBrowse_F7(object sender, EventArgs e)
        {
            SmReport.ReportManager oReport = new SmReport.ReportManager(SysObj, CommandInfo["rep_file"].ToString());
            // Set the parameter's value.
            SmLib.SysFunc.DSCopyWithFilter(oBrowse.frmBrw.oBrowse, ref DataSourceReport, "tbPh");
            oReport.Preview(DataSourceReport);
            SmLib.SysFunc.ResetFilter(ref DataSourceReport, "tbPh");
        }
        public static string GetTableShow(bool KindReport)
        {
            string strFieldShow = string.Empty;
            strFieldShow = "ngay_ct;Ma_ct0;so_ct;ma_kh;";
            if (strLan.Equals("V"))
            {
                strFieldShow += "ten_kh;dien_giai;tk;tk_du;";
            }
            else
            {
                strFieldShow += "ten_kh2;dien_giai;tk;tk_du;";
            }

            if (KindReport)
            {
                strFieldShow += "ps_no;ps_co;ma_vv;ma_phi;";
            }
            else
            {
                strFieldShow += "ps_no_nt;ps_co_nt;ma_vv;ma_phi;";
            }

            if (strLan.Equals("V"))
            {
                strFieldShow += "ten_tk;ten_tk_du;ma_ct;ma_dvcs";
            }
            else
            {
                strFieldShow += "ten_tk2;ten_tk2_du;ma_ct;ma_dvcs";
            }
            return strFieldShow;
        }
        //public static string GetStrBrowse1(bool KindReport,int loai)
        //{
        //    string ketqua="";
        //    if (KindReport == true)
        //    { 
                
        //    }
        //    return ketqua;
        //}
        //public static string GetStrBrowse2(bool KindReport)
        //{
        //    string ketqua = "";
        //    return ketqua;
        //}
    }
}
