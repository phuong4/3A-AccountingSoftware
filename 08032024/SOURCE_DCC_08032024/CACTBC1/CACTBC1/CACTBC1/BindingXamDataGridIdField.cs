using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Data;
using Infragistics.Windows.DataPresenter;
using System.Windows;

namespace CACTBC1
{
    class BindingStatusVoucher : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!string.IsNullOrEmpty(value.ToString().Trim()))
            {
                DataRow[] rows = StartUp.tbStatus.Select("Ma_post =" + value);
                if (rows.Count() > 0)
                {
                    DataRow row = rows[0] as DataRow;
                    return StartUp.tbStatus.Rows.IndexOf(row);
                }
            }
            DataRow[] drs = StartUp.tbStatus.Select("Default = 1");
            if (drs.Length > 0)
                return StartUp.tbStatus.Rows.IndexOf(drs[0] as DataRow);
            else
                return -1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value == null ? StartUp.DmctInfo["ma_post"] : value;
        }

        #endregion
    }
    class BindingReadonlyCNo : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (FrmCACTBC1.IsInEditMode != null)
                if (FrmCACTBC1.IsInEditMode.Value)
                    if (value != null)
                        return value.ToString().Trim().Equals("1") ? false : true;
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    class BindingReadonly : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (FrmCACTBC1.IsInEditMode != null)
                if (FrmCACTBC1.IsInEditMode.Value)
                    if (values != null)
                    {
                        if (values[0] != DBNull.Value && values[0] != DependencyProperty.UnsetValue)
                        {
                            string[] paraStr = parameter.ToString().Trim().Split(';');

                            switch (paraStr[0])
                            {
                                case "PH":
                                    switch (paraStr[1])
                                    {
                                        case "ong_ba":
                                            if (StartUp.M_ong_ba.Equals("1"))
                                            {
                                                return false;
                                            }
                                            break;
                                        case "ngay_lct":
                                            if (StartUp.M_ngay_lct.Equals("1"))
                                            {
                                                return false;
                                            }
                                            break;
                                        case "ma_gd":
                                            //if (FrmCACTBC1.currActionTask == SmDefine.ActionTask.Add || FrmCACTBC1.currActionTask == SmDefine.ActionTask.Copy)
                                                if (values != null)
                                                {
                                                    if (values[0] != DBNull.Value && values[0] != DependencyProperty.UnsetValue)
                                                    {
                                                        int count = StartUp.DsTrans.Tables[1].DefaultView.Count;
                                                        if (count == 0)
                                                        {
                                                            return false;
                                                        }
                                                        else
                                                        {
                                                            return !(StartUp.DsTrans.Tables[1].DefaultView[0]["tk_i"] == DBNull.Value || string.IsNullOrEmpty(StartUp.DsTrans.Tables[1].DefaultView[0]["tk_i"].ToString()));
                                                        }
                                                    }
                                                }
                                            break;
                                        case "han_tt":
                                            {
                                                if (values[0] != DBNull.Value && values[0] != DependencyProperty.UnsetValue)
                                                {
                                                    string ma_gd = values[0].ToString();
                                                    if (ma_gd == "4")
                                                    {
                                                        return false;
                                                    }
                                                }
                                            }
                                            break;
                                    }
                                    break;
                                case "CT":
                                    if (paraStr[1].Equals("tien"))
                                    {
                                        if (!values[2].Equals(DependencyProperty.UnsetValue))
                                        {
                                            bool checkSuaTien = (bool)values[2];
                                            if (checkSuaTien)
                                                return false;
                                        }
                                        if (!values[0].Equals(DependencyProperty.UnsetValue) && !values[3].Equals(DependencyProperty.UnsetValue) && !values[1].Equals(DependencyProperty.UnsetValue) && !string.IsNullOrEmpty(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"].ToString()))
                                        {
                                            double tien_nt = System.Convert.ToDouble(values[0]);
                                            double ty_gia = System.Convert.ToDouble(StartUp.DsTrans.Tables[0].DefaultView[0]["ma_gd"].ToString().IndexOfAny(new char[] { '2', '5' }) >= 0 ? values[3] : StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"]);
                                            if (tien_nt * ty_gia == 0)
                                                return false;
                                        }
                                    }
                                    else if (paraStr[1].Equals("tien_tt"))
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
                                            double ty_gia = System.Convert.ToDouble(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"].ToString());
                                            if (tien_nt * ty_gia == 0)
                                                return false;
                                        }
                                    }
                                    else if (paraStr[1].Equals("TT_QD"))
                                    {
                                        if (!values[0].Equals(DependencyProperty.UnsetValue) && !values[1].Equals(DependencyProperty.UnsetValue))
                                        {
                                            return values[0].ToString() == values[1].ToString();
                                        }
                                    }
                                    break;
                            }
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
    //Binding dia chi
    public class KindCT : IValueConverter
    {
        private string[] listBtPb = new string[] { "2", "4", "5", "6", "7", "8", "9" };
        private string[] listGdCt = new string[] { "2", "3", "4", "5", "6", "7", "8", "9" };
        private string[] listGdHd = new string[] { "1","" };
        
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string ma_gd = value.ToString();
            string para = parameter.ToString();

            if (listGdCt.Contains(ma_gd) && para == "CT")
                return "Visible";

            if (listGdHd.Contains(ma_gd) && para == "HD")
                return "Visible";

            if (listBtPb.Contains(ma_gd) && para == "PB")
                return "Visible";

            if (para == "CTTT" && !listGdCt.Contains(ma_gd) && !listGdHd.Contains(ma_gd))
                return "Visible";

            return "Collapsed";

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    public class GrdCtColumnVisible : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility result = Visibility.Visible;
            if (values != null)
            {
                if (values[0] != DBNull.Value && values[0] != DependencyProperty.UnsetValue)
                {
                    switch (parameter.ToString().Trim())
                    {
                        case "ma_kh_i":
                            {
                                int ma_gd = 2;
                                int.TryParse(values[0].ToString(), out ma_gd);
                                result = ma_gd == 3 ? Visibility.Visible : Visibility.Collapsed;
                            }
                            break;
                        case "ten_kh_i":
                            {
                                int ma_gd = 2;
                                int.TryParse(values[0].ToString(), out ma_gd);
                                result = ma_gd == 3 &&  StartUp.M_LAN.Equals("V") ? Visibility.Visible : Visibility.Collapsed;
                            }
                            break;
                        case "ten_kh_i2":
                            {
                                int ma_gd = 2;
                                int.TryParse(values[0].ToString(), out ma_gd);
                                result = ma_gd == 3  && StartUp.M_LAN.Equals("E") ? Visibility.Visible : Visibility.Collapsed;
                            }
                            break;
                        case "ten_tk_i":
                            {
                                result =  StartUp.M_LAN.Equals("V") ? Visibility.Visible : Visibility.Collapsed;

                            }
                            break;
                        case "ten_tk_i2":
                            {                                
                                result =   StartUp.M_LAN.Equals("E") ? Visibility.Visible : Visibility.Collapsed;
                            }
                            break;
                        case "tien_tt":
                            {
                                result = values[0].ToString().IndexOfAny(new char[] { '2', '5' }) >= 0 && values[1].ToString() != StartUp.M_ma_nt0 ? Visibility.Visible : Visibility.Collapsed;
                            }
                            break;
                        case "tien":
                            {
                                result = values[0].ToString() != StartUp.M_ma_nt0 ? Visibility.Visible : Visibility.Collapsed;
                            }
                            break;
                    }
                }
            }
            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
