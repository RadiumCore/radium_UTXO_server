using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;

namespace radium_UTXO_server

{
    internal static class UtxoServer
    { 

        public static ConfigFileReader config = new ConfigFileReader();
        public static DateTime stats_update = DateTime.Now;

        public static bool wallet_connected = false;


        public static  Collection<utxo> utxos = new Collection<utxo>();
        private static int SyncHeight = 1;

        public static  Thread SyncThread = new Thread(new ThreadStart(Sync));

        private static bool synced;


        private static int highestnetworkblock = 0;

        public static bool Synced
        {
            get {
                if (highestnetworkblock == SyncHeight)
                    return true;
                else
                    return false;
            }            
        }

        public static double sync_percent
        {
            get
            {
                decimal a = SyncHeight / highestnetworkblock;
                decimal b = a * 100;
                decimal c = Math.Round(b, 5);

                return Math.Round( (double)(SyncHeight / highestnetworkblock) * 100, 5);
            }
        }

        private static void Sync()
        {
            LoadSaved();

           BitnetClient bcMain = new BitnetClient();
           
            bcMain.Credentials = new NetworkCredential(config.lookup("rpc_user"), config.lookup("rpc_pass"));
            bcMain.Url = new Uri("http://" + config.lookup("rpc_ip") + ":" + config.lookup("rpc_port"));
            
          

            //  Dim Synced As Boolean = False
                       


            while (true) {
            highestnetworkblock = bcMain.GetBlockCount();

            if (SyncHeight == highestnetworkblock)
                {
                    Thread.Sleep(6000);
                    continue;
                }
            for (int curblock = SyncHeight; curblock <= highestnetworkblock; curblock++)
            {
                OverwriteConsoleLine("syncing block " + curblock + "utxo count " + utxos.Count());
                JObject BlockChainBlock = default(JObject);
                JToken transaction = default(JToken);
                BlockChainBlock = bcMain.TryInvokeMethod("getblockbynumber", curblock);
                JArray txarray = (JArray)BlockChainBlock["result"]["tx"];
                foreach (JToken txid in BlockChainBlock["result"]["tx"])
                {
                    transaction = bcMain.TryInvokeMethod("gettransaction", txid)["result"];
                   
                    foreach (JToken vin in transaction["vin"]){
                        if (vin["coinbase"] != null) { continue; }
                        utxos.Remove(utxos.SingleOrDefault(i => i.txid == (string)vin["txid"] && i.index == (int)vin["vout"]));

                    }
                    foreach (JToken vout in transaction["vout"])
                    {
                        if((decimal)vout["value"] == 0) { continue; }
                        if ((double)vout["value"] < .00001) { continue; }
                        if ((string)vout["scriptPubKey"]["type"] == "nulldata") { continue; }
                        if ((string)vout["scriptPubKey"]["type"] == "nonstandard") {
                            continue;
                        }

                        if ( utxos.Any(i => i.txid == (string)txid && i.index == (int)vout["n"])) 
                        {
                            continue;
                        }

                        utxos.Add(new utxo((string)txid, (int)vout["n"], (decimal)vout["value"], (string)vout["scriptPubKey"]["addresses"][0]));

                    }
                
                }


                SyncHeight = curblock;
                if (curblock % 100 == 1)
                {
                    Save(utxos, SyncHeight);
                }

                    SyncHeight = curblock;
            }
            };
            //ObservableCollection<User> TempSmartChainAccounts = GetMySmartChainAccounts(ref bcSync);




        }



        public static void Save(Collection<utxo> _utxos, int _SyncHeight)
        {
            SaveFile savedstate = new SaveFile
            {
                sync_height = _SyncHeight,
                utxos = _utxos,
               
            };

            string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\smartchain";

            if ((!System.IO.Directory.Exists(path)))
                System.IO.Directory.CreateDirectory(path);

            FileStream SaveDataFilestream = new FileStream(path + "\\utxos.sc", FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
          
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            try
            {
                bf.Serialize(SaveDataFilestream, savedstate);               
            }
            catch (Exception ex1)
            {
                throw ex1;
            }

            SaveDataFilestream.Close();

            Console.WriteLine("sucessfully wrote save data");
          
            return;
        }

        public static void LoadSaved()
        {
            

            string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\smartchain";

            if ((!System.IO.Directory.Exists(path)))
                System.IO.Directory.CreateDirectory(path);

            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

            if (File.Exists(path + "\\utxos.sc"))
            {
                FileStream DataFileStream = new FileStream(path + "\\utxos.sc", FileMode.Open);
                
                try
                {
                   SaveFile save  = (SaveFile)bf.Deserialize(DataFileStream);
                    utxos = save.utxos;
                    SyncHeight = save.sync_height;

                    DataFileStream.Close();
                }
                catch (Exception ex)
                {
                    DataFileStream.Close();
                }
            }

          
               
           
            //End If

        }

        public static void OverwriteConsoleLine(string line)
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
            Console.Write(line);
        }

    }
       
           
    


}
