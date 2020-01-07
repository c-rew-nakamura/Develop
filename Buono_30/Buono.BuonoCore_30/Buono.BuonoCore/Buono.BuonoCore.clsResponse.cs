using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Buono.BuonoCore
{
    [Serializable]
    public class clsResponse : IDisposable
    {
        public clsResponse()
        {
            _Initialize();
        }

        ~clsResponse()
        {
            Dispose();
        }

        public void Dispose()
        {
            lock (this)
            {
                myTBLResponse_ = null;
                if (myDSResponse_ != null)
                {
                    for (int i = 0; i < myDSResponse_.Tables.Count; i++)
                    {
                        if (myDSResponse_.Tables[i] != null)
                        {
                            myDSResponse_.Tables[i].Rows.Clear();
                            myDSResponse_.Tables[i].Columns.Clear();
                            myDSResponse_.Tables[i].Dispose();
                            myDSResponse_.Tables.Remove(myDSResponse_.Tables[i]);
                        }
                    }
                }
                myTBLResponse_ = null;
                myFile_ = null;
            }
        }

        //####################################################################
        //
        // field
        //
        //####################################################################

        private bool myHasError_ = false;
        /// <summary>
        /// エラーの有無を取得する。
        /// true:エラー有　false:エラーなし
        /// </summary>
        public bool xHasError
        {
            get { return myHasError_; }
        }

        private string myMessage_ = "";
        /// <summary>
        /// メッセージを取得、設定する。
        /// </summary>
        public string xMessage
        {
            get { return myMessage_; }
            set { myMessage_ = value; }
        }

        // 戻り値の入れ物
        //private Data.DataTable myTBLRet_;
        private object myTBLResponse_;

        // 戻り値テーブルの入れ物
        private DataSet myDSResponse_;

        // 転送ファイル関連
        private object myFile_;
        /// <summary>
        /// ダウンロードファイルを取得、設定する。
        /// </summary>
        public byte[] xDownloadFile
        {
            get { return (byte[])myFile_; }
            set { myFile_ = (object)value; }
        }

        private string myFileName_ = "";
        /// <summary>
        /// ダウンロードファイル名を取得、設定する。
        /// </summary>
        public string xDownloadFileName
        {
            get { return myFileName_; }
            set { myFileName_ = value; }
        }

        //####################################################################
        //
        // function
        //
        //####################################################################

        private void _Initialize()
        {
            // TBLRet
            DataTable dt = new DataTable();
            DataColumn column = null;
            dt = new DataTable();
            dt.TableName = "TBLRet";

            column = new DataColumn("SEQ", typeof(long));
            column.AutoIncrement = true;
            dt.Columns.Add(column);

            column = new DataColumn("ITEM", typeof(string));
            column.DefaultValue = "";
            dt.Columns.Add(column);

            column = new DataColumn("VALUE", typeof(string));
            column.DefaultValue = "";
            dt.Columns.Add(column);

            myTBLResponse_ = dt;

            myDSResponse_ = new DataSet();
        }

        /// <summary>
        /// エラーメッセージを設定する。
        /// </summary>
        /// <param name="pErrorMessage">エラーメッセージ</param>

        public void xSetError(string pErrorMessage)
        {
            myMessage_ = pErrorMessage;
            myHasError_ = true;
        }

        /// <summary>
        /// テーブルを追加する。
        /// </summary>
        /// <param name="pDataTable">追加対象テーブル</param>
        /// <returns>bool</returns>
        public bool xAddDataTable(DataTable pDataTable)
        {
            bool ret = false;
            if (myDSResponse_.Tables.Contains(pDataTable.TableName) == true)
            {
                ret = false;
            }
            else
            {
                try
                {
                    myDSResponse_.Tables.Add(pDataTable.Copy());
                    ret = true;
                }
                catch
                {
                    ret = false;
                }
            }

            return ret;
        }

        /// <summary>
        /// テーブル取得処理
        /// </summary>
        /// <param name="pTableName">テーブル名</param>
        /// <returns>DataTable</returns>


        public DataTable xGetDataTable(string pTableName)
        {
            if (myDSResponse_.Tables.Contains(pTableName) == false)
            {
                return null;
            }

            DataTable dt = new DataTable();
            dt = myDSResponse_.Tables[pTableName].Copy();

            return dt;
        }
        /// <summary>
        /// Index=0のテーブルを返す。
        /// </summary>
        /// <returns>DataTable</returns>
        public DataTable xGetDataTable()
        {
            int index = 0;
            if (index < myDSResponse_.Tables.Count)
            { }
            else
            {
                return null;
            }

            DataTable dt = new DataTable();
            dt = myDSResponse_.Tables[index];

            return dt.Copy();
        }
        /// <summary>
        /// 指定したIndexのテーブルを返す。
        /// </summary>
        /// <param name="TableIndex">テーブルのIndex</param>
        /// <returns>DataTable</returns>
        public DataTable xGetDataTable(int pTableIndex)
        {
            int index = pTableIndex;
            if (index < myDSResponse_.Tables.Count)
            { }
            else
            {
                return null;
            }

            DataTable dt = new DataTable();
            dt = myDSResponse_.Tables[index];

            return dt.Copy();
        }

        /// <summary>
        /// 処理開始時刻を設定する。
        /// </summary>
        public void xSetStartTime()
        {
            _SetStartTime();
        }
        private void _SetStartTime()
        {
            DataRow row = null;
            DataRow[] rows = null;

            DataTable dt = (DataTable)myTBLResponse_;
            rows = dt.Select("ITEM = '-- StartTime' ");
            if (rows.Length > 0)
            {
                foreach (DataRow dr in rows)
                {
                    dt.Rows.Remove(dr);
                }
            }

            row = dt.NewRow();
            row["ITEM"] = "-- StartTime";
            row["VALUE"] = clsSystemInfo.xNow.ToString("yyyy/MM/dd HH:mm:ss fff");
            dt.Rows.Add(row);
        }

        /// <summary>
        /// 処理終了時刻を設定する。
        /// </summary>
        public void xSetEndTime()
        {
            _SetEndTime();
        }
        private void _SetEndTime()
        {
            DataRow row = null;
            DataRow[] rows = null;

            DataTable dt = (DataTable)myTBLResponse_;
            rows = dt.Select("ITEM = '-- EndTime' ");
            if (rows.Length > 0)
            {
                foreach (DataRow dr in rows)
                {
                    dt.Rows.Remove(dr);
                }
            }

            row = dt.NewRow();
            row["ITEM"] = "-- EndTime";
            row["VALUE"] = clsSystemInfo.xNow.ToString("yyyy/MM/dd HH:mm:ss fff");
            dt.Rows.Add(row);
        }

        /// <summary>
        /// 戻り値を設定する。
        /// </summary>
        /// <param name="pITEM">項目名</param>
        /// <param name="pVALUE">項目値</param>
        public void xSetReturn(string pITEM, string pVALUE)
        {
            _SetReturn(pITEM, pVALUE);
        }
        private void _SetReturn(string pITEM, string pVALUE)
        {
            DataRow row = null;
            DataRow[] rows = null;

            DataTable dt = (DataTable)myTBLResponse_;
            rows = dt.Select("ITEM = " + "'" + pITEM + "'");
            if (rows.Length > 0)
            {
                foreach (DataRow dr in rows)
                {
                    dt.Rows.Remove(dr);
                }
            }

            row = dt.NewRow();
            row["ITEM"] = pITEM;
            row["VALUE"] = pVALUE;
            dt.Rows.Add(row);
        }

        /// <summary>
        /// 戻り値を取得する。
        /// </summary>
        /// <param name="pITEM">項目名</param>
        /// <returns>string 項目値</returns>
        public string xGetReturn(string pITEM)
        {
            return _GetReturn(pITEM);
        }
        private string _GetReturn(string pITEM)
        {
            string ret = "";
            DataRow[] rows = null;

            DataTable dt = (DataTable)myTBLResponse_;
            rows = dt.Select("ITEM = " + "'" + pITEM + "'");
            if (rows.Length > 0)
            {
                ret = rows[0]["VALUE"].ToString();
            }

            return ret;
        }
    }

}
