using System;
using System.Collections.Generic;
using System.Text;

namespace Buono.BuonoCore
{
    public class cnstDBSettingName
    {
        public static string DBServer = "DBServer";
        public static string DBMS = "DBMS";
        public static string DBPort = "DBPort";
        public static string DBName = "DBName";
        public static string DBUserID = "DBUserID";
        public static string DBPassword = "DBPassword";
        public static string ReadOnly = "ReadOnly";
    }

    public class cnstDBMS
    {
        public static string DBMS_MSSQLServer = "MSSQLServer";
        public static string DBMS_PostgreSQL = "PostgreSQL";
        public static string DBMS_Oracle = "Oracle";
        public static string DBMS_MSAccess = "MSAccess";
    }

    public class cnstDBKind
    {
        public static string Sys = "Sys";
        public static string Log = "Log";
        public static string App = "App";
        public static string Droid = "Droid";
        public static string Mother = "Mother";
    }
}
