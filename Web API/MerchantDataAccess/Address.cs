//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace MerchantDataAccess
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;

    public partial class Address
    {
        [JsonIgnore]
        public System.Guid id { get; set; }
        [JsonIgnore]
        public System.Guid merchantid { get; set; }
        public string country { get; set; }
        public string state { get; set; }
        public string postcode { get; set; }
        public string suburb { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        [JsonIgnore]
        public virtual Merchant Merchant { get; set; }
    }
}
