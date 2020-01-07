using System;
using System.Collections.Generic;
using System.Text;

namespace Buono.BuonoDB
{
    public static class clsDBLogger
    {

        //####################################################################
        //
        // private variable
        //
        //####################################################################

        //####################################################################
        //
        // public function
        //
        //####################################################################


        //####################################################################
        //
        // private function
        //
        //####################################################################

        /// <summary>
        /// DBLogを出力する。
        /// </summary>
        /// <param name="Domain"></param>
        /// <param name="UserID"></param>
        /// <param name="MachineName"></param>
        /// <param name="IPAddress"></param>
        /// <param name="JobID"></param>
        /// <param name="SessionID"></param>
        /// <param name="From"></param>
        /// <param name="Message"></param>
        public static void xWriteDBJobLog(
            string Domain,
            string UserID, string MachineName, string IPAddress, string JobID,
            string SessionID, string From, string Message)
        {
            string[] para = new string[7];
            para[0] = UserID;
            para[1] = MachineName;
            para[2] = JobID;
            para[3] = SessionID;
            para[4] = From;
            para[5] = Message;
            para[6] = IPAddress;

            using (clsDBLogWriter writer = new clsDBLogWriter())
            {
                writer.xWriteDBLog(Domain, para);
            }

            para = null;
        }
    }
}
