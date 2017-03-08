using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core;
using Core.Entities;
using NReadability;

namespace NewsAspNet.Controllers
{
    public class HomeController : Controller
    {
        NewsContext db = new NewsContext();
        static string _login = "a";//string.Empty;
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public bool AddLogin(string login)
        {
            if(db.TLogin.Any(_=>_.Login==login))
                new System.Net.WebException("user == null");
            var newLogin = new TLogin()
            {
                Login = login
            };
            db.TLogin.Add(newLogin);
            db.SaveChanges();
            return true;
        }

        [HttpPost]
        public bool AddSource(int type, string data, string login)
        {
            var user = db.TLogin.FirstOrDefault(_ => _.Login == login);
            if (user == null) new System.Net.WebException("user == null");

            var Source = new TSource()
            {
                Type = type,
                Link = data
            };
            user.TSources.Add(Source);
            db.SaveChanges();
            ViewBag.Message = "Your application description page.";
            return true;
        }

        public JsonResult Contact()
        {
            var news=ParseNRead("https://meduza.io");
            ViewBag.Message = news.ToString();
            return Json(news);
            
        }
       List<string> ParseNRead(string link)
        {
            var urls= NRParser.GetInfoBySearchStr(link);
            var RawItems =new List<string>();
            foreach (var url in urls)
            {
                var transcoder = new NReadabilityWebTranscoder();
                bool success;

                string transcodedContent =transcoder.Transcode(url, out success);

                if (success)
                {
                    var user = db.TLogin.FirstOrDefault(_ => _.Login == _login);
                    if (user == null) new System.Net.WebException("user == null");

                    var Article = new TArticle()
                    {
                        Data = transcodedContent,
                        Date=DateTime.Now
                    };
                    user.TSources.First()
                        .TArticles.Add(Article);

                    db.SaveChanges();
                    RawItems.Add(transcodedContent);//todo
                }
            }
            return RawItems;
        }
    }
}