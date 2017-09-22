using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SlackBotWebApi.Controllers
{
    public class OAuthController : Controller
    {
        //public IActionResult Index()
        //{
        //    return View();
        //}

        public JsonResult Connect(string code)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            result.Add("status", "ok");
            result.Add("code", code);
            
            return new JsonResult(result);
        }
    }
}