using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.DataPresenter.Events;
using Infragistics.Windows.Editors;
using Sm.Windows.Controls;
using SmDefine;
using SmErrorLib;
using SmLib;
using SmLib.SM.FormBrowse;
using SmLib.SM.SMFormBrowse;
using SmReport;
using SmVoucherLib;
using SysLib;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AA_BC05
{
	internal class StartUp : StartupBase
	{
		public static DataSet dsReport;

		public static FormBrowse oBrowse;

		public static FrmAA_BC05 _frmAA_BC05;

		public static DataRow commandInfo;

		public static DateTime M_ngay_ct0;

		public static string M_ma_nt0;

		public static int kindStyleReport;

		public static string tableList;

		private static SqlCommand cmd;

		public static DataTable tbDetail;

        public static string Ws_Id;

        public static DateTime zStartdate;
        public static DateTime zEndDate;

        public static DateTime ngay_ct;

		public static DateTime M_NGAY_KS;

		static StartUp()
		{
			StartUp.dsReport = new DataSet();
			StartUp.kindStyleReport = -1;
			StartUp.tableList = "v_dmvt";
			StartUp.cmd = new SqlCommand();
			StartUp.tbDetail = null;
		}

		public StartUp()
		{
		}

		public static void CallGridAA_BC05(bool isFirstLoad, object Startdate, object EndDate, string ma_kho, string strFilter, string strCondition, int kindReport)
		{
			if (!isFirstLoad)
			{
				StartUp.dsReport.Tables.Remove("tbDetail");
				StartUp.tbDetail = StartupBase.SysObj.ExcuteReader(StartUp.cmd).Tables[0].Copy();
				StartUp.tbDetail.TableName = "tbDetail";
				StartUp.oBrowse.frmBrw.oBrowse.DataSource = StartUp.tbDetail.DefaultView;
				StartUp.dsReport.Tables.Add(StartUp.tbDetail);
				StartUp.oBrowse.frmBrw.oBrowse.FieldLayouts[0].SummaryDefinitions.Clear();
				StartUp.oBrowse.UpdateSumaryFields();
			}
			else
			{
				StartUp.ngay_ct = (DateTime)EndDate;
                StartUp.zStartdate = (DateTime)Startdate;
                StartUp.zEndDate = (DateTime)EndDate;
                StartUp.cmd.CommandText = string.Concat("Exec ", StartUp.commandInfo["store_proc"], " @Ma_kho,@Startdate, @EndDate, @Filter, @Condition");
                StartUp.cmd.Parameters.Add("@Startdate", SqlDbType.VarChar).Value = (string.IsNullOrEmpty(Startdate.ToString()) ? "" : string.Format("{0:yyyyMMdd}", (DateTime)Startdate));
                StartUp.cmd.Parameters.Add("@EndDate", SqlDbType.VarChar).Value = (string.IsNullOrEmpty(EndDate.ToString()) ? "" : string.Format("{0:yyyyMMdd}", (DateTime)EndDate));
				StartUp.cmd.Parameters.Add("@Ma_kho", SqlDbType.VarChar).Value = ma_kho;
				StartUp.cmd.Parameters.Add("@Filter", SqlDbType.NVarChar).Value = strFilter;
				StartUp.cmd.Parameters.Add("@Condition", SqlDbType.NVarChar).Value = strCondition;
				StartUp.tbDetail = StartupBase.SysObj.ExcuteReader(StartUp.cmd).Tables[0].Copy();
				StartUp.tbDetail.TableName = "tbDetail";
				StartUp.dsReport.Tables.Add(StartUp.tbDetail);
				StartUp.kindStyleReport = kindReport;
				StartUp.oBrowse = new FormBrowse(StartupBase.SysObj, StartUp.tbDetail.DefaultView, StartUp.fieldShow(StartUp.kindStyleReport));
				StartUp.oBrowse.Esc += new FormBrowse.GridKeyUp_Esc(StartUp.oBrowse_Esc);
				StartUp.oBrowse.F3 += new FormBrowse.GridKeyUp_F3(StartUp.oBrowse_F3);
				StartUp.oBrowse.F5 += new FormBrowse.GridKeyUp_F5(StartUp.oBrowse_F5);
				StartUp.oBrowse.F7 += new FormBrowse.GridKeyUp_F7(StartUp.oBrowse_F7);
				StartUp.oBrowse.DataGrid.Loaded += new RoutedEventHandler(StartUp.DataGrid_Loaded);
				StartUp.oBrowse.CTRL_R += new FormBrowse.GridKeyUp_CTRL_R(StartUp.oBrowse_CTRL_R);
				StartUp.oBrowse.frmBrw.oBrowse.FieldSettings.AllowEdit = new bool?(false);
				StartUp.oBrowse.frmBrw.Title = (StartupBase.M_LAN.Equals("V") ? SysFunc.Cat_Dau(StartUp.commandInfo["bar"].ToString()) : SysFunc.Cat_Dau(StartUp.commandInfo["bar2"].ToString()));
				ToolBar toolBar = StartUp.oBrowse.frmBrw.ToolBar.FindName("tbReport") as ToolBar;
				if (toolBar != null)
				{
					(toolBar.FindName("btnEdit") as Sm.Windows.Controls.ToolBarButton).Text = "Tạo phiếu nhập";
					(toolBar.FindName("btnDetail") as Sm.Windows.Controls.ToolBarButton).Text = "Tạo phiếu xuất";
					(toolBar.FindName("btnDetail") as Sm.Windows.Controls.ToolBarButton).ImagePath = "Images\\Edit.png";
				}
			}
			StartUp.oBrowse.SetRowColorByTag("bold", "1", Colors.Blue);
			if (isFirstLoad)
			{
				StartUp.oBrowse.frmBrw.LanguageID = "AA_BC05_1";
				StartUp.oBrowse.ShowDialog();
				StartUp._frmAA_BC05.Close();
			}
		}

		private static void DataGrid_Loaded(object sender, RoutedEventArgs e)
		{
			BasicGridView basicGridView = (BasicGridView)sender;
			basicGridView.RecordsInViewChanged += new EventHandler<RecordsInViewChangedEventArgs>((object s, RecordsInViewChangedEventArgs args) => {
				for (int i = 0; i < basicGridView.Records.Count; i++)
				{
					DataRecord item = basicGridView.Records[i] as DataRecord;
					if (item != null)
					{
						RecordPresenter normal = RecordPresenter.FromRecord(item);
						if ((normal == null ? false : item.RecordType == RecordType.DataRecord))
						{
							if (!((item.DataItem as DataRowView)["bold"].ToString().Trim() == "1"))
							{
								normal.FontWeight = FontWeights.Normal;
							}
							else
							{
								normal.FontWeight = FontWeights.Bold;
							}
						}
					}
				}
			});
			basicGridView.Loaded -= new RoutedEventHandler(StartUp.DataGrid_Loaded);
		}

		public static string fieldShow(int kindReport)
		{
			string str;
			string empty = string.Empty;
			string upper = StartupBase.SysObj.GetOption("M_LAN").ToString().ToUpper();
			string str1 = upper;
			if (str1 != null)
			{
				if (str1 != "V")
				{
					switch (kindReport)
					{
						case 0:
						{
							empty = StartUp.commandInfo["Ebrowse1"].ToString();
							break;
						}
						case 1:
						{
							empty = StartUp.commandInfo["Ebrowse2"].ToString();
							break;
						}
					}
					str = empty;
					return str;
				}
				switch (kindReport)
				{
					case 0:
					{
						empty = StartUp.commandInfo["Vbrowse1"].ToString();
						break;
					}
					case 1:
					{
						empty = StartUp.commandInfo["Vbrowse2"].ToString();
						break;
					}
				}
				str = empty;
				return str;
			}
			switch (kindReport)
			{
				case 0:
				{
					empty = StartUp.commandInfo["Ebrowse1"].ToString();
					break;
				}
				case 1:
				{
					empty = StartUp.commandInfo["Ebrowse2"].ToString();
					break;
				}
			}
			str = empty;
			return str;
		}

		private static void oBrowse_CTRL_R(object sender, EventArgs e)
		{
			StartUp.CallGridAA_BC05(false, StartUp._frmAA_BC05.txtNgay1.Value, StartUp._frmAA_BC05.txtNgay.Value, StartUp._frmAA_BC05.txtMaKho.Text.Trim(), StartUp._frmAA_BC05.getFilter(), StartUp._frmAA_BC05.getCondition(), StartUp.kindStyleReport);
		}

		public static void oBrowse_Esc(object sender, EventArgs e)
		{
		}

		private static void oBrowse_F3(object sender, EventArgs e)
		{

            SqlCommand cmd = new SqlCommand("exec [dbo].[HTQ_create_PND] @Startdate, @EndDate");
            cmd.Parameters.Add("@Startdate", SqlDbType.VarChar).Value = (string.IsNullOrEmpty(StartUp.zStartdate.ToString()) ? "" : string.Format("{0:yyyyMMdd}", (DateTime)StartUp.zStartdate));
            cmd.Parameters.Add("@EndDate", SqlDbType.VarChar).Value = (string.IsNullOrEmpty(StartUp.zEndDate.ToString()) ? "" : string.Format("{0:yyyyMMdd}", (DateTime)StartUp.zEndDate));
            //StartUp.SysObj.ExcuteReader(cmd);
            StartUp.SysObj.ExcuteNonQuery(cmd);

            ExMessageBox.Show(9391, StartupBase.SysObj, "Tạo phiếu nhập kho. Thành công!", "", MessageBoxButton.OK, MessageBoxImage.Asterisk);

		}

		private static void oBrowse_F5(object sender, EventArgs e)
		{
            SqlCommand cmd = new SqlCommand("exec [dbo].[HTQ_create_PXD] @Startdate, @EndDate");
            cmd.Parameters.Add("@Startdate", SqlDbType.VarChar).Value = (string.IsNullOrEmpty(StartUp.zStartdate.ToString()) ? "" : string.Format("{0:yyyyMMdd}", (DateTime)StartUp.zStartdate));
            cmd.Parameters.Add("@EndDate", SqlDbType.VarChar).Value = (string.IsNullOrEmpty(StartUp.zEndDate.ToString()) ? "" : string.Format("{0:yyyyMMdd}", (DateTime)StartUp.zEndDate));
            //StartUp.SysObj.ExcuteReader(cmd);
            StartUp.SysObj.ExcuteNonQuery(cmd);

            ExMessageBox.Show(9392, StartupBase.SysObj, "Tạo phiếu xuất kho. Thành công!", "", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

		public static void oBrowse_F7(object sender, EventArgs e)
		{
			ReportManager reportManager = new ReportManager(StartupBase.SysObj, StartUp.commandInfo["rep_file"].ToString(), StartUp.kindStyleReport);
			SysFunc.DSCopyWithFilter(StartUp.oBrowse.frmBrw.oBrowse, ref StartUp.dsReport, "tbDetail");
			reportManager.Preview(StartUp.dsReport);
			SysFunc.ResetFilter(ref StartUp.dsReport, "tbDetail");
		}

		public override void Run()
		{
			StartupBase.Namespace = "AA_BC05";
			try
			{
				StartUp.M_ngay_ct0 = (DateTime)StartupBase.SysObj.GetSysVar("M_NGAY_KY1");
				StartUp.M_NGAY_KS = (DateTime)StartupBase.SysObj.GetSysVar("M_NGAY_KS");
				StartUp.M_ma_nt0 = StartupBase.SysObj.GetOption("M_MA_NT0").ToString();
				StartUp.Ws_Id = StartupBase.SysObj.GetOption("M_WS_ID").ToString();
				StartUp.commandInfo = SysFunc.GetCommandInfo(StartupBase.SysObj, StartupBase.Menu_Id);
				if (StartUp.commandInfo != null)
				{
					StartUp._frmAA_BC05 = new FrmAA_BC05()
					{
						Title = (StartupBase.M_LAN.Equals("V") ? SysFunc.Cat_Dau(StartUp.commandInfo["bar"].ToString()) : SysFunc.Cat_Dau(StartUp.commandInfo["bar2"].ToString()))
					};
					StartUp._frmAA_BC05.ShowDialog();
				}
			}
			catch (Exception exception)
			{
				ErrorLog.CatchMessage(exception);
			}
		}
	}
}