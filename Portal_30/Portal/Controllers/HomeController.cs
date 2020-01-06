using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Buono.BuonoCore;
using Buono.BuonoDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Portal.Models;
using Portal.Classes;
using Microsoft.AspNetCore.Http;

namespace Portal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            StringBuilder SQL = null;
            clsResponse response = new clsResponse();
            DataTable dt = null;
            DateTime stDate = clsSystemInfo.xNow;
            string session = "";

            session = HttpContext.Session.GetString("eMail");
            if (session == null)
            {
                HttpContext.Session.SetString("eMail", "aaa@bbb.com");
            }

            ViewData["SessionID"] = HttpContext.Session.GetString("eMail");

            ViewData["Title"] = "TOP@" + clsSystemInfo.xSystemID;

            clsDB db = new clsDB(clsDBInfoPool.xGetInstance().xGetDBInfo(cnstDBKind.Sys));
            db.xDBOpen();

            SQL = new StringBuilder();
            SQL.Append("select * from tbl_news ");
            SQL.Append("order by 連番 desc");
            response = db.xSelect(SQL.ToString());
            db.xDBClose();

            dt = response.xGetDataTable();

            DateTime edDate = clsSystemInfo.xNow;

            clsArgs args = new clsArgs();
            args.xAddDataTable(dt);

            ViewData["stGetDataTime"] = stDate.ToString("HH:mm:ss fff");
            ViewData["edGetDataTime"] = edDate.ToString("HH:mm:ss fff");
            
            return View(args);
        }
        public IActionResult Laboratory()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
