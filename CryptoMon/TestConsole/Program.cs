using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using Newtonsoft.Json;
using System.Reflection;
using CryptoLib.Abstractions;
using CryptoLib;
using System.Collections.Concurrent;
using CryptoLib.Helpers;

namespace TestConsole
{

    

    class Program
    {
        static void Main(string[] args)
        {
            using (var ws = new WebSocket("wss://streamer.cryptocompare.com/socket.io/?EIO=2&transport=websocket"))
            {
                ws.OnMessage += (sender, e) =>
                    Console.WriteLine(e.Data);

                ws.Connect();
                ws.Connect();
                //socket.Send("42[\"SubAdd\",{\"subs\":[\"0~Cryptsy~BTC-USD\",\"0~Bitstamp~BTC~USD\",\"0~OKCoin~BTC~USD\",\"0~Coinbase~BTC~USD\",\"0~Poloniex~BTC~USD\",\"0~Cexio~BTC~USD\",\"0~BTCE~BTC~USD\",\"0~BitTrex~BTC~USD\",\"0~Kraken~BTC~USD\",\"0~Bitfinex~BTC~USD\",\"0~LocalBitcoins~BTC~USD\",\"0~itBit~BTC~USD\",\"0~HitBTC~BTC~USD\",\"0~Coinfloor~BTC~USD\",\"0~Huobi~BTC~USD\",\"0~LakeBTC~BTC~USD\",\"0~Coinsetter~BTC~USD\",\"0~CCEX~BTC~USD\",\"0~MonetaGo~BTC~USD\",\"0~Gatecoin~BTC~USD\",\"0~Gemini~BTC~USD\",\"0~CCEDK~BTC~USD\",\"0~Exmo~BTC~USD\",\"0~Yobit~BTC~USD\",\"0~BitBay~BTC~USD\",\"0~QuadrigaCX~BTC~USD\",\"0~BitSquare~BTC~USD\",\"0~TheRockTrading~BTC~USD\",\"0~Quoine~BTC~USD\",\"0~LiveCoin~BTC~USD\"]}]");
                //socket.Send("42[\"SubAdd\",{\"subs\":[\"0~Cryptsy~BTC~USD\",\"0~Bitstamp~BTC~USD\",\"0~OKCoin~BTC~USD\",\"0~Coinbase~BTC~USD\",\"0~Poloniex~BTC~USD\",\"0~Cexio~BTC~USD\",\"0~BTCE~BTC~USD\",\"0~BitTrex~BTC~USD\",\"0~Kraken~BTC~USD\",\"0~Bitfinex~BTC~USD\",\"0~LocalBitcoins~BTC~USD\",\"0~itBit~BTC~USD\",\"0~HitBTC~BTC~USD\",\"0~Coinfloor~BTC~USD\",\"0~Huobi~BTC~USD\",\"0~LakeBTC~BTC~USD\",\"0~Coinsetter~BTC~USD\",\"0~CCEX~BTC~USD\",\"0~MonetaGo~BTC~USD\",\"0~Gatecoin~BTC~USD\",\"0~Gemini~BTC~USD\",\"0~CCEDK~BTC~USD\",\"0~Exmo~BTC~USD\",\"0~Yobit~BTC~USD\",\"0~BitBay~BTC~USD\",\"0~QuadrigaCX~BTC~USD\",\"0~BitSquare~BTC~USD\",\"0~TheRockTrading~BTC~USD\",\"0~Quoine~BTC~USD\",\"0~LiveCoin~BTC~USD\"]}]");
                ws.Send("42[\"SubAdd\",{\"subs\":[\"2~Bitfinex~BTC~USD\",\"2~Coinbase~BTC~USD\",\"2~Kraken~BTC~USD\",\"2~Poloniex~BTC~USD\",\"2~Bittrex~BTC~USD\"" +
                    ",\"2~Bitfinex~ETH~USD\",\"2~Coinbase~ETH~USD\",\"2~Kraken~ETH~USD\",\"2~Poloniex~ETH~USD\",\"2~Bittrex~ETH~USD\"" +
                    ",\"2~Bitfinex~XRP~BTC\",\"2~Coinbase~XRP~BTC\",\"2~Kraken~XRP~BTC\",\"2~Poloniex~XRP~BTC\",\"2~Bittrex~XRP~BTC\"" +
                    ",\"2~Bitfinex~REP~BTC\",\"2~Coinbase~REP~BTC\",\"2~Kraken~REP~BTC\",\"2~Poloniex~REP~BTC\",\"2~Bittrex~REP~BTC\"" +
                    ",\"5~CCCAGG~BTC~USD\",\"5~CCCAGG~ETH~USD\",\"5~CCCAGG~XRP~BTC\",\"5~CCCAGG~REP~BTC\"" +
                    "]}]");
                Console.ReadKey(true);
            }
        }
    }
}
