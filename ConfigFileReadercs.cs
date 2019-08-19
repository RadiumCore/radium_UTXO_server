using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace radium_UTXO_server

{
    public class ConfigFileReader
    {
        private JObject options = new JObject();

        public ConfigFileReader()
        {
            string path = Directory.GetCurrentDirectory().ToString() + "/utxo.conf";

           
             if (!File.Exists(path))
                return;    

                
            using (StreamReader sr = new StreamReader(path))
            {

                string json = sr.ReadToEnd();
               
                try {
                    options = JObject.Parse(json);

                }
                catch(Exception e)
                {

                }

              
            }
        }


      



        public string lookup(string key)
        {
            if (!options.ContainsKey(key))
                throw new Exception("The key '" + key + "' was not set in the config: " + Directory.GetCurrentDirectory().ToString());
            return options[key].ToString();
        }

        public string lookup_or_default(string key)
        {
            if (!options.ContainsKey(key))
                return "";
            return options[key].ToString();
        }

        public bool haskey(string key)
        {
            return options.ContainsKey(key);
        }
    }
}