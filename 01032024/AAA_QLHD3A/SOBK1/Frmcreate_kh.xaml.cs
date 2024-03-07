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
using SmErrorLib;

namespace AAA_QLHD3A
{
    /// <summary>
    /// Interaction logic for FrmSOCTPNF_PN.xaml
    /// </summary>
    public partial class Frmcreate_kh : Form
    {
        CodeValueBindingObject Voucher_Ma_nt0;
        public DataRowView drvFrmAA_PODMHDM_PN;
        public bool isOk = false;

        public Frmcreate_kh(ref DataTable tbSource)
        {
            InitializeComponent();
            SmLib.SysFunc.LoadIcon(this);
            Loaded += new RoutedEventHandler(FrmPoctpxf_PN_Loaded);
            this.Title = "Thêm mới khách hàng, NCC"; //SmLib.SysFunc.Cat_Dau(ten_vt.ToString());
            this.EscToClose = true;
            Grdcreate_kh.DataSource = tbSource.DefaultView;

        }

        private void FrmPoctpxf_PN_Loaded(object sender, RoutedEventArgs e)
        {
            if (Grdcreate_kh.Records.Count > 0)
            {
                Grdcreate_kh.Focus();
                Grdcreate_kh.ActiveRecord = Grdcreate_kh.Records[0];
            }

            isOk = false;
        }

        private void ConfirmGridView_OnOk(object sender, RoutedEventArgs e)
        {
            var dtView = Grdcreate_kh.DataSource as DataView;
            string err = "";
            foreach (DataRowView r in dtView)
            {
                string ma_kh = r["ma_kh"].ToString().Trim().ToUpper();
                r.Row.ClearErrors();

                if (ma_kh != "")
                {
                    r["ma_kh"] = ma_kh;
                    string msg = "";
                    if (!CheckValid(ma_kh, out msg))
                    {
                        r.Row.SetColumnError("ma_kh", msg);
                        err += msg + Environment.NewLine;
                    }
                    else
                    {
                        var founds = dtView.Table.Select(" ma_kh IS NOT NULL AND '" + ma_kh + "' Like (ma_kh +'%')");
                        if (founds.Length > 1)
                        {
                            r.Row.SetColumnError("ma_kh", "Mã đã có hoặc lồng nhau");

                            err += "Mã đã có hoặc lồng nhau" + Environment.NewLine;
                        }
                    }
                }
            }
            if (dtView.Table.HasErrors)
            {
                MessageBox.Show("Loi du lieu:" + err);
                //  Grdcreate_kh.r
                return;
            }

            isOk = true;
            this.Close();
        }

        private void ConfirmGridView_OnCancel(object sender, RoutedEventArgs e)
        {
            isOk = false;
            this.Close();
        }




        public bool CheckValid(string ma_kh, out string msg)
        {
            bool result = true;
            msg = "";

            if (ma_kh.Trim() != "")
            {
                string sb = SmLib.SysFunc.CheckInValidCode(StartUp.SysObj, ma_kh.Trim());
                if (sb != "" && result == true)
                {
                    msg += "Mã không được chứa các ký tự " + "[" + sb + "]" + "!";
                    result = false;
                }
                if (result == true)
                {
                    try
                    {

                        System.Data.SqlClient.SqlCommand cmdGet = new System.Data.SqlClient.SqlCommand("exec dbo.CheckExistListId @ma_dm, @" + StartUp.SqlTableKey);
                        cmdGet.Parameters.Add("@ma_dm", SqlDbType.Char).Value = "dmkh";
                        cmdGet.Parameters.Add("@" + StartUp.SqlTableKey, SqlDbType.Char).Value = ma_kh.Trim();
                        int ListExist = (int)StartUp.SysObj.ExcuteScalar(cmdGet);
                        if (ListExist > 0)
                        {
                            msg += "Mã đã có hoặc mã lồng nhau!";
                            result = false;
                        }

                    }
                    catch (Exception ex)
                    {
                        ErrorLog.CatchMessage(ex);
                        result = false;
                    }
                }
            }
            return result;


        }
    }
}
