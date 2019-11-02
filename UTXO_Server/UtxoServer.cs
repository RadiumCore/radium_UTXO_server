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
        public static Collection<stake> stakes = new Collection<stake>();
        public static int SyncHeight = 1;

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
                OverwriteConsoleLine("syncing block: " + curblock + " utxo count: " + utxos.Count() + " stake count: " + stakes.Count());
                JObject BlockChainBlock = default(JObject);
                JToken transaction = default(JToken);
                BlockChainBlock = bcMain.TryInvokeMethod("getblockbynumber", curblock);
                JArray txarray = (JArray)BlockChainBlock["result"]["tx"];
                for(int index = 0; index <= BlockChainBlock["result"]["tx"].Count() -1; index ++)
                {
                    transaction = bcMain.TryInvokeMethod("gettransaction", BlockChainBlock["result"]["tx"][index])["result"];
                   
                    if(curblock > 2734 && index == 1)
                        {
                            // this is a stake transaction. 
                            try
                            {
                                stakes.Add(new stake((string)transaction["txid"], (decimal)GetStakeReward(curblock), (string)transaction["vout"][transaction["vout"].Count() -1]["scriptPubKey"]["addresses"][0], curblock));
                            }
                            catch { };


                        }

                        // removed spent utxo's form utxo pool
                        foreach (JToken vin in transaction["vin"]){
                        if (vin["coinbase"] != null) { continue; }
                        utxos.Remove(utxos.SingleOrDefault(i => i.txid == (string)vin["txid"] && i.index == (int)vin["vout"]));
                    }
                   
                    foreach (JToken vout in transaction["vout"])
                    {
                        // dont track utxo's with:
                        // no value,
                        if ((decimal)vout["value"] == 0) { continue; }
                        // tiny value
                        if ((double)vout["value"] < .00001) { continue; }
                        // null data (no value)
                        if ((string)vout["scriptPubKey"]["type"] == "nulldata") { continue; }
                        // non standard (no value
                        if ((string)vout["scriptPubKey"]["type"] == "nonstandard") { continue; }
                        // dont add duplicate tx's (shouldnt happen)
                        if ( utxos.Any(i => i.txid == (string)transaction["txid"] && i.index == (int)vout["n"])) {continue;}

                        utxos.Add(new utxo((string)transaction["txid"], (int)vout["n"], (decimal)vout["value"], (string)vout["scriptPubKey"]["addresses"][0]));

                    }
                
                }


                SyncHeight = curblock;
                if (curblock % 100 == 1)
                {
                    Save(utxos, SyncHeight, stakes);
                }

                    SyncHeight = curblock;
            }
            };
            //ObservableCollection<User> TempSmartChainAccounts = GetMySmartChainAccounts(ref bcSync);




        }



        public static void Save(Collection<utxo> _utxos, int _SyncHeight, Collection<stake> _stakes)
        {
            SaveFile savedstate = new SaveFile
            {
                sync_height = _SyncHeight,
                utxos = _utxos,
               
            };

            stakesSaveFile savedStakes = new stakesSaveFile
            {
                Stakes = _stakes,
            };




            string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\smartchain";

            if ((!System.IO.Directory.Exists(path)))
                System.IO.Directory.CreateDirectory(path);

            FileStream SaveDataFilestream = new FileStream(path + "\\utxos.sc", FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
            FileStream SaveStakeFilestream = new FileStream(path + "\\stakes.sc", FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);

            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            try
            {
                bf.Serialize(SaveDataFilestream, savedstate);
                bf.Serialize(SaveStakeFilestream, savedStakes);
            }
            catch (Exception ex1)
            {
                throw ex1;
            }

            SaveDataFilestream.Close();
            SaveStakeFilestream.Close();

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

            if (File.Exists(path + "\\stakes.sc"))
            {
                FileStream DataFileStream = new FileStream(path + "\\stakes.sc", FileMode.Open);

                try
                {
                    stakesSaveFile _stakes = (stakesSaveFile)bf.Deserialize(DataFileStream);
                    stakes = _stakes.Stakes;
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

        public static Double GetStakeReward(int block)
        {
            int nYear = 525600;
             int DEV_FUND_BLOCK_HEIGHT = 1655000;
            if (block >= DEV_FUND_BLOCK_HEIGHT && block + 1 < DEV_FUND_BLOCK_HEIGHT + nYear)
                return 0.5;
            else if (block >= DEV_FUND_BLOCK_HEIGHT + nYear && block + 1 < DEV_FUND_BLOCK_HEIGHT + nYear *2)
                return 0.485;
            else if (block >= DEV_FUND_BLOCK_HEIGHT + nYear *2 && block + 1 < DEV_FUND_BLOCK_HEIGHT + nYear *3)
                return 0.47;
            else if (block >= DEV_FUND_BLOCK_HEIGHT + nYear * 3 && block + 1 < DEV_FUND_BLOCK_HEIGHT + nYear * 4)
                return 0.456;
            else if (block >= DEV_FUND_BLOCK_HEIGHT + nYear * 4 && block + 1 < DEV_FUND_BLOCK_HEIGHT + nYear * 5)
                return 0.442;
            else if (block >= DEV_FUND_BLOCK_HEIGHT + nYear * 5 && block + 1 < DEV_FUND_BLOCK_HEIGHT + nYear * 6)
                return 0.429;
            return 0;



        }

            
            
        }

    }
       
           
    



