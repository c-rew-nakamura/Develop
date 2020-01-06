using Buono.BuonoCore;
using System;
using System.Collections.Generic;
using System.Text;
using Npgsql;
using System.Data;
using System.IO;
using System.Threading;

namespace Buono.BuonoDB
{
    internal class clsPostgreSQL : clsDBBase, IDisposable
    {
        internal clsPostgreSQL(clsDBInfo pDBInfo)
        {
            myDBInfo_ = pDBInfo;
        }
        ~clsPostgreSQL()
        {
            this.Dispose();
        }
        public override void Dispose()
        {
            _DBClose();
        }

        //####################################################################
        //
        // field
        //
        //####################################################################

        private clsDBInfo myDBInfo_;

        ////private OleDbConnection myConnection_;
        ////private OleDbDataReader myReader_;
        ////private OleDbTransaction myTransaction_;
        private NpgsqlConnection myConnection_;
        private NpgsqlDataReader myReader_;
        private NpgsqlTransaction myTransaction_;

        private DataTable myTable_;


        //####################################################################
        //
        // function
        //
        //####################################################################

        internal override clsResponse xDBOpen()
        {
            return _DBOpen();
        }
        private clsResponse _DBOpen()
        {
            clsResponse response = new clsResponse();
            string msg = "";

            NpgsqlConnectionStringBuilder sb = new NpgsqlConnectionStringBuilder();
            sb.Host     = myDBInfo_.xDBServer;
            sb.Port     = Convert.ToInt32(myDBInfo_.xDBPort);
            sb.Database = myDBInfo_.xDBName;
            sb.Username = myDBInfo_.xDBUserID;
            sb.Password = myDBInfo_.xDBPassword;
            if (myDBInfo_.xDBSslMode == "true")
            {
                sb.SslMode = SslMode.Require;
            }
            
            sb.Timeout = 20;
            sb.CommandTimeout = 20;
            
            myConnection_ = new NpgsqlConnection(sb.ConnectionString);

            if (myDBInfo_.xDBReadOnly)
            {
                myConnection_.ConnectionString += "ApplicationIntent=ReadOnly;";
            }

            try
            {
                myConnection_.Open();
            }
            catch (Exception ex)
            {
                msg = "ＤＢのオープンに失敗しました。";
                msg += ex.Message + Environment.NewLine;
                response.xSetError(msg);
                msg += ex.StackTrace;
                ////clsTextLogger.xWriteTextLog(
                ////    MethodBase.GetCurrentMethod().DeclaringType.FullName + "."
                ////    + MethodBase.GetCurrentMethod().Name, msg);
            }

            return response;
        }

        internal override void xDBClose()
        {
            _DBClose();
        }
        private void _DBClose()
        {
            try
            {
                if (myConnection_ != null)
                {
                    try
                    {
                        myConnection_.Close();
                        myConnection_.Dispose();
                    }
                    catch { }
                }
                if (myTransaction_ != null)
                {
                    myTransaction_.Dispose();
                }
            }
            catch (Exception ex)
            { }
            finally
            {
                if (myConnection_ != null)
                {
                    myConnection_.Dispose();
                    myConnection_ = null;
                }
                if (myTransaction_ != null)
                {
                    myTransaction_.Dispose();
                    myTransaction_ = null;
                }
            }
        }

