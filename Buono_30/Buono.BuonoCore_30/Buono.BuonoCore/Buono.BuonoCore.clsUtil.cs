using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Text;

namespace Buono.BuonoCore
{
    public class clsUtil
    {
        //####################################################################
        //
        // function
        //
        //####################################################################

        /// <summary>
        /// IPアドレスを返す。
        /// </summary>
        /// <returns></returns>
        public static IPAddress xGetLocalIPAddress()
        {
            string ipAddress = "";

            // 物理インターフェース情報をすべて取得
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();


            // 各インターフェースごとの情報を調べる
            foreach (var adapter in interfaces)
            {
                //ネットワーク接続しているか調べる
                if (adapter.OperationalStatus == OperationalStatus.Up &&
                    adapter.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                    adapter.NetworkInterfaceType != NetworkInterfaceType.Tunnel &&
                    !adapter.Description.ToLower().StartsWith("hyper-v"))
                {
                    // インターフェースに設定されたIPアドレス情報を取得
                    var properties = adapter.GetIPProperties();

                    // 設定されているすべてのユニキャストアドレスについて
                    foreach (var unicast in properties.UnicastAddresses)
                    {
                        if (unicast.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            // IPv4アドレス
                            ipAddress = unicast.Address.ToString();
                            break;
                        }
                    }
                }

                ////// 有効なインターフェースのみを対象とする
                ////if (adapter.OperationalStatus != OperationalStatus.Up)
                ////{
                ////    continue;
                ////}
                ////if (adapter.Description.ToLower().StartsWith("hyper-v"))
                ////{
                ////    continue;
                ////}
                if (ipAddress != "") break;
            }

            return IPAddress.Parse(ipAddress);
        }

        public static PhysicalAddress xGetLocalMacAddress()
        {
            string macAddress = "";

            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface adapter in adapters)
            {

                // ネットワーク接続状態が UP のアダプタのみ表示 
                if (adapter.OperationalStatus == OperationalStatus.Up)
                {

                    IPInterfaceProperties ip_prop = adapter.GetIPProperties();

                    // 物理（MAC）アドレスの取得
                    PhysicalAddress phy = adapter.GetPhysicalAddress();
                    macAddress = phy.ToString();
                    if (macAddress != "") break;
                }
            }
            return PhysicalAddress.Parse(macAddress);
        }

        // ------------------------------------------------------------
        //
        //  シリアライズ関連
        //
        // ------------------------------------------------------------

        /// <summary>
        /// オブジェクトをシリアライズする。
        /// </summary>
        /// <param name="Obj">対象のオブジェクト</param>
        /// <returns>byte[]</returns>
        public static byte[] xSerialize(object pObj)
        {
            return _Serialize(pObj);
        }
        private static byte[] _Serialize(object pObj)
        {
            byte[] ret = new byte[0];
            string msg = "";

            MemoryStream ms = null;
            BinaryFormatter bf = null;

            try
            {
                bf = new BinaryFormatter();
                ms = new MemoryStream();

                bf.Serialize(ms, pObj);
                ret = ms.ToArray();
            }
            catch (Exception ex)
            {
                msg = "Serialize中に予期せぬエラーが発生しました！";
                msg += Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace;
                ////////clsTextLogger.xWriteTextLog(
                ////////    MethodBase.GetCurrentMethod().DeclaringType.FullName + "." +
                ////////    MethodBase.GetCurrentMethod().Name, msg);
                return null;
            }

            return ret;
        }

        /// <summary>
        /// オブジェクトをデシリアライスする。
        /// </summary>
        /// <param name="Bytes">対象のバイト配列</param>
        /// <returns>object</returns>
        public static object xDeserialize(byte[] pBytes)
        {
            return _Deserialize(pBytes);
        }
        private static object _Deserialize(byte[] pBytes)
        {
            object ret = null;
            string msg = "";
            MemoryStream ms = null;
            BinaryFormatter bf = null;

