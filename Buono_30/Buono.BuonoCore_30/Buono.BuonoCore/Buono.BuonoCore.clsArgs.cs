using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace Buono.BuonoCore
{
    [Serializable]
    public class clsArgs : IDisposable
    {
        public clsArgs()
        {
            _Initialize();
        }

        ~clsArgs()
        {
            Dispose();
        }

        public void Dispose()
        {
            lock (this)
            {
                myOwnerForm_ = null;
                myOwnerWindow_ = null;
                myTBLArgs_ = null;
                myObjList_ = null;
                myDataSet_ = null;
                myAssemblyInfo_ = null;
                myDBInfo_ = null;
                myFileInfo_ = null;
            }
        }

        //####################################################################
        //
        // field
        //
        //####################################################################

        private object myOwnerForm_ = null;
        /// <summary>
        /// OwnerFormを取得、設定する。
        /// </summary>
        public object xOwnerForm
        {
            get { return myOwnerForm_; }
            set { myOwnerForm_ = value; }
        }

        private object myOwnerWindow_ = null;
        /// <summary>
        /// OwnewWindowを取得、設定する。
        /// </summary>
        public object xOwnerWindow
        {
            get { return myOwnerWindow_; }
            set { myOwnerWindow_ = value; }
        }

        private string myDomain_ = "";
        /// <summary>
        /// ドメインを取得、設定する。
        /// </summary>
        public string xDomain
        {
            get { return myDomain_; }
            set { myDomain_ = value; }
        }

        private string myJobID_ = "";
        /// <summary>
        /// JobIDを取得、設定する。
        /// </summary>
        public string xJobID
        {
            get { return myJobID_; }
            set { myJobID_ = value; }
        }

        private string myLoginUserID_ = "";
        /// <summary>
        /// ログインユーザーIDを取得、設定する。
        /// </summary>
        public string xLoginUserID
        {
            get { return myLoginUserID_; }
            set { myLoginUserID_ = value; }
        }

        private string myLoginPassword_ = "";
        /// <summary>
        /// ログインパスワードを取得、設定する。
        /// </summary>
        public string xLoginPassword
        {
            get { return myLoginPassword_; }
            set { myLoginPassword_ = value; }
        }

        private string mySessionID_ = "";
        /// <summary>
        /// セッションIDを取得、設定する。
        /// </summary>
        public string xSessionID
        {
            get { return mySessionID_; }
            set { mySessionID_ = value; }
        }

        private string myFunctionID_ = "";
        /// <summary>
        /// ファンクションIDを取得、設定する。
        /// </summary>
        public string xFunctionID
        {
            get { return myFunctionID_; }
            set { myFunctionID_ = value; }
        }

        private string mySystemID_ = "";
        /// <summary>
        /// システムIDを取得、設定する。
        /// </summary>
        public string xSystemID
        {
            get { return mySystemID_; }
            set { mySystemID_ = value; }
        }

        private string myMachineName_ = "";
        /// <summary>
        /// 依頼者の端末名を取得する。
        /// </summary>
        public string xMachineName
        {
            get { return myMachineName_; }
        }

        private string myIPAddress_ = "";
        /// <summary>
        /// 依頼者のIPアドレスを取得する。
        /// </summary>
        public string xIPAddress
        {
            get { return myIPAddress_; }
        }

        private string myMacAddress_ = "";
        public string xMacAddress
        {
            get { return myMacAddress_; }
        }

        private string myCurrentDate_ = "";
        /// <summary>
        /// 業務日付を取得、設定する。
        /// </summary>
        public string xCurrentDate
        {
            get { return myCurrentDate_; }
            set { myCurrentDate_ = value; }
        }

        private string myCostomerCode_ = "";
        /// <summary>
        /// 契約者コード
        /// </summary>
        public string xCostomerCode
        {
            get { return myCostomerCode_; }
            set { myCostomerCode_ = value; }
        }
        private string myUpdateMode_ = "";
        /// <summary>
        /// 更新モードを取得、設定する。
        /// </summary>
        public string xUpdateMode
        {
            get { return myUpdateMode_; }
            set { myUpdateMode_ = value; }
        }

        private object myTBLArgs_ = null;

        private Dictionary<string, object> myObjList_;

        private DataSet myDataSet_;

        private clsArgs_DBInfo myDBInfo_;
        /// <summary>
        /// データベース情報を取得、設定する。
        /// </summary>
        public clsArgs_DBInfo xDBInfo
        {
            get { return myDBInfo_; }
            set { myDBInfo_ = value; }
        }

        private clsArgs_AssemblyInfo myAssemblyInfo_;
        /// <summary>
        /// 実行アセンブリ情報を取得、設定する。
        /// </summary>
        public clsArgs_AssemblyInfo xAssemblyInfo
        {
            get { return myAssemblyInfo_; }
            set { myAssemblyInfo_ = value; }
        }

        private clsArgs_FileInfo myFileInfo_ = null;
        public byte[] xUploadFile
        {
            get { return (byte[])myFileInfo_.xFile; }
            set { myFileInfo_.xFile = (object)value; }
        }

        public string xUploadFileName
        {
            get { return myFileInfo_.xFileName; }
            set { myFileInfo_.xFileName = value; }
        }

        //####################################################################
        //
        // function
        //
        //####################################################################

        private void _Initialize()
        {
            myDataSet_ = new DataSet();

            DataTable dt = new DataTable("TBLArgs");
            DataColumn column = null;

            column = new DataColumn("SEQ", typeof(long));
            column.AutoIncrement = true;
            dt.Columns.Add(column);
            column = new DataColumn("引数名", typeof(string));
            column.DefaultValue = "";
            dt.Columns.Add(column);
            column = new DataColumn("引数値", typeof(string));
            column.DefaultValue = "";
            dt.Columns.Add(column);

            myTBLArgs_ = dt;

            myMachineName_ = clsSystemInfo.xMachineName;
            myIPAddress_ = clsSystemInfo.xIPAddress.ToString();
            myMacAddress_ = clsSystemInfo.xMacAddress.ToString();
            myCurrentDate_ = clsSystemInfo.xNow.ToString("yyyy/MM/dd");

            //myAssembly_
            myAssemblyInfo_ = new clsArgs_AssemblyInfo();
            myAssemblyInfo_.xExecutePara = new string[0];

            // clsArgs_DB
            myDBInfo_ = new clsArgs_DBInfo();
            myDBInfo_.xDBKind = "App";

            // clsArgs_FileInfo
            myFileInfo_ = new clsArgs_FileInfo();
        }

        /// <summary>
        /// 実行するアセンブリ情報を設定する。
        /// </summary>
        /// <param name="AssemblyName"></param>
        /// <param name="ClassName"></param>
        /// <param name="MethodName"></param>
        /// <param name="Para"></param>
        /// <returns></returns>
        public bool xSetExecuteInfo(
            string pAssemblyName, string pClassName,
            string pMethodName, string[] pPara)
        {
            bool ret = true;

            myAssemblyInfo_.xExecuteAssemblyName = pAssemblyName;
            myAssemblyInfo_.xExecuteClassName = pClassName;
            myAssemblyInfo_.xExecuteMethodName = pMethodName;
            myAssemblyInfo_.xExecutePara = pPara;

            return ret;
        }

        public clsArgs_FileInfo xFileInfo
        {
            get { return myFileInfo_; }
            set { myFileInfo_ = value; }
        }

        public void xAddDataTable(DataTable pTable)
        {
            myDataSet_.Tables.Add(pTable.Copy());
        }

        public DataTable xGetDataTable()
        {
            if (myDataSet_.Tables.Count > 0)
            {
                return myDataSet_.Tables[0].Copy();
            }
            else
            {
                return null;
            }
        }
        public DataTable xGetDataTable(int pTableIndex)
        {
            if (pTableIndex < myDataSet_.Tables.Count)
            {
                return myDataSet_.Tables[pTableIndex].Copy();
            }
            else
            {
                return null;
            }
        }
        public DataTable xGetDataTable(string pTableName)
        {
            if (myDataSet_.Tables.Contains(pTableName))
            {
                return myDataSet_.Tables[pTableName].Copy();
            }
            else
            {
                return null;
            }
        }


        public bool xDataTableExists(string pTableName)
        {
            bool ret = true;
            if (myDataSet_.Tables.Contains(pTableName))
            {
                ret = true;
            }
            else
            {
                ret = false;
            }
            return ret;
        }

        public void xAddObject(string pKey, object pObject)
        {
            if (myObjList_ == null)
            {
                myObjList_ = new Dictionary<string, object>();
            }
            myObjList_.Add(pKey, pObject);
        }

        public object xGetObject(string pKey)
        {
            object ret = null;

            ret = myObjList_[pKey];

            return ret;
        }

        public void xSetArgs(string pArgName, string pArgValue)
        {
            DataTable dt = (DataTable)myTBLArgs_;
            DataRow[] rows = dt.Select("引数名 =" + "'" + pArgName + "'");
            if (rows.Length > 0)
            {
                rows[0]["引数値"] = pArgValue;
            }
            else
            {
                DataRow row = dt.NewRow();
                row["引数名"] = pArgName;
                row["引数値"] = pArgValue;
                dt.Rows.Add(row);
            }
        }

        public string xGetArgs(string pArgName)
        {
            string ret = "";
            DataTable dt = (DataTable)myTBLArgs_;
            DataRow[] rows = dt.Select("引数名 =" + "'" + pArgName + "'");

            if (rows.Length > 0)
            {
                ret = rows[0]["引数値"].ToString().Trim();
            }

            return ret;
        }

        public bool xWriteFile(string pFileName)
        {
            return _WriteFile(pFileName);
        }
        private bool _WriteFile(string pFileName)
        {
            bool ret = true;
            FileStream fs = null;

            try
            {
                fs = new FileStream(
                    pFileName, FileMode.Create, FileAccess.Write);

                if (myFileInfo_.xFile == null)
                {
                    string msg = "";
                    msg += "対象ファイルが存在しません。(clsArgs_WriteFile)";
                    ret = false;
                    return ret;
                }

                byte[] temp = (byte[])myFileInfo_.xFile;
                fs.Write(temp, 0, temp.Length);
            }
            catch
            {
                ret = false;
                return ret;
            }
            finally
            {
                fs.Close();
                fs = null;
            }

            return ret;
        }
    }

    [Serializable]
    public class clsArgs_AssemblyInfo : IDisposable
    {
        ~clsArgs_AssemblyInfo()
        {
            Dispose();
        }
        public void Dispose()
        {
            xExecuteAssemblyDir = null;
            xExecuteAssemblyName = null;
            xExecuteClassName = null;
            xExecuteMethodName = null;
            xExecutePara = null;
        }

        public string xExecuteAssemblyDir = "";
        public string xExecuteAssemblyName = "";
        public string xExecuteClassName = "";
        public string xExecuteMethodName = "";
        public string[] xExecutePara = null;
    }

    [Serializable]
    public class clsArgs_DBInfo : IDisposable
    {
        ~clsArgs_DBInfo()
        {
            Dispose();
        }
        public void Dispose()
        {
            xDBKind = null;
            xSQL = null;
        }

        public string xDBKind = "";
        public string xSQL = "";
    }

    [Serializable]
    public class clsArgs_FileInfo : IDisposable
    {
        ~clsArgs_FileInfo()
        {
            Dispose();
        }
        public void Dispose()
        {
            xFile = null;
            xFileName = null;
        }

        public object xFile = null;
        public string xFileName = "";
    }

    [Serializable]
    public class cnstFunctionID
    {
        public static string Login = "Login";
        public static string AutoLogin = "AutoLogin";
        public static string Logout = "Logout";
        public static string ChengePassword = "ChengePassword";
        public static string GetData = "GetData";
        public static string Get伝票番号 = "Get伝票番号";
        public static string ExecuteSQL = "ExecuteSQL";
        public static string GetMenuName = "GetMenuName";
        public static string GetMenu = "GetMenu";
        public static string GetModuleInfo = "GetModuleInfo";
        public static string SendFax = "SendFax";
        public static string PostDocument = "PostDocument";
        public static string WriteDBLog = "WriteDBLog";

        public static string 排他 = "排他";
        public static string 排他解除 = "排他解除";

        public static string PutFile = "PutFile";
        public static string GetFile = "GetFile";

        public static string SendClientLog = "SendClientLog";

        // ▼ DroidJob -- 廃要素
        public static string RegistDroidJob = "RegistDroidJob";
        public static string GetDroidJobResult = "GetDroidJobResult";
        public static string DownloadFileFromDroidJobID = "DownloadFileFromDroidJobID";
        public static string DeleteFileFromDroidJobID = "DeleteFileFromDroidJobID";
        // ▲ DroidJob -- 廃要素

        // ▼ MotherJob
        public static string RegistMotherJob = "RegistMotherJob";
        public static string GetMotherJobResult = "GetMotherJobResult";
        public static string DownloadFileFromMotherJobID = "DownloadFileFromMotherJobID";
        public static string DeleteFileFromMotherJobID = "DeleteFileFromMotherJobID";
        // ▲ MotherJob

        // ▼ Fax
        public static string GetSendFaxJobInfo = "GetSendFaxJobInfo";
        public static string AddSendFaxJob = "AddSendFaxJob";
        public static string DeleteSendFaxJob = "DeleteSendFaxJob";

        public static string GetArchiveFaxJobInfo = "GetArchiveFaxJobInfo";
        public static string DeleteArchiveFaxJob = "DeleteArchiveFaxJob";
        // ▲ Fax
    }

    [Serializable]
    public class cnstArgs
    {
        /// <summary>
        /// -  LoginMode
        /// </summary>
        public static string LoginMode = "-- LoginMode";
        /// <summary>
        /// -- ForcibleLogin
        /// </summary>
        public static string LoginMode_Forcible = "-- ForcibleLogin";
        /// <summary>
        /// -- StandardLogin
        /// </summary>
        public static string LoginMode_Standard = "-- StandardLogin";
        /// <summary>
        /// -- LoginByAD
        /// </summary>
        public static string LoginMode_ByAD = "-- LoginByAD";

        /// <summary>
        /// -- LoginUser_anonymous
        /// </summary>
        public static string LoginUser_anonymous = "-- LoginUser_anonymous";

        /// <summary>
        /// -- BatchJobNo
        /// </summary>

        // -- 廃要素
        public static string DroidJobID = "-- DroidJobID";
        public static string DroidJobIDList = "-- DroidJobIDList";
        // -- 廃要素

        public static string MotherJobID = "-- MotherJobID";
        public static string MotherJobIDList = "-- MotherJobIDList";
        /// <summary>
        /// -- TableName
        /// </summary>
        public static string TableName = "-- TableName";

        /// <summary>
        /// -- FileCategory
        /// </summary>
        public static string FileCategory = "-- FileCategory";
        public static string FileCategory_共有 = "-- FileCategory_Share";
        /// <summary>
        /// -- FileName
        /// </summary>
        public static string FileName = "-- FileName";

        /// <summary>
        /// -- DirName
        /// </summary>
        public static string DirName = "-- DirName";

        /// <summary>
        /// -- SearchPattern
        /// </summary>
        public static string SearchPattern = "-- SearchPattern";

        public static string WithTransaction = "-- WithTransaction";

        // 伝票番号取得用
        public static string cnstテーブル名 = "-- TableName";
        public static string cnst伝票番号項目名 = "-- 伝票番号項目名";
        public static string cnst伝票番号連番桁数 = "-- 伝票番号連番桁数";

    }

    public class cnstSocketFunctionID
    {
        public static string Login = "Login";
        public static string Logoff = "Logoff";
        public static string GetData = "GetData";
        public static string SendFile = "SendFile";
        public static string WriteDBLog = "WriteDBLog";
    }

}
