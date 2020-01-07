using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Buono.BuonoCore;
using System.Data;

namespace Portal.Controllers
{
    public class RestServer : ControllerBase
    {
        [HttpPost]
        public ContentResult Create(string pInput)
        {
            clsAnswer para = (clsAnswer)Newtonsoft.Json.JsonConvert.DeserializeObject(pInput, typeof(clsAnswer));
            
            
            DataTable dt = new DataTable("TBL0");
            dt.Columns.Add(new DataColumn("設定項目ID", typeof(string)));
            dt.Columns.Add(new DataColumn("設定値", typeof(string)));

            for (int i = 0; i < 10; i++)
            {
                dt.Rows.Add(new string[]
                {
                    i.ToString("00000"), i.ToString("00000") + "あいうえお"
                });
            }

            ////var result = Newtonsoft.Json.JsonConvert.SerializeObject(response);

            ////var ss = Newtonsoft.Json.JsonConvert.DeserializeObject(result, typeof(DataTable));

            clsAnswer rr = new clsAnswer();
            rr.xAAA = "あいうえお";
            rr.xBBB = para.xBBB;
            rr.xCCC = dt.Copy();

            string result = Newtonsoft.Json.JsonConvert.SerializeObject(rr);

            return Content(result);
        }


        [HttpGet]
        public ContentResult GetAll(string pInput)
        {
            DataTable dt = new DataTable("TBL0");
            dt.Columns.Add(new DataColumn("設定項目ID", typeof(string)));
            dt.Columns.Add(new DataColumn("設定値", typeof(string)));

            for (int i = 0;i<10;i++)
            {
                dt.Rows.Add(new string[]
                {
                    i.ToString("00000"), i.ToString("00000") + "あいうえお"
                });
            }

            ////var result = Newtonsoft.Json.JsonConvert.SerializeObject(response);

            ////var ss = Newtonsoft.Json.JsonConvert.DeserializeObject(result, typeof(DataTable));

            clsAnswer rr = new clsAnswer();
            rr.xAAA = "あいうえお";
            rr.xBBB = "abcdefg";
            rr.xCCC = dt;
            var result = Newtonsoft.Json.JsonConvert.SerializeObject(rr);

            ////clsAnswer ee = (clsAnswer)Newtonsoft.Json.JsonConvert.DeserializeObject(result, typeof(clsAnswer));
            return Content(result);
        }
    }

    public class clsAnswer
    {
        private string AAA = "";
        public string xAAA
        {
            get { return AAA; }
            set { AAA = value; }
        }

        private string BBB = "";
        public string xBBB
        {
            get { return BBB; }
            set { BBB = value; }
        }

        private DataTable CCC = new DataTable("TBL0");
        public DataTable xCCC
        {
            get { return CCC; }
            set { CCC = value; }
        }
    }
}
