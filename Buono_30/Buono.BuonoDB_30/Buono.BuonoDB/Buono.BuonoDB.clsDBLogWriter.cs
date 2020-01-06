using Buono.BuonoCore;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Buono.BuonoDB
{
    internal class clsDBLogWriter : IDisposable
    {

        ~clsDBLogWriter()
        {
            this.Dispose();
        }

        public void Dispose()
        {
        }

        //####################################################################
        //
        // public function
        //
        //####################################################################

        internal void xWriteDBLog(string Domain, string[] Para)
        {
            _WriteDBJobLog(Domain, Para);
        }

        //####################################################################
        //
        // private function
        //
        //####################################################################

        private clsResponse _WriteDBJobLog(string pDomain, string[] pPara)
        {
            clsResponse response = new clsResponse();
            string msg = "";

            string 利用者コード = pPara[0];
            string 端末名 = pPara[1];
            string ジョブID = pPara[2];
            string セッションID = pPara[3];
            string 発信元 = pPara[4];
            string ログメッセージ = pPara[5];
            string IPアドレス = pPara[6];

            DateTime dateTime = clsSystemInfo.xNow;

            StringBuilder SQL = new StringBuilder();
            SQL.Append("insert into TBLログ (");
            SQL.Append("処理日時,");
            SQL.Append("ジョブID,");
            SQL.Append("PID,");
            SQL.Append("セッションID,");
            SQL.Append("ログメッセージ,");
            SQL.Append("発信元,");
            SQL.Append("利用者コード,");
            SQL.Append("端末名,");
            SQL.Append("IPアドレス)");
            SQL.Append(" values(");
            SQL.Append("N'" + clsUtil.xSanitizeSQL(dateTime.ToString("yyyy/MM/dd HH:mm:ss")) + "',");
            SQL.Append("N'" + clsUtil.xSanitizeSQL(ジョブID) + "',");
            SQL.Append("N'" + clsUtil.xSanitizeSQL(System.Diagnostics.Process.GetCurrentProcess().Id.ToString()) + "',");
            SQL.Append("N'" + clsUtil.xSanitizeSQL(セッションID) + "',");
            SQL.Append("N'" + clsUtil.xSanitizeSQL(ログメッセージ) + "',");
            SQL.Append("N'" + clsUtil.xSanitizeSQL(発信元) + "',");
            SQL.Append("N'" + clsUtil.xSanitizeSQL(利用者コード) + "',");
            SQL.Append("N'" + clsUtil.xSanitizeSQL(端末名) + "',");
            SQL.Append("N'" + clsUtil.xSanitizeSQL(IPアドレス) + "')");

            clsDB db = null;
            string kindOfDB = pDomain + "_" + cnstDBKind.Log;
            try
            {
                db = new clsDB(clsDBInfoPool.xGetInstance().xGetDBInfo(kindOfDB));

                response = db.xDBOpen();
                if (response.xHasError)
                {
                    msg = "DBLogの書き込みに失敗しました！(DBOpen)";
                    msg += Environment.NewLine + response.xMessage;
                    clsTextLogger.xWriteTextLog(
                        MethodBase.GetCurrentMethod().DeclaringType.FullName + "." +
                        MethodBase.GetCurrentMethod().Name, msg);
                    return response;
                }

                response = db.xExecuteSQL(SQL.ToString());
                if (response.xHasError)
                {
                    msg = "DBLogの書き込みに失敗しました！(ExecuteSQL)";
                    msg += Environment.NewLine + response.xMessage;
                    clsTextLogger.xWriteTextLog(
                        MethodBase.GetCurrentMethod().DeclaringType.FullName + "." +
                        MethodBase.GetCurrentMethod().Name, msg);
                    return response;
                }
            }
            catch (Exception ex)
            {
                msg = "DBログ出力中にエラーが発生しました！";
                msg += ex.Message + Environment.NewLine;
                response.xSetError(msg);
                msg += ex.StackTrace;
                clsTextLogger.xWriteTextLog(
                    MethodBase.GetCurrentMethod().DeclaringType.FullName + "."
                    + MethodBase.GetCurrentMethod().Name, msg);
                return response;
            }
            finally
            {
                if (db != null)
                {
                    db.xDBClose();
                    db.Dispose();
                    db = null;
                }
            }

            return response;
        }
    }
}
