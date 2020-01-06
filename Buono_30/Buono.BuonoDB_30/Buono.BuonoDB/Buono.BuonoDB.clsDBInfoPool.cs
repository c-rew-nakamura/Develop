using Buono.BuonoCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace Buono.BuonoDB
{
    public class clsDBInfoPool 
    {

        //####################################################################
        //
        // field
        //
        //####################################################################

        private static clsDBInfoPool me_ = null;

        public static clsDBInfoPool xGetInstance()
        {
            if (me_ == null)
            {
                me_ = new clsDBInfoPool();
                me_._Initialize();
            }
            return me_;
        }
        private void _Initialize()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("Key", typeof(string)));
            dt.Columns.Add(new DataColumn("DBMS", typeof(string)));
            dt.Columns.Add(new DataColumn("DBServer", typeof(string)));
            dt.Columns.Add(new DataColumn("DBPort", typeof(string)));
            dt.Columns.Add(new DataColumn("DBName", typeof(string)));
            dt.Columns.Add(new DataColumn("DBUserID", typeof(string)));
            dt.Columns.Add(new DataColumn("DBPassword", typeof(string)));
            dt.Columns.Add(new DataColumn("DBSslMode", typeof(string)));
            dt.Columns.Add(new DataColumn("DBReadOnly", typeof(string)));
            xGetInstance().myTBLInfo_ = dt.Clone();

            xGetInstance().myDBConf_ = clsSystemInfo.xAppSettings.GetSection("dbConnection");
        }

        private DataTable myTBLInfo_ = null;
        private IConfiguration myDBConf_ = null;

        //####################################################################
        //
        // function
        //
        //####################################################################

        /// <summary>
        /// DB種別を指定し、該当するclsDBInfoを返す。
        /// 該当するclsDBInfoが存在しない場合は、設定ファイルより作成したうえで返す。
        /// </summary>
        /// <param name="pKindOfDB">DB種別</param>
        /// <returns>clsDBInfo</returns>
        public clsDBInfo xGetDBInfo(string pKindOfDB)
        {
            clsDBInfo dbInfo = null;
            DataTable dt = xGetInstance().myTBLInfo_;
            DataRow[] rows = null;
            DataRow row = null;

            lock (xGetInstance().myTBLInfo_)
            {
                string key = clsSystemInfo.xDomain + "_" + pKindOfDB;
                rows = dt.Select("Key=" + "'" + key + "'");
                if (rows.Length > 0)
                {
                    dbInfo = new clsDBInfo(
                        rows[0]["DBMS"].ToString(),
                        rows[0]["DBServer"].ToString(),
                        rows[0]["DBPort"].ToString(),
                        rows[0]["DBName"].ToString(),
                        rows[0]["DBUserID"].ToString(),
                        rows[0]["DBPassword"].ToString(),
                        rows[0]["DBSslMode"].ToString(),
                        Convert.ToBoolean(rows[0]["DBReadOnly"]));
                }
                else
                {
                    string DBMS = "";
                    string DBServer = "";
                    string DBPort = "";
                    string DBName = "";
                    string DBUserID = "";
                    string DBPassword = "";
                    string DBSslMode = "";
                    string DBReadOnly = "";

                    DBMS = xGetInstance().myDBConf_.GetSection(key)["DBMS"];
                    if (DBMS == null) DBMS = "";
                    DBServer = xGetInstance().myDBConf_.GetSection(key)["DBServer"];
                    if (DBServer == null) DBServer = "";
                    DBPort = xGetInstance().myDBConf_.GetSection(key)["DBPort"];
                    if (DBPort == null) DBPort = "";
                    DBName = xGetInstance().myDBConf_.GetSection(key)["DBName"];
                    if (DBName == null) DBName = "";
                    DBUserID = xGetInstance().myDBConf_.GetSection(key)["DBUserID"];
                    if (DBUserID == null) DBUserID = "";
                    DBPassword = xGetInstance().myDBConf_.GetSection(key)["DBPassword"];
                    if (DBPassword == null) DBPassword = "";
                    DBSslMode = xGetInstance().myDBConf_.GetSection(key)["DBSslMode"];
                    if (DBSslMode == null) DBSslMode = "false";
                    DBReadOnly = xGetInstance().myDBConf_.GetSection(key)["DBReadOnly"];
                    if (DBReadOnly == null) DBReadOnly = "false";

                    row = dt.NewRow();
                    row["Key"]          = key;
                    row["DBMS"]         = DBMS;
                    row["DBServer"]     = DBServer;
                    row["DBPort"]       = DBPort;
                    row["DBName"]       = DBName;
                    row["DBUserID"]     = DBUserID;
                    row["DBPassword"]   = DBPassword;
                    row["DBSslMode"]    = DBSslMode;
                    row["DBReadOnly"]   = DBReadOnly;
                    dt.Rows.Add(row);

                    dbInfo = new clsDBInfo();
                    dbInfo.xDBMS = DBMS;
                    dbInfo.xDBServer = DBServer;
                    dbInfo.xDBPort = DBPort;
                    dbInfo.xDBName = DBName;
                    dbInfo.xDBUserID = DBUserID;
                    dbInfo.xDBPassword = DBPassword;
                    dbInfo.xDBSslMode = DBSslMode;
                    dbInfo.xDBReadOnly = Convert.ToBoolean(DBReadOnly);
                }
            }

            rows = null;

            return dbInfo;
        }

        //####################################################################
        //
        // private function
        //
        //####################################################################

    }
}
