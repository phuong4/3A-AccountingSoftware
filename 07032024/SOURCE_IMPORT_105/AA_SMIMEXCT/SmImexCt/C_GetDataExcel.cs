using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using Sm.Windows.Controls;
using System.Windows;
using System.IO;
using System.Data.SqlClient;
using System.Runtime.InteropServices;

namespace AA_SMIMEXCT
{
    class C_GetDataExcel
    {
        public static string StrBrowse = "";
        public static string StrBrowseFieldNull = "";
        //private const int RF_PROCESSMESSAGE = 0xA123;
        //private const int RF_PROCESSWAITINGSHOW = 0xA126;
        //private const int RF_PROCESSWAITING = 0xA125;
       
        //[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        //public static extern int SendMessage(IntPtr hwnd, [MarshalAs(UnmanagedType.U4)] int Msg, IntPtr wParam, IntPtr lParam);

        public static DataSet GetData(ImportInfo info, string convertFont)
        {
            DataSet output = new DataSet();
            try
            {
                DataTable tbExcel = GetDataFromExcel(info.FileName, info.FieldNotNull);
                DataTable tbStrucImex = GetStructTable(info.TableTemplate);
                if (tbExcel == null || tbStrucImex == null)
                    return null;
                
                output.Tables.Add(tbExcel.Copy());
                output.Tables.Add(tbStrucImex.Copy());

                ConverDateTime(ref output);
                if (convertFont.Trim() == "1")
                    ConverFont(ref output);
            }
            catch (Exception ex)
            {
                if(StartUp.waiting != null)
                    StartUp.waiting.Close();
                ExMessageBox.Show(140, StartUp.SysObj, "[" + ex.Message + "]", "", MessageBoxButton.OK, MessageBoxImage.Information);
                return null;
            }

            return output;
        }



        public static DataTable GetDataFromExcel(string _fileName, string field_not_null)
        {          
            string HDR = "Yes";
            string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + _fileName + ";Extended Properties=\"Excel 8.0;HDR=" + HDR + ";IMEX=1\"";

            DataSet output = new DataSet();

            using (OleDbConnection conn = new OleDbConnection(strConn))
            {
                try
                {
                    conn.Open();
                }
                catch (Exception ex)
                {
                    ExMessageBox.Show(145, StartUp.SysObj,"[" + ex.Message + "]", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    return null;
                }
                try
                {
                    DataTable dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });

                    foreach (DataRow row in dt.Rows)
                    {
                        string sheet = row["TABLE_NAME"].ToString();
                        OleDbCommand cmd = new OleDbCommand("SELECT * FROM [" + sheet + "]", conn);
                        cmd.CommandType = CommandType.Text;

                        DataTable outputTable = new DataTable(sheet);
                        
                        outputTable.Columns.Add("Stt_(stt):IV", typeof(int));
                        outputTable.Columns[0].AutoIncrement = true;
                        outputTable.Columns[0].AutoIncrementSeed = 2;
                        outputTable.Columns[0].AutoIncrementStep = 1;

                        output.Tables.Add(outputTable);
                        new OleDbDataAdapter(cmd).Fill(outputTable);
                        break;
                    }
                }
                catch (Exception ex)
                {
                    if(StartUp.waiting != null)
                        StartUp.waiting.Close();
                    ExMessageBox.Show(150, StartUp.SysObj, "[" + ex.Message + "]", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    conn.Close();
                    return null;
                }
                conn.Close();
            }
            if (output.Tables.Count == 0)
                return null;

            DataTable tb_excel = XoaDongTrang(output.Tables[0]);
            XoaCotTrang(ref tb_excel);
            tb_excel.TableName = "DataExcel";
            return SetColumnName(tb_excel, field_not_null);

        }


        public static DataTable XoaDongTrang(DataTable tbExcel)
        {
            bool co_so_lieu = false;
            DataTable tb_temp = tbExcel.Clone();
            for (int i = 0; i < tbExcel.Rows.Count; i++)
            {
                co_so_lieu = false;
                for (int j = 1; j < tbExcel.Columns.Count; j++)
                {
                    if (tbExcel.Rows[i][j].ToString().Trim() != "")
                    {
                        co_so_lieu = true;
                        break;
                    }
                }
                if (co_so_lieu == true)
                {
                    tb_temp.ImportRow(tbExcel.Rows[i]);
                }
            }
            return tb_temp;
        }

        public static void XoaCotTrang(ref DataTable tbExcel)
        {
            string column_name = "";
            int vi_tri_cot = 0;
            for (int i = 0; i < tbExcel.Columns.Count; i++)
            {
                column_name = tbExcel.Columns[i].ColumnName.Trim();

                if (column_name.IndexOf("F") == 0 && int.TryParse(column_name.Substring(1, column_name.Length - 1), out vi_tri_cot))
                {
                    tbExcel.Columns.Remove(column_name);
                    i = -1;
                }
            }
           
        }

