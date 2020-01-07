using Buono.BuonoCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Buono.BuonoDB
{
    public class clsDB : IDisposable
    {
        public clsDB(clsDBInfo pDBInfo)
        {
            myDBInfo_ = pDBInfo;
            _Initialize();
        }

        ~clsDB()
        {
            Dispose();
        }
        public void Dispose()
        {
            lock (this)
            {
                if (myDBInfo_ != null) myDBInfo_.Dispose();
                myDBInfo_ = null;
                if (myDB_ != null) myDB_.Dispose();
                myDB_ = null;
            }
        }


        //####################################################################
        //
        // field
        //
        //####################################################################

        private clsDBInfo myDBInfo_;
        public clsDBInfo xDBInfo
        {
            get { return myDBInfo_; }
            set { myDBInfo_ = value; }
        }

        private clsDBBase myDB_;
        public clsDBBase xCurrentDB
        {
            get { return myDB_; }
        }

        //####################################################################
        //
        // function
        //
        //####################################################################

        private void _Initialize()
        {
            if (myDBInfo_.xDBMS == cnstDBMS.DBMS_PostgreSQL)
            {
                myDB_ = new clsPostgreSQL(myDBInfo_);
            }
            else if (myDBInfo_.xDBMS == cnstDBMS.DBMS_Oracle)
            {
                ////myDB_ = new clsOracle(myDBInfo_);
            }
            else if (myDBInfo_.xDBMS == cnstDBMS.DBMS_MSSQLServer)
            {
                myDB_ = new clsSQLServer(myDBInfo_);
            }
        }

        public clsResponse xDBOpen()
        {
            return myDB_.xDBOpen();
        }

        public void xDBClose()
        {
            try { myDB_.xDBClose(); }
            catch { }
            try { myDB_.Dispose(); }
            catch { }
            myDB_ = null;
        }

        public clsResponse xSelect(string SQL)
        {
            return myDB_.xSelect(SQL);
        }

        public clsResponse xSelectToFile(string SQL)
        {
            return myDB_.xSelectToFile(SQL);
        }

        public clsResponse xExecuteSQL(string SQL)
        {
            return myDB_.xExecuteSQL(SQL);
        }
        public clsResponse xExecuteSQL(string SQL, int CommandTimeout)
        {
            return myDB_.xExecuteSQL(SQL, false, CommandTimeout);
        }

        public clsResponse xExecuteSQL(string SQL, bool WithTransaction)
        {
            return myDB_.xExecuteSQL(SQL, WithTransaction);
        }
        public clsResponse xExecuteSQL(string SQL, bool WithTransaction, int CommandTimeout)
        {
            return myDB_.xExecuteSQL(SQL, WithTransaction, CommandTimeout);
        }

        public clsResponse xReaderOpen(string SQL)
        {
            return myDB_.xReaderOpen(SQL);
        }

        public void xReaderClose()
        {
            myDB_.xReaderClose();
        }

        public clsResponse xReaderRead()
        {
            return myDB_.xReaderRead();
        }

        public clsResponse xGetTableList(string Where)
        {
            return myDB_.xGetTableList(Where);
        }

        public clsResponse xGetVersion()
        {
            return myDB_.xGetVersion();
        }

        public clsResponse xBeginTrans()
        {
            return myDB_.xBeginTrans();
        }

        public clsResponse xCommit()
        {
            return myDB_.xCommit();
        }

        public clsResponse xRollBack()
        {
            return myDB_.xRollBack();
        }

        public clsResponse xToInsertString(string TableName, string OutDirName, string OutFileName)
        {
            return myDB_.xToInsertString(TableName, OutDirName, OutFileName);
        }

        public clsResponse xToTabString(string TableName, string OutDirName, string OutFileName)
        {
            return myDB_.xToTabString(TableName, OutDirName, OutFileName);
        }
    }
}
