using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoLib.Analysis.Data
{
    public class CandleStick : IDataPointWrapper<CandleStickDataPoint>
    {

        public List<CandleStickDataPoint> Data { get; set; }
        public int Index { get; set; }
        public CandleStick(List<CandleStickDataPoint> data)
        {
            this.Data = data;
        }

        public bool Increase
        {
            get
            {
                if (Open < Close)
                    return true;
                else
                    return false;
            }
        }

        private decimal? _High;
        public decimal High
        {
            get
            {
                if(_High == null)
                    _High = Data.Max(x => x.Price);
                return _High.Value;
            }
        }
        private decimal? _Low;
        public decimal Low
        {
            get
            {
                if (_Low == null)
                    _Low = Data.Max(x => x.Price);
                return _Low.Value;
            }
        }
        private decimal? _Average;
        public decimal Average
        {
            get
            {
                if (_Average == null)
                    _Average = Data.Average(x => x.Price);
                return _Average.Value;
            }
        }
        private decimal? _Open;
        public decimal Open
        {
            get
            {
                if (_Open == null)
                    _Open = Data.Where(x => x.Time == Start).Select(x => x.Price).FirstOrDefault();
                return _Open.Value;
            }
        }
        private decimal? _Close;
        public decimal Close
        {
            get
            {
                if (_Close == null)
                    _Close = Data.Where(x => x.Time == End).Select(x => x.Price).FirstOrDefault();
                return _Close.Value;
            }
        }
        private DateTime _Start;
        public DateTime Start
        {
            get
            {
                if (_Start == null)
                    _Start = Data.Min(x => x.Time);
                return _Start;
            }
        }
        private DateTime _End;
        public DateTime End
        {
            get
            {
                if (_End == null)
                    _End = Data.Max(x => x.Time);
                return _End;
            }
        }

    }

}
