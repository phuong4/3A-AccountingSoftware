using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
using System.Data;

namespace TTSODMHDB
{
    public class BindingReadonly : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (FrmPoctpna.IsInEditMode != null)
                if (FrmPoctpna.IsInEditMode.Value)
                    if (values != null)
                    {
                        string[] paraStr = parameter.ToString().Trim().Split(';');
                        switch (paraStr[0].ToUpper())
                        {
                            #region PH
                            case "PH":
                                {
                                    bool isInEditMode = false;
                                    bool.TryParse(values[0].ToString(), out isInEditMode);

                                    switch (paraStr[1])
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
                                }
                                break;
                            #endregion
                            #region CT
                            case "CT":
                                {
                                    switch (paraStr[1])
                                    {
                                        #region gia_nt2
                                        case "gia_nt2":
                                            {
                                                decimal so_luong = 0;
                                                decimal.TryParse(values[0].ToString(), out so_luong);
                                                if (so_luong != 0)
                                                    return false;
                                            }
                                            break;
                                        #endregion

                                        #region tien_nt2
                                        case "tien_nt2":
                                            {
                                                decimal so_luong = 0, gia_nt2 = 0;
                                                bool sua_tien = false;
                                                bool.TryParse(values[2].ToString(), out sua_tien);

                                                decimal.TryParse(values[0].ToString(), out so_luong);
                                                decimal.TryParse(values[1].ToString(), out gia_nt2);

                                                if (so_luong == 0 || (so_luong * gia_nt2) == 0 || sua_tien == true)
                                                    return false;
                                            }
                                            break;
                                        #endregion

                                        #region tl_ck
                                        case "tl_ck":
                                            {
                                                decimal tien_nt2 = 0;
                                              
                                                decimal.TryParse(values[0].ToString(), out tien_nt2);
                                               
                                                if (tien_nt2 != 0)
                                                    return false;
                                            }
                                            break;
                                        #endregion

                                        #region ck_nt
                                        case "ck_nt":
                                            {
                                                decimal tien_nt2 = 0, tl_ck = 0;
                                                bool sua_tien = false;
                                                bool.TryParse(values[2].ToString(), out sua_tien);

                                                decimal.TryParse(values[0].ToString(), out tien_nt2);
                                                decimal.TryParse(values[1].ToString(), out tl_ck);

                                                if (tien_nt2 * tl_ck != 0 || sua_tien == true)
                                                    return false;
                                            
                                            }
                                            break;
                                        #endregion

                                        #region Thue_nt
                                        case "thue_nt":
                                            {
                                                //decimal tien_nt2 = 0, thue_suat = 0;
                                                //bool sua_tien = false;
                                                //bool.TryParse(values[2].ToString(), out sua_tien);

                                                //decimal.TryParse(values[0].ToString(), out tien_nt2);
                                                //decimal.TryParse(values[1].ToString(), out thue_suat);
                                             
                                                //if (sua_tien == true || (tien_nt2 * (thue_suat / 100)) == 0)
                                                    return false;
                                            }
                                            break;
                                        #endregion

                                        #region Gia_nt
                                        case "gia_nt":
                                            {
                                                decimal so_luong = 0;
                                                decimal.TryParse(values[0].ToString(), out so_luong);
                                                if (so_luong != 0)
                                                    return false;
                                            }
                                            break;
                                        //decimal so_luong = 0, gia_nt2 = 0, ty_gia = 0;
                                        //bool sua_tien = false;
                                        //bool.TryParse(values[2].ToString(), out sua_tien);

                                        //decimal.TryParse(values[0].ToString(), out so_luong);
                                        //decimal.TryParse(values[1].ToString(), out gia_nt2);
                                        //decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"].ToString(), out ty_gia);

                                        //if (so_luong != 0 && ((gia_nt2 * ty_gia) == 0 || sua_tien == true))
                                        //    return false;


                                        #endregion

                                        #region tien_nt
                                        case "tien_nt":
                                            {
                                                decimal so_luong = 0, gia_nt = 0;
                                                bool sua_tien = false;
                                                bool.TryParse(values[2].ToString(), out sua_tien);

                                                decimal.TryParse(values[0].ToString(), out so_luong);
                                                decimal.TryParse(values[1].ToString(), out gia_nt);

                                                if (so_luong == 0 || (so_luong * gia_nt) == 0 || sua_tien == true)
                                                    return false;
                                            }
                                            break;
                                        #endregion

                                        #region Gia2
                                        case "gia2":
                                            {
                                                decimal gia_nt2 = 0, ty_gia = 0;
                                                bool sua_tien = false;

                                                decimal.TryParse(values[0].ToString(), out gia_nt2);
                                                bool.TryParse(values[1].ToString(), out sua_tien);
                                                decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"].ToString(), out ty_gia);

                                                if (sua_tien == true || (gia_nt2 * ty_gia) == 0)
                                                    return false;
                                            }
                                            break;
                                        #endregion

                                        #region Tien2
                                        case "tien2":
                                            {
                                                decimal tien_nt2 = 0, ty_gia = 0;
                                                bool sua_tien = false;

                                                decimal.TryParse(values[0].ToString(), out tien_nt2);
                                                bool.TryParse(values[1].ToString(), out sua_tien);
                                                decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"].ToString(), out ty_gia);

                                                if (sua_tien == true || (tien_nt2 * ty_gia) == 0)
                                                    return false;
                                            }
                                            break;
                                        #endregion

                                        #region Thue
                                        case "thue":
                                            {
                                                //decimal tien2 = 0, thue_suat = 0;
                                                //bool sua_tien = false;
                                                //bool.TryParse(values[2].ToString(), out sua_tien);

                                                //decimal.TryParse(values[0].ToString(), out tien2);
                                                //decimal.TryParse(values[1].ToString(), out thue_suat);

                                                //if (sua_tien == true || (tien2 * (thue_suat / 100)) == 0)
                                                    return false;
                                            }
                                            break;
                                        #endregion

                                        #region ck
                                        case "ck":
                                            {
                                                decimal tien2 = 0, tl_ck = 0;
                                                bool sua_tien = false;
                                                bool.TryParse(values[2].ToString(), out sua_tien);

                                                decimal.TryParse(values[0].ToString(), out tien2);
                                                decimal.TryParse(values[1].ToString(), out tl_ck);

                                                if ((tien2 * tl_ck) != 0 || sua_tien == true)
                                                    return false;
                                            }
                                            break;
                                        #endregion

                                        #region Gia
                                        case "gia":
                                            {
                                                decimal so_luong = 0;
                                                decimal.TryParse(values[0].ToString(), out so_luong);
                                                bool sua_tien = false;
                                                bool.TryParse(values[1].ToString(), out sua_tien);
                                                if (sua_tien == true)
                                                    return false;
                                            }
                                            break;
                                        //decimal so_luong = 0, gia_nt2 = 0, ty_gia = 0;
                                        //bool sua_tien = false;
                                        //bool.TryParse(values[2].ToString(), out sua_tien);

                                        //decimal.TryParse(values[0].ToString(), out so_luong);
                                        //decimal.TryParse(values[1].ToString(), out gia_nt2);
                                        //decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"].ToString(), out ty_gia);

                                        //if (so_luong != 0 && ((gia_nt2 * ty_gia) == 0 || sua_tien == true))
                                        //    return false;


                                        #endregion

                                        #region tien
                                        case "tien":
                                            {
                                                decimal so_luong = 0, gia = 0;
                                                bool sua_tien = false;
                                                bool.TryParse(values[2].ToString(), out sua_tien);

                                                decimal.TryParse(values[0].ToString(), out so_luong);
                                                decimal.TryParse(values[1].ToString(), out gia);

                                                if (so_luong == 0 || (so_luong * gia) == 0 || sua_tien == true)
                                                    return false;
                                            }
                                            break;
                                        #endregion
                                    }
                                }
                                break;
                            #endregion
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

    public class IdRecordConverterXamDataGrid : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((int)value + 1);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

    }

    public class BindingXamDataGridIdField : IValueConverter
    {
        #region IValueConverter Members
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return System.Convert.ToInt32(value) + 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        #endregion
    }

    public class BindingStatusVoucher : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!string.IsNullOrEmpty(value.ToString().Trim()))
                return StartUp.tbStatus.Rows.IndexOf(StartUp.tbStatus.Select("Ma_post =" + value)[0] as DataRow);
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            if (value != null)
            {
                int select_index = -1;
                int.TryParse(value.ToString(), out select_index);
                if (select_index != -1)
                    return StartUp.tbStatus.Rows[select_index]["ma_post"];
            }
            return StartUp.DmctInfo["ma_post"];
        }

        #endregion
    }

    public class BindingTextStatus : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((bool)value == true)
                return "Xử lý";
            else
                return "Trạng thái";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class BindingReadOnly : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isreadonly = true;
            if (parameter != null)
            {
                string paraStr = parameter.ToString();
                switch (paraStr)
                {
                    case "gia_nt0":
                        {
                            decimal so_luong = 0;
                            decimal.TryParse(values[0].ToString(), out so_luong);
                            if (so_luong != 0)
                                isreadonly = false;
                        }
                        break;
                    case "tien_nt0":
                        {
                            decimal so_luong = 0, gia_nt0 = 0;
                            bool sua_tien = false;
                            bool.TryParse(values[2].ToString(), out sua_tien);

                            decimal.TryParse(values[0].ToString(), out so_luong);
                            decimal.TryParse(values[1].ToString(), out gia_nt0);

                            if (so_luong == 0 || (so_luong * gia_nt0) == 0 || sua_tien == true)
                                isreadonly = false;
                        }
                        break;
                    case "gia0":
                        {
                            decimal so_luong = 0, gia_nt0 = 0, ty_gia = 0;
                            bool sua_tien = false;
                            bool.TryParse(values[2].ToString(), out sua_tien);

                            decimal.TryParse(values[0].ToString(), out so_luong);
                            decimal.TryParse(values[1].ToString(), out gia_nt0);
                            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"].ToString(), out ty_gia);

                            if (so_luong != 0 && ((gia_nt0 * ty_gia) == 0 || sua_tien == true))
                                isreadonly = false;
                        }
                        break;
                    case "tien0":
                        {
                            decimal tien_nt0 = 0, ty_gia = 0;
                            bool sua_tien = false;

                            decimal.TryParse(values[0].ToString(), out tien_nt0);
                            bool.TryParse(values[1].ToString(), out sua_tien);
                            decimal.TryParse(StartUp.DsTrans.Tables[0].DefaultView[0]["ty_gia"].ToString(), out ty_gia);

                            if (sua_tien == true || (tien_nt0 * ty_gia) == 0)
                                isreadonly = false;
                        }
                        break;
                    case "tk_vt":
                        {
                            decimal sua_tk_vt = 0;
                            decimal.TryParse(values[0].ToString(), out sua_tk_vt);
                            bool isInEditMode = false;
                            bool.TryParse(values[1].ToString(), out isInEditMode);

                            if (sua_tk_vt == 1 && isInEditMode == true)
                                isreadonly = false;
                        }
                        break;

                    //Grid Chi Phi
                    case "cp_nt":
                        {
                            isreadonly = false;
                            break;
                        }
                    ////cp trong tab chi phí
                    case "cp":
                        {
                            decimal cp_nt = 0;
                            bool sua_tien = false;
                            decimal.TryParse(values[0].ToString(), out cp_nt);
                            bool.TryParse(values[1].ToString(), out sua_tien);
                            if (sua_tien == true || cp_nt == 0)
                                isreadonly = false;
                            break;
                        }

                    //cục thuế trong grd HD thuế
                    case "ma_kh2":
                        {
                            decimal tk_cn = 0;
                            decimal.TryParse(values[0].ToString(), out tk_cn);
                            if (tk_cn == 1)
                                isreadonly = false;
                            break;
                        }
                }
            }

            return isreadonly;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
