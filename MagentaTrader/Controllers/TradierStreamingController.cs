using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MagentaTrader.Controllers
{
    public class TradierStreamingController : ApiController
    {
        private static readonly ConcurrentQueue<StreamWriter> _streammessage = new ConcurrentQueue<StreamWriter>();

        [HttpGet]
        [Route("api/GetTradierStream")]
        public HttpResponseMessage GetStreamingPrice(HttpRequestMessage request)
        {
            HttpResponseMessage response = request.CreateResponse();
            Action<Stream, HttpContent, TransportContext> onStreamAvailable = delegate(Stream stream, HttpContent headers, TransportContext context)
            {
                if (stream != null) {
                    StreamWriter outStream = new StreamWriter(stream);

                    TimeSpan interval = new TimeSpan(0, 0, 2); // Two seconds

                    for (int i = 0; i < 50; i++)
                    {
                        outStream.WriteLine("event: testEvent");
                        outStream.WriteLine("data: stream" + i);
                        outStream.WriteLine("");
                        outStream.Flush();

                        Thread.Sleep(interval);
                    }
                }
            };
            response.Content = new PushStreamContent(onStreamAvailable, "text/event-stream");
            return response;
        }
    }

    public class StreamingPrice
    {
        public String symbol;
        public decimal open;
        public decimal close;
        public decimal high;
        public decimal low;
        public DateTime timeStamp;
    }
}