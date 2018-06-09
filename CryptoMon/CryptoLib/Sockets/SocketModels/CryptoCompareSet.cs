using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;

namespace CryptoLib.Sockets.SocketModels
{
    
    public class CryptoCompareSet
    {

        public bool Ignore
        {
            get
            {
                if (_LastUpdate == null)
                {
                    return true;
                }
                else if(this._Type == "5" && (string.IsNullOrEmpty(_LOW24HOUR) || string.IsNullOrEmpty(_OPEN24HOUR) ||
                    string.IsNullOrEmpty(_HIGH24HOUR) || string.IsNullOrEmpty(_LASTMARKET)))
                {
                    return true;
                }                
                else
                {
                    return false;
                }
            }
         } 
        public string _Type;    
        public string Type
        {
            get
            {                
                return _Type;                
            }
        }
        public string _ExchangeName;        
        public string ExchangeName
        {
            get
            {
                return _ExchangeName;
            }
        }
        public string _FromCurrency;        
        public string FromCurrency
        {
            get
            {
                return _FromCurrency;
            }
        }
        public string _ToCurrency;        
        public string ToCurrency
        {
            get
            {               
                return _ToCurrency;                
            }
        }
        public string _Flag;        
        public int Flag
        {
            get
            {               
                return int.Parse(_Flag);                
            }
        }
        public string _Price;        
        public decimal Price
        {
            get
            {
                decimal result;
                bool status = decimal.TryParse(_Price, out result);
                if (status == true)
                {
                    return result;
                }
                else
                {
                    status = decimal.TryParse(_Price, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"), out result);
                    if (status == true)
                    {
                        return result;
                    }
                    return 0;
                }
            }
        }
        public string _LastUpdate;        
        public long LastUpdate
        {
            get
            {               
                if(_LastUpdate == null)
                {                    
                    return 0;
                }
                return long.Parse(_LastUpdate);               
            }
        }
        public string _LastVolume;        
        public decimal LastVolume
        {
            get
            {
                decimal result;
                bool status = decimal.TryParse(_LastVolume, out result);
                if (status == true)
                {
                    return result;
                }
                else
                {
                    status = decimal.TryParse(_LastVolume, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"), out result);
                    if (status == true)
                    {
                        return result;
                    }
                    return 0;
                }
            }
        }
        public string _LastVolumeTo;        
        public decimal LastVolumeTo
        {
            get
            {
                decimal result;
                bool status = decimal.TryParse(_LastVolumeTo, out result);
                if (status == true)
                {
                    return result;
                }
                else
                {
                    status = decimal.TryParse(_LastVolumeTo, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"), out result);
                    if (status == true)
                    {
                        return result;
                    }
                    return 0;
                }
            }
        }

        public string _LastTradeId;        
        public int LastTradeId
        {
            get
            {
                int val = 0;
                bool result = int.TryParse(_LastTradeId, out val);
                if (result == true)
                    return val;
                else
                    return 0;                
            }
        }
        public string _Volume24h;        
        public decimal Volume24h
        {
            get
            {
                decimal result;
                bool status = decimal.TryParse(_Volume24h, out result);
                if (status == true)
                {
                    return result;
                }
                else
                {
                    status = decimal.TryParse(_Volume24h, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"), out result);
                    if (status == true)
                    {
                        return result;
                    }
                    return 0;
                }
            }
        }

        private string _Volume24hTo;        
        public decimal Volume24hTo
        {
            get
            {
                decimal result;
                bool status = decimal.TryParse(_Volume24hTo, out result);
                if (status == true)
                {
                    return result;
                }
                else
                {
                    status = decimal.TryParse(_Volume24hTo, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"), out result);
                    if (status == true)
                    {
                        return result;
                    }
                    return 0;
                }
            }
        }
        private int _MaskInt;        
        public int MaskInt
        {
            get
            {                
                return _MaskInt;                
            }
        }

        public string Message { get; set; }

        private List<KeyValuePair<string, int>> FLAGS = new List<KeyValuePair<string, int>>()
        {
            new KeyValuePair<string, int>("PRICEUP"  , 0x1 ), // hex for binary 1
            new KeyValuePair<string, int>("PRICEDOWN", 0x2 ), // hex for binary 10
            new KeyValuePair<string, int>("PRICEUNCHANGED" , 0x4 ), // hex for binary 100
            new KeyValuePair<string, int>("BIDUP"    , 0x8 ), // hex for binary 1000
            new KeyValuePair<string, int>("BIDDOWN"  , 0x10), // hex for binary 10000
            new KeyValuePair<string, int>("BIDUNCHANGED"   , 0x20), // hex for binary 100000
            new KeyValuePair<string, int>("OFFERUP"  , 0x40), // hex for binary 1000000
            new KeyValuePair<string, int>("OFFERDOWN", 0x80), // hex for binary 10000000
            new KeyValuePair<string, int>("OFFERUNCHANGED" , 0x100 ), // hex for binary 100000000
            new KeyValuePair<string, int>("AVGUP"    , 0x200 ), // hex for binary 1000000000
            new KeyValuePair<string, int>("AVGDOWN"  , 0x400 ), // hex for binary 10000000000
            new KeyValuePair<string, int>("AVGUNCHANGED"   , 0x800 ) // hex for binary 100000000000
        };
        private List<KeyValuePair<string, int>> FIELDS = new List<KeyValuePair<string, int>>()
        {
            new KeyValuePair<string, int>("TYPE", 0x0),
            new KeyValuePair<string, int>("MARKET", 0x0),
            new KeyValuePair<string, int>("FROMSYMBOL", 0x0  ), // hex for binary 0 it is a special case of fields that are always there
            new KeyValuePair<string, int>("TOSYMBOL"   , 0x0  ), // hex for binary 0 it is a special case of fields that are always there
            new KeyValuePair<string, int>("FLAGS"     , 0x0  ), // hex for binary 0 it is a special case of fields that are always there
            new KeyValuePair<string, int>("PRICE"     ,  0x1  ), // hex for binary 1
            new KeyValuePair<string, int>("BID"       ,  0x2  ), // hex for binary 10
            new KeyValuePair<string, int>("OFFER"     ,  0x4  ), // hex for binary 100
            new KeyValuePair<string, int>("LASTUPDATE", 0x8  ), // hex for binary 1000
            new KeyValuePair<string, int>("AVG"       ,  0x10 ), // hex for binary 10000
            new KeyValuePair<string, int>("LASTVOLUME", 0x20 ), // hex for binary 100000
            new KeyValuePair<string, int>("LASTVOLUMETO",  0x40 ), // hex for binary 1000000
            new KeyValuePair<string, int>("LASTTRADEID", 0x80 ), // hex for binary 10000000
            new KeyValuePair<string, int>("VOLUMEHOUR" , 0x100), // hex for binary 100000000
            new KeyValuePair<string, int>("VOLUMEHOURTO"  ,  0x200), // hex for binary 1000000000
            new KeyValuePair<string, int>("VOLUME24HOUR"  ,  0x400), // hex for binary 10000000000
            new KeyValuePair<string, int>("VOLUME24HOURTO" ,  0x800), // hex for binary 100000000000
            new KeyValuePair<string, int>("OPENHOUR"   , 0x1000  ), // hex for binary 1000000000000
            new KeyValuePair<string, int>("HIGHHOUR"  ,  0x2000  ), // hex for binary 10000000000000
            new KeyValuePair<string, int>("LOWHOUR"    , 0x4000  ), // hex for binary 100000000000000
            new KeyValuePair<string, int>("OPEN24HOUR", 0x8000  ), // hex for binary 1000000000000000
            new KeyValuePair<string, int>("HIGH24HOUR", 0x10000 ), // hex for binary 10000000000000000
            new KeyValuePair<string, int>("LOW24HOUR" ,  0x20000 ), // hex for binary 100000000000000000
            new KeyValuePair<string, int>("LASTMARKET", 0x40000 ), // hex for binary 1000000000000000000, this is a special case and will only appear on CCCAGG messages
        };
        
        //public string _BID { get; set; }
        //public decimal BID { get; set; }

        //public string _OFFER { get; set; }
        //public decimal OFFER { get; set; }

        //public string _AVG { get; set; }
        //public decimal AVG { get; set; }

        //public string _VOLUMEHOUR { get; set; }
        //public decimal VOLUMEHOUR { get; set; }

        public string _VOLUMEHOURTO { get; set; }
        public decimal VOLUMEHOURTO
        {
            get
            {
                decimal result;
                bool status = decimal.TryParse(_VOLUMEHOURTO, out result);
                if (status == true)
                {
                    return result;
                }
                else
                {
                    status = decimal.TryParse(_VOLUMEHOURTO, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"), out result);
                    if (status == true)
                    {
                        return result;
                    }
                    return 0;
                }
            }
        }

        public string _VOLUME24HOUR { get; set; }
        public decimal VOLUME24HOUR
        {
            get
            {
                decimal result;
                bool status = decimal.TryParse(_VOLUME24HOUR, out result);
                if (status == true)
                {
                    return result;
                }
                else
                {
                    status = decimal.TryParse(_VOLUME24HOUR, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"), out result);
                    if (status == true)
                    {
                        return result;
                    }
                    return 0;
                }
            }
        }

        public string _VOLUME24HOURTO { get; set; }
        public decimal VOLUME24HOURTO
        {
            get
            {
                decimal result;
                bool status = decimal.TryParse(_VOLUME24HOURTO, out result);
                if (status == true)
                {
                    return result;
                }
                else
                {
                    status = decimal.TryParse(_VOLUME24HOURTO, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"), out result);
                    if (status == true)
                    {
                        return result;
                    }
                    return 0;
                }
            }
        }

        public string _OPENHOUR { get; set; }
        public decimal OPENHOUR
        {
            get
            {
                decimal result;
                bool status = decimal.TryParse(_OPENHOUR, out result);
                if (status == true)
                {
                    return result;
                }
                else
                {
                    status = decimal.TryParse(_OPENHOUR, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"), out result);
                    if (status == true)
                    {
                        return result;
                    }
                    return 0;
                }
            }
        }

        public string _HIGHHOUR { get; set; }
        public decimal HIGHHOUR
        {
            get
            {
                decimal result;
                bool status = decimal.TryParse(_HIGHHOUR, out result);
                if (status == true)
                {
                    return result;
                }
                else
                {
                    status = decimal.TryParse(_HIGHHOUR, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"), out result);
                    if (status == true)
                    {
                        return result;
                    }
                    return 0;
                }
            }
        }

        public string _LOWHOUR { get; set; }
        public decimal LOWHOUR
        {
            get
            {
                decimal result;
                bool status = decimal.TryParse(_LOWHOUR, out result);
                if (status == true)
                {
                    return result;
                }
                else
                {
                    status = decimal.TryParse(_LOWHOUR, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"), out result);
                    if (status == true)
                    {
                        return result;
                    }
                    return 0;
                }
            }
        }

        public string _OPEN24HOUR { get; set; }
        public decimal OPEN24HOUR
        {
            get
            {
                decimal result;
                bool status = decimal.TryParse(_OPEN24HOUR, out result);
                if (status == true)
                {
                    return result;
                }
                else
                {
                    status = decimal.TryParse(_OPEN24HOUR, System.Globalization.NumberStyles.Any, 
                        System.Globalization.CultureInfo.CreateSpecificCulture("en-US"), out result);
                    if (status == true)
                    {
                        return result;
                    }
                    return 0;
                }
            }
        }

        public string _HIGH24HOUR { get; set; }
        public decimal HIGH24HOUR
        {
            get
            {
                decimal result;
                bool status = decimal.TryParse(_HIGH24HOUR, out result);
                if (status == true)
                {
                    return result;
                }
                else
                {
                    status = decimal.TryParse(_HIGH24HOUR, System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.CreateSpecificCulture("en-US"), out result);
                    if (status == true)
                    {
                        return result;
                    }
                    return 0;
                }
            }
        }

        public string _LOW24HOUR { get; set; }
        public decimal LOW24HOUR
        {
            get
            {
                decimal result;
                bool status = decimal.TryParse(_LOW24HOUR, out result);
                if (status == true)
                {
                    return result;
                }
                else
                {
                    status = decimal.TryParse(_LOW24HOUR, System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.CreateSpecificCulture("en-US"), out result);
                    if (status == true)
                    {
                        return result;
                    }
                    return 0;
                }
            }
        }

        public string _LASTMARKET { get; set; }
        public string LASTMARKET
        {
            get
            {
                return _LASTMARKET;
            }
        }


        public CryptoCompareSet(string message)
        {
            this.Message = message;            
            Unpack();
        }

        public int Length { get; set; }
        public void Unpack()
        {
            try { 
                string[] valuesArray = Message.Split('~');
                Length = valuesArray.Length;

                if (Length < 6) { return; }
                    
                string mask = valuesArray[Length - 1];
                this._MaskInt = Int32.Parse(mask, System.Globalization.NumberStyles.HexNumber); 

               //dynamic unpackedCurrent = new ExpandoObject();

                int currentField = 0;

                foreach (var property in this.FIELDS)
                {
                    //maskInt & property.Value
                    if (property.Value == 0)
                    {
                        string val = valuesArray[currentField];
                        if (property.Key == "TYPE")
                            _Type = val;
                        else if (property.Key == "MARKET")
                            _ExchangeName = val;
                        else if (property.Key == "FROMSYMBOL")
                            _FromCurrency = val;
                        else if (property.Key == "TOSYMBOL")
                            _ToCurrency = val;
                        else if (property.Key == "FLAGS")
                            _Flag = val;                        
                        currentField++;
                    }
                    else if (Convert.ToBoolean(this._MaskInt & property.Value))
                    {                        
                        string val = valuesArray[currentField];
                        if (property.Key == "PRICE")
                            _Price = val;
                        else if (property.Key == "LASTUPDATE")
                            _LastUpdate = val;
                        else if (property.Key == "LASTVOLUME")
                            _LastVolume = val;
                        else if (property.Key == "LASTVOLUMETO")
                            _LastVolumeTo = val;
                        else if (property.Key == "LASTTRADEID")
                            _LastTradeId = val;
                        else if (property.Key == "VOLUME24HOUR")
                            _Volume24h = val;
                        else if (property.Key == "VOLUME24HOURTO")
                            _Volume24hTo = val;
                        if (_Type == "5")
                        { 
                            //if (property.Key == "BID")
                            //    _BID = val;
                            //else if (property.Key == "OFFER")
                            //    _OFFER = val;
                            //else if (property.Key == "AVG")
                            //    _AVG = val;
                            //else if (property.Key == "VOLUMEHOUR")
                            //    _VOLUMEHOUR = val;
                            if (property.Key == "VOLUMEHOURTO")
                                _VOLUMEHOURTO = val;
                            else if (property.Key == "VOLUME24HOUR")
                                _VOLUME24HOUR = val;
                            else if (property.Key == "VOLUME24HOURTO")
                                _VOLUME24HOURTO = val;
                            else if (property.Key == "OPENHOUR")
                                _OPENHOUR = val;
                            else if (property.Key == "HIGHHOUR")
                                _HIGHHOUR = val;
                            else if (property.Key == "LOWHOUR")
                                _LOWHOUR = val;
                            else if (property.Key == "OPEN24HOUR")
                                _OPEN24HOUR = val;
                            else if (property.Key == "HIGH24HOUR")
                                _HIGH24HOUR = val;
                            else if (property.Key == "LOW24HOUR")
                                _LOW24HOUR = val;
                            else if (property.Key == "LASTMARKET")
                                _LASTMARKET = val;
                        }
                        currentField++;
                    }                    
                }                
            }
            catch (Exception ex)
            {
                Helpers.Helper.WriteToEventLog(ex);              
            }
        }

        private void Initialize()
        {
            KeyValuePair<string, int> type = new KeyValuePair<string, int>("TYPE", 0x0);


        }
        

        public void Decode()
        {
            try
            {         
                
            }
            catch(Exception ex)
            {
                Helpers.Helper.WriteToEventLog(ex);             
            }
        }
    }
}