            try
            {
                ms = new MemoryStream(pBytes);
                ms.Write(pBytes, 0, pBytes.Length);
                ms.Seek(0, SeekOrigin.Begin);

                bf = new BinaryFormatter();
                bf.AssemblyFormat = FormatterAssemblyStyle.Simple;
                try
                {
                    ret = (object)bf.Deserialize(ms);
                }
                catch (Exception ex)
                {
                    string hh = ex.Message;
                }
            }
            catch (Exception ex)
            {
                msg = "Deserialize中に予期せぬエラーが発生しました！";
                msg += Environment.NewLine + ex.Message + Environment.NewLine + ex.StackTrace;
                ////////clsTextLogger.xWriteTextLog(
                ////////    MethodBase.GetCurrentMethod().DeclaringType.FullName + "." +
                ////////    MethodBase.GetCurrentMethod().Name, msg);
                return null;
            }

            return ret;
        }

        // ------------------------------------------------------------
        //
        //  SQL関連
        //
        // ------------------------------------------------------------

        /// <summary>
        /// テーブルからInsert文を生成して返す。
        /// </summary>
        /// <param name="pTableName">Insertするテーブル名</param>
        /// <param name="pTable">対象テーブル</param>
        /// <param name="pRowIndex">対象テーブルの行No</param>
        /// <returns>string SQL</returns>
        public static string xTableToInsertSQL(
            string pTableName, DataTable pTable, int pRowIndex)
        {
            return _TableToInsertSQL(pTableName, pTable, pRowIndex);
        }
        private static string _TableToInsertSQL(
            string pTableName, DataTable pTable, int pRowIndex)
        {
            string ret = "";

            string SQL = "insert into " + pTableName; ;
            StringBuilder vales = new StringBuilder();

            for (int i = 0; i < pTable.Columns.Count; i++)
            {
                if (i > 0) vales.Append(",");
                vales.Append("N'" + pTable.Rows[pRowIndex][i].ToString() + "'");
            }

            ret = SQL + " values(" + vales.ToString() + ")";

            return ret;
        }

        public static string xTableToUpdateSQL(
            string pTableName, DataTable pTable, int pRowIndex, string[] pKeys)
        {
            return _TableToUpdateSQL(pTableName, pTable, pRowIndex, pKeys);
        }
        private static string _TableToUpdateSQL(
            string pTableName, DataTable pTable, int pRowIndex, string[] pKeys)
        {
            string ret = "";

            string SQL = "update " + pTableName;
            StringBuilder vales = new StringBuilder();

            for (int i = 0; i < pTable.Columns.Count; i++)
            {
                if (i > 0) vales.Append(",");
                vales.Append(pTable.Columns[i].ColumnName + "=");
                vales.Append("'" + pTable.Rows[pRowIndex][i].ToString() + "'");
            }

            StringBuilder where = new StringBuilder();
            for (int i = 0; i < pKeys.Length; i++)
            {
                if (i > 0) where.Append(",");
                where.Append(" " + pKeys[i] + "=");
                where.Append("'" + pTable.Rows[pRowIndex][pKeys[i]].ToString() + "'");
            }

            ret = SQL + " set " + vales.ToString();
            if (pKeys.Length > 0)
            {
                ret += " where " + where.ToString();
            }

            return ret;
        }

        public static string xTableToDeleteSQL(
            string pTableName, DataTable pTable, int pRowIndex, string[] pKeys)
        {
            return _TableToDeleteSQL(pTableName, pTable, pRowIndex, pKeys);
        }
        private static string _TableToDeleteSQL(
            string pTableName, DataTable pTable, int pRowIndex, string[] pKeys)
        {
            string ret = "";

            string SQL = "delete from " + pTableName;

            StringBuilder where = new StringBuilder();
            for (int i = 0; i < pKeys.Length; i++)
            {
                if (i > 0) where.Append(",");
                where.Append(" " + pKeys[i] + "=");
                where.Append("'" + pTable.Rows[pRowIndex][pKeys[i], DataRowVersion.Original].ToString() + "'");
            }

            ret = SQL;
            if (pKeys.Length > 0)
            {
                ret += " where " + where.ToString();
            }

            return ret;
        }

