using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Buono.BuonoCore
{
    public class clsSettingFileHandler : IDisposable
    {

        ////~clsSettingFileHandler()
        ////{
        ////    Dispose();
        ////}

        public void Dispose()
        {
            lock (this)
            {
                if (myData_ != null)
                {
                    myData_.Dispose();
                    myData_ = null;
                }
            }
        }

        //######################################################
        //
        //  private variable
        //
        //######################################################

        private string myJobID_;
        private string myFileName_;
        private DataTable myData_;



        //######################################################
        //
        //  public function
        //
        //######################################################

        public string xJobID
        {
            get { return myJobID_; }
            set { myJobID_ = value; }
        }

        public string xFileName
        {
            get { return myFileName_; }
            set { myFileName_ = value; }
        }

        public bool xLoad()
        {
            return _Load();
        }
        public bool xLoad(string pFileName)
        {
            myFileName_ = pFileName;
            return _Load();
        }

        public bool xSave()
        {
            return _Save();
        }
        public bool xSave(string pFileName)
        {
            myFileName_ = pFileName;
            return _Save();
        }

        public bool xCreate()
        {
            return _Create();
        }

        public bool xSetItem(string pItem, string pValue)
        {
            return _SetItem("", "", pItem, pValue);
        }
        public bool xSetItem(string pKind, string pItem, string pValue)
        {
            return _SetItem("", pKind, pItem, pValue);
        }
        public bool xSetItem(string pSEQ, string pKind, string pItem, string pValue)
        {
            return _SetItem(pSEQ, pKind, pItem, pValue);
        }

        public string xGetItem(string pItem)
        {
            return _GetItem(pItem);
        }
        public string xGetItem(string pKind, string pItem)
        {
            return _GetItem(pKind, pItem);
        }

        public DataTable xGetDefine()
        {
            _MakeDefine();
            return myData_.Copy();
        }

        public DataTable xGetData()
        {
            return myData_.Copy();
        }

        public bool xSetData(DataTable pDataTable)
        {
            bool ret = true;
            myData_ = pDataTable.Copy();
            return ret;
        }

        public DataTable xSettingFileDefine
        {
            get
            {
                _MakeDefine();
                return myData_.Copy();
            }
        }

        //######################################################
        //
        //  private function
        //
        //######################################################

        private void _Initialize()
        {

        }

        private void _MakeDefine()
        {
            DataTable dt = new DataTable();
            DataColumn column = new DataColumn("表示順", typeof(string));
            column.DefaultValue = "";
            dt.Columns.Add(column);

            column = new DataColumn("分類", typeof(string));
            column.DefaultValue = "";
            dt.Columns.Add(column);

            column = new DataColumn("設定名", typeof(string));
            column.DefaultValue = "";
            dt.Columns.Add(column);

            column = new DataColumn("設定値", typeof(string));
            column.DefaultValue = "";
            dt.Columns.Add(column);

            myData_ = dt;
        }

        private bool _Load()
        {
            bool ret = true;
            byte[] inData = null;
            FileStream file = null;

            if (File.Exists(myFileName_))
            {
            }
            else
            {
                ret = false;
                return ret;
            }

            try
            {
                int count = 0;
                while (count < 150)
                {
                    try
                    {
                        file = new FileStream(
                            myFileName_, FileMode.Open, FileAccess.Read);
                        count = 999;
                    }
                    catch
                    {
                        count++;
                        Thread.Sleep(100);
                    }
                }
                inData = new byte[file.Length];
                file.Read(inData, 0, (int)file.Length);
            }
            catch
            {
                ret = false;
                return ret;
            }
            finally
            {
                file.Close();
                file.Dispose();
                file = null;
            }

            // ▼ 複合化する
            string key = clsCrypt.xGetPublicKey();

            //DESCryptoServiceProviderオブジェクトの作成
            DESCryptoServiceProvider
                des = new DESCryptoServiceProvider();

            //共有キーと初期化ベクタを決定
            //パスワードをバイト配列にする
            byte[] bytesKey = Encoding.UTF8.GetBytes(key);
            //共有キーと初期化ベクタを設定
            des.Key = ResizeBytesArray(bytesKey, des.Key.Length);
            des.IV = ResizeBytesArray(bytesKey, des.IV.Length);

            //Base64で文字列をバイト配列に戻す
            //byte[] bytesIn = Convert.FromBase64String(str);
            byte[] bytesIn = inData;
            //暗号化されたデータを読み込むためのMemoryStream
            MemoryStream msIn = new MemoryStream(bytesIn);
            //DES復号化オブジェクトの作成
            ICryptoTransform desdecrypt = des.CreateDecryptor();
            //読み込むためのCryptoStreamの作成
            CryptoStream
                cryptStreem = new CryptoStream(
                                    msIn, desdecrypt,
                                    CryptoStreamMode.Read);

            //復号化されたデータを取得するためのStreamReader
            StreamReader srOut
                = new StreamReader(cryptStreem, Encoding.UTF8);
            //復号化されたデータを取得する
            DataSet ds = new DataSet();
            ds.ReadXml(srOut);
            if (ds.Tables.Count > 0)
            {
                myData_ = ds.Tables[0].Copy();
            }
            else
            {
                _MakeDefine();
            }
            //閉じる
            srOut.Close();
            cryptStreem.Close();
            msIn.Close();
            // ▲ 複合化する

            return ret;
        }

        private bool _Save()
        {
            bool ret = true;
            clsResponse response = new clsResponse();

            // ▼ 設定データをバイト配列に変換する
            DataSet ds = new DataSet();
            ds.Tables.Add(myData_.Copy());

            MemoryStream ms = new MemoryStream();
            ds.WriteXml(ms, XmlWriteMode.IgnoreSchema);

            byte[] bytesIn = ms.ToArray();
            // ▲ 設定データをバイト配列に変換する

            // ▼ 暗号化する
            string key = clsCrypt.xGetPublicKey();

            //DESCryptoServiceProviderオブジェクトの作成
            DESCryptoServiceProvider
                des = new DESCryptoServiceProvider();

            //共有キーと初期化ベクタを決定
            //パスワードをバイト配列にする
            byte[] bytesKey = Encoding.UTF8.GetBytes(key);
            //共有キーと初期化ベクタを設定
            des.Key = ResizeBytesArray(bytesKey, des.Key.Length);
            des.IV = ResizeBytesArray(bytesKey, des.IV.Length);

            //暗号化されたデータを書き出すためのMemoryStream
            MemoryStream msOut = new MemoryStream();
            //DES暗号化オブジェクトの作成
            ICryptoTransform desdecrypt = des.CreateEncryptor();
            //書き込むためのCryptoStreamの作成
            CryptoStream cryptStreem = new CryptoStream(
                                    msOut, desdecrypt,
                                    CryptoStreamMode.Write);
            //書き込む
            cryptStreem.Write(bytesIn, 0, bytesIn.Length);
            cryptStreem.FlushFinalBlock();
            //暗号化されたデータを取得
            byte[] bytesOut = msOut.ToArray();

            //閉じる
            cryptStreem.Close();
            msOut.Close();
            // ▲ 暗号化する

            // ▼ ファイルに書き出す
            FileStream file = null;
            try
            {
                file = new FileStream(
                    myFileName_, FileMode.Create, FileAccess.Write);
                file.Write(bytesOut, 0, bytesOut.Length);
                file.Flush();
            }
            catch (Exception ex)
            {
                ret = false;
                string msg = ex.Message + ex.StackTrace;
                clsTextLogger.xWriteTextLog(
                    MethodBase.GetCurrentMethod().DeclaringType.FullName + "."
                    + MethodBase.GetCurrentMethod().Name, msg);
                return ret;
            }
            finally
            {
                if (file != null)
                {
                    file.Close();
                    file.Dispose();
                    file = null;
                }
            }
            // ▲ ファイルに書き出す

            // ▼　アクセス権変更
            clsACL acl = null;
            try
            {
                acl = new clsACL();
                response = acl.xChengeFileSecurity(myFileName_);
            }
            catch (Exception ex)
            {
                ret = false;
                response.xSetError(ex.Message + Environment.NewLine + ex.StackTrace);
            }
            if (response.xHasError)
            {
                ret = false;
                clsTextLogger.xWriteTextLog(
                    MethodBase.GetCurrentMethod().DeclaringType.FullName + "."
                    + MethodBase.GetCurrentMethod().Name, response.xMessage);
            }
            // ▲　アクセス権変更

            return ret;
        }

        private bool _SetItem(string pSEQ, string pKind, string pItem, string pValue)
        {
            bool ret = true;

            DataRow[] rows = null;

            try
            {
                rows = myData_.Select("分類=" + "'" + pKind + "'" + " and 設定名=" + "'" + pItem + "'");
                if (rows.Length > 0)
                {
                    rows[0]["設定値"] = pValue;
                }
                else
                {
                    myData_.Rows.Add(new object[] { pSEQ, pKind, pItem, pValue });
                }
            }
            catch
            {
                ret = false;
            }

            return ret;
        }

        private string _GetItem(string pItem)
        {
            string ret = "";

            DataRow[] rows = null;

            try
            {
                rows = myData_.Select("設定名=" + "'" + pItem + "'");
                if (rows.Length > 0)
                {
                    ret = rows[0]["設定値"].ToString();
                }
            }
            catch
            { }

            return ret;
        }

        private string _GetItem(string pKind, string pItem)
        {
            string ret = "";

            DataRow[] rows = null;

            try
            {
                rows = myData_.Select("分類=" + "'" + pKind + "'" + " and 設定名=" + "'" + pItem + "'");
                if (rows.Length > 0)
                {
                    ret = rows[0]["設定値"].ToString();
                }
            }
            catch (Exception ex)
            {
                string hh = "";
            }

            return ret;
        }

        private bool _Create()
        {
            bool ret = true;

            _MakeDefine();

            return ret;
        }


        private byte[] ResizeBytesArray(byte[] pBytes, int pNewSize)
        {
            byte[] newBytes = new byte[pNewSize];
            if (pBytes.Length <= pNewSize)
            {
                for (int i = 0; i < pBytes.Length; i++)
                    newBytes[i] = pBytes[i];
            }
            else
            {
                int pos = 0;
                for (int i = 0; i < pBytes.Length; i++)
                {
                    newBytes[pos++] ^= pBytes[i];
                    if (pos >= newBytes.Length)
                        pos = 0;
                }
            }
            return newBytes;
        }

    }
}
