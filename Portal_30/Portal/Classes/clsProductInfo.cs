using Buono.BuonoCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Portal.Classes
{
    public class clsProductInfo : clsProductInfoBase
    {
        public clsProductInfo()
        {
            mySystemID__ = mySystemID_;
            mySystemName__ = mySystemName_;
            myCopyright_yyyy__ = myCopyright_yyyy_;
            myCopyright_Link__ = myCopyright_Link_;
            myShowLicense__ = myShowLicense_;
            __Initialize();
            _Initialize();
            xOptionLanguageList = myOptionLanguageList_;
        }

        //######################################################
        //
        //  private variable
        //
        //######################################################

        /// <summary>
        /// システムごとに適切に設定してください。
        /// </summary>
        private string mySystemID_ = "Portal30";
        private string mySystemName_ = "Buono (Powered by .NET Core 3.0)";
        private string myCopyright_yyyy_ = "2020";
        private string myCopyright_Link_ = "https://www.c-rew.com";
        private bool myShowLicense_ = true;
        private List<string> myOptionLanguageList_ = new List<string> { "zh-CN" };

        //######################################################
        //
        //  public function
        //
        //######################################################

        public override string[] xGetSystemVersionNumber()
        {
            return base.xGetSystemVersionNumber();
        }

        //######################################################
        //
        //  private function
        //
        //######################################################

        private void _Initialize()
        {
            // ▼▼ Buono

            // Initial Module
            myTBLVersion__.Rows.Add(new object[] { mySystemID_, mySystemName_, "0.0.0.1", "2020/01/03", "2099/12/31" });

            // Buonoを追加
            myTBLVersion__.Rows.Add(new object[] { mySystemID_, mySystemName_, "0.0.0.2", "2020/01/03", "2099/12/31" });

            // もろもろ
            myTBLVersion__.Rows.Add(new object[] { mySystemID_, mySystemName_, "0.0.0.3", "2020/01/03", "2099/12/31" });

            // DB追加
            myTBLVersion__.Rows.Add(new object[] { mySystemID_, mySystemName_, "0.0.0.4", "2020/01/03", "2099/12/31" });

            // systen日時をutc+9hに変更
            myTBLVersion__.Rows.Add(new object[] { mySystemID_, mySystemName_, "0.0.0.5", "2020/01/03", "2099/12/31" });
  
            // appsettingsをclsSystemInfoへ
            myTBLVersion__.Rows.Add(new object[] { mySystemID_, mySystemName_, "0.0.0.6", "2020/01/03", "2099/12/31" });

            // postgreSQLの非ssl接続のテスト
            myTBLVersion__.Rows.Add(new object[] { mySystemID_, mySystemName_, "0.0.0.7", "2020/01/03", "2099/12/31" });

            // postgreSQLをssl接続に戻す
            myTBLVersion__.Rows.Add(new object[] { mySystemID_, mySystemName_, "0.0.0.8", "2020/01/03", "2099/12/31" });

            // RestServerを追加（テスト用）
            myTBLVersion__.Rows.Add(new object[] { mySystemID_, mySystemName_, "0.0.0.9", "2020/01/03", "2099/12/31" });
            // RestClientに対応（テスト用）
            myTBLVersion__.Rows.Add(new object[] { mySystemID_, mySystemName_, "0.0.0.10", "2020/01/03", "2099/12/31" });

            // tbl_newsのフォーマット変更に対応
            myTBLVersion__.Rows.Add(new object[] { mySystemID_, mySystemName_, "0.0.0.11", "2020/01/11", "2099/12/31" });
            // ▲▲ Buono


            // ▼▼ License 
            myLicense__ = "";
            myLicense__ += "【ライセンス】" + Environment.NewLine;
            myLicense__ += "　・　[受入テストライセンス]が付与されています。" + Environment.NewLine;
            myLicense__ += "　・　現在のライセンス失効日は、" + base.xExpireDate + "です。" + Environment.NewLine;
            // ▲▲ License 
        }

    }
}
