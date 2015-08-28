using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using System.Xml;
using Newtonsoft.Json;

namespace MagentaTrader.Controllers
{
    public class QTController
    {
        //[Authorize]
        //[Route("api/GetQuestradeAccessToken/{refreshtoken}")]
        //public Models.QuestradeAccessToken getQTAccessToken(string refreshtoken)
        //{
        //    var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://practicelogin.questrade.com/oauth2/token?grant_type=refresh_token&refresh_token=" + refreshtoken);

        //    httpWebRequest.Method = "GET";
        //    httpWebRequest.Accept = "HTTP/1.1";
        //    httpWebRequest.ContentType = "application/json; charset=utf-8";

        //    try
        //    {
        //        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        //        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
        //        {
        //            var result = streamReader.ReadToEnd();
        //            JavaScriptSerializer js = new JavaScriptSerializer();
        //            dynamic results = JsonConvert.DeserializeObject(result);
        //            string token = results["refresh_token"];
        //            string access_token = results["access_token"];
        //            string api_server = results["api_server"];

        //            httpWebRequest = (HttpWebRequest)WebRequest.Create(api_server + "v1/accounts");
        //            httpWebRequest.Method = "GET";
        //            httpWebRequest.Accept = "HTTP/1.1";
        //            httpWebRequest.ContentType = "application/json; charset=utf-8;";
        //            httpWebRequest.Headers.Add("Authorization", "Bearer " + access_token);

        //            try
        //            {
        //                httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        //                using (var streamReaderr = new StreamReader(httpResponse.GetResponseStream()))
        //                {
        //                    result = streamReaderr.ReadToEnd();
        //                    JavaScriptSerializer jss = new JavaScriptSerializer();
        //                    dynamic resultss = JsonConvert.DeserializeObject(result);
        //                }
        //            }
        //            catch
        //            {
        //                Models.QuestradeAccessToken x = new Models.QuestradeAccessToken();
        //                return x;
        //            }
        //            Models.QuestradeAccessToken y = (Models.QuestradeAccessToken)js.Deserialize(result, typeof(Models.QuestradeAccessToken));
        //            return y;
        //        }
        //    }
        //    catch
        //    {
        //        Models.QuestradeAccessToken x = new Models.QuestradeAccessToken();
        //        return x;
        //    }
        //}

        //[Authorize]
        //[Route("api/GetQuestradeAccounts/{accesstoken}/{apiserver}")]
        //public Models.QuestradeAccountsModel getAccounts(string accesstoken, string apiserver)
        //{
        //    var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiserver + "v1/accounts");

        //    httpWebRequest.Method = WebRequestMethods.Http.Get;
        //    httpWebRequest.Accept = "HTTP/1.1";
        //    httpWebRequest.ContentType = "application/json; charset=utf-8;";
        //    httpWebRequest.Headers.Add("Authorization", "Bearer " + accesstoken);

        //    try
        //    {
        //        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        //        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
        //        {
        //            var result = streamReader.ReadToEnd();
        //            JavaScriptSerializer js = new JavaScriptSerializer();
        //            dynamic results = JsonConvert.DeserializeObject(result);
        //            Models.QuestradeAccountsModel x = (Models.QuestradeAccountsModel)js.Deserialize(result, typeof(Models.QuestradeAccountsModel));
        //            return x;
        //        }
        //    }
        //    catch
        //    {
        //        Models.QuestradeAccountsModel x = new Models.QuestradeAccountsModel();
        //        return x;
        //    }
        //}

    }
}