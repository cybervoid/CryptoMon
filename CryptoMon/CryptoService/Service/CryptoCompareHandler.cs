using CryptoLib;
using CryptoLib.Abstractions;
using CryptoLib.Sockets;
using CryptoLib.Sockets.SocketModels;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace CryptoService.Service
{
    class PingModel
    {
        [JsonProperty("sid")]
        public string SID { get; set; }
        
        [JsonProperty("upgrades")]
        public string[] Upgrades { get; set; }

        [JsonProperty("pingInterval")]
        public string PingInterval { get; set; }

        [JsonProperty("pingTimeout")]
        public string PingTimeout { get; set; }
    }

    class CryptoCompareHandler
    {
        private WebSocketSharp.WebSocket socket;
        private bool isPinging = false;
        private ConcurrentQueue<string> Queue = new ConcurrentQueue<string>();
        Thread ProcessQueueThread;  //Background thread to insert records into the database... fires async background tasks
        BlockingCollection<InstrumentManager> Manager { get; set; } //Holds time index when a particular currency type was updated.
        public CryptoCompareHandler()
        {
            socket = new WebSocketSharp.WebSocket("wss://streamer.cryptocompare.com/socket.io/?EIO=2&transport=websocket");
        }
        private static System.Timers.Timer Timer;
        public bool Start()
        {
            Timer = new System.Timers.Timer();
            Timer.Interval += 60000;
            Timer.AutoReset = true;
            Timer.Elapsed += Timer_OnTimedEvent;
            Timer.Enabled = true;
            Timer.Start();
            
            Manager = new BlockingCollection<InstrumentManager>();

            socket.OnMessage += (sender, e) =>
                Socket_OnMessage(sender, e);
            socket.OnError += (sender, e) =>
                Console.WriteLine("Error: " + e.Message);
            socket.OnOpen += (sender, e) =>
                Console.WriteLine("Open: " + e.ToString());
            socket.OnClose += (sender, e) =>
                Console.WriteLine("Close: " + e.ToString());
            Initiliaze();
            ProcessQueueThread = new Thread(ProcessQueue);            
            ProcessQueueThread.IsBackground = true;            
            ProcessQueueThread.Start();
            
            return true;
        }

        private void Timer_OnTimedEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            Console.WriteLine("Timed event fired");
            if (socket.IsAlive == true)
            {
                socket.Close();
            }
            Initiliaze();            
        }
        
        private void Initiliaze()
        {            
            Console.WriteLine();
            
            socket.Connect();
            //socket.Send("42[\"SubAdd\",{\"subs\":[\"0~Cryptsy~BTC-USD\",\"0~Bitstamp~BTC~USD\",\"0~OKCoin~BTC~USD\",\"0~Coinbase~BTC~USD\",\"0~Poloniex~BTC~USD\",\"0~Cexio~BTC~USD\",\"0~BTCE~BTC~USD\",\"0~BitTrex~BTC~USD\",\"0~Kraken~BTC~USD\",\"0~Bitfinex~BTC~USD\",\"0~LocalBitcoins~BTC~USD\",\"0~itBit~BTC~USD\",\"0~HitBTC~BTC~USD\",\"0~Coinfloor~BTC~USD\",\"0~Huobi~BTC~USD\",\"0~LakeBTC~BTC~USD\",\"0~Coinsetter~BTC~USD\",\"0~CCEX~BTC~USD\",\"0~MonetaGo~BTC~USD\",\"0~Gatecoin~BTC~USD\",\"0~Gemini~BTC~USD\",\"0~CCEDK~BTC~USD\",\"0~Exmo~BTC~USD\",\"0~Yobit~BTC~USD\",\"0~BitBay~BTC~USD\",\"0~QuadrigaCX~BTC~USD\",\"0~BitSquare~BTC~USD\",\"0~TheRockTrading~BTC~USD\",\"0~Quoine~BTC~USD\",\"0~LiveCoin~BTC~USD\"]}]");
            //socket.Send("42[\"SubAdd\",{\"subs\":[\"0~Cryptsy~BTC~USD\",\"0~Bitstamp~BTC~USD\",\"0~OKCoin~BTC~USD\",\"0~Coinbase~BTC~USD\",\"0~Poloniex~BTC~USD\",\"0~Cexio~BTC~USD\",\"0~BTCE~BTC~USD\",\"0~BitTrex~BTC~USD\",\"0~Kraken~BTC~USD\",\"0~Bitfinex~BTC~USD\",\"0~LocalBitcoins~BTC~USD\",\"0~itBit~BTC~USD\",\"0~HitBTC~BTC~USD\",\"0~Coinfloor~BTC~USD\",\"0~Huobi~BTC~USD\",\"0~LakeBTC~BTC~USD\",\"0~Coinsetter~BTC~USD\",\"0~CCEX~BTC~USD\",\"0~MonetaGo~BTC~USD\",\"0~Gatecoin~BTC~USD\",\"0~Gemini~BTC~USD\",\"0~CCEDK~BTC~USD\",\"0~Exmo~BTC~USD\",\"0~Yobit~BTC~USD\",\"0~BitBay~BTC~USD\",\"0~QuadrigaCX~BTC~USD\",\"0~BitSquare~BTC~USD\",\"0~TheRockTrading~BTC~USD\",\"0~Quoine~BTC~USD\",\"0~LiveCoin~BTC~USD\"]}]");
            socket.Send("42[\"SubAdd\",{\"subs\":[\"2~Bitfinex~BTC~USD\",\"2~Coinbase~BTC~USD\",\"2~Kraken~BTC~USD\",\"2~Poloniex~BTC~USD\",\"2~Bittrex~BTC~USD\"" + 
                ",\"2~Bitfinex~ETH~USD\",\"2~Coinbase~ETH~USD\",\"2~Kraken~ETH~USD\",\"2~Poloniex~ETH~USD\",\"2~Bittrex~ETH~USD\"" + 
                ",\"2~Bitfinex~XRP~BTC\",\"2~Coinbase~XRP~BTC\",\"2~Kraken~XRP~BTC\",\"2~Poloniex~XRP~BTC\",\"2~Bittrex~XRP~BTC\"" + 
                ",\"2~Bitfinex~REP~BTC\",\"2~Coinbase~REP~BTC\",\"2~Kraken~REP~BTC\",\"2~Poloniex~REP~BTC\",\"2~Bittrex~REP~BTC\"" +
                ",\"5~CCCAGG~BTC~USD\",\"5~CCCAGG~ETH~USD\",\"5~CCCAGG~XRP~BTC\",\"5~CCCAGG~REP~BTC\"" +
                "]}]");
            
        }



        private void ProcessQueue()
        {
            while (true)
            {                
                if (!Queue.IsEmpty)
                {
                    if (Timer.Enabled == false)
                        Timer.Stop();

                    string top;

                    while (Queue.TryDequeue(out top))
                    {
                        string msg = top;                        
                        BackgroundWorker worker = new BackgroundWorker();
                        worker.DoWork += delegate { HandleData(msg); };
                        worker.RunWorkerAsync();                        
                    }
                }
                else
                {
                    if(Timer.Enabled == false)
                        Timer.Start();
                    Thread.Sleep(10);
                }
            }        
        }
        
        private void Socket_OnMessage(object sender, WebSocketSharp.MessageEventArgs e)
        {
            try
            {
                Queue.Enqueue(e.Data);                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }
        }        
        
        private void HandleData(string queuedMessage)
        {         
            string id = Regex.Match(queuedMessage, @"^([0-9]{1,2})").Value;
            string value = queuedMessage.Substring(id.Length);
            if (!string.IsNullOrEmpty(value))
            {
                dynamic thing = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(value);
                if (id == "0" && isPinging == false)
                {
                    PingModel pingModel = JsonConvert.DeserializeObject<PingModel>(value);
                    ThreadPool.QueueUserWorkItem(_ => Ping(pingModel));
                    isPinging = true;
                }
                if (id == "42")
                {
                    CryptoCompareSet set = new CryptoCompareSet(thing[1].ToString());
                    if (set.Length < 6)
                        return;
                    if (set.Type == "5")
                    {
                        if (set.Ignore == true)
                            return;

                        int exchangeID = 0;
                        int instrumentID = 0;
                        int interval = 0;
                        // = Manager.FirstOrDefault(x => x.instrument.InstrumentID == instrument.InstrumentID);
                        using (MarketsEntities context = new MarketsEntities())
                        {
                            Instruments instrument = context.Instruments.FirstOrDefault(x => x.Name == set.LASTMARKET && x.BaseCurrency == set.FromCurrency && x.QuoteCurrency == set.ToCurrency && x.Active == true);
                            if (instrument == null)
                                instrument = DataCalls.AddInstrument(new Instruments() { ExchangeID = 2, MarketID = 1, Name = set.LASTMARKET, BaseCurrency = set.FromCurrency, QuoteCurrency = set.ToCurrency, Active = true, Interval = 60 });
                            exchangeID = instrument.ExchangeID;
                            instrumentID = instrument.InstrumentID;
                            interval = instrument.Interval;
                        };
                        InstrumentManager current = Manager.FirstOrDefault(x => x.InstrumentID == instrumentID && x.Type == 5);

                        if (current != null)
                        {
                            if (DateTime.UtcNow <= current.Last.AddSeconds(interval))
                            {
                                return;
                            }
                            else
                            {
                                current.Last = DateTime.UtcNow;
                            }
                        }
                        else if (current == null)
                        {
                            Manager.Add(new InstrumentManager() { InstrumentID = instrumentID, Last = DateTime.UtcNow, Type = 5 });
                        }

                        CurrentAggData currentAggData = new CurrentAggData()
                        {
                            ExchangeID = exchangeID,
                            InstrumentID = instrumentID,
                            MarketID = 1,
                            Flag = set.Flag,
                            Price = set.Price,
                            LastUpdate = FromUnixTime(set.LastUpdate),
                            LastVolume = set.LastVolume,
                            LastVolumeTo = set.LastVolumeTo,
                            LastTradeId = set.LastTradeId,
                            Volume24Hour = set.VOLUME24HOUR,
                            Volume24HourTo = set.VOLUME24HOURTO,
                            Low24Hour = set.LOW24HOUR,
                            High24Hour = set.HIGH24HOUR,
                            Open24Hour = set.OPEN24HOUR,
                            Volume24h = set.Volume24h,
                            Volume24hTo = set.Volume24hTo,
                            MaskInt = set.MaskInt
                        };
                        DataCalls.AddCurrentAggData(currentAggData);                        
                    }
                    else
                    {
                        if ((set.Flag == 1 || set.Flag == 2 || set.Flag == 4)) //|| set.Flag == 5))
                        {
                            int exchangeID = 0;
                            int instrumentID = 0;
                            int interval = 0;
                            // = Manager.FirstOrDefault(x => x.instrument.InstrumentID == instrument.InstrumentID);
                            using (MarketsEntities context = new MarketsEntities())
                            {
                                Instruments instrument = context.Instruments.FirstOrDefault(x => x.Name == set.ExchangeName && x.BaseCurrency == set.FromCurrency && x.QuoteCurrency == set.ToCurrency && x.Active == true);
                                if (instrument == null)
                                    instrument = DataCalls.AddInstrument(new Instruments() { ExchangeID = 2, MarketID = 1, Name = set.ExchangeName, BaseCurrency = set.FromCurrency, QuoteCurrency = set.ToCurrency, Active = true, Interval = 60 });
                                exchangeID = instrument.ExchangeID;
                                instrumentID = instrument.InstrumentID;
                                interval = instrument.Interval;
                            };
                            InstrumentManager current = Manager.FirstOrDefault(x => x.InstrumentID == instrumentID && x.Type == 4);

                            if (current != null)
                            {
                                if (DateTime.UtcNow <= current.Last.AddSeconds(interval))
                                {
                                    return;
                                }                                
                                else
                                {
                                    current.Last = DateTime.UtcNow;
                                }
                            }
                            else if (current == null)
                            {
                                Manager.Add(new InstrumentManager() { InstrumentID = instrumentID, Last = DateTime.UtcNow, Type = 4 });
                            }

                            CurrentData currentData = new CurrentData()
                            {
                                //Instruments = instrument,
                                ExchangeID = exchangeID,
                                InstrumentID = instrumentID,
                                MarketID = 1,
                                Flag = set.Flag,
                                Price = set.Price,
                                LastUpdate = FromUnixTime(set.LastUpdate),
                                LastVolume = set.LastVolume,
                                LastVolumeTo = set.LastVolumeTo,
                                LastTradeId = set.LastTradeId,
                                Volume24h = set.Volume24h,
                                Volume24hTo = set.Volume24hTo,
                                MaskInt = set.MaskInt
                            };
                            DataCalls.AddCurrentData(currentData);
                        }
                    }
                }
                       
                return;
            }
            else
            {
                return;
            }
        }

        private DateTime FromUnixTime(long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(unixTime);
        }
        private void Ping(PingModel pingModel)
        {
            while(isPinging == true)
            {
                Thread.Sleep(Convert.ToInt32(pingModel.PingInterval));
                if(socket.IsAlive)
                {
                    socket.Send("0[\"ping\"]");
                }
                else
                {
                    isPinging = false;
                }                
            }
        }
        
        public bool Stop()
        {
            isPinging = false;
            return true;
        }
    }
    internal class InstrumentManager
    {
        public int Type { get; set; }
        public int InstrumentID { get; set; }
        //public int Interval { get; set; } = 30;
        public DateTime Last { get; set; }
        
    }
}