        public static string xSanitizeSQL(string pText)
        {
            return _SanitizeSQL(pText, false);
        }
        public static string xSanitizeSQL(string pText, bool pIsList)
        {
            return _SanitizeSQL(pText, pIsList);
        }
        private static string _SanitizeSQL(string pText, bool pIsList)
        {
            string ret = pText;

            if ((pText == null) || (pText.Length == 0))
            {
                ret = "";
            }
            else
            {
                if (!pIsList)
                {
                    ret = ret.Replace("'", "''");
                    ret = ret.Replace(";", "；");
                }
                ret = ret.Replace(@"\", @"\\");
            }

            return ret;
        }

        //
        // DB関連
        //
        public static DataTable xMakeTBLTran(DataTable pDataTable)
        {
            DataTable dt = null;

            try
            {
                dt = new DataTable(pDataTable.TableName);
                dt.Columns.Add(new DataColumn("-伝種区分-", typeof(string)));
                dt.Columns.Add(new DataColumn("-取引区分-", typeof(string)));
                dt.Columns.Add(new DataColumn("-更新区分-", typeof(string)));
                dt.Columns.Add(new DataColumn("-排他Key-", typeof(string)));
                for (int i = 0; i < pDataTable.Columns.Count; i++)
                {
                    dt.Columns.Add(
                        new DataColumn(pDataTable.Columns[i].ColumnName, pDataTable.Columns[i].DataType));
                }
            }
            catch
            {
                dt = null;
            }

            return dt.Clone();
        }

        public static clsResponse xDataTableToFile(DataTable pDataTable, string pFileName)
        {
            clsResponse ret = new clsResponse();
            string msg = "";

            StreamWriter sw = null;

            try
            {
                sw = new StreamWriter(pFileName, false, Encoding.Default);
                for (int i = 0; i < pDataTable.Rows.Count; i++)
                {
                    StringBuilder sb = new StringBuilder();
                    for (int j = 0; j < pDataTable.Columns.Count; j++)
                    {
                        if (j > 0) sb.Append(",");
                        sb.Append("\"" + pDataTable.Rows[i][j].ToString() + "\"");
                    }
                    sw.WriteLine(sb.ToString());
                }
            }
            catch (Exception ex)
            {
                msg = "ファイルの書込みに失敗しました。";
                msg += Environment.NewLine;
                msg += ex.Message;
                msg += ex.StackTrace;
                ret.xSetError(msg);
                return ret;
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                    sw = null;
                }
            }

            return ret;
        }

        public static clsResponse xCreateMdb(string pFileName)
        {
            clsResponse ret = new clsResponse();
            string inFileName = "";
            string outFileName = "";
            string msg = "";

            try
            {
                inFileName = Assembly.GetExecutingAssembly().CodeBase;
                if (Environment.OSVersion.Platform.ToString().ToLower().StartsWith("win"))
                {
                    inFileName = inFileName.Substring(8);
                }
                else
                {
                    inFileName = inFileName.Substring(7);
                }
                inFileName = Path.GetFullPath(
                             Path.GetDirectoryName(inFileName));
                inFileName = Path.GetFullPath(
                             Path.Combine(inFileName, "../Conf/MSAccess.conf"));

                outFileName = Path.GetFullPath(pFileName);

                File.Copy(inFileName, outFileName, true);
                msg = "mdbを生成しました。" + Environment.NewLine;
                msg += "(" + outFileName + ")";
                ////////clsTextLogger.xWriteTextLog(
                ////////    MethodBase.GetCurrentMethod().DeclaringType.FullName + "." +
                ////////    MethodBase.GetCurrentMethod().Name, msg);
            }
            catch (Exception ex)
            {
                msg = "mdbの生成に失敗しました！" + Environment.NewLine;
                msg += ex.Message + Environment.NewLine;
                msg += ex.StackTrace;
                ret.xSetError(msg);
                ////////clsTextLogger.xWriteTextLog(
                ////////    MethodBase.GetCurrentMethod().DeclaringType.FullName + "." +
                ////////    MethodBase.GetCurrentMethod().Name, msg);
            }

            return ret;
        }

