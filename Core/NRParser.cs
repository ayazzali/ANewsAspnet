using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Core
{
    public static class  NRParser
    {

        public static ManualResetEvent allDone = new ManualResetEvent(false);
        static string htmlStr;
        public static List<string> GetInfoBySearchStr(string url)
        {
            string request = "";//За 24 часа", "&tbs=qdr:d
            string searchRequest = "https://www.google.ru/search?q=" + request.Trim().Replace(" ", "+")
                        + "+site:"+url.Trim().Replace("https://", "").Replace("http://", "")
                        + "&newwindow=1&espv=2&biw=1366&bih=638&tbas=0&source=lnt&tbs=qdr:d"
                          + "&sa=X&ved=0ahUKEwj-473g8dnQAhVDjywKHc8ZB1IQpwUIFQ";

            string sLine = GetHtmlCode(searchRequest);
            Console.WriteLine(sLine);

            string pattern = @"<h3 class=""r""><a href=[a-z-A-Z;\?0-9=//\.:"" ]*&amp;";
            var amount = new Regex(pattern).Matches(sLine)
                .Cast<Match>()
                .Select(m => m.Value)
                .ToArray();

            List<string> result = new List<string>();
            foreach (string x in amount)
            {
                result.Add(x.Substring(30, x.Length - 35));
            }

            return result;
        }

        private static String GetHtmlCode(string Url)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(Url);
            myRequest.Method = "GET";
            myRequest.ContentType = "charset=UTF-8";
            IAsyncResult result =  myRequest.BeginGetResponse(new AsyncCallback(FinishWebRequest), myRequest);
          //  result.AsyncWaitHandle.
            allDone.WaitOne(5000);

            return htmlStr;// result;
        }

        private static void FinishWebRequest(IAsyncResult result)
        {

            HttpWebResponse myResponse = (result.AsyncState as HttpWebRequest).EndGetResponse(result) as HttpWebResponse;
            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            StreamReader sr = new StreamReader(myResponse.GetResponseStream(), Encoding.GetEncoding("windows-1251"));//
            string reader = sr.ReadToEnd();
            htmlStr = reader;
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
