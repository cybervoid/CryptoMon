//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CryptoLib
{
    using System;
    using System.Collections.Generic;
    
    public partial class Instruments
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Instruments()
        {
            this.CurrentAggData = new HashSet<CurrentAggData>();
            this.CurrentData = new HashSet<CurrentData>();
            this.MarketData = new HashSet<MarketData>();
        }
    
        public int InstrumentID { get; set; }
        public int MarketID { get; set; }
        public int ExchangeID { get; set; }
        public string Name { get; set; }
        public string BaseCurrency { get; set; }
        public string QuoteCurrency { get; set; }
        public bool Active { get; set; }
        public int Interval { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CurrentAggData> CurrentAggData { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CurrentData> CurrentData { get; set; }
        public virtual Exchanges Exchanges { get; set; }
        public virtual Markets Markets { get; set; }
        public virtual Markets Markets1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MarketData> MarketData { get; set; }
    }
}
