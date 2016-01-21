using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace MagentaTrader.Controllers
{
    public class QuestradeAPIController : ApiController
    {
        private Data.MagentaTradersDBDataContext db = new Data.MagentaTradersDBDataContext();

        // GET api/GetQuestradeAccessToken/AIIAovmjVaPfyEcAJLmMsj6uPAFBDzg60
        [Authorize]
        [Route("api/GetQuestradeAccessToken/{code}")]
        public Models.QuestradeAccessToken GetQuestradeAccessToken(string code) 
        {
            byte[] buffer = Encoding.ASCII.GetBytes("client_id=client id=-4dSSId_7SQg-AAAB0OlUIBvHBZqXQ&code=" + code + "&grant_type=authorization_code&redirect_uri=https://www.magentatrader.com/Software?broker=Questrade");

            HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create("https://practicelogin.questrade.com/oauth2/token");
            webReq.Method = "POST";
            webReq.ProtocolVersion = HttpVersion.Version10;
            webReq.ContentType = "application/x-www-form-urlencoded";
            webReq.ContentLength = buffer.Length;

            Stream postData = webReq.GetRequestStream();
            postData.Write(buffer, 0, buffer.Length);
            postData.Close();

            try
            {
                HttpWebResponse webResp = (HttpWebResponse)webReq.GetResponse();
                using (StreamReader answer = new StreamReader(webResp.GetResponseStream()))
                {
                    var result = answer.ReadToEnd();
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    Models.QuestradeAccessToken t = (Models.QuestradeAccessToken)js.Deserialize(result, typeof(Models.QuestradeAccessToken));
                    return t;
                }
            }
            catch(Exception e)
            {
                return new Models.QuestradeAccessToken();
            }
        }

        // GET api/GetQuestradeSymbolId/api01.iq.questrade.com/AIIAovmjVaPfyEcAJLmMsj6uPAFBDzg60/MSFT
        [Authorize]
        [Route("api/GetQuestradeSymbolId/{api}/{token}/{symbol}")]
        public Models.QuestradeSymbol GetQuestradeSymbolId(string api, string token, string symbol)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://" + api + "/v1/symbols/search?prefix=" + symbol);

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
                    Models.QuestradeSymbolList list = (Models.QuestradeSymbolList)js.Deserialize(result, typeof(Models.QuestradeSymbolList));

                    var symbolData = from d in db.MstSymbols where d.Symbol == symbol select d;

                    if (symbolData.Any())
                    {
                        string exchange = symbolData.First().Exchange;
                        foreach (Models.QuestradeSymbol q in list.symbols)
                        {
                            if (symbol == q.symbol && exchange == q.listingExchange)
                            {
                                return q;
                            }
                        }
                    }
                    return new Models.QuestradeSymbol();
                }
            }
            catch (Exception e)
            {
                return new Models.QuestradeSymbol();
            }
        }

        // GET api/GetQuestradeOptions/api01.iq.questrade.com/AIIAovmjVaPfyEcAJLmMsj6uPAFBDzg60/123456
        [Authorize]
        [Route("api/GetQuestradeOptions/{api}/{token}/{symbolId}")]
        public Models.QuestradeOptionList GetQuestradeOptions(string api, string token, string symbolId)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://" + api + "/v1/symbols/" + symbolId + "/options");

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
                    Models.QuestradeOptionList list = (Models.QuestradeOptionList)js.Deserialize(result, typeof(Models.QuestradeOptionList));
                    return list;
                    //return result.ToString();
                }
            }
            catch (Exception e)
            {
                //return e.ToString();
                return new Models.QuestradeOptionList();
            }
        }


    }
}
