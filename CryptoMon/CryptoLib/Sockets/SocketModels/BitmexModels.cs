using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoLib.Sockets.SocketModels
{
    public class BitmexModel<T>
    {
        public string table { get; set; }
        public string action { get; set; }
        public List<T> data { get; set; }
    }

    public class BitmexQuote
    {
        public string symbol { get; set; }
        public DateTime timestamp { get; set; }
        public int bidSize { get; set; }
        public decimal bidPrice { get; set; }
        public decimal askPrice { get; set; }
        public int askSize { get; set; }
    }

    public class BitmexInstrument
    {
        public string symbol { get; set; }
        public string timestamp { get; set; }

        public decimal bidPrice { get; set; }
        public decimal midPrice { get; set; }
        public decimal askPrice { get; set; }

        public decimal impactBidPrice { get; set; }
        public decimal impactMidPrice { get; set; }
        public decimal impactAskPrice { get; set; }

        public decimal lastPrice { get; set; }
        public int lastChangePcnt { get; set; }

        public long openValue { get; set; }

        public decimal fairPrice { get; set; }
        public decimal markPrice { get; set; }
        public decimal indicativeSettlePrice { get; set; }
    }
}
