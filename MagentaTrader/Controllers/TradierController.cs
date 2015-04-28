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

namespace MagentaTrader.Controllers
{
    public class TradierController : ApiController
    {
        // GET api/GetTradierAccessToken/pjytfG9a
        [Authorize]
        [Route("api/GetTradierAccessToken/{code}")]
        public Models.TradierAccessToken GetTradierAccessToken(string code)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api.tradier.com/v1/oauth/accesstoken");
            byte[] bytedata = Encoding.UTF8.GetBytes("grant_type=authorization_code&code=" + code);
            string authInfo = Convert.ToBase64String(Encoding.Default.GetBytes("DxO1T2KQLG1KoGMu0FT1bTiBXYbmor8B:nd1rKxotBC1nzQy8"));

            httpWebRequest.Method = "POST";
            httpWebRequest.Accept = "*/*";
            httpWebRequest.Headers.Add("Authorization", "Basic " + authInfo);
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.ContentLength = bytedata.Length;

            Stream requestStream = httpWebRequest.GetRequestStream();
            requestStream.Write(bytedata, 0, bytedata.Length);
            requestStream.Close();

            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    Models.TradierAccessToken t = (Models.TradierAccessToken)js.Deserialize(result, typeof(Models.TradierAccessToken));
                    return t;
                }
            }
            catch
            {
                return new Models.TradierAccessToken();
            }
        }

        // GET api/GetTradierUserProfile/098f6bcd4621d373cade4e832627b4f6
        [Authorize]
        [Route("api/GetTradierUserProfile/{token}")]
        public Models.TradierUserProfile GetTradierUserProfile(string token)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api.tradier.com/v1/user/profile");

            httpWebRequest.Method = "GET";
            httpWebRequest.Accept = "application/json";

            // httpWebRequest.Headers.Add("Authorization", "Bearer iFH6oicABZEt3zTAGlQiL3UQkCN2");
            httpWebRequest.Headers.Add("Authorization", "Bearer " + token);

            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    Models.TradierUserProfile o = (Models.TradierUserProfile)js.Deserialize(result, typeof(Models.TradierUserProfile));
                    return o;
                }
            }
            catch
            {
                return new Models.TradierUserProfile();
            }
        }

        // GET api/GetTradierOrder/30950670/098f6bcd4621d373cade4e832627b4f6
        [Authorize]
        [Route("api/GetTradierOrder/{account}/{token}")]
        public Models.TradierOrderAccountOrders GetTradierOrder(string account, string token)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api.tradier.com/v1/accounts/" + account + "/orders");

            httpWebRequest.Method = "GET";
            httpWebRequest.Accept = "application/json";

            // httpWebRequest.Headers.Add("Authorization", "Bearer iFH6oicABZEt3zTAGlQiL3UQkCN2");
            httpWebRequest.Headers.Add("Authorization", "Bearer " + token);

            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    Models.TradierOrderAccountOrders o = (Models.TradierOrderAccountOrders)js.Deserialize(result, typeof(Models.TradierOrderAccountOrders));
                    return o;
                    //return result.ToString();
                }
            }
            catch
            {
                Models.TradierOrderAccountOrders o = new Models.TradierOrderAccountOrders();
                return o;
                //return "";
            }
        }

        // GET api/GetTradierPosition/30950670/098f6bcd4621d373cade4e832627b4f6
        [Authorize]
        [Route("api/GetTradierPosition/{account}/{token}")]
        public Models.TradierPositionAccountPositions GetTradierPosition(string account, string token)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api.tradier.com/v1/accounts/" + account + "/positions");

            httpWebRequest.Method = "GET";
            httpWebRequest.Accept = "application/json";

            // httpWebRequest.Headers.Add("Authorization", "Bearer iFH6oicABZEt3zTAGlQiL3UQkCN2");
            httpWebRequest.Headers.Add("Authorization", "Bearer " + token);

            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    Models.TradierPositionAccountPositions p = (Models.TradierPositionAccountPositions)js.Deserialize(result, typeof(Models.TradierPositionAccountPositions));
                    return p;
                }
            }
            catch
            {
                Models.TradierPositionAccountPositions p = new Models.TradierPositionAccountPositions();
                return p;
            }
        }

        // GET api/GetTradierQuote/MSFT/098f6bcd4621d373cade4e832627b4f6
        [Authorize]
        [Route("api/GetTradierQuote/{symbol}/{token}")]
        public Models.TradierQuoteSymbolQuotes GetTradierQuote(string symbol, string token)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api.tradier.com/v1/markets/quotes?symbols=" + symbol );

            httpWebRequest.Method = "GET";
            httpWebRequest.Accept = "application/json";

            httpWebRequest.Headers.Add("Authorization", "Bearer " + token);
            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    Models.TradierQuoteSymbolQuotes q = (Models.TradierQuoteSymbolQuotes)js.Deserialize(result, typeof(Models.TradierQuoteSymbolQuotes));
                    return q;
                }
            }
            catch(Exception e)
            {
                return new Models.TradierQuoteSymbolQuotes();
            }
        }
    }
}