        //
        //  文字列変換
        //
        public static string xStringToNumeric(string pText)
        {
            return _StringToNumeric(pText, false, "");
        }
        public static string xStringToNumeric(string pText, bool pNullToZero)
        {
            return _StringToNumeric(pText, pNullToZero, "");
        }
        public static string xStringToNumeric(string pText, string pEditString)
        {
            return _StringToNumeric(pText, false, pEditString);
        }
        private static string _StringToNumeric(
            string pText, bool pNullToZero, string pEditString)
        {
            string ret = "";
            string inText = pText;
            inText = inText.Replace("０", "0");
            inText = inText.Replace("１", "1");
            inText = inText.Replace("２", "2");
            inText = inText.Replace("３", "3");
            inText = inText.Replace("４", "4");
            inText = inText.Replace("５", "5");
            inText = inText.Replace("６", "6");
            inText = inText.Replace("７", "7");
            inText = inText.Replace("８", "8");
            inText = inText.Replace("９", "9");
            for (int i = 0; i < pText.Length; i++)
            {
                string val = pText.Substring(i, 1);
                if ((val == "0") ||
                    (val == "1") ||
                    (val == "2") ||
                    (val == "3") ||
                    (val == "4") ||
                    (val == "5") ||
                    (val == "6") ||
                    (val == "7") ||
                    (val == "8") ||
                    (val == "9") ||
                    (val == "-") ||
                    (val == "."))
                {
                    ret += val;
                }
            }

            if (ret.Length == 0)
            {
                if (pNullToZero)
                {
                    ret = "0";
                }
                if (pEditString != "")
                {
                    ret = "0";
                }
            }

            try
            {
                if (pEditString != "")
                {
                    try
                    {
                        ret = Convert.ToDouble(ret).ToString(pEditString);
                    }
                    catch
                    {
                        ret = "0";
                        ret = Convert.ToDouble(ret).ToString(pEditString);
                    }
                }
            }
            catch
            {
            }

            return ret;
        }

        public static string xStringToDate(string pText)
        {
            return _StringToDate(pText);
        }
        private static string _StringToDate(string pText)
        {
            string ret = "";

            DateTime now = clsSystemInfo.xNow;
            string strValue = pText;
            int intValue = 0;

            strValue = _StringToNumeric(strValue, false, "");
            strValue = strValue.Replace("-", null);
            strValue = strValue.Replace(".", null);
            if (strValue.Length > 8)
            {
                strValue = strValue.Substring(0, 8);
            }
            if (strValue.Length > 0)
            {
                intValue = Convert.ToInt32(strValue);
                if (intValue == 0)
                { }
                else
                {
                    if (intValue.ToString().Length <= 2)
                    {
                        intValue = now.Year * 10000 + now.Month * 100 + intValue;
                    }
                    else if (intValue.ToString().Length <= 4)
                    {
                        intValue = now.Year * 10000 + intValue;
                    }
                    else if (intValue.ToString().Length <= 6)
                    {
                        if (intValue == 999999)
                        {
                            intValue = 99999999;
                        }
                        else
                        {
                            intValue = 20000000 + intValue;
                        }
                    }
                }
                ret = intValue.ToString("0000/00/00");
            }
            return ret;
        }

        public static string xStringToTime(string pText)
        {
            return _StringToTime(pText);
        }
        private static string _StringToTime(string pText)
        {
            string ret = "";

            DateTime now = clsSystemInfo.xNow;
            string strValue = pText;
            int intValue = 0;

            strValue = _StringToNumeric(strValue, false, "");
            strValue = strValue.Replace("-", null);
            strValue = strValue.Replace(".", null);
            if (strValue.Length > 6)
            {
                strValue = strValue.Substring(0, 6);
            }
            else if (strValue.Length < 6)
            {
                strValue = strValue.PadLeft(6, '0');
            }
            if (strValue.Length > 0)
            {
                intValue = Convert.ToInt32(strValue);
                ret = intValue.ToString("00:00:00");
            }
            return ret;
        }

        public static string xStringToDateTime(string pText)
        {
            string inText = pText.Trim();
            string date = "";
            string time = "";
            string dateTime = "";

            if (inText.Length < 14)
            {
                inText = inText.PadLeft(14, '0');
            }
            else if (inText.Length > 14)
            {
                inText = inText.Substring(0, 14);
            }

            date = inText.Substring(0, 8);
            date = _StringToDate(date);

            time = inText.Substring(8);
            time = _StringToTime(time);

            dateTime = date + " " + time;

            return dateTime;
        }

        public static string xStringToYYYYMM(string pText)
        {
            return _StringToYYYYMM(pText);
        }
        private static string _StringToYYYYMM(string pText)
        {
            string ret = "";

            DateTime now = clsSystemInfo.xNow;
            string strValue = pText;
            int intValue = 0;

            strValue = _StringToNumeric(strValue, false, "");
            strValue = strValue.Replace("-", null);
            strValue = strValue.Replace(".", null);
            if (strValue.Length > 6)
            {
                strValue = strValue.Substring(0, 6);
            }
            if (strValue.Length > 0)
            {
                intValue = Convert.ToInt32(strValue);
                if (intValue == 0)
                { }
                else
                {
                    if (intValue.ToString().Length <= 2)
                    {
                        intValue = now.Year * 100 + intValue;
                    }
                    else if (intValue.ToString().Length <= 4)
                    {
                        intValue = 200000 + intValue;
                    }
                    else if (intValue.ToString().Length <= 6)
                    {
                        if (intValue == 999999)
                        {
                            intValue = 999999;
                        }
                    }
                }
                ret = intValue.ToString("0000/00");
            }

            return ret;
        }

