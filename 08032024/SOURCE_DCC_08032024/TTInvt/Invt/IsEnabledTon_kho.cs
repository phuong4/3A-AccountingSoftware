using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace TT_Invt
{
    public class IsEnabledTon_kho : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool IsEnabled = false;
            if (parameter != null)
            {
                string paraStr = parameter.ToString();
                switch (paraStr)
                {
                    case "vt_ton_kho":
                    case "sua_tk_vt":
                        {
                            if (values[0] != DependencyProperty.UnsetValue)
                            {
                                string dvt = values[0].ToString().Trim();
                                if (dvt != string.Empty)
                                    IsEnabled = true;
                            }
                        }
                        break;
                    case "gia_ton":
                    case "tk_dtnb":
                    case "tk_cl_vt":
                    case "tk_ck":
                    case "tk_nvl":
                    case "tk_spdd":
                    case "sl":
                        {
                            if (values[0] != DependencyProperty.UnsetValue)
                            {
                                string dvt = values[0].ToString().Trim();
                                int vt_ton_kho = 0;
                                int.TryParse(values[1].ToString(), out vt_ton_kho);
                                if (dvt != string.Empty && vt_ton_kho != 0)
                                    IsEnabled = true;
                            }
                            
                        }
                        break;
                    case "tk_vt":
                        {
                            if (values[0] != DependencyProperty.UnsetValue)
                            {
                                string vt_ton_kho = values[0].ToString().Trim();
                                if (vt_ton_kho == "0")
                                    IsEnabled = true;
                            }
                        } break;
                }
            }
            return IsEnabled;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
