using Buono.BuonoCore;
using System;

namespace Buono.BuonoDB
{
    public abstract class clsDBBase
    {
        public abstract void Dispose();
        internal abstract clsResponse xDBOpen();
        internal abstract void xDBClose();
        internal abstract clsResponse xSelect(string SQL);
        internal abstract clsResponse xSelectToFile(string SQL);
        internal abstract clsResponse xExecuteSQL(string SQL);
        internal abstract clsResponse xExecuteSQL(string SQL, int CommandTimeout);
        internal abstract clsResponse xExecuteSQL(string SQL, bool WithTransaction);
        internal abstract clsResponse xExecuteSQL(string SQL, bool WithTransaction, int CommandTimeout);
        internal abstract clsResponse xReaderOpen(string SQL);
        internal abstract void xReaderClose();
        internal abstract clsResponse xReaderRead();
        internal abstract clsResponse xGetTableList(string Where);
        internal abstract clsResponse xGetVersion();
        internal abstract clsResponse xBeginTrans();
        internal abstract clsResponse xCommit();
        internal abstract clsResponse xRollBack();
        internal abstract clsResponse xToInsertString(string TableName, string OutDirName, string OutFileName);
        internal abstract clsResponse xToTabString(string TableName, string OutDirName, string OutFileName);
    }
}