        public static string xStringToDateW(string pText)
        {
            return _StringToDateW(pText);
        }
        private static string _StringToDateW(string pText)
        {
            string ret = "";

            DateTime now = clsSystemInfo.xNow;
            string strValue = pText;
            int intValue = 0;

            strValue = _StringToNumeric(strValue, false, "");
            strValue = strValue.Replace("-", null);
            strValue = strValue.Replace(".", null);
            if (strValue.Length > 6)
            {
                strValue = strValue.Substring(0, 6);
            }
            if (strValue.Length > 0)
            {
                intValue = Convert.ToInt32(strValue);
                ret = intValue.ToString("00/00/00");
            }
            return ret;
        }

        public static string xStringToCurrency(string pText, string pEditString)
        {
            return _StringToCurrency(pText, pEditString);
        }
        private static string _StringToCurrency(string pText, string pEditString)
        {
            string ret = "";

            string strValue = pText;
            double numValue = 0;
            string sign = "";

            strValue = _StringToNumeric(strValue, false, "");
            if (strValue.StartsWith("-"))
            {
                sign = "-";
            }
            strValue = strValue.Replace("-", null);

            try
            {
                numValue = Convert.ToDouble(strValue);
                ret = numValue.ToString(pEditString);
                ret = sign + ret;
            }
            catch
            {
                ret = "0";
            }

            return ret;
        }

        public static string xChengeBoolToString(bool pValue)
        {
            string ret = "0";

            if (pValue == true)
            {
                ret = "1";
            }

            return ret;
        }

        public static bool xChengeStringToBool(string pValue)
        {
            bool ret = false;

            string inValue = pValue.Trim();

            if (inValue.Length >= 1)
            {
                if (inValue.Substring(0, 1) == "1")
                {
                    ret = true;
                }
            }

            return ret;
        }

        /// <summary>
        /// csvフォーマットの入力文字列受け取り。指定された区切り文字でセパレートし、string[]に変換する。
        /// </summary>
        /// <param name="CsvText">csvフォーマットの入力文字列</param>
        /// <param name="SeparateChar">区切り文字</param>
        /// <param name="BundleChar">括り文字</param>
        /// <returns></returns>
        public static string[] xCsvToStringArray(
            string pCsvText, string pSeparateChar, string pBundleChar)
        {
            return _CsvToStringArray(pCsvText, pSeparateChar, pBundleChar);
        }
        private static string[] _CsvToStringArray(
            string pCsvText, string pSeparateChar, string pBundleChar)
        {
            string inText = "";
            string s = "";
            int pos = 0;
            DataTable dt = null;
            string[] text = new string[0];

            try
            {
                dt = new DataTable();
                dt.Columns.Add(new DataColumn("Item", typeof(string)));

                inText = pCsvText;
                if (pBundleChar.Length > 0)
                {
                    if (inText.StartsWith(pBundleChar)) inText = inText.Substring(pBundleChar.Length);
                    if (inText.EndsWith(pBundleChar)) inText = inText.Substring(0, inText.Length - pBundleChar.Length);
                }

                s = pBundleChar + pSeparateChar + pBundleChar;
                while (true)
                {
                    if (inText.Length <= 0) break;
                    pos = inText.IndexOf(s);
                    if (pos < 0) pos = inText.Length;
                    dt.Rows.Add(new object[] { inText.Substring(0, pos) });
                    if ((inText.Length <= (pos + s.Length)))
                    {
                        inText = "";
                    }
                    else
                    {
                        inText = inText.Substring(pos + s.Length);
                    }
                }

                text = new string[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    text[i] = dt.Rows[i]["Item"].ToString();
                }
            }
            catch
            {
            }
            finally
            {
                if (dt != null)
                {
                    dt.Dispose();
                    dt = null;
                }
            }
            return text;
        }