        internal override clsResponse xSelect(string SQL)
        {
            return _Select(SQL);
        }
        private clsResponse _Select(string pSQL)
        {
            clsResponse response = new clsResponse();
            ////OleDbDataAdapter da = null;
            NpgsqlDataAdapter da = null;
            DataTable dt = null;
            string msg = "";
            int count = 0;

            ////OleDbCommand command = new OleDbCommand();
            NpgsqlCommand command = new NpgsqlCommand();
            command.Connection = this.myConnection_;
            command.Transaction = this.myTransaction_;
            command.CommandText = pSQL;

            command.CommandType = CommandType.Text;

            // 2018/01/13
            command.CommandTimeout = 300;

            try
            {
                ////da = new OleDbDataAdapter(command);
                da = new NpgsqlDataAdapter(command);
                dt = new DataTable();
                count = da.Fill(dt);
                response.xAddDataTable(dt);

                // 2017/12/13 K.Nakamura
                ////////msg = "SQL=" + pSQL;
                ////////clsTextLogger.xWriteTextLog(
                ////////    MethodBase.GetCurrentMethod().DeclaringType.FullName + "." +
                ////////    MethodBase.GetCurrentMethod().Name, msg);
                ////////msg = "取得件数=" + count.ToString("#,##0 件");
                ////////clsTextLogger.xWriteTextLog(
                ////////    MethodBase.GetCurrentMethod().DeclaringType.FullName + "." +
                ////////    MethodBase.GetCurrentMethod().Name, msg);

            }
            catch (Exception ex)
            {
                msg = "SELECTに失敗しました。";
                msg += "(" + myDBInfo_.xDBName + ")" + "(" + pSQL + ")" + Environment.NewLine;
                msg += ex.Message + Environment.NewLine;
                response.xSetError(msg);
                msg += ex.StackTrace;
                ////////clsTextLogger.xWriteTextLog(
                ////////    MethodBase.GetCurrentMethod().DeclaringType.FullName + "." +
                ////////    MethodBase.GetCurrentMethod().Name, msg);
            }
            finally
            {
                response.xSetReturn("-- ResultCount", count.ToString());
                if (command != null)
                {
                    command.Connection = null;
                    command.Transaction = null;
                    command.Dispose();
                    command = null;
                }
                if (da != null)
                {
                    da.Dispose();
                    da = null;
                }
                if (dt != null)
                {
                    dt.Rows.Clear();
                    dt.Columns.Clear();
                    dt.Dispose();
                    dt = null;
                }
            }

            return response;
        }

        internal override clsResponse xSelectToFile(string pSQL)
        {
            return _SelectToFile(pSQL);
        }
        private clsResponse _SelectToFile(string pSQL)
        {
            clsResponse response = new clsResponse();
            string msg = "";
            ////////string fileName = Path.Combine(
            ////////    clsSystemInfo.xTempDirectory, "./file.xml");
            ////////try
            ////////{
            ////////    ////OleDbDataAdapter da = new OleDbDataAdapter(pSQL, this.myConnection_);
            ////////    SqlDataAdapter da = new SqlDataAdapter(pSQL, this.myConnection_);
            ////////    DataSet ds = new DataSet();
            ////////    da.Fill(ds);
            ////////    ds.WriteXml(fileName, XmlWriteMode.IgnoreSchema);
            ////////    response.xSetReturn("FileName", fileName);
            ////////}
            ////////catch (Exception ex)
            ////////{
            ////////    msg = "SELECTに失敗しました。";
            ////////    msg += "(" + myDBInfo_.xDBName + ")" + "(" + pSQL + ")" + Environment.NewLine;
            ////////    msg += ex.Message + Environment.NewLine;
            ////////    response.xSetError(msg);
            ////////    msg += ex.StackTrace;
            ////////    clsTextLogger.xWriteTextLog(
            ////////        MethodBase.GetCurrentMethod().DeclaringType.FullName + "." +
            ////////        MethodBase.GetCurrentMethod().Name, msg);
            ////////}

            return response;
        }

        internal override clsResponse xExecuteSQL(string pSQL)
        {
            return _ExecuteSQL(pSQL, false, 0);
        }
        internal override clsResponse xExecuteSQL(string pSQL, int pCommandTimeout)
        {
            return _ExecuteSQL(pSQL, false, pCommandTimeout);
        }

