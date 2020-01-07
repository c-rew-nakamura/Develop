using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Buono.BuonoCore
{
    /// <summary>
    /// プロダクト管理クラス(Base)。直接使用することはできません。
    /// </summary>
    public class clsProductInfoBase : IDisposable
    {

        ~clsProductInfoBase()
        {
            Dispose();
        }

        public void Dispose()
        {
            lock (this)
            {
                if (myTBLVersion__ != null)
                {
                    myTBLVersion__.Dispose();
                    myTBLVersion__ = null;
                }
            }
        }

        //######################################################
        //
        //  field
        //
        //######################################################

        protected string mySystemID__ = "";
        protected string mySystemName__ = "";
        protected DataTable myTBLVersion__ = null;
        protected string myCopyright_yyyy__ = "";
        protected string myCopyright_Link__ = "";

        protected bool myShowLicense__ = false;
        protected string myLicense__ = "";

        private List<string> myOptionLanguageList__ = new List<string>();
        public List<string> xOptionLanguageList
        {
            get { return myOptionLanguageList__; }
            set { myOptionLanguageList__ = value; }
        }



        //######################################################
        //
        //  public function
        //
        //######################################################

        public virtual string xSystemID
        {
            get { return mySystemID__; }
            set { mySystemID__ = value; }
        }

        public virtual string xSystemName
        {
            get { return mySystemName__; }
            set { mySystemName__ = value; }
        }

        public virtual string xCopyright_yyyy
        {
            get { return myCopyright_yyyy__; }
            set { myCopyright_yyyy__ = value; }
        }
        public virtual string xCopyright_Link
        {
            get { return myCopyright_Link__; }
            set { myCopyright_Link__ = value; }
        }

        public virtual string xGetVersionName
        {
            get { return _GetRow()["Version"].ToString(); }
        }

        public virtual string xReleaseDate
        {
            get { return _GetRow()["ReleaseDate"].ToString(); }
        }

        public virtual string xExpireDate
        {
            get { return _GetRow()["ExpireDate"].ToString(); }
        }

        public virtual bool xShowLicense
        {
            get { return myShowLicense__; }
        }
        public virtual string xLicense
        {
            get { return myLicense__; }
        }

        public virtual string[] xGetSystemVersionNumber()
        {
            return _GetSystemVersionNumber();
        }

        public bool xCanUse
        {
            get
            {
                bool canUse = true;
                DateTime toDay = clsSystemInfo.xNow;
                DateTime expireDate = DateTime.ParseExact(_GetRow()["ExpireDate"].ToString() + " 23:59:59", "yyyy/MM/dd HH:mm:ss", null);

                if (toDay > expireDate) canUse = false;

                return canUse;
            }
        }

        //######################################################
        //
        //  private function
        //
        //######################################################

        protected virtual void __Initialize()
        {
            // Version Table 生成
            myTBLVersion__ = new DataTable();
            myTBLVersion__.Columns.Add("SystemID", typeof(string));
            myTBLVersion__.Columns.Add("SystemName", typeof(string));
            myTBLVersion__.Columns.Add("Version", typeof(string));
            myTBLVersion__.Columns.Add("ReleaseDate", typeof(string));
            myTBLVersion__.Columns.Add("ExpireDate", typeof(string));
        }

        private DataRow _GetRow()
        {
            DataRow row = null;
            DataRow[] rows = null;

            try
            {
                DataTable dt = myTBLVersion__.Copy();
                dt.Columns.Add("SEQ", typeof(long));
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string ver = myTBLVersion__.Rows[i]["Version"].ToString();
                    string v0 = Convert.ToInt32(ver.Split('.')[0]).ToString("000");
                    string v1 = Convert.ToInt32(ver.Split('.')[1]).ToString("000");
                    string v2 = Convert.ToInt32(ver.Split('.')[2]).ToString("000");
                    string v3 = Convert.ToInt32(ver.Split('.')[3]).ToString("000");

                    dt.Rows[i]["SEQ"] = Convert.ToInt64(v0 + v1 + v2 + v3);
                }

                rows = dt.Select("", "SEQ DESC");
                if (rows.Length > 0) row = rows[0];
            }
            catch
            { }

            return row;
        }

        private string[] _GetSystemVersionNumber()
        {
            string[] intRet = new string[4] { "000", "000", "000", "000" };
            DataRow row = null;

            try
            {
                row = _GetRow();
                if (row != null)
                {
                    intRet[0] = Convert.ToInt32(row["Version"].ToString().Split('.')[0]).ToString("000");
                    intRet[1] = Convert.ToInt32(row["Version"].ToString().Split('.')[1]).ToString("000");
                    intRet[2] = Convert.ToInt32(row["Version"].ToString().Split('.')[2]).ToString("000");
                    intRet[3] = Convert.ToInt32(row["Version"].ToString().Split('.')[3]).ToString("000");
                }
            }
            catch
            { }

            return intRet;
        }
    }
}