        /// <summary>
        /// string[]を受け取り、csvフォーマットのテキストを返す。
        /// </summary>
        /// <param name="StringArray">入力string[]</param>
        /// <param name="SeparateChar">区切り文字</param>
        /// <param name="BundleChar">括り文字</param>
        /// <returns></returns>
        public static string xStringArrayToCsv(
            string[] pStringArray, string pSeparateChar, string pBundleChar)
        {
            return _StringArrayToCsv(pStringArray, pSeparateChar, pBundleChar);
        }
        private static string _StringArrayToCsv(
            string[] pStringArray, string pSeparateChar, string pBundleChar)
        {
            string csvText = "";
            StringBuilder sb = null;

            try
            {
                sb = new StringBuilder();

                for (int i = 0; i < pStringArray.Length; i++)
                {
                    if (i > 0) sb.Append(pSeparateChar);
                    sb.Append(pBundleChar + pStringArray[i] + pBundleChar);
                }
            }
            catch
            {
                csvText = "";
                return csvText;
            }

            csvText = sb.ToString();

            return csvText;
        }

        /// <summary>
        /// DataTableを受け取り、csvフォーマットのテキストを返す。
        /// </summary>
        /// <param name="Table">入力DataTable</param>
        /// <param name="SeparateChar">区切り文字</param>
        /// <param name="BundleChar">括り文字</param>
        /// <returns></returns>
        public static string xStringArrayToCsv(
            DataTable pTable, string pSeparateChar, string pBundleChar)
        {
            string[] stringArray = new string[0];

            try
            {
                stringArray = new string[pTable.Rows.Count];
                for (int i = 0; i < pTable.Rows.Count; i++)
                {
                    stringArray[i] = pTable.Rows[i][0].ToString();
                }
            }
            catch
            {
                return "";
            }

            return _StringArrayToCsv(stringArray, pSeparateChar, pBundleChar);
        }

        /// <summary>
        /// DataTableを受け取り、csvフォーマットのテキストを返す。
        /// </summary>
        /// <param name="Table">入力DataTable</param>
        /// <param name="TargetColumnNo">対象Column番号</param>
        /// <param name="SeparateChar">区切り文字</param>
        /// <param name="BundleChar">括り文字</param>
        /// <returns></returns>
        public static string xStringArrayToCsv(
            DataTable pTable, int pTargetColumnNo, string pSeparateChar, string pBundleChar)
        {
            string[] stringArray = new string[0];

            try
            {
                stringArray = new string[pTable.Rows.Count];
                for (int i = 0; i < pTable.Rows.Count; i++)
                {
                    stringArray[i] = pTable.Rows[i][pTargetColumnNo].ToString();
                }
            }
            catch
            {
                return "";
            }

            return _StringArrayToCsv(stringArray, pSeparateChar, pBundleChar);
        }

        public static int xLenB(string pText)
        {
            return Encoding.GetEncoding(932).GetByteCount(pText);
        }

        public string xFixedString(string pText, int pLength)
        {
            string outText = "";


            return outText;
        }

        //
        //  日付関連
        //

        public static clsResponse x日付チェック(string pDate)
        {
            return _日付チェック(pDate);
        }

        public static string xGetFrameworkDir()
        {
            return _GetFrameworkDir();
        }

        /// <summary>
        /// 文字コードを判別する
        /// </summary>
        /// <remarks>
        /// Jcode.pmのgetcodeメソッドを移植したものです。
        /// Jcode.pm(http://openlab.ring.gr.jp/Jcode/index-j.html)
        /// Jcode.pmのCopyright: Copyright 1999-2005 Dan Kogai
        /// </remarks>
        /// <param name="byts">文字コードを調べるデータ</param>
        /// <returns>適当と思われるEncodingオブジェクト。
        /// 判断できなかった時はnull。</returns>
        public static Encoding xGetCode(byte[] pBytes)
        {
            return _GetCode(pBytes);
        }

        public static bool xCheck実在Date(string pDateString)
        {
            return _Check実在Date(pDateString);
        }
        private static bool _Check実在Date(string pDateString)
        {
            bool hasError = true;
            DateTime ymd;
            string tmp = "";

            try
            {
                tmp = clsUtil.xStringToDate(pDateString);
                ymd = DateTime.Parse(tmp);
                hasError = false;
            }
            catch (Exception ex)
            {
                hasError = true;
            }

            return !hasError;
        }