        internal override clsResponse xExecuteSQL(string pSQL, bool pWithTtansaction)
        {
            return _ExecuteSQL(pSQL, pWithTtansaction, 0);
        }
        internal override clsResponse xExecuteSQL(string pSQL, bool pWithTtansaction, int pCommandTimeout)
        {
            return _ExecuteSQL(pSQL, pWithTtansaction, pCommandTimeout);
        }
        private clsResponse _ExecuteSQL(string pSQL, bool pWithTransaction, int pCommandTimeout)
        {
            clsResponse response = new clsResponse();
            string msg = "";

            ////OleDbCommand command = new OleDbCommand();
            NpgsqlCommand command = new NpgsqlCommand();
            int count = 0;

            try
            {
                if (pWithTransaction)
                {
                    myTransaction_ = myConnection_.BeginTransaction();
                }
                if (pCommandTimeout > 0)
                {
                    command.CommandTimeout = pCommandTimeout;
                }
                else
                {
                    // 2018/01/13
                    command.CommandTimeout = 300;
                }
                command.Connection = myConnection_;
                command.Transaction = myTransaction_;
                command.CommandText = pSQL;
                command.CommandType = CommandType.Text;
                count = command.ExecuteNonQuery();
                response.xSetReturn("-- ResultCount", count.ToString());
                if (pWithTransaction)
                {
                    myTransaction_.Commit();
                }
            }
            catch (Exception ex)
            {
                if (pWithTransaction) myTransaction_.Rollback();
                msg = "SQLの実行に失敗しました。" + Environment.NewLine;
                msg += "(" + myDBInfo_.xDBName + ")" + "(" + pSQL + ")" + Environment.NewLine;
                msg += ex.Message + Environment.NewLine;
                response.xSetError(msg);
                msg += ex.StackTrace;
                ////////clsTextLogger.xWriteTextLog(
                ////////    MethodBase.GetCurrentMethod().DeclaringType.FullName + "." +
                ////////    MethodBase.GetCurrentMethod().Name, msg);
            }
            finally
            {
                if (command != null)
                {
                    command.Dispose();
                    command = null;
                }
            }

            return response;
        }

        internal override clsResponse xGetTableList(string pWhere)
        {
            return _GetTableList(pWhere);
        }
        private clsResponse _GetTableList(string pWhere)
        {
            clsResponse response = new clsResponse();

            string SQL = "SELECT name FROM sys.objects";
            SQL += " where type = 'U'";
            if (pWhere.Length > 0)
            {
                SQL += " and name like " + "'" + pWhere + "'";
            }

            SQL += " order by name";
            response = _Select(SQL);
            if (response.xHasError)
            { }
            else
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("テーブル名", typeof(string));
                for (int i = 0; i < response.xGetDataTable(0).Rows.Count; i++)
                {
                    DataRow row = dt.NewRow();
                    row["テーブル名"] = response.xGetDataTable(0).Rows[i]["name"].ToString();
                    dt.Rows.Add(row);
                }
                response = new clsResponse();
                response.xAddDataTable(dt);
            }

            return response;
        }

        internal override clsResponse xGetVersion()
        {
            clsResponse response = new clsResponse();
            string SQL = "SELECT * FROM PRODUCT_COMPONENT_VERSION";
            SQL += " where PRODUCT like 'Oracle%'";

            response = _Select(SQL);
            if (response.xHasError)
            { }
            else
            {
                string version = "";
                version += response.xGetDataTable(0).Rows[0]["PRODUCT"].ToString();
                version += " " + response.xGetDataTable(0).Rows[0]["VERSION"].ToString();
                version += " " + response.xGetDataTable(0).Rows[0]["STATUS"].ToString();
                response.xSetReturn("Version", version);
            }

            return response;
        }

        internal override clsResponse xBeginTrans()
        {
            clsResponse response = new clsResponse();
            try
            {
                myTransaction_ = myConnection_.BeginTransaction();
            }
            catch (Exception ex)
            {
                response.xSetError(ex.Message + ex.StackTrace);
            }

            return response;
        }

        internal override clsResponse xCommit()
        {
            clsResponse response = new clsResponse();
            try
            {
                myTransaction_.Commit();
            }
            catch (Exception ex)
            {
                response.xSetError(ex.Message + ex.StackTrace);
            }

            return response;
        }

