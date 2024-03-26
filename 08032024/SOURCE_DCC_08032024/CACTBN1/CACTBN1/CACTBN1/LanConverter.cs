using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace CACTBN1
{
    public class LanConverter:IMultiValueConverter
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

                        case "LANV":
                            {

                                // int.TryParse(values[0].ToString(), out ma_gd);
                                result = StartUp.M_LAN.Equals("V") ? Visibility.Visible : Visibility.Collapsed;

                            }
                            break;
                        case "LANE":
                            {

                                // int.TryParse(values[0].ToString(), out ma_gd);
                                result = StartUp.M_LAN.Equals("E") ? Visibility.Visible : Visibility.Collapsed;
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
