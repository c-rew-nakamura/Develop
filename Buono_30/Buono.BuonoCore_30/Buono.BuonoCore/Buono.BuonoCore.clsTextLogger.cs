using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Buono.BuonoCore
{
    public class clsTextLogger
    {
        //####################################################################
        //
        // field
        //
        //####################################################################

        private static object myLogFileUsed_ = new object();

        //####################################################################
        //
        // function
        //
        //####################################################################

        public static void xWriteTextLog(string pLogFile, string pFrom, string pMessage)
        {
            lock (myLogFileUsed_)
            {
                _WriteTextJobLog(pLogFile, pFrom, pMessage, "", "");
            }
        }

        public static void xWriteTextLog(string pFrom, string pMessage)
        {
            lock (myLogFileUsed_)
            {
                _WriteTextJobLog("", pFrom, pMessage, "", "");
            }
        }

        public static void xWriteTextLog(
            string pUserID, string pMachineName, string pFrom, string pMessage)
        {
            lock (myLogFileUsed_)
            {
                _WriteTextJobLog("", pFrom, pMessage, pUserID, pMachineName);
            }
        }

        public static void xWriteTextLog(
            string pLogFile, string pUserID, string pMachineName, string pFrom, string pMessage)
        {
            lock (myLogFileUsed_)
            {
                _WriteTextJobLog(pLogFile, pFrom, pMessage, pUserID, pMachineName);
            }
        }

        private static void _WriteTextJobLog(
            string pLogFile, string pFrom, string pMessage, string pUserID, string pMachineName)
        {
            bool ret = true;
            clsResponse clsRet = new clsResponse();

            string userID = "";
            string machineName = "";
            string fileName = "";
            string dirName = "";
            StringBuilder text = null;
            DateTime now = clsSystemInfo.xNow;

            try
            {
                userID = pUserID;
                if (userID.Length == 0)
                {
                    userID = clsSystemInfo.xWindowsUser;
                }
                machineName = pMachineName;
                if (machineName.Length == 0)
                {
                    machineName = clsSystemInfo.xMachineName;
                }
                if (pLogFile.Length > 0)
                {
                    fileName = Path.GetFullPath(pLogFile);
                    dirName = Path.GetDirectoryName(fileName);
                    if (!Directory.Exists(dirName))
                    {
                        Directory.CreateDirectory(dirName);
                    }
                }
                else
                {
                    // ▼　Win7対応
                    dirName = clsSystemInfo.xLogDirectory;
                    if (!Directory.Exists(dirName))
                    {
                        dirName = Assembly.GetAssembly(typeof(clsTextLogger)).CodeBase;
                        if (Environment.OSVersion.Platform.ToString().ToLower().StartsWith("win"))
                        {
                            dirName = Path.GetDirectoryName(dirName.Substring(8));
                        }
                        else
                        {
                            dirName = Path.GetDirectoryName(dirName.Substring(7));
                        }
                    }
                    fileName = Path.GetFullPath(Path.Combine(dirName, "Elpis.log"));
                    // ▲　Win7対応
                }

                // ▼　LogFileが１Mbを超えたらバックアップし、新規作成
                if (File.Exists(fileName))
                {
                    FileInfo fi = null;
                    try
                    {
                        fi = new FileInfo(fileName);
                        if (fi.Length > 10000000)
                        {
                            string dir = Path.GetDirectoryName(fileName);
                            string newFileName = Path.GetFileNameWithoutExtension(fileName) + "_" + now.ToString("yyyyMMddHHmmss") + ".log";
                            newFileName = Path.Combine(dir, newFileName);
                            File.Move(fileName, newFileName);
                        }
                    }
                    catch (Exception ex)
                    {
                        string hh = "";
                        return;
                    }
                    finally
                    {
                        if (fi != null)
                        {
                            fi = null;
                        }
                    }
                }
                // ▲　LogFileが１Mbを超えたらバックアップし、新規作成

                // ▼　Log出力
                //　　　初めて書き込む際は、権限を設定する。
                if (!File.Exists(fileName))
                {
                    using (StreamWriter sw = new StreamWriter(fileName, true, Encoding.UTF8))
                    {
                        text = new StringBuilder();
                        text.Append("## " + Environment.NewLine);
                        text.Append("## Created " + now.ToString("yyyy/MM/dd HH:mm:ss") + Environment.NewLine);
                        text.Append("## " + Environment.NewLine);
                        sw.WriteLine(text.ToString());
                        text = null;
                        sw.Close();
                    }
                    // ▼　ファイルアクセス権変更（winのみ実行）
                    if (Environment.OSVersion.Platform.ToString().ToLower().StartsWith("win"))
                    {
                        clsACL acl = null;
                        try
                        {
                            acl = new clsACL();
                            clsRet = acl.xChengeFileSecurity(fileName);
                        }
                        catch (Exception ex)
                        {
                            clsRet.xSetError(ex.Message + Environment.NewLine + ex.StackTrace);
                        }
                    }
                    // ▲　ファイルアクセス権変更
                }

                using (StreamWriter sw = new StreamWriter(fileName, true, Encoding.UTF8))
                {
                    text = new StringBuilder();
                    text.Append(now.ToString("yyyy/MM/dd HH:mm:ss fff") + ",");

                    text.Append(userID + ",");
                    text.Append(machineName + ",");
                    text.Append(pMessage.Replace(Environment.NewLine, " ") + ",");
                    text.Append(pFrom);
                    sw.WriteLine(text.ToString());
                    text = null;
                    sw.Close();
                }
                // ▲　Log出力


            }
            catch (Exception ex)
            {
                ret = false;
            }
        }
    }
}
