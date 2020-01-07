using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;

namespace Buono.BuonoCore
{
    public class clsSystemInfo
    {
        //####################################################################
        //
        // field
        //
        //####################################################################

        private static bool myIsCreated_ = false;
        /// <summary>
        /// 本クラスが生成済か否かを取得する。
        /// </summary>
        public static bool xIsCreated
        {
            get { return myIsCreated_; }
        }

        /// <summary>
        /// appsettings.json
        /// </summary>
        private static IConfiguration myAppSettings_;
        public static IConfiguration xAppSettings
        {
            get { return myAppSettings_; }
        }

        private static string myEnvironment_ = "azule";
        /// <summary>
        /// 動作環境を取得、設定する。
        /// </summary>
        public static string xEnvironment
        {
            get { return myEnvironment_; }
            set { myEnvironment_ = value; }
        }

        private static string myDomain_ = "nothing";
        /// <summary>
        /// Domainを取得、設定する。
        /// </summary>
        public static string xDomain
        {
            get { return myDomain_; }
            set { myDomain_ = value; }
        }

        private static string mySystemID_ = "nothing";
        /// <summary>
        /// システムIDを取得、設定する。
        /// </summary>
        public static string xSystemID
        {
            get { return mySystemID_; }
            set { mySystemID_ = value; }
        }

        private static IPAddress myIPAddress_ = null;
        /// <summary>
        /// システムを実行している端末のIPアドレスを取得する。
        /// </summary>
        public static IPAddress xIPAddress
        {
            get { return myIPAddress_; }
        }

        private static PhysicalAddress myMacAddress_ = null;
        /// <summary>
        /// システムを実行している端末のMacアドレスを取得する。
        /// </summary>
        public static PhysicalAddress xMacAddress
        {
            get { return myMacAddress_; }
        }

        private static string myConfDirectory_ = "./";
        /// <summary>
        /// 設定ファイル格納ディレクトリを取得、設定する。
        /// </summary>
        public static string xConfDirectory
        {
            get { return myConfDirectory_; }
            set { myConfDirectory_ = value; }
        }

        private static string myDBConfFileName_ = "db.conf";
        /// <summary>
        /// データベース設定ファイル名を取得、設定する。
        /// </summary>
        public static string xDBConfFileName
        {
            get { return myDBConfFileName_; }
        }

        private static string myLogDirectory_ = "./";
        /// <summary>
        /// ログファイル格納ディレクトリを取得、設定する。
        /// </summary>
        public static string xLogDirectory
        {
            get { return myLogDirectory_; }
            set { myLogDirectory_ = value; }
        }

        private static string myTempDirectory_ = "./";
        /// <summary>
        /// 作業用ディレクトリを取得、設定する。
        /// </summary>
        public static string xTempDirectory
        {
            get { return myTempDirectory_; }
            set { myTempDirectory_ = value; }
        }

        private static string myVUpDirectory_ = "./";
        /// <summary>
        /// XUp格納ディレクトリを取得、設定する。
        /// </summary>
        public static string xVUpDirectory
        {
            get { return myVUpDirectory_; }
            set { myVUpDirectory_ = value; }
        }

        private static string myProgramDirectory_ = "./";
        /// <summary>
        /// Program格納ディレクトリを取得、設定する。
        /// </summary>
        public static string xProgramDirectory
        {
            get { return myProgramDirectory_; }
            set { myProgramDirectory_ = value; }
        }

        private static string myAppDataDirectory_ = "";
        /// <summary>
        /// AppDataディレクトリを取得する。
        /// </summary>
        public static string xAppDataDirectory
        {
            get { return myAppDataDirectory_; }
        }

        private static string myWindowsUser_ = "";
        /// <summary>
        /// WindowsUserを取得する。
        /// </summary>
        public static string xWindowsUser
        {
            get { return myWindowsUser_; }
        }

        private static string myMachineName_ = "";
        /// <summary>
        /// 端末名を取得する。
        /// </summary>
        public static string xMachineName
        {
            get { return myMachineName_; }
        }

        private static clsProductInfoBase myProductInfo_ = null;
        /// <summary>
        /// ProductInfoを取得、設定する。
        /// </summary>
        public static clsProductInfoBase xProductInfo
        {
            get { return myProductInfo_; }
            set { myProductInfo_ = value; }
        }

        /// <summary>
        /// TimeZoneを取得、設定する。
        /// </summary>
        public static TimeZoneInfo xTimeZone
        {
            get { return myTimeZone_; }
            set { myTimeZone_ = value; }
        }
        /// <summary>
        /// TimeZone依存の現在を取得する。
        /// </summary>
        public static DateTime xNow
        {
            get 
            {
                return DateTime.UtcNow.AddHours(9);
            }
        }
        private static TimeZoneInfo myTimeZone_ = TimeZoneInfo.Local;
        //private static TimeZoneInfo myTimeZone_ = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");


        //####################################################################
        //
        // function
        //
        //####################################################################