        internal override clsResponse xRollBack()
        {
            clsResponse response = new clsResponse();
            try
            {
                myTransaction_.Rollback();
            }
            catch (Exception ex)
            {
                response.xSetError(ex.Message + ex.StackTrace);
            }

            return response;
        }

        internal override clsResponse xReaderOpen(string pSQL)
        {
            return _ReaderOpen(pSQL);
        }
        private clsResponse _ReaderOpen(string pSQL)
        {
            clsResponse response = new clsResponse();

            try
            {
                ////OleDbCommand command = new OleDbCommand(pSQL, this.myConnection_, this.myTransaction_);
                NpgsqlCommand command = new NpgsqlCommand(pSQL, this.myConnection_, this.myTransaction_);
                ////myReader_ = (OleDbDataReader)command.ExecuteReader();
                myReader_ = (NpgsqlDataReader)command.ExecuteReader();
            }
            catch (Exception ex)
            {
                string msg = "DataReaderのOpenに失敗しました。";
                msg += "(" + myDBInfo_.xDBName + ")" + "(" + pSQL + ")" + Environment.NewLine;
                msg += ex.Message + Environment.NewLine;
                response.xSetError(msg);
                msg += ex.StackTrace;
                ////////clsTextLogger.xWriteTextLog(
                ////////    MethodBase.GetCurrentMethod().DeclaringType.FullName + "." +
                ////////    MethodBase.GetCurrentMethod().Name, msg);
            }
            finally
            {
                myTable_ = null;
            }

            return response;
        }

        internal override void xReaderClose()
        {
            _ReaderClose();
        }
        private void _ReaderClose()
        {
            try
            {
                myReader_.Close();
            }
            catch (Exception ex)
            {
                string msg = "DataReaderのCloseに失敗しました。";
                msg += ex.Message + Environment.NewLine;
                msg += ex.StackTrace;
                ////////clsTextLogger.xWriteTextLog(
                ////////    MethodBase.GetCurrentMethod().DeclaringType.FullName + "." +
                ////////    MethodBase.GetCurrentMethod().Name, msg);
            }
            finally
            {
                myReader_ = null;
                myTable_ = null;
            }
        }

        internal override clsResponse xReaderRead()
        {
            return _ReaderRead();
        }
        private clsResponse _ReaderRead()
        {
            clsResponse response = new clsResponse();
            string msg = "";
            DataTable dt = null;

            try
            {
                if (myReader_.Read())
                {
                    if (myTable_ == null)
                    {
                        myTable_ = new DataTable();
                        dt = myReader_.GetSchemaTable();
                        DataColumn column = null;
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            column = new DataColumn();
                            column.ColumnName = dt.Rows[i]["ColumnName"].ToString();
                            column.DataType = Type.GetType(dt.Rows[i]["DataType"].ToString());
                            myTable_.Columns.Add(column);
                        }
                    }


                    dt = myTable_.Clone();
                    DataRow row = dt.NewRow();
                    for (int i = 0; i < myReader_.FieldCount; i++)
                    {
                        row[i] = myReader_[i];
                    }
                    dt.Rows.Add(row);

                    response.xAddDataTable(dt);
                }
                else
                {
                    response.xSetReturn("EOF", "True");
                    _ReaderClose();
                    return response;
                }
            }
            catch (Exception ex)
            {
                msg = "DataReaderからの読み込みに失敗しました。";
                msg += ex.Message + Environment.NewLine;
                response.xSetError(msg);
                msg += ex.StackTrace;
                ////////clsTextLogger.xWriteTextLog(
                ////////    MethodBase.GetCurrentMethod().DeclaringType.FullName + "." +
                ////////    MethodBase.GetCurrentMethod().Name, msg);
                _ReaderClose();
                return response;
            }

