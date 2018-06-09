using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CryptoSocket
{
    public partial class BitmexService : ServiceBase
    {
        public BitmexService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            ThreadPool.QueueUserWorkItem(_ => MarketData());
        }

        protected override void OnStop()
        {
        }

        #region Quotes
        private void MarketData()
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
                    CryptoLib.Models.Bitmex.Model<CryptoLib.Models.Bitmex.Quote> incomingObject = JsonConvert.DeserializeObject<CryptoLib.Models.Bitmex.Model<CryptoLib.Models.Bitmex.Quote>>(e.Data);
                    PropertyInfo[] incomingProperties = incomingObject.data.FirstOrDefault().GetType().GetProperties();

                    if (incomingObject.data.FirstOrDefault().timestamp.Date > hourTimestamp.Date || incomingObject.data.FirstOrDefault().timestamp.Hour > hourTimestamp.Hour || incomingObject.data.FirstOrDefault().timestamp.Minute > hourTimestamp.Minute)
                    {
                        CryptoLib.MarketsEntities context = new CryptoLib.MarketsEntities();
                        CryptoLib.MarketData marketData = new CryptoLib.MarketData
                        {
                            InstrumentID = 1,
                            Timestamp = incomingObject.data.FirstOrDefault().timestamp,
                            OpenPrice = openPrice,
                            ClosePrice = closePrice,
                            HighPrice = highPrice,
                            LowPrice = lowPrice
                        };
                        if (marketData.OpenPrice != 0 && marketData.ClosePrice != 0 && marketData.HighPrice != 0 && marketData.LowPrice != 0)
                        {
                            context.MarketDatas.Add(marketData);
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
                ws.Send("{\"op\": \"subscribe\", \"args\": \"quote:XBTUSD\"}");
                Console.ReadLine();
            }
        }
        #endregion
    }
}
