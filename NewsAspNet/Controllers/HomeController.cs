using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core;
using Core.Entities;
using NReadability;
using Core.Enums;
using Newtonsoft.Json;
using NLog;
using System.Threading;
using System.Threading.Tasks;

namespace NewsAspNet.Controllers
{
    public class HomeController : Controller
    {
        NewsContext db = new NewsContext();
        static string _login = "a";//string.Empty;
        static DateTime lastGoogleReq = DateTime.Now.AddMinutes(-26);
        static Logger log = LogManager.GetCurrentClassLogger();

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]//old
        public bool AddLogin(string login)
        {
            if (db.TLogin.Any(_ => _.Login == login))
                return false;//System.Net.WebException("user existed");
            var newLogin = new TLogin()
            {
                Login = login
            };
            db.TLogin.Add(newLogin);
            db.SaveChanges();
            return true;
        }

        public ActionResult AddSource(string path, SourceType? type, string login)
        {

            var user = db.TLogin.FirstOrDefault(_ => _.Login == login);
            if (user == null) //return RedirectToAction("Index");// false;//todo
            {
                user = new TLogin()
                {
                    Login = login
                };
                db.TLogin.Add(user);
            }
            var alreadyExist = user.TSources.Any(_ => _.Link == path);
            if (alreadyExist)
                return RedirectToAction("Index"); //false;

            //MAIN
            var Source = new TSource()
            {
                Type = (int)type,
                Link = path
            };
            user.TSources.Add(Source);
            db.SaveChanges();

            ViewBag.Message = "Your application description page.";
            return RedirectToAction("Index");
        }//todo delete



        public JsonResult News(string login)
        {
            //    Parse1(login);
            //var results = new List<Object>();
            var ll = db.TLogin.FirstOrDefault(lll => lll.Login == login);
            if (ll == null)
                return null;

            //var t2 = new Thread(() =>
            ////////{
            ////////    var byGoogle = ll.TSources.Where(_ => _.Type == (int)SourceType.Url).ToList();
            ////////    try
            ////////    {
            ////////        var oldestSource = byGoogle.OrderBy(_ => _.TArticles).First();
            ////////        ParseNReadAndSaveDB(oldestSource.Link);
            ////////    }
            ////////    catch (Exception ex)
            ////////    {
            ////////        log.Error(ex);
            ////////        //return false;// Json(new { success = false });
            ////////    }
            ////////    //return true;//Json(new { success = true });
            ////////});
            ////////t2.Start();
            //var t =  new Task(Parse1(login));
            // t.Start();

            ////parseByGoogle //add parsed
            ////byGoogle.ForEach(_ => 
            ////{
            ////    if (lastGoogleReq.AddMinutes(5) > DateTime.Now)//5 мин чтоб не забанили todo
            ////        return;
            ////    ParseNReadAndSaveDB(_.Link);
            ////});
            //var byGoogle = ll.TSources.Where(_ => _.Type == (int)SourceType.Url).ToList();
            //try
            //{
            //    var oldestSource = byGoogle.OrderBy(_ => _.TArticles).First();
            //    ParseNReadAndSaveDB(oldestSource.Link);
            //}
            //catch (Exception ex) { log.Error(ex); }

            //filter todo

            var sourcesAndArticles = db.TSources.Include("TArticles")
                .Where(_ => _.TLogins.Any(l => l.Id == ll.Id))
                .OrderByDescending(_=>_.Created)//.Where(_=>_.TArticles. .Created.Day == DateTime.Now.Day)
                .Select(_ => new { _.Id, _.Created, _.Link, _.Type, _.TArticles }).ToList();//.Where(_=>_.Created>DateTime.Now.AddDays(-2)}

            return Json(sourcesAndArticles, JsonRequestBehavior.AllowGet);



            var t4 = ll.TSources//
                .Select(_ => new { _.Id, _.Created, _.Link, _.Type, _.TArticles }).ToList();
            var newsNow = db.TParsedNews.Where(_ => _.Created.Day == DateTime.Now.Day);
            var t6 = newsNow.Where(_ => _.TSource.TLogins.Any(l => l.Id == ll.Id))
                .Select(_ => new { _.TSource })
                .ToList();

            var tt = (from a in db.TSources.Include("TArticles").Where(_ => _.TLogins.Any(l => l.Id == ll.Id))
                      where true//a.TArticles.Where(_ => _.Created.Day == DateTime.Now.Day)
                      orderby a.Created
                      select
                      new
                      {
                          a.Id,
                          a.Created,
                          a.Link,
                          a.Type,
                          a.TArticles
                      }
                     //(from b in a.TArticles
                     //where b.Created.Day==DateTime.Now.Day||a.TArticles.Count==0
                     //select b)
                     ).ToList();
            //var ttt=tt.Where(_=>_.TArticles.)


            var sourcesAndArticles2 = db.TSources.Include("TArticles").Where(_ => _.TLogins.Any(l => l.Id == ll.Id))//.Where(_=>_.TArticles. .Created.Day == DateTime.Now.Day)
                            .Select(_ => _.TArticles.Where(a => a.Created.Day == DateTime.Now.Day)).ToList();//.Where(_=>_.All(a=>a.Created.Day==DateTime.Now.Day))  .ToList();


            return Json(sourcesAndArticles, JsonRequestBehavior.AllowGet);
            //return View(s.First());


            //var str = string.Join("<BR><HR>",items.Select(_ => _.Data).ToList().Last());
            //return str;
            ////
            //var news=ParseNRead("https://meduza.io");
            ////ViewBag.Message = news.ToString();
            ////





            //return news[3];
            ////var val = node.Attributes["value"].Value;
        }
        public /*JsonResult*/bool Parse1(string login)
        {
            _login = login;
            //var results = new List<Object>();
            var ll = db.TLogin.FirstOrDefault(lll => lll.Login == login);
            if (ll == null)
                return false;
            var byGoogle = ll.TSources.Where(_ => _.Type == (int)SourceType.Url).ToList();
            try
            {
                var oldestSource = byGoogle.Last();//todo
                ParseNReadAndSaveDB(oldestSource);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return false;// Json(new { success = false });
            }
            return true;//Json(new { success = true });
        }