            return response;
        }

        internal override clsResponse xToInsertString(string pTableName, string pOutDirName, string pOutFileName)
        {
            return _ToInsertString(pTableName, pOutDirName, pOutFileName);
        }
        private clsResponse _ToInsertString(string pTableName, string pOutDirName, string pOutFileName)
        {
            clsResponse response = new clsResponse();
            string msg = "";

            string SQL = "";
            string items = "";
            string values = "";
            long count = 0;
            string fileName = "";
            StreamWriter sw = null;

            try
            {
                SQL = "Select * from " + pTableName;
                ////OleDbCommand command = new OleDbCommand();
                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = this.myConnection_;
                command.CommandText = SQL;
                ////OleDbDataReader dr = command.ExecuteReader();
                NpgsqlDataReader dr = command.ExecuteReader();

                fileName = Path.GetFullPath(Path.Combine(pOutDirName, pOutFileName));
                sw = new StreamWriter(fileName, false, Encoding.Default);
                count = 0;
                while (dr.Read())
                {
                    Thread.Sleep(0);
                    count++;
                    if (count == 1)
                    {
                        items = "";
                        for (int i = 0; i < dr.FieldCount; i++)
                        {
                            if (i > 0) items += ",";
                            items += dr.GetName(i);
                        }
                    }
                    values = "";
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        if (i > 0) values += ",";
                        values += "N'" + dr.GetValue(i).ToString() + "'";
                    }
                    SQL = "insert into " + pTableName + " (" + items + ") values(" + values + ");";
                    sw.WriteLine(SQL);
                }
            }
            catch (Exception ex)
            {
                msg = "テーブルのバックアップに失敗しました。";
                msg += "(" + pTableName + ")" + Environment.NewLine;
                msg += ex.Message + Environment.NewLine;
                msg += ex.StackTrace;
                ////////clsTextLogger.xWriteTextLog(
                ////////    MethodBase.GetCurrentMethod().DeclaringType.FullName + "." +
                ////////    MethodBase.GetCurrentMethod().Name, msg);
                response.xSetError(msg);
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                    sw = null;
                }
            }

            return response;
        }

        internal override clsResponse xToTabString(string pTableName, string pOutDirName, string pOutFileName)
        {
            return _ToTabString(pTableName, pOutDirName, pOutFileName);
        }
        private clsResponse _ToTabString(string pTableName, string pOutDirName, string pOutFileName)
        {
            clsResponse response = new clsResponse();
            string msg = "";

            string SQL = "";
            string items = "";
            string values = "";
            long count = 0;
            string fileName = "";
            StreamWriter sw = null;

            try
            {
                SQL = "Select * from " + pTableName;
                ////OleDbCommand command = new OleDbCommand();
                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = this.myConnection_;
                command.CommandText = SQL;
                ////OleDbDataReader dr = command.ExecuteReader();
                NpgsqlDataReader dr = command.ExecuteReader();

                fileName = Path.GetFullPath(Path.Combine(pOutDirName, pOutFileName));
                sw = new StreamWriter(fileName, false, Encoding.Default);
                count = 0;
                while (dr.Read())
                {
                    Thread.Sleep(0);
                    count++;
                    if (count == 1)
                    {
                        items = "";
                        for (int i = 0; i < dr.FieldCount; i++)
                        {
                            if (i > 0) items += "\t";
                            items += dr.GetName(i);
                        }
                        sw.WriteLine(items);
                    }
                    values = "";
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        if (i > 0) values += "\t";
                        values += dr.GetValue(i).ToString();
                    }
                    sw.WriteLine(values);
                }
            }
            catch (Exception ex)
            {
                msg = "テーブルのバックアップに失敗しました。";
                msg += "(" + pTableName + ")" + Environment.NewLine;
                msg += ex.Message + Environment.NewLine;
                response.xSetError(msg);
                msg += ex.StackTrace;
                ////////clsTextLogger.xWriteTextLog(
                ////////    MethodBase.GetCurrentMethod().DeclaringType.FullName + "." +
                ////////    MethodBase.GetCurrentMethod().Name, msg);
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                    sw = null;
                }
            }

            return response;
        }
    }


}