        /// <summary>
        /// RemoteSutdown
        /// </summary>
        /// <param name="Domain"></param>
        /// <param name="User"></param>
        /// <param name="Password"></param>
        public static void xRemoteShutdown(string pHost, string pDomain, string pUser, string pPassword)
        {
            _RemoteShutdown(pHost, pDomain, pUser, pPassword);
        }
        private static void _RemoteShutdown(string pHost, string pDomain, string pUser, string pPassword)
        {
            Process process = null;
            SecureString password = null;

            process = new Process();
            if (pDomain != "")
            {
                process.StartInfo.Domain = pDomain;
            }
            process.StartInfo.UserName = pUser;
            process.StartInfo.Password = new SecureString();

            password = new SecureString();
            foreach (char c in pPassword.ToCharArray())
            {
                password.AppendChar(c);
            }
            process.StartInfo.Password = password;

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = "shutdown.exe";
            process.StartInfo.Arguments = @"/r /t 10 /m \\" + pHost;
            process.Start();
            process.WaitForExit();
        }

        //####################################################################
        //
        // private function
        //
        //####################################################################


        //
        //  日付関連
        //

        private static clsResponse _日付チェック(string pDate)
        {
            clsResponse ret = new clsResponse();
            string date = _StringToDate(pDate);

            try
            {
                DateTime d = Convert.ToDateTime(date);
            }
            catch (Exception ex)
            {
                ret.xSetError(ex.Message);
                return ret;
            }
            return ret;
        }

        private static string _GetFrameworkDir()
        {
            string ret = "";

            string winDir = "";
            string work = "";
            string[] dirs = null;

            winDir = Environment.GetFolderPath(
                     Environment.SpecialFolder.System);

            work = Path.GetFullPath(Path.Combine(winDir, "../Microsoft.NET/Framework"));

            dirs = Directory.GetDirectories(work, "v2*");

            DataTable dt = new DataTable();
            dt.Columns.Add("ディレクトリ名", typeof(string));
            for (int i = 0; i < dirs.Length; i++)
            {
                dt.Rows.Add(new string[] { dirs[i] });
            }

            DataRow[] rows = null;

            rows = dt.Select("", "ディレクトリ名 DESC");
            if (rows.Length > 0)
            {
                ret = rows[0]["ディレクトリ名"].ToString();
            }

            return ret;
        }

