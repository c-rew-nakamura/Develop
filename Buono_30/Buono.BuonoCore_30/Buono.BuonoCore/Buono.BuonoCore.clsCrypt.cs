using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Buono.BuonoCore
{
    public class clsCrypt
    {
        //####################################################################
        //
        // field
        //
        //####################################################################

        private static string myPublicKey_
            = "<RSAKeyValue><Modulus>sbF35YMf9ZemxKTHqmck+TREENsre5L9vibhHDcf0OGKfnOJxttIADYXq0nmwS1lL75Y4y9cY39s5tFuC3cNGScDkhF4HRae7ncJyGQd2NTnhwSOXKqFS15fJ8Jek0G3I1jgEXmgdnaNd35+wZKUxUmGgAl+MQ2l7/hCSjIM/Mk=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        /// <summary>
        /// パブリックキーを取得する。
        /// </summary>
        /// <returns></returns>
        public static string xGetPublicKey()
        {
            return myPublicKey_;
        }

        private static string myPrivateKey
            = "<RSAKeyValue><Modulus>sbF35YMf9ZemxKTHqmck+TREENsre5L9vibhHDcf0OGKfnOJxttIADYXq0nmwS1lL75Y4y9cY39s5tFuC3cNGScDkhF4HRae7ncJyGQd2NTnhwSOXKqFS15fJ8Jek0G3I1jgEXmgdnaNd35+wZKUxUmGgAl+MQ2l7/hCSjIM/Mk=</Modulus><Exponent>AQAB</Exponent><P>5Lq6ZXciYxORWT5PmX8S+tfeZ5zeZJsjmEG6yAkgqv1dca4HwRp8OBrX0IIA8p92McfHvdyhpz8ayKm/++S3jw==</P><Q>xuEEC4UKIOOzhFycMsbiAbwF9ecEQmzuzKp8pZ0ZOnWZuT/0dr1uBHfhfumn/PKZ3tIPl3NU5+eR3Oul6yeaJw==</Q><DP>zoBHCJQmV4yzDP9UniA74nxCLwlRP0NRP4UYPMEA3V8hniCgn5Zpz71sWrMEaAfPjeHwU1l+GKDAbanTyncTwQ==</DP><DQ>tkt7EbQY3Jza5/CbnE+AymY2cHgZB2oiWafMKWIexd8V0SA2TMDGH4JjR5mVSd51/DA/6mrk4Mz6fq3hn/Lk8Q==</DQ><InverseQ>zjqINI43WKjKIqczvHSjJ9Vu/XVikn1dccpW3wqOpPFA4RQIT1smRvPjR1ufrER0Wfn+WDHECYz9Q2pFxBLFnA==</InverseQ><D>oDzUTPDY8iRma+/Yag8HZX6/Xp/170LXdREkcMg49aldtbhE50E5BY/B+mHzKALiu5IgIAbGfqcbMm5rmQUrliBfuZSAD6qAbU2CjoSTV9DtYsDVjr/0vVsdTiJn95VfmKN6KbcfvWxPdA+JXe8p8vg5WczZ3z/RTYmRhXEpUh0=</D></RSAKeyValue>";

        private static object myLock_GetRandomStrings_ = new object();

        //####################################################################
        //
        // function
        //
        //####################################################################

        //--------------------------------------------------------------------
        //
        // 暗号化関連
        //
        //--------------------------------------------------------------------

        /// <summary>
        /// 公開鍵による暗号化文字列を取得する。
        /// </summary>
        /// <param name="pText">暗号化する文字列</param>
        /// <returns>string 暗号化された文字列</returns>
        public static string xEncryptX(string pText)
        {
            string ret = "";

            //RSACryptoServiceProviderオブジェクトの作成
            CspParameters CSPParam = new CspParameters();
            CSPParam.Flags = CspProviderFlags.NoFlags;
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(CSPParam);

            //公開鍵を指定
            rsa.FromXmlString(myPublicKey_);

            //暗号化する文字列をバイト配列に
            byte[] data = Encoding.UTF8.GetBytes(pText);
            //暗号化する
            //（XP以降の場合のみ2項目にTrueを指定し、OAEPパディングを使用できる）
            byte[] encryptedData = rsa.Encrypt(data, false);

            //Base64で結果を文字列に変換
            ret = Convert.ToBase64String(encryptedData);

            return ret;
        }

        /// <summary>
        /// 公開鍵による暗号化文字列を取得する。
        /// </summary>
        /// <param name="pText">暗号化する文字列</param>
        /// <returns>string 暗号化された文字列</returns>
        public static string xEncryptY(string pText)
        {
            string ret = "";

            // ▼ Textをバイト配列に変換する
            byte[] b = Encoding.UTF8.GetBytes(pText);
            MemoryStream ms = new MemoryStream(b);
            byte[] bytesIn = ms.ToArray();
            // ▲ 設定データをバイト配列に変換する

            // ▼ 暗号化する
            string key = clsCrypt.xGetPublicKey();
            key = "ElpisProject";

            //DESCryptoServiceProviderオブジェクトの作成
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();

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
            ret = Convert.ToBase64String(bytesOut);

            //閉じる
            cryptStreem.Close();
            msOut.Close();
            // ▲ 暗号化する

            return ret;
        }
        public static string xEncrypt(string pText)
        {
            string ret = pText;

            return ret;
        }

        private static byte[] ResizeBytesArray(byte[] pBytes, int pNewSize)
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

        /// <summary>
        /// 秘密鍵による文字列復号化処理
        /// </summary>
        /// <param name="pText">暗号化された文字列</param>
        /// <returns>復号化された文字列</returns>
        public static string xDecryptX(string pText)
        {
            string ret = "";
            string msg = "";

            msg = "xDecrypt #001";
            clsTextLogger.xWriteTextLog(
                MethodBase.GetCurrentMethod().DeclaringType.FullName + "."
                + MethodBase.GetCurrentMethod().Name, msg);

            if (pText.Length == 0) return ret;

            msg = "xDecrypt #002";
            clsTextLogger.xWriteTextLog(
                MethodBase.GetCurrentMethod().DeclaringType.FullName + "."
                + MethodBase.GetCurrentMethod().Name, msg);

            try
            {
                //RSACryptoServiceProviderオブジェクトの作成
                CspParameters CSPParam = new CspParameters();
                CSPParam.Flags = CspProviderFlags.NoFlags;
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(CSPParam);

                // ??????????
                msg = "myPrivateKey : " + myPrivateKey;
                clsTextLogger.xWriteTextLog(
                    MethodBase.GetCurrentMethod().DeclaringType.FullName + "."
                    + MethodBase.GetCurrentMethod().Name, msg);

                //秘密鍵を指定
                rsa.FromXmlString(myPrivateKey);

                //復号化する文字列をバイト配列に
                byte[] data = Convert.FromBase64String(pText);
                //復号化する
                byte[] decryptedData = rsa.Decrypt(data, false);

                //結果を文字列に変換
                ret = Encoding.UTF8.GetString(decryptedData);

                // ??????????
                msg = "結果を文字列に変換 : " + ret;
                clsTextLogger.xWriteTextLog(
                    MethodBase.GetCurrentMethod().DeclaringType.FullName + "."
                    + MethodBase.GetCurrentMethod().Name, msg);

                msg = "xDecrypt #003";
                clsTextLogger.xWriteTextLog(
                    MethodBase.GetCurrentMethod().DeclaringType.FullName + "."
                    + MethodBase.GetCurrentMethod().Name, msg);
            }
            catch (Exception ex)
            {
                msg = "複合化に失敗しました！" + Environment.NewLine;
                msg = ex.Message + Environment.NewLine + ex.StackTrace;
                clsTextLogger.xWriteTextLog(
                    MethodBase.GetCurrentMethod().DeclaringType.FullName + "."
                    + MethodBase.GetCurrentMethod().Name, msg);
            }

            return ret;
        }
        public static string xDecryptY(string pText)
        {
            string ret = "";
            byte[] inData = null;

            // ▼ 複合化する
            string key = clsCrypt.xGetPublicKey();
            key = "FractalProject";

            inData = Convert.FromBase64String(pText);

            //DESCryptoServiceProviderオブジェクトの作成
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();

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
            CryptoStream cryptStreem = new CryptoStream(
                                    msIn, desdecrypt,
                                    CryptoStreamMode.Read);

            //復号化されたデータを取得するためのStreamReader
            StreamReader srOut = new StreamReader(cryptStreem, Encoding.UTF8);
            //復号化されたデータを取得する
            ret = srOut.ReadToEnd();
            //閉じる
            srOut.Close();
            cryptStreem.Close();
            msIn.Close();
            // ▲ 複合化する

            return ret;
        }
        public static string xDecrypt(string pText)
        {
            string ret = pText;

            return ret;
        }

        //
        public static string xGetmd5String(string pInString)
        {
            string s = pInString;

            //文字列をbyte型配列に変換する
            byte[] data = Encoding.ASCII.GetBytes(s);

            //MD5CryptoServiceProviderオブジェクトを作成
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            //ハッシュ値を計算する
            byte[] bs = md5.ComputeHash(data);

            //byte型配列を16進数の文字列に変換
            StringBuilder result = new StringBuilder();
            foreach (byte b in bs)
            {
                result.Append(b.ToString("x2"));
            }

            return result.ToString();
        }

        // ランダム文字数取得
        public static string xGetRandomStrings(int p文字数)
        {
            lock (myLock_GetRandomStrings_)
            {
                return _GetRandomStrings(p文字数, true, false);
            }
        }
        public static string xGetRandomStrings(int p文字数, bool p記号可)
        {
            lock (myLock_GetRandomStrings_)
            {
                return _GetRandomStrings(p文字数, p記号可, false);
            }
        }
        public static string xGetRandomStrings(int p文字数, bool p記号可, bool p日時付加)
        {
            lock (myLock_GetRandomStrings_)
            {
                return _GetRandomStrings(p文字数, p記号可, p日時付加);
            }
        }

        ////--------------------------------------------------------------------
        ////
        //// 日付関連
        ////
        ////--------------------------------------------------------------------
        public static string xToDateString(string pText)
        {
            return _ToDateString(pText);
        }

        public static string xToNumeric(string pText)
        {
            return _ToNumeric(pText);
        }



        //####################################################################
        //
        // private function
        //
        //####################################################################

        private static string _GetRandomStrings(int p文字数, bool p記号可, bool p日時付加)
        {
            if (p文字数 == 0)
            {
                return "";
            }

            string ret = "";
            int intRandom = 0;

            Random random = new Random();
            for (int i = 0; i < p文字数; i++)
            {
                bool flg = false;
                while (flg == false)
                {
                    Thread.Sleep(0);

                    if (p記号可 == true)
                    {
                        intRandom = random.Next(33, 122);
                    }
                    else
                    {
                        intRandom = random.Next(48, 122);
                    }
                    if (((intRandom == 33) || (intRandom == 35) || (intRandom == 36)) ||
                        ((intRandom >= 48) && (intRandom <= 57)) ||
                        ((intRandom >= 65) && (intRandom <= 90)) ||
                        ((intRandom >= 97) && (intRandom <= 122)))
                    {
                        ret += Convert.ToChar(intRandom).ToString();
                        flg = true;
                    }
                }
            }
            random = null;

            if (p日時付加)
            {
                ret = clsSystemInfo.xNow.ToString("yyyyMMddHHmmss") + ret;
            }

            return ret;
        }

        private static string _ToDateString(string pText)
        {
            string ret = "";

            string work = _ToNumeric(pText);

            if (work.Length == 6)
            {
                work = "20" + work;
            }

            if (work.Length == 8)
            {
                work = work.Substring(0, 4) + "/"
                    + work.Substring(4, 2) + "/"
                    + work.Substring(6, 2);
                try
                {
                    DateTime dt = DateTime.Parse(work);
                    ret = dt.ToString("yyyy/MM/dd");
                }
                catch
                { }
            }

            return ret;
        }

        private static string _ToNumeric(string pText)
        {
            string ret = "";

            string[] table = new string[2] { "0123456789０１２３４５６７８９", "01234567890123456789" };
            for (int i = 0; i < pText.Length; i++)
            {
                for (int j = 0; j < table[0].Length; j++)
                {
                    if (pText.Substring(i, 1) == table[0].Substring(j, 1))
                    {
                        ret += table[1].Substring(j, 1);
                    }
                }
            }
            return ret;
        }
    }
}
