using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace radium_UTXO_server
{
    [Serializable]
    public class stake
    {
        public string txid;
        public decimal value;
        public string address;
        public int block;

        public stake(string _txid,  decimal _value, string _address, int _block)
        {
            txid = _txid;
            value = _value;
            address = _address;
            block = _block;
        }

        public JObject ToJson()
        {
            JObject result = new JObject();
            result.Add("txid", txid);
            result.Add("value", value);
            result.Add("address", address);
            result.Add("block", block);
            return result;
        }
    }
}
