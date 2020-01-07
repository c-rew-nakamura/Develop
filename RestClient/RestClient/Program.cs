using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System;
using System.Data;
using System.IO;
using System.Net;

namespace RestClient0
{
    class Program
    {
        private static Program me_;
        static void Main(string[] args)
        {
            me_ = new Program();
            me_._Test();
        }

        private void _Test_Get()
        {

            // クライアント＆リクエストの作成
            var client = new RestClient();
            var request = new RestRequest();

            // URLの設定
            client.BaseUrl = new Uri("https://localhost:44348/RestServer/GetAll");
            //client.BaseUrl = new Uri("https://www.c-rew.com/RestServer/GetAll");

            // メソッド、パラメータの指定
            request.Method = Method.GET;
            

            DataTable dt = new DataTable("TBL0");
            dt.Columns.Add(new DataColumn("設定項目ID", typeof(string)));
            dt.Columns.Add(new DataColumn("設定値", typeof(string)));

            for (int i = 0; i < 3; i++)
            {
                dt.Rows.Add(new string[]
                {
                    i.ToString("00000"), i.ToString("00000") + "あいうえお"
                });
            }
            clsAnswer cls = new clsAnswer();
            cls.xAAA = "あいうえお";
            cls.xBBB = "abcdefg";
            cls.xCCC = dt.Copy();

            string para = Newtonsoft.Json.JsonConvert.SerializeObject(cls);
            request.AddParameter("pInput", para, ParameterType.GetOrPost);
            // ParameterTypeはいろいろあるが、GETとPOSTで特に指定なく
            // stringパラメータを設定する場合は、GetOrPost

            ////////// ファイルをアップロードする場合
            ////////request.AddFile("ファイルパラメータ名", "ファイルパス", "ContentType");
            ////////// ContentTypeは拡張子などから適切なものを選ぶ

            // リクエスト送信
            var response = client.Execute(request);

            clsAnswer rr = (clsAnswer)Newtonsoft.Json.JsonConvert.DeserializeObject(response.Content, typeof(clsAnswer));

            
            ////////// レスポンスがファイルなどで、復元したい場合
            ////////File.WriteAllBytes("出力先のパス", response.RawBytes);

            // レスポンスのステータスコードが欲しいなどの場合
            bool isOK = response.StatusCode == HttpStatusCode.OK;
            // ステータスコード以外にも様々な情報がresponseに入っているので適宜

        }


        private void _Test()
        {

            // クライアント＆リクエストの作成
            var client = new RestClient();
            var request = new RestRequest();

            // URLの設定
            //client.BaseUrl = new Uri("https://localhost:44348/RestServer/Create");
            client.BaseUrl = new Uri("https://www.c-rew.com/RestServer/Create");

            // メソッド、パラメータの指定
            request.Method = Method.POST;

            DataTable dt = new DataTable("TBL0");
            dt.Columns.Add(new DataColumn("設定項目ID", typeof(string)));
            dt.Columns.Add(new DataColumn("設定値", typeof(string)));

            for (int i = 0; i < 3; i++)
            {
                dt.Rows.Add(new string[]
                {
                    i.ToString("00000"), i.ToString("00000") + "あいうえお"
                });
            }
            clsAnswer cls = new clsAnswer();
            cls.xAAA = "あいうえお";
            cls.xBBB = "abcdefg";
            cls.xCCC = dt.Copy();

            string para = Newtonsoft.Json.JsonConvert.SerializeObject(cls);
            request.AddParameter("pInput", para, ParameterType.GetOrPost);
            // ParameterTypeはいろいろあるが、GETとPOSTで特に指定なく
            // stringパラメータを設定する場合は、GetOrPost

            ////////// ファイルをアップロードする場合
            ////////request.AddFile("ファイルパラメータ名", "ファイルパス", "ContentType");
            ////////// ContentTypeは拡張子などから適切なものを選ぶ

            // リクエスト送信
            DateTime stTime = DateTime.Now;
            var response = client.Execute(request);
            DateTime edTime = DateTime.Now;

            clsAnswer rr = (clsAnswer)Newtonsoft.Json.JsonConvert.DeserializeObject(response.Content, typeof(clsAnswer));
            Console.WriteLine(rr.xCCC.Rows[1][1].ToString());

            TimeSpan ts = edTime - stTime;
            Console.WriteLine("処理時間:" + ts.TotalMilliseconds.ToString("#0.# ms"));

            ////////// レスポンスがファイルなどで、復元したい場合
            ////////File.WriteAllBytes("出力先のパス", response.RawBytes);

            // レスポンスのステータスコードが欲しいなどの場合
            bool isOK = response.StatusCode == HttpStatusCode.OK;
            // ステータスコード以外にも様々な情報がresponseに入っているので適宜

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
