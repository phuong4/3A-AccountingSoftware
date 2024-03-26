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
using SmReport;
using System.Data;

namespace SOTH1
{
    /// <summary>
    /// Interaction logic for Arcd1F10.xaml
    /// </summary>
    public partial class SOTH1F10 : FormFilter
    {
        public DataTable dtOption;

        public DataTable DataOption
        {
            get
            {
                if (dtOption != null)
                    return dtOption;

                dtOption = new DataTable("F10");
                dtOption.Columns.Add("group1", typeof(string));
                dtOption.Columns.Add("group2", typeof(string));
                dtOption.Columns.Add("group3", typeof(string));
                dtOption.Columns.Add("sortby", typeof(string));

                dtOption.Columns.Add("ps_no1", typeof(double));
                dtOption.Columns.Add("ps_no2", typeof(double));
                dtOption.Columns.Add("ps_co1", typeof(double));
                dtOption.Columns.Add("ps_co2", typeof(double));

                dtOption.Columns.Add("no_ck1", typeof(double));
                dtOption.Columns.Add("no_ck2", typeof(double));
                dtOption.Columns.Add("co_ck1", typeof(double));
                dtOption.Columns.Add("co_ck2", typeof(double));

                dtOption.Rows.Add(new object[] { "0", "0", "0", //Nhóm theo 1, 2, 3
                    "1", //Sắp xếp theo
                    0, 0, //Phát sinh nợ từ ... đến ...
                    0, 0, //Phát sinh có từ ... đến ...
                    0, 0, //Dư nợ cuối kỳ từ ... đến ...
                    0, 0 //Dư có cuối kỳ từ ... đến ...
                });
                return dtOption;
            }
            set { dtOption = value; }
        }


        private bool isOK = false;
        //public FrmARSD1DKAF10()
        //{
        //    InitializeComponent();
        //}

        public SOTH1F10(DataTable GroupSelectedTable)
        {
            if (GroupSelectedTable != null)
                dtOption = GroupSelectedTable.Copy();
             
            InitializeComponent();
            DataContext = DataOption;
        }
 
        private void ConfirmGridView_OnOk(object sender, RoutedEventArgs e)
        {
            isOK = true;
            Close();
        }

        public new bool ShowDialog()
        {
            base.ShowDialog();
            return isOK;
        }

        private void ConfirmGridView_Loaded(object sender, RoutedEventArgs e)
        {
            txtGrp1.Focus();
        }

        private void FormFilter_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

    }

    public class GroupValidConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string[] arr = {"1", "2", "3"};
            string sReturn = "";
            if (values[0] != DependencyProperty.UnsetValue && values[1] != DependencyProperty.UnsetValue && values[0] != null && values[1] != null)
            {
                IEnumerable<string> arrResult = arr.Except(new string[] { values[0].ToString(), values[1].ToString() });
                foreach (string c in arrResult)
                {
                    sReturn += "," + c;
                }
            }
            sReturn = "0" + sReturn;
            return sReturn;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    public class GroupEnableConvert : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool result = true;
            foreach (object c in values)
            {
                if (c != null && c != DependencyProperty.UnsetValue)
                {
                    if (c.ToString() == "0")
                        result = false;
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
