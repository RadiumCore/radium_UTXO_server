using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;

namespace radium_UTXO_server
{
   
        [Serializable()]
        public class SaveFile
        {
            public int sync_height;
            public Collection<utxo> utxos = new Collection<utxo>();
        }
    
}