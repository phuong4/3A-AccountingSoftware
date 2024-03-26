using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace CACTPC1
{
    public class MaNt0Converter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Console.WriteLine("Vao ne");
            return StartUp.DsTrans.Tables[0].DefaultView[0]["ma_nt"].ToString() == StartUp.SysObj.GetOption("M_MA_NT0").ToString()? 
                "Visible":"Hidden";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class NotMaNt0Converter : IValueConverter
    {

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Console.WriteLine("Vao ne");
            return value.ToString().Trim().Equals(StartUp.SysObj.GetOption("M_MA_NT0").ToString().Trim()) ? "Hidden" : "Visible";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    public class CheckBoxConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int suatien = 0;
            int.TryParse(value.ToString(), out suatien);
            return suatien == 1 ? true : false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool ischecked = bool.Parse(value.ToString());
            return ischecked == true ? 1 : 0;
        }
    }
    public class BoolInverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    public class IsReadOnlyPh : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isInEditMode = false;
            bool.TryParse(value.ToString(), out isInEditMode);
            switch (parameter.ToString())
            {
                case "ong_ba":
                    if (isInEditMode && StartUp.M_ong_ba.Equals("1"))
                    {
                        return false;
                    }
                    break;
                case "ngay_lct":
                    if (isInEditMode && StartUp.M_ngay_lct.Equals("1"))
                    {
                        return false;
                    }
                    break;
            }
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }

    
}
