using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoLib.Analysis.Data
{
    
    public interface IDataPointWrapper<T>
    {
        List<T> Data { get; set; }
    }


}
