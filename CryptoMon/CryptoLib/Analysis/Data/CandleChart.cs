using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoLib.Analysis.Data
{
    public class CandleChart
    {
        public List<CandleStick> Candles { get; set; }
        public Instruments Instrument { get; set; }

        public CandleChart(List<CandleStick> candles, Instruments instrument)
        {
            this.Candles = candles;
            this.Instrument = instrument;
        }

        private decimal? _Min;
        public decimal Min
        {
            get
            {
                if(_Min == null)
                {
                    _Min = Candles.Min(x => x.Low);
                }
                return _Min.Value;
            }
        }
        private decimal? _Max;
        public decimal Max
        {
            get
            {
                if (_Max == null)
                {
                    _Max = Candles.Max(x => x.High);
                }
                return _Max.Value;
            }
        }

        private DateTime _BeginTime;
        public DateTime BeginTime
        {
            get
            {
                if (_BeginTime == null)
                {
                    _BeginTime = Candles.Min(x => x.Start);
                }
                return _BeginTime;
            }
        }

        private DateTime _EndTime;
        public DateTime EndTime
        {
            get
            {
                if (_EndTime == null)
                {
                    _EndTime = Candles.Min(x => x.End);
                }
                return _EndTime;
            }
        }
    }
}