        /// <summary>
        /// SystemInfoを創成する。
        /// </summary>
        /// <param name="ProductInfo">プロダクト情報</param>
        /// <returns></returns>
        public static bool xCreate(IConfiguration pConfiguration, clsProductInfoBase pProductInfo)
        {
            myAppSettings_ = pConfiguration;
           
            return _Create(pProductInfo);
        }

        private static bool _Create(clsProductInfoBase pProductInfo)
        {
            myEnvironment_ = myAppSettings_["Environment"];
            myDomain_ = myAppSettings_["Domain"];
            myProductInfo_ = pProductInfo;
            mySystemID_    = myProductInfo_.xSystemID;
            myWindowsUser_ = Environment.UserName;
            myMachineName_ = Environment.MachineName;

            try
            {
                myTimeZone_ = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
            }
            catch { }

            try
            {
                myIPAddress_ = clsUtil.xGetLocalIPAddress();
            }
            catch (Exception ex) { }
            try
            {
                myMacAddress_ = clsUtil.xGetLocalMacAddress();
            }
            catch (Exception ex) { }

            if (myEnvironment_ == "local")
            {
                _SetSystemUseDirectory();
            }
            myIsCreated_ = true;

            if (!myProductInfo_.xCanUse)
            {
                ////string msg = "";
                ////msg += "◆　";
                ////msg += mySystemID_;
                ////msg += " は、利用期間満了のため使用できません　◆";
                ////clsTextLogger.xWriteTextLog(myLoginUser_, myMachineName_, "clsSystemInfo", msg);

                ////mySystemID_ = "";
                ////myAppDataDir_ = "";
                ////////myConfDir_ = "";
                ////myLogDir_ = "";
                ////myTempDir_ = "";
                ////myVUpDir_ = "";
                ////////myProgramDir_ = "";
                ////myLoginUser_ = "";
                ////myMachineName_ = "";
                ////mySysConfFileName_ = "";
                ////myDBConfFileName_ = "";
            }

            return myProductInfo_.xCanUse;
        }

        private static bool _SetSystemUseDirectory()
        {
            bool ret = true;
            string dir = "";
            string homeDir = "";
            string msg = "";

            try
            {
                // ▼　Win7対応
                homeDir = Assembly.GetAssembly(typeof(clsSystemInfo)).CodeBase;
                if (Environment.OSVersion.Platform.ToString().ToLower().StartsWith("win"))
                {
                    myProgramDirectory_ = Path.GetDirectoryName(homeDir.Substring(8));
                }
                else
                {
                    myProgramDirectory_ = Path.GetDirectoryName(homeDir.Substring(7));
                }


                homeDir = Path.Combine(myProgramDirectory_, "../");
                homeDir = Path.GetFullPath(homeDir);
                // ▲　Win7対応

                // ConfDirectory
                // (1) /Programからみて、../Conf
                // (2) /Programからみて、../../Conf
                // (3) else /Program
                dir = Path.Combine(homeDir, "Conf");
                if (Directory.Exists(dir))
                {
                    myConfDirectory_ = dir;
                }
                else
                {
                    dir = Path.GetFullPath(Path.Combine(homeDir, "../Conf"));
                    if (Directory.Exists(dir))
                    {
                        myConfDirectory_ = dir;
                    }
                    else
                    {
                        myConfDirectory_ = myProgramDirectory_;
                    }
                }
                myConfDirectory_ = Path.GetFullPath(myConfDirectory_);

                // LogDirectory
                // (1) /Programからみて、../Log
                // (2) /Programからみて、../../Log
                // (3) else /Program
                dir = Path.Combine(homeDir, "Log");
                if (Directory.Exists(dir))
                {
                    myLogDirectory_ = dir;
                }
                else
                {
                    dir = Path.GetFullPath(Path.Combine(homeDir, "../Log"));
                    if (Directory.Exists(dir))
                    {
                        myLogDirectory_ = dir;
                    }
                    else
                    {
                        myLogDirectory_ = myProgramDirectory_;
                    }
                }
                myLogDirectory_ = Path.GetFullPath(myLogDirectory_);

                // TempDirectory
                // (1) /Programからみて、../Temp
                // (2) /Programからみて、../../Temp
                // (3) else /Programからみて、../Tempを作成
                dir = Path.Combine(homeDir, "Temp");
                if (Directory.Exists(dir))
                {
                    myTempDirectory_ = dir;
                }
                else
                {
                    dir = Path.GetFullPath(Path.Combine(homeDir, "../Temp"));
                    if (Directory.Exists(dir))
                    {
                        myTempDirectory_ = dir;
                    }
                    else
                    {
                        dir = Path.Combine(homeDir, "Temp");
                        Directory.CreateDirectory(dir);
                        myTempDirectory_ = dir;
                    }
                }
                myTempDirectory_ = Path.GetFullPath(myTempDirectory_);

                // VUp
                dir = Path.Combine(homeDir, "Vup");
                myVUpDirectory_ = dir;

                // User/AppData
                dir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                dir = Path.Combine(dir, "Buono");
                dir = Path.Combine(dir, mySystemID_);

                myAppDataDirectory_ = Path.GetFullPath(dir);
            }
            catch
            {
                ret = false;
            }

            return ret;
        }
    }
}
