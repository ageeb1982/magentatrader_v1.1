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
            byte[] buffer = Encoding.ASCII.GetBytes("client_id=client id=kaHYGqLhISsA2i3YsvVwKu0kfwAAOA&code=" + code + "&grant_type=authorization_code&redirect_uri=https://www.magentatrader.com/Software?broker=Questrade");
            //byte[] buffer = Encoding.ASCII.GetBytes("client_id=client id=-4dSSId_7SQg-AAAB0OlUIBvHBZqXQ&code=" + code + "&grant_type=authorization_code&redirect_uri=https://www.magentatrader.com/Software?broker=Questrade");
             
            HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create("https://practicelogin.questrade.com/oauth2/token");
            //HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create("https://login.questrade.com/oauth2/token");
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

        // GET api/GetQuestradeOptions/api01.iq.questrade.com/AIIAovmjVaPfyEcAJLmMsj6uPAFBDzg60/123456
        [Authorize]
        [Route("api/GetQuestradeOptionQuotes")]
        public Models.QuestradeOptionQuoteList GetQuestradeOptionQuotes([FromUri] List<String> optionList) 
        {
            var api = optionList[0];
            var token = optionList[1];

            //var optionType = filters[2];
            //var underlyingId = Int32.Parse(filters[3]);
            //var expiryDate = filters[4];
            //var minstrikePrice = Double.Parse(filters[5]);
            //var maxstrikePrice = Double.Parse(filters[6]);

            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://" + api + "/v1/markets/quotes/options");

            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Headers.Add("Authorization", "Bearer " + token);

            try
            {
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    //"{\"optionQuotes\":[{\"underlying\":\"FB\",\"underlyingId\":2067121,\"symbol\":\"FB15Apr16P108.00\",\"symbolId\":13185733,\"bidPrice\":0.57,\"bidSize\":51,\"askPrice\":0.59,\"askSize\":46,\"lastTradePriceTrHrs\":0.53,\"lastTradePrice\":0.53,\"lastTradeSize\":1,\"lastTradeTick\":\"Up\",\"lastTradeTime\":\"2016-04-05T15:52:39.286000-04:00\",\"volume\":362,\"openPrice\":0.71,\"highPrice\":0.71,\"lowPrice\":0.46,\"volatility\":26.90889,\"delta\":-0.231203,\"gamma\":0.049115,\"theta\":-0.085695,\"vega\":0.059367,\"rho\":-0.008083,\"openInterest\":3112,\"delay\":0,\"isHalted\":false,\"VWAP\":0.544751},{\"underlying\":\"FB\",\"underlyingId\":2067121,\"symbol\":\"FB15Apr16P109.00\",\"symbolId\":13185734,\"bidPrice\":0.76,\"bidSize\":1275,\"askPrice\":0.79,\"askSize\":44,\"lastTradePriceTrHrs\":0.76,\"lastTradePrice\":0.76,\"lastTradeSize\":1,\"lastTradeTick\":\"Up\",\"lastTradeTime\":\"2016-04-05T15:56:15.056000-04:00\",\"volume\":1186,\"openPrice\":0.94,\"highPrice\":1,\"lowPrice\":0.62,\"volatility\":26.215661,\"delta\":-0.285027,\"gamma\":0.054757,\"theta\":-0.095523,\"vega\":0.066187,\"rho\":-0.009988,\"openInterest\":3173,\"delay\":0,\"isHalted\":false,\"VWAP\":0.710269},{\"underlying\":\"FB\",\"underlyingId\":2067121,\"symbol\":\"FB15Apr16P110.00\",\"symbolId\":12594067,\"bidPrice\":1.02,\"bidSize\":246,\"askPrice\":1.05,\"askSize\":19,\"lastTradePriceTrHrs\":1.05,\"lastTradePrice\":1.05,\"lastTradeSize\":10,\"lastTradeTick\":\"Equal\",\"lastTradeTime\":\"2016-04-05T15:59:49.919000-04:00\",\"volume\":2569,\"openPrice\":1.2,\"highPrice\":1.3,\"lowPrice\":0.82,\"volatility\":25.674474,\"delta\":-0.34363,\"gamma\":0.059333,\"theta\":-0.103485,\"vega\":0.071718,\"rho\":-0.012072,\"openInterest\":13830,\"delay\":0,\"isHalted\":false,\"VWAP\":0.975675},{\"underlying\":\"FB\",\"underlyingId\":2067121,\"symbol\":\"FB15Apr16P111.00\",\"symbolId\":13185735,\"bidPrice\":1.35,\"bidSize\":393,\"askPrice\":1.38,\"askSize\":35,\"lastTradePriceTrHrs\":1.41,\"lastTradePrice\":1.41,\"lastTradeSize\":1,\"lastTradeTick\":\"Up\",\"lastTradeTime\":\"2016-04-05T15:58:30.886000-04:00\",\"volume\":823,\"openPrice\":1.51,\"highPrice\":1.65,\"lowPrice\":1.12,\"volatility\":25.180011,\"delta\":-0.405678,\"gamma\":0.062534,\"theta\":-0.109042,\"vega\":0.075588,\"rho\":-0.014291,\"openInterest\":1912,\"delay\":0,\"isHalted\":false,\"VWAP\":1.186427},{\"underlying\":\"FB\",\"underlyingId\":2067121,\"symbol\":\"FB15Apr16P112.00\",\"symbolId\":13185736,\"bidPrice\":1.75,\"bidSize\":196,\"askPrice\":1.79,\"askSize\":32,\"lastTradePriceTrHrs\":1.8,\"lastTradePrice\":1.8,\"lastTradeSize\":20,\"lastTradeTick\":\"Up\",\"lastTradeTime\":\"2016-04-05T15:59:45.286000-04:00\",\"volume\":1048,\"openPrice\":1.97,\"highPrice\":2.1,\"lowPrice\":1.44,\"volatility\":24.681762,\"delta\":-0.469612,\"gamma\":0.064154,\"theta\":-0.111835,\"vega\":0.077546,\"rho\":-0.016593,\"openInterest\":2983,\"delay\":0,\"isHalted\":false,\"VWAP\":1.638578},{\"underlying\":\"FB\",\"underlyingId\":2067121,\"symbol\":\"FB15Apr16P113.00\",\"symbolId\":13185737,\"bidPrice\":2.25,\"bidSize\":27,\"askPrice\":2.28,\"askSize\":8,\"lastTradePriceTrHrs\":2.31,\"lastTradePrice\":2.31,\"lastTradeSize\":5,\"lastTradeTick\":\"Up\",\"lastTradeTime\":\"2016-04-05T15:58:27.256000-04:00\",\"volume\":1208,\"openPrice\":2.4,\"highPrice\":2.6,\"lowPrice\":1.83,\"volatility\":24.294423,\"delta\":-0.533766,\"gamma\":0.06411,\"theta\":-0.11172,\"vega\":0.077493,\"rho\":-0.018922,\"openInterest\":4471,\"delay\":0,\"isHalted\":false,\"VWAP\":2.085107},{\"underlying\":\"FB\",\"underlyingId\":2067121,\"symbol\":\"FB15Apr16P114.00\",\"symbolId\":13185738,\"bidPrice\":2.83,\"bidSize\":20,\"askPrice\":2.88,\"askSize\":43,\"lastTradePriceTrHrs\":2.95,\"lastTradePrice\":2.95,\"lastTradeSize\":39,\"lastTradeTick\":\"Equal\",\"lastTradeTime\":\"2016-04-05T15:58:33.446000-04:00\",\"volume\":1305,\"openPrice\":3,\"highPrice\":3,\"lowPrice\":2.35,\"volatility\":24.057836,\"delta\":-0.596503,\"gamma\":0.062449,\"theta\":-0.108779,\"vega\":0.075485,\"rho\":-0.021222,\"openInterest\":2953,\"delay\":0,\"isHalted\":false,\"VWAP\":2.715785},{\"underlying\":\"FB\",\"underlyingId\":2067121,\"symbol\":\"FB15Apr16P115.00\",\"symbolId\":12594068,\"bidPrice\":3.45,\"bidSize\":2790,\"askPrice\":3.55,\"askSize\":77,\"lastTradePriceTrHrs\":3.6,\"lastTradePrice\":3.6,\"lastTradeSize\":1,\"lastTradeTick\":\"Equal\",\"lastTradeTime\":\"2016-04-05T15:58:39.086000-04:00\",\"volume\":298,\"openPrice\":3.73,\"highPrice\":3.8,\"lowPrice\":2.91,\"volatility\":23.446986,\"delta\":-0.656335,\"gamma\":0.059335,\"theta\":-0.1033,\"vega\":0.071721,\"rho\":-0.023442,\"openInterest\":12476,\"delay\":0,\"isHalted\":false,\"VWAP\":3.262785},{\"underlying\":\"FB\",\"underlyingId\":2067121,\"symbol\":\"FB15Apr16P116.00\",\"symbolId\":13185739,\"bidPrice\":4.2,\"bidSize\":2383,\"askPrice\":4.3,\"askSize\":10,\"lastTradePriceTrHrs\":4.2,\"lastTradePrice\":4.2,\"lastTradeSize\":1,\"lastTradeTick\":\"Up\",\"lastTradeTime\":\"2016-04-05T15:56:30.204000-04:00\",\"volume\":165,\"openPrice\":4.5,\"highPrice\":4.5,\"lowPrice\":3.55,\"volatility\":23.257182,\"delta\":-0.712016,\"gamma\":0.055025,\"theta\":-0.095734,\"vega\":0.066512,\"rho\":-0.025538,\"openInterest\":1687,\"delay\":0,\"isHalted\":false,\"VWAP\":4.11109},{\"underlying\":\"FB\",\"underlyingId\":2067121,\"symbol\":\"FB15Apr16P117.00\",\"symbolId\":13185740,\"bidPrice\":5.05,\"bidSize\":1068,\"askPrice\":5.15,\"askSize\":132,\"lastTradePriceTrHrs\":5.1,\"lastTradePrice\":5.1,\"lastTradeSize\":1,\"lastTradeTick\":\"Up\",\"lastTradeTime\":\"2016-04-05T12:36:06.946000-04:00\",\"volume\":42,\"openPrice\":5.35,\"highPrice\":5.35,\"lowPrice\":4.45,\"volatility\":23.818786,\"delta\":-0.762616,\"gamma\":0.049837,\"theta\":-0.086635,\"vega\":0.060241,\"rho\":-0.027478,\"openInterest\":400,\"delay\":0,\"isHalted\":false,\"VWAP\":4.661904}]}"
                    //string json = "{" + "\"filters\": [{" + "\"optionType\":\"" + optionType + "\"," + "\"underlyingId\":" + underlyingId + "," + "\"expiryDate\":\"" + expiryDate + "\"," + "\"minstrikePrice\":" + minstrikePrice + "," + "\"maxstrikePrice\":" + maxstrikePrice + "}]}";
                    int i = 0;
                    string json = "{\"optionIds\": [";
                    foreach (String o in optionList)
                    {
                        if (i > 1)
                        {
                            if (i == 2) json = json + o.ToString();
                            else json = json + "," + o.ToString();
                        }
                        i++;
                    }
                    json = json + "]}";

                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        Models.QuestradeOptionQuoteList list = (Models.QuestradeOptionQuoteList)js.Deserialize(result, typeof(Models.QuestradeOptionQuoteList));
                        return list;
                        //return result.ToString();
                    }
                }
            }
            catch (Exception e)
            {
                //return e.ToString();
                return new Models.QuestradeOptionQuoteList();
            }
        }
    }
}