        private static DataTable SetColumnName(DataTable tb, string field_not_null)//field_not_null là những trường ko đẹp phép bỏ trống
        {
            try
            {
                StrBrowse = "";
                StrBrowseFieldNull = "stt:H=Dòng";
                string browse = "";
                string[] listkey = field_not_null.Split(';');
                for (int i = 0; i < tb.Columns.Count; i++)
                {
                    string columnName = tb.Columns[i].ColumnName.ToString();
                    int index_open = columnName.LastIndexOf('(');
                    int index_close = columnName.LastIndexOf(')');
                    if (index_open == -1 || index_close == -1)
                    {
                        if(StartUp.waiting != null)
                            StartUp.waiting.Close();
                        //SendMessage(StartUp.SysObj.HandleWaiting, RF_PROCESSWAITING, IntPtr.Zero, new IntPtr((int)'1'));
                        ExMessageBox.Show(160, StartUp.SysObj,  string.Format("Tên cột << [{0}] >> trong file  excel không đúng định dạng!", columnName), "", MessageBoxButton.OK, MessageBoxImage.Information);
                        return null;
                    }
                    string field_ = columnName.Substring(index_open + 1, index_close - index_open - 1).Trim();
                    string temp = field_ + ":H=" + columnName.Substring(0, index_open - 1).Trim() + columnName.Substring(index_close + 1, columnName.Length - index_close - 1).Trim();
                    browse += temp + ";";
                    tb.Columns[i].ColumnName = field_;

                    if (listkey.Contains(field_))
                        StrBrowseFieldNull += ";" + temp;
                }
                StrBrowse = browse.Substring(0, browse.Length - 1);
                StrBrowse = StrBrowse.Replace("#", ".");
                StrBrowseFieldNull = StrBrowseFieldNull.Replace("#", ".");
               
            }
            catch (Exception ex)
            {
                ExMessageBox.Show(165, StartUp.SysObj,"[" + ex.Message + "]", "", MessageBoxButton.OK, MessageBoxImage.Information);
                return null;
            }
            return tb;
        }

        public static DataTable GetStructTable(string tableName)
        {
            if (tableName == null || tableName == "")
                return null;
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText += string.Format("SELECT * FROM information_schema.columns WHERE table_name like '{0}'", tableName.Trim());
            DataSet ds = StartUp.SysObj.ExcuteReader(cmd);
            if (ds == null || ds.Tables.Count == 0)
                return null;
            ds.Tables[0].TableName = "StrucImex";
            return ds.Tables[0];
        }

        public static void ConverFont(ref DataSet dtImport)
        {

            for (int i = 0; i < dtImport.Tables["DataExcel"].Rows.Count; i++)
            {

                {
                    for (int j = 0; j < dtImport.Tables["DataExcel"].Columns.Count; j++)
                    {
                        DataRow[] _rowLength = dtImport.Tables["StrucImex"].Select("column_name = '" + dtImport.Tables["DataExcel"].Columns[j] + "'");
                        if (_rowLength.Length > 0)
                        {
                            int _maxLength = 0;

                            //là kiểu string thì convert
                            if (int.TryParse(_rowLength[0]["character_maximum_length"].ToString(), out _maxLength))
                            {
                                if (_maxLength != -1)
                                {
                                    string str_Convert = ConvertTcvn3ToUnicode(dtImport.Tables["DataExcel"].Rows[i][j].ToString().Trim());
                                    if (string.IsNullOrEmpty(str_Convert))
                                        continue;
                                    dtImport.Tables["DataExcel"].Rows[i][j] = str_Convert;// ConvertTcvn3ToUnicode(dtImport.Tables["DataExcel"].Rows[i][j].ToString().Trim());
                                }
                            }


                        }
                    }

                }
            }
        }

        private static void ConverDateTime(ref DataSet dtImport)
        {

            for (int i = 0; i < dtImport.Tables["DataExcel"].Rows.Count; i++)
            {

                {
                    for (int j = 0; j < dtImport.Tables["DataExcel"].Columns.Count; j++)
                    {
                        DataRow[] _rowLength = dtImport.Tables["StrucImex"].Select("column_name = '" + dtImport.Tables["DataExcel"].Columns[j] + "'");
                        if (_rowLength.Length > 0)
                        {
                            if (_rowLength[0]["data_type"].ToString().Trim() == "smalldatetime" && !dtImport.Tables["DataExcel"].Rows[i][j].GetType().FullName.Equals("System.DateTime"))
                            {
                                string[] _datatime = dtImport.Tables["DataExcel"].Rows[i][j].ToString().Replace(" ", "").Replace("/", "-").Split('-');
                                if (_datatime.Length == 3)
                                {
                                    dtImport.Tables["DataExcel"].Rows[i][j] = _datatime[2].Substring(0, 4) + (_datatime[1].Length > 1 ? _datatime[1] : "0" + _datatime[1]) + (_datatime[0].Length > 1 ? _datatime[0] : "0" + _datatime[0]);

                                }
                             
                            }
                        }
                    }

                }
            }
        }
        
