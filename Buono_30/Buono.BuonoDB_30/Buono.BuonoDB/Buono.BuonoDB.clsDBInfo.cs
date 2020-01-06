using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Buono.BuonoCore;

namespace Buono.BuonoDB
{
    public class clsDBInfo : IDisposable
    {
        /// <summary>
        /// clsDBInfoを初期化します。
        /// </summary>
        public clsDBInfo()
        {
            if (clsSystemInfo.xEnvironment == "heroku")
            {
                _GetHerokuDBInfo();
            }
        }

        ~clsDBInfo()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            myDBServer_ = "";
            myDBMS_ = "";
            myDBPort_ = "";
            myDBName_ = "";
            myDBUserID_ = "";
            myDBPassword_ = "";
            myDBSslMode_ = "";
        }

        //####################################################################
        //
        // constructor
        //
        //####################################################################

        /// <summary>
        /// 引数を使用してclsDBInfoを初期化します。
        /// </summary>
        /// <param name="pDBMS">DBMS</param>
        /// <param name="pServer">DBServer</param>
        /// <param name="pDBPort">接続ポート番号</param>
        /// <param name="pDBName">カタログ名</param>
        /// <param name="pDBUserID">ユーザーID</param>
        /// <param name="pDBPassword">パスワード</param>
        public clsDBInfo(
            string pDBMS, string pDBServer, string pDBPort, string pDBName,
            string pDBUserID, string pDBPassword, string pDBSslMode, bool pDBReadOnly, int pDBTimeoutSeconds = 0)
        {
            myDBServer_ = pDBServer;
            myDBMS_ = pDBMS;
            myDBPort_ = pDBPort;
            myDBName_ = pDBName;
            myDBUserID_ = pDBUserID;
            myDBPassword_ = pDBPassword;
            myDBSslMode_ = pDBSslMode;
            myDBReadOnly_ = pDBReadOnly;
            if (pDBTimeoutSeconds == 0) { }
            else
            {
                myDBTimeoutSeconds_ = pDBTimeoutSeconds;
            }
        }

        //####################################################################
        //
        // field
        //
        //####################################################################

        private string myDBServer_ = "";
        /// <summary>
        /// DBServerを取得、設定する。
        /// </summary>
        public string xDBServer
        {
            get { return myDBServer_; }
            set { myDBServer_ = value; }
        }

        private string myDBMS_ = "";
        /// <summary>
        /// DBMSを取得、設定する。
        /// </summary>
        public string xDBMS
        {
            get { return myDBMS_; }
            set { myDBMS_ = value; }
        }

        private string myDBPort_ = "";
        /// <summary>
        /// 接続ポート番号を取得、設定する。
        /// </summary>
        public string xDBPort
        {
            get { return myDBPort_; }
            set { myDBPort_ = value; }
        }

        private string myDBName_ = "";
        /// <summary>
        /// カタログ名を取得、設定する。
        /// </summary>
        public string xDBName
        {
            get { return myDBName_; }
            set { myDBName_ = value; }
        }

        private string myDBUserID_ = "";
        /// <summary>
        /// 接続ユーザーIDを取得、設定する。
        /// </summary>
        public string xDBUserID
        {
            get { return myDBUserID_; }
            set { myDBUserID_ = value; }
        }

        private string myDBPassword_ = "";
        /// <summary>
        /// 接続パスワードを取得、設定する。
        /// </summary>
        public string xDBPassword
        {
            get { return myDBPassword_; }
            set { myDBPassword_ = value; }
        }

        private string myDBSslMode_ = "";
        /// <summary>
        /// SSLモードを取得、設定する。
        /// </summary>
        public string xDBSslMode
        {
            get { return myDBSslMode_; }
            set { myDBSslMode_ = value; }
        }

        private int myDBTimeoutSeconds_ = 30;
        /// <summary>
        /// タイムアウト閾値を取得、設定する。
        /// </summary>
        public int xDBTimeoutSeconds
        {
            get { return myDBTimeoutSeconds_; }
            set { myDBTimeoutSeconds_ = value; }
        }

        private bool myDBReadOnly_ = false;
        /// <summary>
        /// ReadOnlyを取得、設定する。
        /// trueの場合はReadOnly。
        /// </summary>
        public bool xDBReadOnly
        {
            get { return myDBReadOnly_; }
            set { myDBReadOnly_ = value; }
        }

        //####################################################################
        //
        // function
        //
        //####################################################################

        /// <summary>
        /// herokuのローカルDB情報dbInfoを生成する。 
        /// </summary>
        /// <param name="pConfFilePath">設定ファイルパス</param>
        /// <param name="pKindOfDB">KindOfDB</param>
        /// <returns>clsDBInfo</returns>
        public static clsDBInfo xGetHerokuDBInfo()
        {
            clsDBInfo me = new clsDBInfo();
            me._GetHerokuDBInfo();

            return me;
        }
        private void _GetHerokuDBInfo()
        {
            try
            {
                var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
                var databaseUri = new Uri(databaseUrl);
                var userInfo = databaseUri.UserInfo.Split(':');
                xDBMS = cnstDBMS.DBMS_PostgreSQL;
                xDBServer = databaseUri.Host;
                xDBPort = databaseUri.Port.ToString();
                xDBUserID = userInfo[0];
                xDBPassword = userInfo[1];
                xDBName = databaseUri.LocalPath.TrimStart('/');
            }
            catch (Exception ex) { }
        }


        /// <summary>
        /// 設定ファイル(db.conf)からdbInfoを生成する。 
        /// </summary>
        /// <param name="pConfFilePath">設定ファイルパス</param>
        /// <param name="pKindOfDB">KindOfDB</param>
        /// <returns>clsDBInfo</returns>
        public static clsDBInfo xLoadConf(string pConfFilePath, string pKindOfDB)
        {
            clsDBInfo me = new clsDBInfo();
            me._LoadConfFile(pConfFilePath, pKindOfDB);

            return me;
        }
        private void _LoadConfFile(string pConfFilePath, string pKindOfDB)
        {
            string path = "";

            try
            {
                path = Path.GetFullPath(pConfFilePath);
                if (File.Exists(path))
                {
                    using (clsSettingFileHandler sfh = new clsSettingFileHandler())
                    {
                        if (sfh.xLoad(path))
                        {
                            myDBServer_ = sfh.xGetItem(pKindOfDB, cnstDBSettingName.DBServer);
                            myDBMS_ = sfh.xGetItem(pKindOfDB, cnstDBSettingName.DBMS);
                            myDBPort_ = sfh.xGetItem(pKindOfDB, cnstDBSettingName.DBPort);
                            myDBName_ = sfh.xGetItem(pKindOfDB, cnstDBSettingName.DBName);
                            myDBUserID_ = sfh.xGetItem(pKindOfDB, cnstDBSettingName.DBUserID);
                            myDBPassword_ = sfh.xGetItem(pKindOfDB, cnstDBSettingName.DBPassword);
                            if (sfh.xGetItem(pKindOfDB, cnstDBSettingName.ReadOnly).ToLower() == "yes")
                            {
                                myDBReadOnly_ = true;
                            }
                        }
                        sfh.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                string hh = ex.Message;
            }
        }
    }
}
