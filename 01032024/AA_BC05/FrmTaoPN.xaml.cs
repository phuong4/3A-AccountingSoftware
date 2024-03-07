using Infragistics.Windows.Editors;
using Sm.Windows.Controls;
using SmLib;
using SysLib;
using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace AA_BC05
{
	public partial class FrmTaoPN : Form
	{
		public bool isOk = false;

		public int kind = 1;

		public string Ma_nt_ht = "";

		public string so_hd = "";

		public string ngay_hd = "";

		public string filterma_qs = "";

		public string ma_kho = "";

		public string ma_nx = "";

		public DataTable tbInfoPX = null;

		public FrmTaoPN()
		{
			this.InitializeComponent();
			SysFunc.LoadIcon(this);
			base.BindingSysObj = StartupBase.SysObj;
			this.GrdOkCancel.pnlButton.btnCancel.Visibility = System.Windows.Visibility.Collapsed;
		}

		private void ConfirmGridView_OnOk(object sender, RoutedEventArgs e)
		{
			if (!(this.txtNgay.Value == null ? false : !string.IsNullOrEmpty(this.txtNgay.Value.ToString())))
			{
				ExMessageBox.Show(571, StartupBase.SysObj, "Ngày chứng từ không hợp lệ!", "Fast Book 11 .NET", MessageBoxButton.OK, MessageBoxImage.Asterisk);
				this.txtNgay.Focus();
			}
			else if (!this.txtNgay.IsValueValid)
			{
				ExMessageBox.Show(572, StartupBase.SysObj, "Ngày chứng từ không hợp lệ!", "", MessageBoxButton.OK, MessageBoxImage.Asterisk);
				this.txtNgay.Focus();
			}
			else if (this.txtNgay.dValue < StartUp.M_NGAY_KS)
			{
				ExMessageBox.Show(573, StartupBase.SysObj, "Ngày chứng từ phải sau ngày khóa sổ!", "", MessageBoxButton.OK, MessageBoxImage.Asterisk);
				this.txtNgay.Focus();
			}
			else if (string.IsNullOrEmpty(this.txtMa_qs_px.Text.Trim()))
			{
				ExMessageBox.Show(597, StartupBase.SysObj, "Chưa vào mã quyển sổ phiếu nhập", "", MessageBoxButton.OK, MessageBoxImage.Asterisk);
				this.txtMa_qs_px.IsFocus = true;
			}
			else if (string.IsNullOrEmpty(this.txtso_ct_px.Text.Trim()))
			{
				ExMessageBox.Show(598, StartupBase.SysObj, "Chưa vào số chứng từ!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Asterisk);
				this.txtso_ct_px.Focus();
			}
			else if (string.IsNullOrEmpty(this.txtMa_kh.Text.Trim()))
			{
				ExMessageBox.Show(594, StartupBase.SysObj, "Chưa vào mã khách!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Asterisk);
				this.txtMa_kh.IsFocus = true;
			}
			else if (!string.IsNullOrEmpty(this.txtTk.Text.Trim()))
			{
				this.isOk = true;
				base.Close();
			}
			else
			{
				ExMessageBox.Show(592, StartupBase.SysObj, "Chưa vào tài khoản có!", "Xac nhan nhap lieu", MessageBoxButton.OK, MessageBoxImage.Asterisk);
				this.txtTk.IsFocus = true;
			}
		}

		private void Form_Loaded(object sender, RoutedEventArgs e)
		{
			bool flag;
			this.txtso_ct_px.MaxLength = base.BindingSysObj.GetDatabaseFieldLength("so_ct");
			this.txtDien_giai.MaxLength = base.BindingSysObj.GetDatabaseFieldLength("dien_giai");
			this.txtNgay.dValue = DateTime.Today;
			if (this.tbInfoPX == null)
			{
				flag = false;
			}
			else
			{
				flag = (this.tbInfoPX == null ? true : this.tbInfoPX.Rows.Count != 0);
			}
			if (flag)
			{
				this.txtMa_gd.Filter = "ma_ct = 'PND' and status = 1 and ma_gd IN ('9')";
				this.txtMa_gd.Text = this.tbInfoPX.Rows[0]["ma_gd"].ToString();
				this.txtMa_gd.SearchInit();
				this.txtMa_gd_PreviewLostFocus(this.txtMa_gd, null);
				this.txtMa_qs_px.Text = this.tbInfoPX.Rows[0]["ma_qs"].ToString();
				this.txtso_ct_px.Text = this.tbInfoPX.Rows[0]["so_ct"].ToString().Trim();
				this.txtDien_giai.Text = this.tbInfoPX.Rows[0]["dien_giai"].ToString();
			}
			else
			{
				SqlCommand sqlCommand = new SqlCommand()
				{
					CommandText = "if(select count(1) from dmqs where ma_cts LIKE '%{0}%') = 1"
				};
				SqlCommand sqlCommand1 = sqlCommand;
				sqlCommand1.CommandText = string.Concat(sqlCommand1.CommandText, " select ma_qs from dmqs where ma_cts LIKE '%{1}%'");
				this.txtMa_gd.Filter = "ma_ct = 'PND' and status = 1 and ma_gd IN ('9')";
				sqlCommand.CommandText = string.Format(sqlCommand.CommandText, "PND", "PND");
				this.txtMa_gd.Text = "1";
				this.txtMa_gd.SearchInit();
				this.txtMa_gd_PreviewLostFocus(this.txtMa_gd, null);
				DataSet dataSet = base.BindingSysObj.ExcuteReader(sqlCommand);
				if (dataSet.Tables.Count == 1)
				{
					this.txtMa_qs_px.Text = dataSet.Tables[0].Rows[0]["ma_qs"].ToString();
				}
			}
			this.txtMa_nt.Text = StartupBase.M_MA_NT0;
			if (this.Ma_nt_ht != StartupBase.M_MA_NT0)
			{
				AutoCompleteTextBox txtMaNt = this.txtMa_nt;
				string[] mMANT0 = new string[] { "ma_nt IN ('", StartupBase.M_MA_NT0, "','", this.Ma_nt_ht, "')" };
				txtMaNt.Filter = string.Concat(mMANT0);
			}
			this.txtNgay.Focus();
		}

		public string GetNewSoct(string ma_qs)
		{
			string str;
			string str1;
			if (Convert.ToInt16(base.BindingSysObj.GetOption("M_AUTO_SOCT").ToString()) == 1)
			{
				str = string.Concat("SELECT transform, so_ct + 1 as so_ct FROM dmqs WHERE ma_qs = '", ma_qs.Trim(), "'");
			}
			else
			{
				string str2 = string.Concat("EXEC  [GetNewSoct] '", ma_qs.Trim(), "'");
				str = str2;
				str = str2;
			}
			DataTable item = base.BindingSysObj.ExcuteReader(new SqlCommand(str)).Tables[0];
			if (item.Rows.Count > 0)
			{
				DataRow dataRow = item.Rows[0];
				if ((dataRow[1] == null ? false : dataRow[1] != DBNull.Value))
				{
					string str3 = dataRow[1].ToString();
					str1 = string.Format(dataRow[0].ToString(), Convert.ToDouble(str3));
					return str1;
				}
			}
			str1 = "";
			return str1;
		}

		private void txtMa_gd_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			if (this.txtMa_gd.RowResult != null)
			{
				this.txtTen_gd.Text = (StartupBase.M_LAN.Equals("V") ? this.txtMa_gd.RowResult["ten_gd"].ToString() : this.txtMa_gd.RowResult["ten_gd2"].ToString());
			}
		}

		private void txtMa_kh_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			if (this.txtMa_kh.RowResult != null)
			{
				this.txtTen_kh.Text = (StartupBase.M_LAN.Equals("V") ? this.txtMa_kh.RowResult["ten_kh"].ToString() : this.txtMa_kh.RowResult["ten_kh2"].ToString());
			}
		}

		private void txtMa_qs_px_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			if (e.NewFocus != this.GrdOkCancel.pnlButton.btnOk)
			{
				this.txtso_ct_px.Text = this.GetNewSoct(this.txtMa_qs_px.Text);
			}
		}

		private void txtTk_PreviewLostFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			if (this.txtTk.RowResult != null)
			{
				this.txtTen_tk.Text = (StartupBase.M_LAN.Equals("V") ? this.txtTk.RowResult["ten_tk"].ToString() : this.txtTk.RowResult["ten_tk2"].ToString());
			}
		}
	}
}