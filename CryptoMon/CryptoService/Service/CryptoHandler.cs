using CryptoLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoService.Service
{
    public class CryptoHandler
    {
        public List<Instruments> Instruments { get; set; }
        private List<MarketData> _MarketDataList;
        public List<MarketData> MarketDataList
        {
            get
            {
                if (_MarketDataList == null)
                {
                    this._MarketDataList = SetMarketDataList(Instruments);
                }
                return _MarketDataList;
            }
        }       
        public CryptoHandler(List<Instruments> instruments)
        {
            this.Instruments = instruments;
            this._MarketDataList = SetMarketDataList(Instruments); 
        }        
        public void Update(CryptoLib.Sockets.SocketModels.BitmexModel<CryptoLib.Sockets.SocketModels.BitmexQuote> incomingObject) //
        {
            //Find the correct MarketData object in the MarketDataList            
            //MarketData current = MarketDataList.FirstOrDefault(x => x.Instruments.Instrument.Equals(incomingObject.data.FirstOrDefault().symbol));
            ////Update that specific object's values
            //if (current != null)
            //{               
            //    current.ClosePrice  = ((incomingObject.data.FirstOrDefault().askPrice + incomingObject.data.FirstOrDefault().bidPrice) / 2);
            //    current.HighPrice = Math.Max(current.ClosePrice, current.HighPrice);
            //    current.LowPrice = Math.Min(current.ClosePrice, current.LowPrice);
            //    current.Timestamp = incomingObject.data.FirstOrDefault().timestamp;                
            //    if (current.OpenPrice == 0)
            //        current.OpenPrice = current.ClosePrice;                
            //    if (current.LowPrice == 0)
            //        current.LowPrice = current.ClosePrice;
            //}            
        }

        private List<MarketData> SetMarketDataList(List<Instruments> instruments)
        {
            List<CryptoLib.MarketData> marketDataList = new List<CryptoLib.MarketData>();
            if (instruments.Count > 0)
            {
                foreach (var item in instruments)
                {
                    marketDataList.Add(new MarketData()
                    {
                        InstrumentID = item.InstrumentID,
                        MarketID = item.MarketID,
                        ExchangeID = item.ExchangeID,
                        Timestamp = DateTime.Now,
                        Instruments = item
                    });
                }
            }
            return marketDataList;
        }
        /// <summary>
        /// Call this method to save the current market data list.
        /// </summary>
        public void Save()
        {
            try
            {                
                if (this._MarketDataList != null & this._MarketDataList.Count > 0)
                {
                    Parallel.ForEach(MarketDataList, x => SaveMarketData(x));
                }                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void SaveMarketData(MarketData marketData)
        {
            if (marketData.OpenPrice != 0 && marketData.ClosePrice != 0 && marketData.HighPrice != 0 && marketData.LowPrice != 0)
            {
                using (MarketsEntities context = new CryptoLib.MarketsEntities())
                {
                    context.Instruments.Attach(marketData.Instruments);
                    context.MarketData.Add(marketData);                    
                    context.SaveChanges();
                };
            }
        }
        /// <summary>
        /// Refresh the MarketDataList before starting the next iteration.
        /// </summary>
        public void Refresh()
        {
            if (this._MarketDataList != null & this._MarketDataList.Count > 0)
            {
                Parallel.ForEach(MarketDataList, x => RefreshMarketData(x));
            }
        }

        private void RefreshMarketData(MarketData marketData)
        {
            marketData.OpenPrice = marketData.ClosePrice;
            marketData.ClosePrice = 0;
            marketData.Timestamp = DateTime.Now;
            marketData.LowPrice = 0;
            marketData.HighPrice = 0;            
        }
    }
}

