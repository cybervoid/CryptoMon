using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Topshelf;
using CryptoLib;
using Newtonsoft.Json;
using System.IO;
using CryptoLib.Abstractions;
using CryptoLib.Sockets.SocketModels;
using CryptoLib.Sockets;

namespace CryptoService.Service
{
    public class CryptoHosting //: ServiceControl
    {
        int count = 0;
        static decimal openPrice = 0;
        static decimal closePrice = 0;
        static decimal highPrice = 0;
        static decimal lowPrice = 0;
        static DateTime hourTimestamp = DateTime.UtcNow;
        static WebSocketSharp.WebSocket webSocket;
        static CryptoHandler handler;

        public CryptoHosting()
        {
            //Constructor things.
            webSocket = new WebSocketSharp.WebSocket("wss://www.bitmex.com/realtime");
        }

        /// <summary>
        /// Called when the service starts
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            //handler = new CryptoHandler(DataCalls.GetInstruments(1));

            //string filter = "[";
            //foreach (var item in handler.Instruments)
            //{
            //    filter += string.Format($"\"quote:{item.Instrument}\",");
            //}
            //filter = filter.Substring(0, filter.Length -1) + "]"; //Remove the trailing comma and add the closing bracket.
            //webSocket.OnMessage += WebSocket_OnMessage; //Subscribe to a new on Message event.
            //webSocket.Connect();
            //string socket = "{\"op\": \"subscribe\", \"args\": " + filter + "}";             
            //webSocket.Send(socket);
            //return true;
            return true;
        }

        /// <summary>
        /// Called when the service stops
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            webSocket.Close();
            return true;
        }

        private void WebSocket_OnMessage(object sender, WebSocketSharp.MessageEventArgs e)
        {
            if (count >= 2)
            {
                try
                {
                    BitmexModel<CryptoLib.Sockets.SocketModels.BitmexQuote> incomingObject = JsonConvert.DeserializeObject<BitmexModel<BitmexQuote>>(e.Data);                                        
                    handler.Update(incomingObject); //Use this instead.                    
                    if (incomingObject.data.FirstOrDefault().timestamp.Date > hourTimestamp.Date ||
                    incomingObject.data.FirstOrDefault().timestamp.Hour > hourTimestamp.Hour ||
                    incomingObject.data.FirstOrDefault().timestamp.Minute > hourTimestamp.Minute
                    )
                    {
                        hourTimestamp = DateTime.UtcNow; //definitely change the time stamp so it doesn't accidently cause an infinite loop
                        handler.Save(); //Save the object.
                        handler.Refresh(); //Refresh the object
                        try
                        {
                            hourTimestamp = incomingObject.data.FirstOrDefault().timestamp;
                        }
                        catch(Exception)
                        {
                            hourTimestamp = DateTime.UtcNow;
                        }                       
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error:" + ex.Message);
                }
            }
            else
            {
                Console.WriteLine(e.Data.ToString());
                count++;
            }
        }

    }
}