        //private static string ConvertTcvn3ToUnicode(string input)
        //{
        //    string output = input;
        //    char[] TCVN3 = new char[] {'µ', '¸', '¶', '·', '¹', 
        //                                '¨', '»', '¾', '¼', '½', 'Æ', 
        //                                '©', 'Ç', 'Ê', 'È', 'É', 'Ë', 
        //                                '®', 'Ì', 'Ð', 'Î', 'Ï', 'Ñ', 
        //                                'ª', 'Ò', 'Õ', 'Ó', 'Ô', 'Ö', 
        //                                '×', 'Ý', 'Ø', 'Ü', 'Þ', 
        //                                'ß', 'ã', 'á', 'â', 'ä', 
        //                                '«', 'å', 'è', 'æ', 'ç', 'é', 
        //                                '¬', 'ê', 'í', 'ë', 'ì', 'î', 
        //                                'ï', 'ó', 'ñ', 'ò', 'ô', 
        //                                '­', 'õ', 'ø', 'ö', '÷', 'ù', 
        //                                'ú', 'ý', 'û', 'ü', 'þ', 
        //                                '¡', '¢', '§', '£', '¤', '¥', '¦'};

        //    char[] Unicode = new char[] {'à', 'á', 'ả', 'ã', 'ạ', 
        //                                'ă', 'ằ', 'ắ', 'ẳ', 'ẵ', 'ặ', 
        //                                'â', 'ầ', 'ấ', 'ẩ', 'ẫ', 'ậ', 
        //                                'đ', 'è', 'é', 'ẻ', 'ẽ', 'ẹ', 
        //                                'ê', 'ề', 'ế', 'ể', 'ễ', 'ệ', 
        //                                'ì', 'í', 'ỉ', 'ĩ', 'ị', 
        //                                'ò', 'ó', 'ỏ', 'õ', 'ọ', 
        //                                'ô', 'ồ', 'ố', 'ổ', 'ỗ', 'ộ', 
        //                                'ơ', 'ờ', 'ớ', 'ở', 'ỡ', 'ợ', 
        //                                'ù', 'ú', 'ủ', 'ũ', 'ụ', 
        //                                'ư', 'ừ', 'ứ', 'ử', 'ữ', 'ự', 
        //                                'ỳ', 'ý', 'ỷ', 'ỹ', 'ỵ', 
        //                                'Ă', 'Â', 'Đ', 'Ê', 'Ô', 'Ơ', 'Ư'
        //                                };

        //    for (int i = 0; i < 74; i++)
        //    {
        //        if (input.IndexOf(TCVN3[i]) >= 0)
        //            output = output.Replace(TCVN3[i], Unicode[i]);
        //    }
        //    return output;

        //}



        public static string TCNV3String = "ÊÈèÉÌéÐÒÕúóÓÔíÝáãìõâêòµ¸¶·¹¨»¾¼½Æ©ÇË®ÎÏÑªÖ×ØÜÞßäôù«åæç¬ëîïñ­øö÷ýûüþ¡¢§£¤¥¦";
        public static string UnicodeString = "ấẩốẫèộéềếỳúểễớíỏóỡừõờũàáảãạăằắẳẵặâầậđẻẽẹêệìỉĩịòọụựôồổỗơởợùủưứửữýỷỹỵĂÂĐÊÔƠƯ";

        public static List<char> GetAllChar(string s)
        {
            List<char> result = new List<char>();
            int n = s.Length;
            for (int i = 0; i < n; i++)
                if (!result.Contains(s[i]) && TCNV3String.Contains(s[i]))
                    result.Add(s[i]);
            int count = result.Count;
            for (int i = 0; i < count - 1; i++)
                for (int j = i + 1; j < count; j++)
                    if (TCNV3String.IndexOf(result[i]) > TCNV3String.IndexOf(result[j]))
                    {
                        char temp = result[i];
                        result[i] = result[j];
                        result[j] = temp;
                    }
            return result;
        }

        public static string ConvertTcvn3ToUnicode(string input)
        {
            input = input.Trim();
            List<char> l = GetAllChar(input);
            int n = l.Count();
            for (int i = 0; i < n; i++)
            {
                int index = TCNV3String.IndexOf(l[i]);
                if ((index < 0))
                    continue;
                input = input.Replace(TCNV3String[index], UnicodeString[index]);
            }
            return input;
        }
    }
}
