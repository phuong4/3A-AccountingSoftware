using System;
using System.Data;
using System.Data.SqlClient;
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
using Infragistics.Windows.DataPresenter;


namespace AAA_SODMHDB
{
    /// <summary>
    /// Interaction logic for FrmMATCKTV.xaml
    /// </summary>
    public partial class FrmMATCKTV : FormList
    {
        DataTable dtDMTCKTV;
        public string strMatc = string.Empty;
        public string strTentc = string.Empty;

        public FrmMATCKTV()
        {
            InitializeComponent();
        }

        public FrmMATCKTV(string txtMatc)
        {
            InitializeComponent();
            SmLib.SysFunc.LoadIcon(this);
            strMatc = txtMatc;
        }

        private void FormDMMATC_Loaded(object sender, RoutedEventArgs e)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "Select Cast(0 as bit) is_check, ma_ktv, ten_ktv, ten_ktv2 From dmktv";
            dtDMTCKTV = StartUp.SysObj.ExcuteReader(cmd).Tables[0];

            for (int i = 0; i < dtDMTCKTV.Rows.Count; i++)
            {
                for (int j = 0; j < strMatc.Length; j++)
                {
                    if (dtDMTCKTV.Rows[i]["ma_ktv"].ToString().Trim().Equals(strMatc[j].ToString()))
                    {
                        dtDMTCKTV.Rows[i]["is_check"] = "True";
                    }
                }
            }
            if (StartUp.M_LAN == "V")
            {
                GrdMaTCKTV.FieldLayouts[0].Fields["ten_ktv2"].Visibility = Visibility.Collapsed;
                GrdMaTCKTV.FieldLayouts[0].Fields["ten_ktv2"].Width = new FieldLength(0);
            }
            else
            {
                GrdMaTCKTV.FieldLayouts[0].Fields["ten_ktv"].Visibility = Visibility.Collapsed;
                GrdMaTCKTV.FieldLayouts[0].Fields["ten_ktv"].Width = new FieldLength(0);

                Title = "Tax type list";
            }

            GrdMaTCKTV.DataSource = dtDMTCKTV.DefaultView;
        }

        private void FormDMMATC_Closed(object sender, EventArgs e)
        {
            strMatc = "";
            strTentc = "";
            for (int i = 0; i < dtDMTCKTV.DefaultView.Count; i++)
            {
                if (dtDMTCKTV.DefaultView[i]["is_check"].ToString().Trim().Equals("True"))
                {
                    strMatc = strMatc + ", ";
                    strMatc = strMatc + dtDMTCKTV.DefaultView[i]["ma_ktv"].ToString().Trim();
                    strTentc = strTentc + ", ";
                    strTentc = strTentc + dtDMTCKTV.DefaultView[i]["ten_ktv"].ToString().Trim();
                }
            }
            if (strMatc.Length > 2)
            {
                strMatc = strMatc.Substring(2, strMatc.Length - 2);
            }
            if (strTentc.Length > 2)
            {
                strTentc = strTentc.Substring(2, strTentc.Length - 2);
            }
        }

        private void FormDMMATC_PreviewKeyUp(object sender, KeyEventArgs e)
        {


        }

        private void FormDMMATC_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.A)
            {
                foreach (DataRow row in dtDMTCKTV.Rows)
                    row["is_check"] = "True";
            }

            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.U)
            {
                foreach (DataRow row in dtDMTCKTV.Rows)
                    row["is_check"] = "False";
            }

            if (e.Key == Key.Space)
            {
                DataRecord dr = GrdMaTCKTV.ActiveRecord as DataRecord;
                if (dr != null)
                {
                    if (dtDMTCKTV.Rows[dr.Index]["is_check"].ToString().Trim().Equals("True"))
                    {
                        dtDMTCKTV.Rows[dr.Index]["is_check"] = "False";
                    }
                    else
                    {
                        dtDMTCKTV.Rows[dr.Index]["is_check"] = "True";
                    }
                }

            }
        }
    }
}
