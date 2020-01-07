using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Portal.Models;
using Portal.Classes;
using Microsoft.AspNetCore.Http;

namespace Portal.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public LoginController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public IActionResult Index()
        {
            string session = HttpContext.Session.GetString("eMail");
            if (session == null) { session = ""; }

            LoginViewModel model = new LoginViewModel();
            model.Email = session;

            return View(model);
        }

        [HttpPost]
        public ActionResult PostLogin(string pEmail, string pPassword)
        {
            HttpContext.Session.SetString("eMail", pEmail);

            LoginViewModel model = new LoginViewModel();
            model.Email = pEmail;
            model.Password = pPassword;
            model.Message = "Email='" + pEmail + "'  " + "Password='" + pPassword + "'";
            model.Message = HttpContext.Session.GetString("eMail");

            return View("Index", model);
        }
    }
}
