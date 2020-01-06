using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;

namespace Buono.BuonoCore
{
    public class clsACL : IDisposable
    {
        public void Dispose()
        {
        }

        /// <summary>
        /// ファイル（ディレクトリ）操作権限をEveryone-FullControlに変更する。
        /// </summary>
        /// <param name="pFileName"></param>
        /// <returns>clsResponse</returns>
        public clsResponse xChengeFileSecurity(string pFileName)
        {
            return _ChengeFileSecurity(pFileName);
        }
        private clsResponse _ChengeFileSecurity(string pFileName)
        {
            clsResponse ret = new clsResponse();
            string fileName = "";
            string msg = "";
            FileInfo fileInfo = null;
            FileSecurity fileSec = null;
            FileSystemAccessRule accessRule = null;

            // winのみ実行
            if (Environment.OSVersion.Platform.ToString().ToLower().StartsWith("win"))
            { }
            else
            {
                return ret;
            }

            try
            {
                fileName = Path.GetFullPath(pFileName);
                if (!File.Exists(fileName))
                {
                    msg = "ファイル未登録！" + "(" + fileName + ")";
                    ret.xSetError(msg);
                    return ret;
                }
            }
            catch (Exception ex)
            {
                msg = " 予期せぬエラーが発生しました。";
                msg += Environment.NewLine + ex.Message;
                ret.xSetError(msg);
                msg += Environment.NewLine + ex.StackTrace;
                clsTextLogger.xWriteTextLog(
                    MethodBase.GetCurrentMethod().DeclaringType.FullName + "."
                  + MethodBase.GetCurrentMethod().Name, msg);
                return ret;
            }

            try
            {
                fileInfo = new FileInfo(fileName);
                fileSec = new FileSecurity();
                fileSec = fileInfo.GetAccessControl();

                // アクセス権限を指定
                // Everyoneに対し、フルコントロールの許可
                // （サブフォルダ、及び、ファイルにも適用）
                accessRule = new FileSystemAccessRule(
                    "Everyone",
                    FileSystemRights.FullControl,
                    AccessControlType.Allow);
                fileSec.AddAccessRule(accessRule);
                fileInfo.SetAccessControl(fileSec);
            }
            catch (Exception ex)
            {
                msg = " 予期せぬエラーが発生しました。";
                msg += Environment.NewLine + ex.Message;
                ret.xSetError(msg);
                msg += Environment.NewLine + ex.StackTrace;
                clsTextLogger.xWriteTextLog(
                    MethodBase.GetCurrentMethod().DeclaringType.FullName + "."
                  + MethodBase.GetCurrentMethod().Name, msg);
                return ret;
            }
            return ret;
        }
    }
}
