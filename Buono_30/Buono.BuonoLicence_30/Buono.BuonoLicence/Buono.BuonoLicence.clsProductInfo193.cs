using System;
using System.Collections.Generic;
using Buono.BuonoCore;

namespace Buono.BuonoLicence
{
    public class clsProductInfo193 : clsProductInfoBase
    {
        public clsProductInfo193(string pEnvironment)
        {
            myEnvironment__ = pEnvironment;
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
        private string mySystemID_ = "Buono30";
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
            myTBLVersion__.Rows.Add(new object[] { mySystemID_, mySystemName_, "0.0.0.1", "2019/12/28", "2099/12/31" });
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
