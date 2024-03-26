using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Data;
using Infragistics.Windows.DataPresenter;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;

namespace CACTPC1
{
    public class BindingVisibility : IMultiValueConverter
    {
        string[] listBtPb = new string[] { "2", "4", "5", "6", "7", "8", "9" };
        string[] listGdCt = new string[] { "2", "3", "4", "5", "6", "7", "9","" };
        string[] listGdCp = new string[] { "8" };
        string[] listGdHd = new string[] { "1" };
        
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string[] paraStr = parameter.ToString().Trim().Split(';');
            Visibility result = Visibility.Visible;

            if (values != null && values[0] != DependencyProperty.UnsetValue)
            {
                switch (paraStr[0])
                {
                    case "MaNt0":
                    case "NotMaNt0":
                        {
                            return new SmVoucherLib.BindingVisibility().Convert(values, targetType, parameter, culture);
                        }
                    case "FieldCT":
                        {
                            switch (paraStr[1])
                            {
                                case "ma_gd":
                                    {
                                        int ma_gd = 2;
                                        int.TryParse(values[0].ToString(), out ma_gd);
                                        if (ma_gd == 2 || ma_gd == 3)
                                        {
                                            result = Visibility.Visible;
                                        }
                                        else if (ma_gd == 9)
                                        {
                                            result = Visibility.Collapsed;
                                        }
                                    }
                                    break;
                                case "ma_kh":
                                    {
                                        string ma_gd = values[0].ToString();
                                        
                                        if (ma_gd == "3")
                                            result = Visibility.Visible;
                                        else 
                                            result = Visibility.Collapsed;
                                    }
                                    break;

                                case "ty_gia_ht2":
                                    {
                                        if (values[0].ToString() == "2")
                                            return Visibility.Visible;
                                        return Visibility.Collapsed;
                                    }
                                case "tien_tt":
                                    {
                                        if (values[0].ToString() == "4")
                                            return Visibility.Collapsed;
                                        return Visibility.Visible;
                                    }
                            }
                            if (paraStr.Length > 2)
                            {
                                switch (paraStr[2])
                                {

                                    case "ten_kh_i":
                                        {
                                            string ma_gd = values[0].ToString();

                                            if (ma_gd == "3" && StartUp.M_LAN.Equals("V"))
                                                result = Visibility.Visible;
                                            else
                                                result = Visibility.Collapsed;
                                        }
                                        break;
                                    case "ten_kh2_i":
                                        {
                                            string ma_gd = values[0].ToString();

                                            if (ma_gd == "3" && StartUp.M_LAN.Equals("E"))
                                                result = Visibility.Visible;
                                            else
                                                result = Visibility.Collapsed;
                                        }
                                        break;

                                }
                            }

                        }
                        break;
                    case "KindCT":
                        {
                            string ma_gd = values[0].ToString();
                            string para = paraStr[1].ToString();

                            if (listGdCt.Contains(ma_gd) && para == "CHI")
                                return Visibility.Visible;

                            if (listGdHd.Contains(ma_gd) && para == "HD")
                                return Visibility.Visible;

                            if (listGdCp.Contains(ma_gd) && para == "CP")
                                return Visibility.Visible;

                            return Visibility.Collapsed;
                        }
                    case "KindBT":
                        {
                            string ma_gd = values[0].ToString();
                            string para = paraStr[1].ToString();

                            if (listBtPb.Contains(ma_gd) && para == "PB")
                                return Visibility.Visible;

                            return Visibility.Collapsed;
                        }
                    case "KindTG":
                        {
                            string ma_gd = values[0].ToString();
                            string ma_nt = values[1].ToString();
                            string para = paraStr[1].ToString();

                            if (StartUp.M_Gd_2Tg_List.Contains(ma_gd) && para == "GS"
                                && ma_nt != StartUp.M_ma_nt0)
                                return Visibility.Visible;

                            return Visibility.Collapsed;
                        }
                    case "KindText":
                        {
                            string ma_gd = values[0].ToString();
                            string ma_nt = values[1].ToString();
                            string para = paraStr[1].ToString();

                            if (StartUp.M_Gd_2Tg_List.Contains(ma_gd) && para == "GS"
                                && ma_nt != StartUp.M_ma_nt0)
                                return "TGGD";

                            return "TGGS";
                        }
                }
            }
            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
             string[] paraStr = parameter.ToString().Trim().Split(';');
            Visibility result = Visibility.Visible;

         
                switch (paraStr[0])
                {
                    case "FieldCT":
                        {
                            switch (paraStr[1])
                            {
                                case "ma_kh":
                                    return new string[] { FrmCACTPC1.Ma_GD_Value.Text };
                            }
                        }
                        break;
                }
           
