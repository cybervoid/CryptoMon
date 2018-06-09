using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoLib.Helpers;

namespace CryptoLib.Abstractions
{
    public static class DataCalls
    {
        public static List<Instruments> GetInstruments(int exchangeID)
        {
            List<Instruments> instruments = new List<Instruments>();
            using (MarketsEntities context = new MarketsEntities())
            {
                instruments = context.Instruments.Where(x => x.ExchangeID == exchangeID && x.Active == true).Select(y => y).ToList();
            };
            return instruments;
        }        

        public static Instruments AddInstrument(Instruments instrument)
        {
            using (MarketsEntities context = new MarketsEntities())
            {
                if (instrument.Exchanges != null)
                    context.Exchanges.Attach(instrument.Exchanges);
                if (instrument.Markets != null)
                    context.Markets.Attach(instrument.Markets);
                //  instruments = context.Instruments.Where(x => x.ExchangeID == exchangeID).Select(y => y).ToList();
                instrument = context.Instruments.Add(instrument);
                context.SaveChanges();
            };
            return instrument;
        }

        public static CurrentData AddCurrentData(CurrentData currentData)
        {
            using (MarketsEntities context = new MarketsEntities())
            {
                if (currentData.Instruments != null)
                    context.Instruments.Attach(currentData.Instruments);
                if (currentData.Markets != null)
                    context.Markets.Attach(currentData.Markets);
                if (currentData.Exchanges != null)
                    context.Exchanges.Attach(currentData.Exchanges);

                currentData = context.CurrentData.Add(currentData);
                context.SaveChanges();
            }
            return currentData;
        }
        /// <summary>
        /// Gets all CurrentData within a date-range and for the specific intrument
        /// </summary>
        /// <param name="instrumentID"></param>
        /// <param name="minuteInterval">The time span to include in this list.</param>
        /// <returns>Returns a threadsafe collection</returns>
        public static BlockingCollection<CurrentData> GetCurrentData(int instrumentID, int minuteInterval)
        {
            BlockingCollection<CurrentData> collection = new BlockingCollection<CurrentData>();
            using (MarketsEntities context = new MarketsEntities())
            {
                DateTime timeCutoff = DateTime.UtcNow.AddMinutes(-minuteInterval);
                var query = context.CurrentData.Where(x => x.InstrumentID == instrumentID && x.LastUpdate > timeCutoff).OrderBy(y => y.LastUpdate).Select(p => p).ToList();                
                query.ForEach(q => collection.Add(q)); //Add the list to the blockingcollection.
            }
            return collection;
        }
        /// <summary>
        /// Gets the aggreggated data already provided by the socket
        /// </summary>
        /// <param name="instrumentID"></param>
        /// <param name="minuteInterval"></param>
        /// <returns></returns>
        internal static BlockingCollection<CurrentAggData> GetCurrentAggData(int instrumentID, int minuteInterval)
        {
            BlockingCollection<CurrentAggData> collection = new BlockingCollection<CurrentAggData>();
            using (MarketsEntities context = new MarketsEntities())
            {
                DateTime timeCutoff = DateTime.UtcNow.AddMinutes(-minuteInterval);
                var query = context.CurrentAggData.Where(x => x.InstrumentID == instrumentID && x.LastUpdate > timeCutoff).OrderBy(y => y.LastUpdate).Select(p => p).ToList();
                query.ForEach(q => collection.Add(q)); //Add the list to the blockingcollection.
            }
            return collection;
        }

        public static List<CurrentData> GetRecentCurrentData(int instrumentID, int minuteInterval, int lastIndex)
        {
            List<CurrentData> data = new List<CurrentData>();
            using (MarketsEntities context = new MarketsEntities())
            {
                DateTime timeCutoff = DateTime.UtcNow.AddMinutes(-minuteInterval);
                var query = context.CurrentData.Where(x => x.CurrentID > lastIndex && x.InstrumentID == instrumentID && x.LastUpdate > timeCutoff).OrderBy(y => y.LastUpdate).Select(p => p).ToList();
            }
            return data;
        }

        public static List<CurrentAggData> GetRecentCurrentAggData(int instrumentID, int minuteInterval, int lastIndex)
        {
            List<CurrentAggData> data = new List<CurrentAggData>();
            using (MarketsEntities context = new MarketsEntities())
            {
                DateTime timeCutoff = DateTime.UtcNow.AddMinutes(-minuteInterval);
                var query = context.CurrentAggData.Where(x => x.CurrentAggID > lastIndex && x.InstrumentID == instrumentID && x.LastUpdate > timeCutoff).OrderBy(y => y.LastUpdate).Select(p => p).ToList();
            }
            return data;
        }

        public static CurrentAggData AddCurrentAggData(CurrentAggData currentAggData)
        {
            using (MarketsEntities context = new MarketsEntities())
            {
                if (currentAggData.Instruments != null)
                    context.Instruments.Attach(currentAggData.Instruments);
                if (currentAggData.Markets != null)
                    context.Markets.Attach(currentAggData.Markets);
                if (currentAggData.Exchanges != null)
                    context.Exchanges.Attach(currentAggData.Exchanges);

                currentAggData = context.CurrentAggData.Add(currentAggData);
                context.SaveChanges();
            }
            return currentAggData;
        }

        public static BlockingCollection<MarketData> GetMarketDataQueue(Instruments instrument, int minuteInterval)
        {
            BlockingCollection<MarketData> collection = new BlockingCollection<MarketData>();
            using (MarketsEntities context = new MarketsEntities())
            {
                DateTime timeCutoff = DateTime.UtcNow.AddMinutes(-minuteInterval);
                var query= context.MarketData.Where(x => x.InstrumentID == instrument.InstrumentID && x.Timestamp > timeCutoff).OrderBy(y => y.Timestamp).Select(p => p).ToList();
                query.ForEach(q => collection.Add(q));
            };
            return collection;
        }
        public static BlockingCollection<MarketData> UpdateMarketData(this BlockingCollection<MarketData> collection, Instruments instrument, int minuteInterval, int index)
        {            
            using (MarketsEntities context = new MarketsEntities())
            {
                DateTime timeCutoff = DateTime.UtcNow.AddMinutes(-minuteInterval);
                var query = context.MarketData.Where(x => x.InstrumentID == instrument.InstrumentID && x.MarketDataID > index && x.Timestamp > timeCutoff ).OrderBy(y => y.Timestamp).Select(p => p).ToList();
                query.ForEach(q => collection.Add(q));
            };
            return collection;
        }
    }
}
