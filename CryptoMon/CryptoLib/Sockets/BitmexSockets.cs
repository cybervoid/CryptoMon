using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CryptoLib.Sockets
{
    public class BitmexSockets
    {
        static SocketModels.BitmexQuote hourStart = new SocketModels.BitmexQuote { bidPrice = -1, askPrice = -1 };
        static SocketModels.BitmexQuote hourlyHigh = new SocketModels.BitmexQuote { bidPrice = -1, askPrice = -1 };
        static SocketModels.BitmexQuote hourlyLow = new SocketModels.BitmexQuote { bidPrice = -1, askPrice = -1 };
        static SocketModels.BitmexQuote hourEnd = new SocketModels.BitmexQuote();
        static DateTime workingHour = DateTime.Now;

        public static void Quotes(string operation = "subscribe", string arguments = "quote:XBTUSD")
        {
            decimal openPrice = 0;
            decimal closePrice = 0;
            decimal highPrice = 0;
            decimal lowPrice = 0;
            DateTime hourTimestamp = DateTime.UtcNow;

            using (var ws = new WebSocketSharp.WebSocket("wss://www.bitmex.com/realtime"))
            {
                ws.OnMessage += (sender, e) =>
                {
                    SocketModels.BitmexModel<SocketModels.BitmexQuote> incomingObject = JsonConvert.DeserializeObject<SocketModels.BitmexModel<SocketModels.BitmexQuote>>(e.Data);
                    PropertyInfo[] incomingProperties = incomingObject.data.FirstOrDefault().GetType().GetProperties();

                    if (incomingObject.data.FirstOrDefault().timestamp.Date > hourTimestamp.Date || incomingObject.data.FirstOrDefault().timestamp.Hour > hourTimestamp.Hour || incomingObject.data.FirstOrDefault().timestamp.Minute > hourTimestamp.Minute)
                    {
                        CryptoLib.MarketsEntities context = new CryptoLib.MarketsEntities();
                        CryptoLib.MarketData marketData = new CryptoLib.MarketData
                        {
                            InstrumentID = 1,
                            MarketID = 1,
                            Timestamp = incomingObject.data.FirstOrDefault().timestamp,
                            OpenPrice = openPrice,
                            ClosePrice = closePrice,
                            HighPrice = highPrice,
                            LowPrice = lowPrice
                        };
                        if (marketData.OpenPrice != 0 && marketData.ClosePrice != 0 && marketData.HighPrice != 0 && marketData.LowPrice != 0)
                        {
                            context.MarketData.Add(marketData);
                            context.SaveChanges();
                        }

                        openPrice = 0;
                        closePrice = 0;
                        highPrice = 0;
                        lowPrice = 0;
                        hourTimestamp = incomingObject.data.FirstOrDefault().timestamp;
                    }

                    decimal incomingMidPrice = ((incomingObject.data.FirstOrDefault().askPrice + incomingObject.data.FirstOrDefault().bidPrice) / 2);
                    highPrice = Math.Max(incomingMidPrice, highPrice);
                    lowPrice = Math.Min(incomingMidPrice, lowPrice);
                    closePrice = incomingMidPrice;
                    if (openPrice == 0)
                        openPrice = incomingMidPrice;
                    if (lowPrice == 0)
                        lowPrice = incomingMidPrice;
                };

                ws.Connect();
                //String.Format("{\"op\": \"{0}\", \"args\": \"{1}\"}", operation, arguments);
                ws.Send("{\"op\": \"subscribe\", \"args\": \"quote:XBTUSD\"}");
                Console.ReadLine();
            }
        }
    }
}