        List<string> ParseNReadAndSaveDB(TSource S)
        {
            var link = S.Link;

            if (lastGoogleReq.AddMinutes(20) > DateTime.Now)//5 мин чтоб не забанили todo
                return null;
            lastGoogleReq = DateTime.Now;
            var urls = NRParser.GetInfoBySearchStr(link);//dictionary


            var RawItems = new List<string>();
            foreach (var url in urls)
            {
                var transcoder = new NReadabilityWebTranscoder();
                bool success=false;
                string transcodedContent = "";
                try
                {
                    transcodedContent = transcoder.Transcode(url.Key, out success);
                }
                catch (Exception e) { log.Error(url + " не смогли распарсить ", e); }//todo
                if (success)
                {
                    var user = db.TLogin.FirstOrDefault(_ => _.Login == _login);
                    if (user == null) throw new ArgumentNullException();
                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(transcodedContent);
                    var bodyNode = doc.DocumentNode.SelectSingleNode("//div[@id='readInner']")
                        .InnerText;
                    try
                    {
                        var Article = new TArticle()
                        {
                            Data = bodyNode,
                            OwnLink = url.Key,
                            Title = url.Value,//doc.DocumentNode.SelectSingleNode("//div[@id='readInner']").InnerText.Substring(0, 200),
                            Date = DateTime.Now
                        };////////////////////////////////////////////////////////////////////////////!!!!

                        S.TArticles.Add(Article);
                        //user.TSources.First(_ => _.Type == (int)SourceType.Url)
                        //    .TArticles.Add(Article);
                    }
                    catch { log.Error("упал при Article = new TArticle()..."); }
                    db.SaveChanges();
                    RawItems.Add(bodyNode);//todo  не сделано 
                }
            }
            return RawItems;
        }
    }
}