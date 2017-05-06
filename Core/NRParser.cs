using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace Core
{
    public static class NRParser
    {
        static Logger log = LogManager.GetCurrentClassLogger();
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        static string htmlStr;
        static StreamReader htmlStream;
        public static Dictionary<string, string> GetInfoBySearchStr(string url)
        {
            string request = "";//За 24 часа", "&tbs=qdr:d
            string searchRequest = "https://www.google.ru/search?q=" + request.Trim().Replace(" ", "+")
                        + "+site:" + url.Trim().Replace("https://", "").Replace("http://", "")
                        + "&newwindow=1&espv=2&biw=1366&bih=638&tbas=0&source=lnt&tbs=qdr:d"
                          + "&sa=X&ved=0ahUKEwj-473g8dnQAhVDjywKHc8ZB1IQpwUIFQ";

            string sLine = GetHtmlCode(searchRequest);
            log.Info(sLine);

            Dictionary<string, string> result = new Dictionary<string, string>();
            #region old
            //string pattern = @"<h3 class=""r""><a href=[a-z-A-Z;\?0-9=//\.:"" ]*&amp;";//<span class="st">
            //var amount = new Regex(pattern).Matches(sLine)
            //    .Cast<Match>()
            //    .Select(m => m.Value)
            //    .ToArray();

            //foreach (string x in amount)
            //{
            //    result.Add(x.Substring(30, x.Length - 35));
            //}
            #endregion
            var doc = new HtmlAgilityPack.HtmlDocument();
            if (string.IsNullOrEmpty(sLine))
                doc.Load(htmlStream);
            else
                doc.LoadHtml(sLine);
            var gs = doc.DocumentNode.SelectNodes("//div[@class='g']");
            foreach (var g in gs)
            {
                try
                {
                    var src = g.SelectSingleNode(".//h3[@class='r']/a").Attributes["href"].Value;
                    var almostUsefulSrcCount = src.IndexOf("&amp");
                    src = src.Substring(7, almostUsefulSrcCount - 7);

                    var description = g.SelectSingleNode(".//span[@class='st']").InnerText;
                    description = description.Substring(0, description.Length - 9);//убираем мусор 
                    var t = description.LastIndexOf('.');
                    if (t != -1)
                    {
                        description = description.Substring(0, t);
                    }
                    result.Add(src, description);//todo
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }


            }
            if (result.Count == 0)
            {
                log.Warn("мы ничего не распарсили у гугла по запросу: " + searchRequest);
                System.IO.File.WriteAllText(
                    "C:\\Temp\\News\\badGooglePages\\" + DateTime.Now.ToShortTimeString() + ".html"
                    , sLine);
            }
            return result;
        }

        private static String GetHtmlCode(string Url)
        {
            var client = new WebClient();
            return client.DownloadString(Url);

            //old
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(Url);
            myRequest.Method = "GET";
            myRequest.ContentType = "charset=UTF-8";
            IAsyncResult result = myRequest.BeginGetResponse(new AsyncCallback(FinishWebRequest), myRequest);
            //  result.AsyncWaitHandle.
            allDone.WaitOne(10000);

            return htmlStr;// result;
        }

        private static void FinishWebRequest(IAsyncResult result)
        {

            HttpWebResponse myResponse = (result.AsyncState as HttpWebRequest).EndGetResponse(result) as HttpWebResponse;
            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            StreamReader sr = new StreamReader(myResponse.GetResponseStream(), Encoding.GetEncoding("windows-1251"));//
            htmlStream = sr;

            //string reader = sr.ReadToEnd();
            //htmlStr = reader;
            allDone.Set();
        }

        // Abort the request if the timer fires.
        //private void TimeoutCallback(object state, bool timedOut)
        //{
        //    if (timedOut)
        //    {
        //        HttpWebRequest request = state as HttpWebRequest;
        //        if (request != null)
        //        {
        //            request.Abort();
        //        }
        //    }
        //}

    }


}
