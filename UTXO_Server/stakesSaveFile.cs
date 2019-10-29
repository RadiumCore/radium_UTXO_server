using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;

namespace radium_UTXO_server
{
   
        [Serializable()]
        public class stakesSaveFile
        {
            public int sync_height;
            public Collection<stake> Stakes = new Collection<stake>();
        }
    
}