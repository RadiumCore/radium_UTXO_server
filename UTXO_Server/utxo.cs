using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace radium_UTXO_server
{
    [Serializable]
    public class utxo
    {
        public string txid;
        public int index;
        public decimal value;
        public string address;

        public utxo(string _txid, int _index, decimal _value, string _address)
        {
            txid = _txid;
            index = _index;
            value = _value;
            address = _address;
        }

        public JObject ToJson()
        {
            JObject result = new JObject();
            result.Add("txid", txid);
            result.Add("index", index);
            result.Add("value", value);
            return result;
        }
    }
}