        /// <summary>
        /// 文字コードを判別する
        /// </summary>
        /// <remarks>
        /// Jcode.pmのgetcodeメソッドを移植したものです。
        /// Jcode.pm(http://openlab.ring.gr.jp/Jcode/index-j.html)
        /// Jcode.pmのCopyright: Copyright 1999-2005 Dan Kogai
        /// </remarks>
        /// <param name="byts">文字コードを調べるデータ</param>
        /// <returns>適当と思われるEncodingオブジェクト。
        /// 判断できなかった時はnull。</returns>
        private static Encoding _GetCode(byte[] pBytes)
        {
            const byte bEscape = 0x1B;
            const byte bAt = 0x40;
            const byte bDollar = 0x24;
            const byte bAnd = 0x26;
            const byte bOpen = 0x28;    //'('
            const byte bB = 0x42;
            const byte bD = 0x44;
            const byte bJ = 0x4A;
            const byte bI = 0x49;

            int len = pBytes.Length;
            byte b1, b2, b3, b4;

            //Encode::is_utf8 は無視

            bool isBinary = false;
            for (int i = 0; i < len; i++)
            {
                b1 = pBytes[i];
                if (b1 <= 0x06 || b1 == 0x7F || b1 == 0xFF)
                {
                    //'binary'
                    isBinary = true;
                    if (b1 == 0x00 && i < len - 1 && pBytes[i + 1] <= 0x7F)
                    {
                        //smells like raw unicode
                        return Encoding.Unicode;
                    }
                }
            }
            if (isBinary)
            {
                return null;
            }

            //not Japanese
            bool notJapanese = true;
            for (int i = 0; i < len; i++)
            {
                b1 = pBytes[i];
                if (b1 == bEscape || 0x80 <= b1)
                {
                    notJapanese = false;
                    break;
                }
            }
            if (notJapanese)
            {
                return Encoding.ASCII;
            }

            for (int i = 0; i < len - 2; i++)
            {
                b1 = pBytes[i];
                b2 = pBytes[i + 1];
                b3 = pBytes[i + 2];

                if (b1 == bEscape)
                {
                    if (b2 == bDollar && b3 == bAt)
                    {
                        //JIS_0208 1978
                        //JIS
                        return Encoding.GetEncoding(50220);
                    }
                    else if (b2 == bDollar && b3 == bB)
                    {
                        //JIS_0208 1983
                        //JIS
                        return Encoding.GetEncoding(50220);
                    }
                    else if (b2 == bOpen && (b3 == bB || b3 == bJ))
                    {
                        //JIS_ASC
                        //JIS
                        return Encoding.GetEncoding(50220);
                    }
                    else if (b2 == bOpen && b3 == bI)
                    {
                        //JIS_KANA
                        //JIS
                        return Encoding.GetEncoding(50220);
                    }
                    if (i < len - 3)
                    {
                        b4 = pBytes[i + 3];
                        if (b2 == bDollar && b3 == bOpen && b4 == bD)
                        {
                            //JIS_0212
                            //JIS
                            return Encoding.GetEncoding(50220);
                        }
                        if (i < len - 5 &&
                            b2 == bAnd && b3 == bAt && b4 == bEscape &&
                            pBytes[i + 4] == bDollar && pBytes[i + 5] == bB)
                        {
                            //JIS_0208 1990
                            //JIS
                            return Encoding.GetEncoding(50220);
                        }
                    }
                }
            }

            //should be euc|sjis|utf8
            //use of (?:) by Hiroki Ohzaki <ohzaki@iod.ricoh.co.jp>
            int sjis = 0;
            int euc = 0;
            int utf8 = 0;
            for (int i = 0; i < len - 1; i++)
            {
                b1 = pBytes[i];
                b2 = pBytes[i + 1];
                if (((0x81 <= b1 && b1 <= 0x9F) || (0xE0 <= b1 && b1 <= 0xFC)) &&
                    ((0x40 <= b2 && b2 <= 0x7E) || (0x80 <= b2 && b2 <= 0xFC)))
                {
                    //SJIS_C
                    sjis += 2;
                    i++;
                }
            }
            for (int i = 0; i < len - 1; i++)
            {
                b1 = pBytes[i];
                b2 = pBytes[i + 1];
                if (((0xA1 <= b1 && b1 <= 0xFE) && (0xA1 <= b2 && b2 <= 0xFE)) ||
                    (b1 == 0x8E && (0xA1 <= b2 && b2 <= 0xDF)))
                {
                    //EUC_C
                    //EUC_KANA
                    euc += 2;
                    i++;
                }
                else if (i < len - 2)
                {
                    b3 = pBytes[i + 2];
                    if (b1 == 0x8F && (0xA1 <= b2 && b2 <= 0xFE) &&
                        (0xA1 <= b3 && b3 <= 0xFE))
                    {
                        //EUC_0212
                        euc += 3;
                        i += 2;
                    }
                }
            }
            for (int i = 0; i < len - 1; i++)
            {
                b1 = pBytes[i];
                b2 = pBytes[i + 1];
                if ((0xC0 <= b1 && b1 <= 0xDF) && (0x80 <= b2 && b2 <= 0xBF))
                {
                    //UTF8
                    utf8 += 2;
                    i++;
                }
                else if (i < len - 2)
                {
                    b3 = pBytes[i + 2];
                    if ((0xE0 <= b1 && b1 <= 0xEF) && (0x80 <= b2 && b2 <= 0xBF) &&
                        (0x80 <= b3 && b3 <= 0xBF))
                    {
                        //UTF8
                        utf8 += 3;
                        i += 2;
                    }
                }
            }
            //M. Takahashi's suggestion
            //utf8 += utf8 / 2;

            Debug.WriteLine(
                string.Format("sjis = {0}, euc = {1}, utf8 = {2}", sjis, euc, utf8));
            if (euc > sjis && euc > utf8)
            {
                //EUC
                return Encoding.GetEncoding(51932);
            }
            else if (sjis > euc && sjis > utf8)
            {
                //SJIS
                return Encoding.GetEncoding(932);
            }
            else if (utf8 > euc && utf8 > sjis)
            {
                //UTF8
                return Encoding.UTF8;
            }

            return null;
        }
    }
}
