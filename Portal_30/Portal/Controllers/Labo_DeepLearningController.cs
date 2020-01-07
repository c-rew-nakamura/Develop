using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Portal.Models;

namespace Portal.Controllers
{
    public class Labo_DeepLearningController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public Labo_DeepLearningController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
