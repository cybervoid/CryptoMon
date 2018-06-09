using CryptoLib.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoLib.Helpers;
using CryptoLib.Analysis.Data;
using System.Threading;

namespace CryptoLib.Analysis
{
    public class CoinQueue
    {
        /// <summary>
        /// An instrument is a Coin Data on a specific exchange. 
        /// </summary>
        public Instruments Instrument { get; set; }
        /// <summary>
        /// The desired total duration for tracking a coin.
        /// </summary>
        public int MinuteInterval { get; set; }
        /// <summary>
        /// The raw coin data provided by the socket stream
        /// </summary>
        public BlockingCollection<CurrentData> CurrentData { get; set; }
        private static readonly object coinLock = new object();
        public int CurrentDataIndex { get; set; } 
        /// <summary>
        /// The aggregated data provided by the socket stream
        /// </summary>
        public BlockingCollection<CurrentAggData> CurrentAggData { get; set; }
        private static readonly object coinAggLock = new object();
        public int CurrentAggDataIndex { get; set; }
        public UpdateClass Updater { get; set; }
        public bool Empty
        {
            get
            {
                if ((this.CurrentData == null || this.CurrentData.Count <= 0) && (this.CurrentAggData == null || this.CurrentAggData.Count <= 0 ))
                    return true;
                else
                    return false;
            }
        }
        public CoinQueue(Instruments instrument)
        {
            this.Instrument = instrument;
            Updater = new UpdateClass();
            this.MinuteInterval = 60 * 24 * 2;
            Start();
        }

        private void Start()
        {            
            if(this.Instrument != null)
            {
                lock (coinLock)
                {
                    this.CurrentData = DataCalls.GetCurrentData(Instrument.InstrumentID, MinuteInterval);
                    if (this.CurrentData == null || this.CurrentData.Count <= 0)
                        CurrentDataIndex = -1;
                    else
                        CurrentDataIndex = this.CurrentData.Max(x => x.CurrentID);
                }
                lock(coinAggLock)
                {
                    this.CurrentAggData = DataCalls.GetCurrentAggData(Instrument.InstrumentID, MinuteInterval);
                    if (this.CurrentAggData == null || this.CurrentAggData.Count <= 0)
                        CurrentAggDataIndex = -1;
                    else
                        CurrentAggDataIndex = this.CurrentAggData.Max(x => x.CurrentAggID);
                }
            }

            //Updater.StartUpdate += (sender, e) => Update_OnStart(sender, e);       
            Update_OnStart();
        }
       
        public void Update_OnStart()
        {
            if (Empty == false) //Don't start the thread if this instrument is not collecting data.
            {
                Thread update = new Thread(Update);
                update.IsBackground = true;
                update.Start();
            }
        }

        //Call this method to update 
        public void Update()
        {
            while (Empty == false) //Note - opportunity for efficiency... >:)
            {
                //step 1: Remove old data from the collections.
                lock (coinLock)
                {
                    if (CurrentData != null && CurrentData.Count > 0)
                    {
                        //Step 1: Remove old data from the collections.                     
                        CurrentData.TakeAndBreak(x => x.LastUpdate > DateTime.UtcNow.AddMinutes(-MinuteInterval));
                        //Step 2: Get new data from the database and add it to the collections.
                        DataCalls.GetRecentCurrentData(Instrument.InstrumentID, MinuteInterval, CurrentDataIndex).ForEach(x => CurrentData.Add(x));
                        CurrentDataIndex = this.CurrentData.Max(x => x.CurrentID);
                    }
                }
                lock (coinAggLock)
                {
                    if (CurrentAggData != null && CurrentAggData.Count > 0)
                    {
                        CurrentAggData.TakeAndBreak(x => x.LastUpdate > DateTime.UtcNow.AddMinutes(-MinuteInterval));
                        DataCalls.GetRecentCurrentAggData(Instrument.InstrumentID, MinuteInterval, CurrentAggDataIndex).ForEach(x => CurrentAggData.Add(x));
                        CurrentAggDataIndex = this.CurrentAggData.Max(x => x.CurrentAggID);
                    }
                }
                Thread.Sleep(50000);
            }
        }
        
        public CandleChart GetCandles(DateTime startTime, int timeInterval)
        {
            //Create a list of CandleSticks
            List<CandleStick> candles = new List<CandleStick>();

            DateTime currentTime = startTime;
            DateTime endInterval = startTime.AddMinutes(timeInterval);
            
            TimeSpan span = DateTime.Now.Subtract(startTime);
            int maxIntervals = Convert.ToInt32(Math.Ceiling(((decimal)span.Minutes / (decimal)timeInterval)));

            int currentIndex = 0;
            //int lastIndex = 0;
            int currentDataCount = CurrentData.Count;
            //Generate the data for each candle stick
            for (int i = 0; i < maxIntervals; )
            {
                if(currentIndex >= currentDataCount) { break; }
                List<CandleStickDataPoint> candleStickDataPoints = new List<CandleStickDataPoint>();                
                for (int ii = currentIndex; ii < currentDataCount; ii++)                    
                {                    
                    CurrentData data = CurrentData.ElementAt<CurrentData>(ii);
                    if (data != null)
                    {                       
                        if (data.LastUpdate >= startTime && data.LastUpdate < endInterval)
                        {
                            candleStickDataPoints.Add(new CandleStickDataPoint() { Price = data.Price, Time = data.LastUpdate });
                        }
                        
                        if (data.LastUpdate >= endInterval)
                            break;
                    }
                    currentIndex++;
                }
                if (candleStickDataPoints != null && candleStickDataPoints.Count > 0)
                {
                    candles.Add(new CandleStick(candleStickDataPoints) { Index = i });
                    i++;
                }                
            }
            return new CandleChart(candles, this.Instrument);   
        }
    }

    public class UpdateClass
    {
        public event EventHandler StartUpdate;
    }
}
