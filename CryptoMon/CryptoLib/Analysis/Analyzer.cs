using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoLib.Analysis
{
    public class Analyzer
    {
        public List<Instruments> Instruments { get; set; }
        public List<CoinQueue> CoinQueues { get; set; }

        public Analyzer()
        {
            this.CoinQueues = new List<CoinQueue>();
        }

        public void InitializeQueues()
        {
            Instruments = Abstractions.DataCalls.GetInstruments(2);
            foreach (var instrument in Instruments)
            {
                CoinQueue coin = new CoinQueue(instrument);
                if(coin.Empty == false)    
                    CoinQueues.Add(coin); //Add a new instance of a coin queue to the list of CoinQueues for each instrument.
            }
        }
    }
}
