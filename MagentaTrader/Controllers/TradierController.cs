﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
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
        private Data.MagentaTradersDBDataContext db = new Data.MagentaTradersDBDataContext();
        private void AccessLog(string log)
        {
            try
            {
                Data.SysAcessLog NewAccessLog = new Data.SysAcessLog();

                string currentUserName = User.Identity.Name;

                NewAccessLog.UserId = (from d in db.MstUsers where d.UserName.Equals(currentUserName) select d).FirstOrDefault().Id;
                //NewAccessLog.UserId = 2;
                NewAccessLog.LogDateTime = DateTime.Now;
                NewAccessLog.Log = log;

                db.SysAcessLogs.InsertOnSubmit(NewAccessLog);
                db.SubmitChanges();
            }
            catch { }
        }

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

                    AccessLog("Tradier Access Token");

                    return t;
                }
            }
            catch
            {
                return new Models.TradierAccessToken();
            }
        }

        // GET api/GetTradierAccessToken/velocity/pjytfG9a
        [Authorize]
        [Route("api/GetTradierAccessToken/velocity/{code}")]
        public Models.TradierAccessToken GetTradierAccessTokenVelocity(string code)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api.tradier.com/v1/oauth/accesstoken");
            byte[] bytedata = Encoding.UTF8.GetBytes("grant_type=authorization_code&code=" + code);
            string authInfo = Convert.ToBase64String(Encoding.Default.GetBytes("38Ng8OopHazxYkvkHcYwA718F30U0MH7:HFWAobtl6YYYnuG2"));

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

                    AccessLog("Tradier Access Token");

                    return t;
                }
            }
            catch
            {
                return new Models.TradierAccessToken();
            }
        }

        // GET api/GetTradierTimeSales/velocity/aapl/098f6bcd4621d373cade4e832627b4f6
        [Authorize]
        [Route("api/GetTradierTimeSales/velocity/{symbol}/{token}")]
        public Models.TradierTimeSalesSeries GetTradierTimeSalesVelocity(string symbol, string token)
        {
            string start = DateTime.Now.AddDays(-38).ToString("yyyy-MM-dd HH:mm");
            string end = DateTime.Now.ToString("yyyy-MM-dd HH:mm");

            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api.tradier.com/v1/markets/timesales?symbol=" + symbol + "&interval=15min&start=" + start + "&end=" + end);

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
                    Models.TradierTimeSalesSeries q = (Models.TradierTimeSalesSeries)js.Deserialize(result, typeof(Models.TradierTimeSalesSeries));
                    return q;
                }
            }
            catch
            {
                return new Models.TradierTimeSalesSeries();
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
            catch
            {
                return new Models.TradierQuoteSymbolQuotes();
            }
        }

        // GET api/GetTradierPreviewEquity
        [Authorize]
        [HttpPost]
        [Route("api/GetTradierPreviewEquity")]
        public Models.TradierPreviewEquityOrder GetTradierPreviewEquity(Models.TradierAddOrder order)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api.tradier.com/v1/accounts/" + order.account + "/orders");
            string postString = "";

            switch (order.type)
            {
                case "market":
                    postString = "class=equity" +
                                "&symbol=" + order.symbol +
                                "&duration=" + order.duration +
                                "&side=" + order.side +
                                "&quantity=" + order.quantity +
                                "&type=" + order.type +
                                "&preview=true";
                    break;
                case "limit":
                    postString = "class=equity" +
                                "&symbol=" + order.symbol +
                                "&duration=" + order.duration +
                                "&side=" + order.side +
                                "&quantity=" + order.quantity +
                                "&type=" + order.type +
                                "&price=" + order.price +
                                "&preview=true";
                    break;
                case "stop":
                    postString = "class=equity" +
                                "&symbol=" + order.symbol +
                                "&duration=" + order.duration +
                                "&side=" + order.side +
                                "&quantity=" + order.quantity +
                                "&type=" + order.type +
                                "&stop=" + order.stop +
                                "&preview=true";
                    break;
                case "stop_limit":
                    postString = "class=equity" +
                                "&symbol=" + order.symbol +
                                "&duration=" + order.duration +
                                "&side=" + order.side +
                                "&quantity=" + order.quantity +
                                "&type=" + order.type +
                                "&price=" + order.price +
                                "&stop=" + order.stop +
                                "&preview=true";
                    break;
            }

            byte[] bytedata = Encoding.UTF8.GetBytes(postString);

            httpWebRequest.Method = "POST";
            httpWebRequest.Accept = "application/json";
            httpWebRequest.Headers.Add("Authorization", "Bearer " + order.token);

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
                    Models.TradierPreviewEquityOrder p = (Models.TradierPreviewEquityOrder)js.Deserialize(result, typeof(Models.TradierPreviewEquityOrder));
                    return p;
                }
            }
            catch
            {
                return new Models.TradierPreviewEquityOrder();
            } 
        }

        // GET api/GetTradierBuyEquity
        [Authorize]
        [HttpPost]
        [Route("api/GetTradierBuyEquity")]
        public Models.TradierAddOrderStatusOrder GetTradierBuyEquity(Models.TradierAddOrder order)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api.tradier.com/v1/accounts/" + order.account + "/orders");
            string postString = "";

            switch (order.type)
            {
                case "market":
                    postString = "class=equity" +
                                "&symbol=" + order.symbol +
                                "&duration=" + order.duration +
                                "&side=" + order.side +
                                "&quantity=" + order.quantity +
                                "&type=" + order.type;
                    break;
                case "limit":
                    postString = "class=equity" +
                                "&symbol=" + order.symbol +
                                "&duration=" + order.duration +
                                "&side=" + order.side +
                                "&quantity=" + order.quantity +
                                "&type=" + order.type +
                                "&price=" + order.price;
                    break;
                case "stop":
                    postString = "class=equity" +
                                "&symbol=" + order.symbol +
                                "&duration=" + order.duration +
                                "&side=" + order.side +
                                "&quantity=" + order.quantity +
                                "&type=" + order.type +
                                "&stop=" + order.stop;
                    break;
                case "stop_limit":
                    postString = "class=equity" +
                                "&symbol=" + order.symbol +
                                "&duration=" + order.duration +
                                "&side=" + order.side +
                                "&quantity=" + order.quantity +
                                "&type=" + order.type +
                                "&price=" + order.price +
                                "&stop=" + order.stop;
                    break;
            }

            byte[] bytedata = Encoding.UTF8.GetBytes(postString);

            httpWebRequest.Method = "POST";
            httpWebRequest.Accept = "application/json";
            httpWebRequest.Headers.Add("Authorization", "Bearer " + order.token);

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
                    Models.TradierAddOrderStatusOrder p = (Models.TradierAddOrderStatusOrder)js.Deserialize(result, typeof(Models.TradierAddOrderStatusOrder));
                    return p;
                }
            }
            catch
            {
                return new Models.TradierAddOrderStatusOrder();
            }
        }

        // GET api/GetTradierOptionExpiration
        [Authorize]
        [Route("api/GetTradierOptionExpiration/{symbol}/{token}")]
        public Models.TradierOptionExpirationRoot GetTradierOptionExpiration(string symbol, string token)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api.tradier.com/v1/markets/options/expirations?symbol=" + symbol);

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
                    Models.TradierOptionExpirationRoot q = (Models.TradierOptionExpirationRoot)js.Deserialize(result, typeof(Models.TradierOptionExpirationRoot));
                    return q;
                }
            }
            catch
            {
                return new Models.TradierOptionExpirationRoot();
            }
        }

        // GET api/GetTradierOptionChain
        [Authorize]
        [Route("api/GetTradierOptionChain/{symbol}/{expiration}/{token}")]
        public Models.TradierOptionChainRoot GetTradierOptionChain(string symbol, string expiration, string token)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://api.tradier.com/v1/markets/options/chains?symbol=" + symbol + "&expiration=" + expiration);

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
                    Models.TradierOptionChainRoot q = (Models.TradierOptionChainRoot)js.Deserialize(result, typeof(Models.TradierOptionChainRoot));
                    return q;
                }
            }
            catch
            {
                return new Models.TradierOptionChainRoot();
            }
        }

    }
}