            throw new NotImplementedException();
        }

        #endregion
    }
    public class BindingReadonly : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (FrmCACTPC1.IsInEditMode != null)
                if (FrmCACTPC1.IsInEditMode.Value)
                    if (values != null)
                    {
                        string[] paraStr = parameter.ToString().Trim().Split(';');
                        switch (paraStr[0])
                        {
                            case "PH":
                                {
                                    switch (paraStr[1].ToString())
                                    {
                                        case "ong_ba":
                                            bool isInEditMode = false;
                                            bool.TryParse(values[0].ToString(), out isInEditMode);

                                            if (isInEditMode && StartUp.M_ong_ba.Equals("1"))
                                            {
                                                return false;
                                            }
                                            break;
                                        case "ngay_lct":

                                            bool.TryParse(values[0].ToString(), out isInEditMode);
                                            if (isInEditMode && StartUp.M_ngay_lct.Equals("1"))
                                            {
                                                return false;
                                            }
                                            break;
                                        case "ma_gd":
                                            //if (FrmCACTPC1.currActionTask == SmDefine.ActionTask.Edit)
                                            //    return true;
                                            int count = StartUp.DsTrans.Tables[1].DefaultView.Count;
                                            if (count > 0)
                                            {
                                                //if (StartUp.DsTrans.Tables[0].DefaultView[0]["ispostgt"] != DBNull.Value && !string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["ispostgt"].ToString()))
                                                //{
                                                //    if (StartUp.DsTrans.Tables[0].DefaultView[0]["ispostgt"].ToString().Equals("True"))
                                                //        return true;
                                                //}
                                                if (StartUp.DsTrans.Tables[2].DefaultView.Count > 0)
                                                {
                                                    if (!string.IsNullOrEmpty(StartUp.DsTrans.Tables[2].DefaultView[0]["so_ct0"].ToString()))
                                                        return true;
                                                }
                                                if (StartUp.DsTrans.Tables[1].DefaultView[0]["tk_i"] != DBNull.Value && !string.IsNullOrEmpty(StartUp.DsTrans.Tables[1].DefaultView[0]["tk_i"].ToString().Trim()))
                                                {
                                                    return true;
                                                }
                                            }
                                            return false;
                                            break;
                                    }
                                    return true;
                                }
                                break;
                            case "CT":
                                {
                                    if (values[0] != DBNull.Value && values[0] != DependencyProperty.UnsetValue)
                                    {
                                        string hdStr = values[0].ToString().Trim();
                                        switch (hdStr)
                                        {
                                            //trường hợp loại hd là 0
                                            case "0":
                                                {
                                                    string[] FieldsNotAllowEdit = { "so_seri0","kh_mau_hd", "so_ct0", "ma_kh_t", "ma_thue_i", "thue_suat", "thue_nt", "tk_thue_i", "thue", "tt_nt", "tt" };

                                                    if (FieldsNotAllowEdit.Contains(paraStr[1]))
                                                    {
                                                        return true;
                                                    }
                                                }
                                                break;
                                            case "1":
                                                {
                                                    string[] FieldsNotAllowEdit = { "tt_nt", "tt" };
                                                    if (FieldsNotAllowEdit.Contains(paraStr[1]))
                                                    {
                                                        return true;
                                                    }
                                                }
                                                break;
                                            case "2":
                                                {
                                                    string[] FieldsNotAllowEdit = { "tt_nt", "tt" };
                                                    if (FieldsNotAllowEdit.Contains(paraStr[1]))
                                                    {
                                                        return true;
                                                    }
                                                }
                                                break;
                                            case "4":
                                                {
                                                    string[] FieldsNotAllowEdit = { "tt_nt", "tt" };
                                                    if (FieldsNotAllowEdit.Contains(paraStr[1]))
                                                    {
                                                        return true;
                                                    }
                                                }
                                                break;
                                            case "5":
                                                {
                                                    string[] FieldsNotAllowEdit = { "tk_thue_i", "kh_mau_hd", "so_seri0", "so_ct0", "ma_kh_t", "ten_kh_t", "dia_chi_t", "mst_t", "ten_vt_t", "ma_thue_i", "thue_suat", "tt_nt", "tt", "ghi_chu_t" };
                                                    if (FieldsNotAllowEdit.Contains(paraStr[1]))
                                                    {
                                                        return true;
                                                    }
                                                }
                                                break;
                                        }
                                        if (paraStr[1].Equals("tien"))
                                        {
                                            if (!values[3].Equals(DependencyProperty.UnsetValue))
                                            {
                                                bool checkSuaTien = (bool)values[3];
                                                if (checkSuaTien)
                                                    return false;
                                            }
                                            if (!values[1].Equals(DependencyProperty.UnsetValue) && !values[2].Equals(DependencyProperty.UnsetValue) && !string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"].ToString()))
                                            {
                                                double tien_nt = System.Convert.ToDouble(values[1]);
                                                double ty_gia = System.Convert.ToDouble(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"]);
                                                if (tien_nt * ty_gia == 0)
                                                    return false;
                                            }
                                            return true;
                                        }
                                        else if (paraStr[1].Equals("thue_nt"))
                                        {
                                            if (!values[3].Equals(DependencyProperty.UnsetValue))
                                            {
                                                bool checkSuaTien = (bool)values[3];
                                                if (checkSuaTien)
                                                    return false;
                                            }
                                            if (!values[1].Equals(DependencyProperty.UnsetValue) && !values[2].Equals(DependencyProperty.UnsetValue))
                                            {
                                                double tien = System.Convert.ToDouble(values[1]);
                                                double thue_suat = System.Convert.ToDouble(values[2]);
                                                if (tien * thue_suat == 0)
                                                    return false;
                                            }
                                            return true;
                                        }
                                        else if (paraStr[1].Equals("thue"))
                                        {
                                            if (!values[3].Equals(DependencyProperty.UnsetValue))
                                            {
                                                bool checkSuaTien = (bool)values[3];
                                                if (checkSuaTien)
                                                    return false;
                                            }
                                            if (!values[1].Equals(DependencyProperty.UnsetValue) && !values[2].Equals(DependencyProperty.UnsetValue))
                                            {
                                                double tien = System.Convert.ToDouble(values[1]);
                                                double thue_suat = System.Convert.ToDouble(values[2]);
                                                if (tien * thue_suat == 0)
                                                    return false;
                                            }
                                            return true;
                                        }
                                        else if (paraStr[1].Equals("ma_kh2_t"))
                                        {
                                            //Debug.WriteLine(".................// Converter ma_kh2_t //................");
                                            //Debug.WriteLine(values[0], "loai_hd");
                                            //Debug.WriteLine(values[1], "tk_thue_cn");

                                            if (values[1] != null && !values[1].Equals(DependencyProperty.UnsetValue))
                                            {
                                                if (values[1].ToString().Trim().Equals("1"))
                                                    return false;
                                                else
                                                    return true;
                                            }
                                            else
                                            {
                                                return true;
                                            }
                                        }
                                    }
                                    return false;
                                }
                                break;
                            case "CTCHI":
                                {
                                    switch (paraStr[1])
                                    {
                                        case "tien_tt":
                                            if (!values[2].Equals(DependencyProperty.UnsetValue))
                                            {
                                                bool Voucher_Ma_nt0 = true;
                                                bool.TryParse(values[2].ToString(), out Voucher_Ma_nt0);
                                                if (!Voucher_Ma_nt0)
                                                {
                                                    //Check sửa tiền
                                                    if (!values[3].Equals(DependencyProperty.UnsetValue))
                                                    {
                                                        if ((bool)values[3])
                                                            return false;
                                                    }
                                                    if (!values[0].Equals(DependencyProperty.UnsetValue) && !string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"].ToString()))
                                                    {
                                                        decimal ty_gia = 0, tien_nt = 0;
                                                        decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"].ToString(), out ty_gia);
                                                        decimal.TryParse(values[0].ToString(), out tien_nt);
                                                        if (ty_gia * tien_nt == 0)
                                                            return false;
                                                    }
                                                }
                                            }
                                            break;
                                        case "tien":
                                            if (!values[3].Equals(DependencyProperty.UnsetValue))
                                            {
                                                bool Voucher_Ma_nt0 = true;
                                                bool.TryParse(values[3].ToString(), out Voucher_Ma_nt0);
                                                if (!Voucher_Ma_nt0)
                                                {
                                                    if (!values[5].Equals(DependencyProperty.UnsetValue))
                                                    {
                                                        if ((bool)values[5])
                                                            return false;
                                                    }
                                                    if (!values[4].Equals(DependencyProperty.UnsetValue))
                                                    {
                                                        string ma_gd = values[4].ToString();
                                                        if (ma_gd.Equals("2") || ma_gd.Equals("3"))
                                                        {
                                                            if (!values[0].Equals(DependencyProperty.UnsetValue) && !values[1].Equals(DependencyProperty.UnsetValue))
                                                            {
                                                                decimal ty_gia = 0, tien_nt = 0;
                                                                decimal.TryParse(values[0].ToString(), out ty_gia);
                                                                decimal.TryParse(values[1].ToString(), out tien_nt);
                                                                if (ty_gia * tien_nt == 0)
                                                                    return false;
                                                            }
                                                        }
                                                        else if (ma_gd.Equals("9"))
                                                        {
                                                            if (!values[1].Equals(DependencyProperty.UnsetValue) && !string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"].ToString()))
                                                            {
                                                                decimal ty_gia = 0, tien_nt = 0;
                                                                decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"].ToString(), out ty_gia);
                                                                decimal.TryParse(values[1].ToString(), out tien_nt);
                                                                if (ty_gia * tien_nt == 0)
                                                                    return false;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            break;
                                    }
                                }
                                break;
                            case "CTHD":
                                {
                                    switch (paraStr[1])
                                    {
                                        case "tien":
                                            if (!values[0].Equals(DependencyProperty.UnsetValue) && !values[1].Equals(DependencyProperty.UnsetValue))
                                            {
                                                double tien_nt = System.Convert.ToDouble(values[0]);
                                                double ty_gia = 1;
                                                if (StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"] != DBNull.Value && !string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"].ToString()))
                                                {
                                                    ty_gia = System.Convert.ToDouble(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"]);
                                                }
                                                if (tien_nt * ty_gia == 0)
                                                    return false;
                                            }
                                            if (!values[2].Equals(DependencyProperty.UnsetValue))
                                            {
                                                bool checkSuaTien = (bool)values[2];
                                                if (checkSuaTien)
                                                    return false;
                                            }
                                            return true;
                                        case "TT_QD":
                                            if (!values[0].Equals(DependencyProperty.UnsetValue) && !values[1].Equals(DependencyProperty.UnsetValue))
                                            {
                                                return values[0].ToString() == values[1].ToString();
                                            }

                                            return false;
                                    }
                                    break;
                                }
                            case "CTGT":
                                {
                                    if (FrmCACTPC1.IsInEditModeThue != null)
                                        if (FrmCACTPC1.IsInEditModeThue.Value)
                                        {
                                            int count = StartUp.DsTrans.Tables[0].DefaultView.Count;
                                            if (count > 0)
                                            {
                                                if (StartUp.DsTrans.Tables[0].DefaultView[0]["ma_gd"] != DBNull.Value && !string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_gd"].ToString()) && StartUp.DsTrans.Tables[0].DefaultView[0]["ma_gd"].ToString().Equals("8"))
                                                {
                                                    if (paraStr[1].Equals("ma_so_thue"))
                                                    {
                                                        if (values[1].ToString().Equals("True"))
                                                            return (values[0].ToString().Trim().Equals("") || values[0] == DependencyProperty.UnsetValue) ? false : true;
                                                    }
                                                    else if (paraStr[1].Equals("t_tien"))
                                                    {
                                                        if (!values[2].Equals(DependencyProperty.UnsetValue))
                                                        {
                                                            bool checkSuaTien = (bool)values[2];
                                                            if (checkSuaTien)
                                                                return false;
                                                        }
                                                        if (!values[0].Equals(DependencyProperty.UnsetValue) && !values[1].Equals(DependencyProperty.UnsetValue) && !string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"].ToString()))
                                                        {
                                                            double tien_nt = System.Convert.ToDouble(values[0]);
                                                            double ty_gia = System.Convert.ToDouble(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"]);
                                                            if (tien_nt * ty_gia == 0)
                                                                return false;
                                                        }
                                                    }
                                                    else if (paraStr[1].Equals("t_thue"))
                                                    {
                                                        if (!values[2].Equals(DependencyProperty.UnsetValue))
                                                        {
                                                            bool checkSuaTien = (bool)values[2];
                                                            if (checkSuaTien)
                                                                return false;
                                                        }
                                                        if (!values[0].Equals(DependencyProperty.UnsetValue) && !values[1].Equals(DependencyProperty.UnsetValue))
                                                        {
                                                            double tien = System.Convert.ToDouble(values[0]);
                                                            double thue_suat = System.Convert.ToDouble(values[1]);
                                                            if (tien * thue_suat == 0)
                                                                return false;
                                                        }
                                                    }
                                                    else if (paraStr[1].Equals("ma_kh2_t"))
                                                    {
                                                        if (values[0] != null)
                                                            return values[0].ToString().Trim().Equals("1") ? false : true;
                                                    }
                                                }
                                            }
                                        }
                                }
                                break;
                        }
                    }
            return true;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
