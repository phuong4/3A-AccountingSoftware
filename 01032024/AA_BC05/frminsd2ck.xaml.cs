using Infragistics.Windows.Editors;
using Sm.Windows.Controls;
using SmErrorLib;
using SmLib;
using SmReport;
using SysLib;
using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace AA_BC05
{
	public partial class FrmAA_BC05 : FormFilter
	{
		public FrmAA_BC05()
		{
			this.InitializeComponent();
			base.BindingSysObj = StartupBase.SysObj;
			SysFunc.LoadIcon(this);
		}

		private void ConfirmGridView_OnCancel(object sender, RoutedEventArgs e)
		{
			base.Close();
		}

		private void ConfirmGridView_OnOk(object sender, RoutedEventArgs e)
		{
			try
			{
				if (this.validateInput())
				{
					if (Keyboard.FocusedElement.GetType().Equals(typeof(TextBoxAutoComplete)))
					{
						TextBoxAutoComplete focusedElement = Keyboard.FocusedElement as TextBoxAutoComplete;
						if (focusedElement.ParentControl != null)
						{
							if (!focusedElement.ParentControl.CheckLostFocus())
							{
								return;
							}
						}
					}
					StartUp.dsReport.Tables.Clear();
					DataTable dataTable = new DataTable("tbInfo");
					dataTable.Columns.Add("DateTime");
					dataTable.Columns.Add("Ma_Kho");
					dataTable.Columns.Add("Ten_Kho");
					DataRowCollection rows = dataTable.Rows;
					object[] text = new object[] { this.txtNgay.Text, this.txtMaKho.Text.ToString(), this.tblTenKho.Text.ToString() };
					rows.Add(text);
					StartUp.dsReport.Tables.Add(dataTable);
					string filter = this.getFilter();
					string condition = this.getCondition();
					base.Hide();
					StartUp.CallGridAA_BC05(true, this.txtNgay1.Value, this.txtNgay.Value, this.txtMaKho.Text.Trim(), filter, condition, int.Parse(this.cbMau_bc.Value.ToString()));
				}
			}
			catch (Exception exception)
			{
				ErrorLog.CatchMessage(exception);
			}
		}

		public string ConvertDataToSql(object value, Type ValueType)
		{
			string str;
			string str1 = "";
			string str2 = ValueType.ToString();
			if (str2 != null)
			{
				if (str2 == "System.String")
				{
					str1 = string.Format("'{0}'", (value as string).Replace("'", "'"));
					str = str1;
					return str;
				}
				else
				{
					if (str2 != "System.DateTime")
					{
						str1 = string.Format("'{0}'", value);
						str = str1;
						return str;
					}
					DateTime dateTime = (DateTime)value;
					str1 = string.Format("'{0}'", dateTime.ToString("yyyyMMdd"));
					str = str1;
					return str;
				}
			}
			str1 = string.Format("'{0}'", value);
			str = str1;
			return str;
		}

		private void formFilter_Loaded(object sender, RoutedEventArgs e)
		{
			string str;
			string str1;
			this.txtMaVT.SearchInit();
			this.txtMaKho.SearchInit();
			TextBlock textBlock = this.tblTenVT;
			if (this.txtMaVT.RowResult == null)
			{
				str = "";
			}
			else
			{
				str = (StartupBase.SysObj.GetOption("M_LAN").ToString() == "V" ? this.txtMaVT.RowResult["ten_vt"].ToString() : this.txtMaVT.RowResult["ten_vt2"].ToString());
			}
			textBlock.Text = str;
			TextBlock textBlock1 = this.tblTenKho;
			if (this.txtMaKho.RowResult == null)
			{
				str1 = "";
			}
			else
			{
				str1 = (StartupBase.SysObj.GetOption("M_LAN").ToString() == "V" ? this.txtMaKho.RowResult["ten_kho"].ToString() : this.txtMaKho.RowResult["ten_kho2"].ToString());
			}
			textBlock1.Text = str1;
			this.txtNgay1.Focus();
			this.GridSearch.SysObj = StartupBase.SysObj;
			this.GridSearch.tableList = StartUp.tableList;
		}

		public string getCondition()
		{
			string str = " 1=1 ";
			if (!string.IsNullOrEmpty(this.txtMaKho.Text.ToString()))
			{
				str = string.Concat(str, " and ma_kho = '", this.txtMaKho.Text.ToString().Trim(), "'");
			}
			if (!string.IsNullOrEmpty(this.txtMaVT.Text.ToString()))
			{
				str = string.Concat(str, " and ma_vt = '", this.txtMaVT.Text.ToString().Trim(), "'");
			}
			if (!string.IsNullOrEmpty(this.txtMaDVCS.Text.Trim()))
			{
				str = string.Concat(str, " and ma_kho IN ( SELECT ma_kho FROM dmkho where ma_dvcs like  '", this.txtMaDVCS.Text.Trim(), "%')");
			}
			return str;
		}

		public string getFilter()
		{
			string str = " 1=1 ";
			this.GridSearch._GenerateSQLString();
			if (this.GridSearch.arrStrFilter != null)
			{
				if (!string.IsNullOrEmpty(this.GridSearch.arrStrFilter[0]))
				{
					str = string.Concat(str, " and ", this.GridSearch.arrStrFilter[0]);
				}
			}
			return str;
		}

		private void txtMaKho_LostFocus(object sender, RoutedEventArgs e)
		{
			if (this.txtMaKho.RowResult != null)
			{
				this.tblTenKho.Text = (StartupBase.M_LAN.Equals("V") ? this.txtMaKho.RowResult["ten_kho"].ToString() : this.txtMaKho.RowResult["ten_kho2"].ToString());
			}
			else
			{
				this.tblTenKho.Text = "";
			}
		}

		private void txtMaVT_LostFocus(object sender, RoutedEventArgs e)
		{
			if (this.txtMaVT.RowResult != null)
			{
				this.tblTenVT.Text = (StartupBase.M_LAN.Equals("V") ? this.txtMaVT.RowResult["ten_vt"].ToString() : this.txtMaVT.RowResult["ten_vt2"].ToString());
			}
			else
			{
				this.tblTenVT.Text = "";
			}
		}

		public bool validateInput()
		{
			bool flag;
			if (!(this.txtNgay.Value == null ? false : !string.IsNullOrEmpty(this.txtNgay.Value.ToString())))
			{
				ExMessageBox.Show(1845, StartupBase.SysObj, "Ngày lọc chứng từ không hợp lệ!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Asterisk);
				this.txtNgay.Focus();
				flag = false;
			}
			else if (!this.txtNgay.IsValueValid)
			{
				ExMessageBox.Show(1850, StartupBase.SysObj, "Ngày lọc chứng từ không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Asterisk);
				this.txtNgay.Focus();
				flag = false;
			}
			//else if (!this.txtMaKho.CheckLostFocus())
			//{
			//	ExMessageBox.Show(1860, StartupBase.SysObj, "Mã kho không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Asterisk);
			//	this.txtMaKho.IsFocus = true;
			//	flag = false;
			//}
			//else if (string.IsNullOrEmpty(this.txtMaKho.Text.Trim()))
			//{
			//	ExMessageBox.Show(1861, StartupBase.SysObj, "Chưa nhập mã kho!", "", MessageBoxButton.OK, MessageBoxImage.Asterisk);
			//	this.txtMaKho.IsFocus = true;
			//	flag = false;
			//}
			else if (this.txtMaVT.CheckLostFocus())
			{
				flag = true;
			}
			else
			{
				ExMessageBox.Show(1865, StartupBase.SysObj, "Mã vật tư không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Asterisk);
				this.txtMaVT.IsFocus = true;
				flag = false;
			}
			return flag;
		}

        private void txtMaPX_LostFocus(object sender, RoutedEventArgs e)
        {
            if (this.txtMapx.RowResult != null)
            {
                this.tblTenpx.Text = (StartupBase.M_LAN.Equals("V") ? this.txtMapx.RowResult["ten_px"].ToString() : this.txtMapx.RowResult["ten_px2"].ToString());
            }
            else
            {
                this.tblTenpx.Text = "";
            }
        }
    }
}