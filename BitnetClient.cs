using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace radium_UTXO_server

{
    public class BitnetClient
    {
        public BitnetClient()
        {
        }

        public BitnetClient(string a_sUri)
        {
            Url = new Uri(a_sUri);
        }

        public Uri Url;

        public ICredentials Credentials;
        public JObject TryInvokeMethod(string a_sMethod, params object[] a_params)
        {
            try { return InvokeMethod(a_sMethod, a_params); }
            catch
            {
                Console.WriteLine("retry invoke method " + a_sMethod + " " + (string)a_params[0].ToString());
                System.Threading.Thread.Sleep(100);
                return TryInvokeMethod(a_sMethod, a_params);
            }
        }

        

        private JObject InvokeMethod(string a_sMethod, params object[] a_params)
        {
            //Console.WriteLine("RPC-CALL " + a_sMethod);

            HttpWebRequest webRequest__1 = (HttpWebRequest)WebRequest.Create(Url);
            webRequest__1.Credentials = Credentials;

            webRequest__1.ContentType = "application/json-rpc";
            webRequest__1.Method = "POST";

            JObject joe = new JObject
            {
                ["jsonrpc"] = "1.0",
                ["id"] = "1",
                ["method"] = a_sMethod
            };

            if (a_params != null)
            {
                if (a_params.Length > 0)
                {
                    JArray props = new JArray();
                    foreach (object p in a_params)
                    {
                        props.Add(p);
                    }
                    joe.Add(new JProperty("params", props));
                }
            }

            string s = JsonConvert.SerializeObject(joe);
            //Dim t As String = "{""jsonrpc"":""1.0"",""id"":""1"",""method"":""sendmany"",""params"":[""R"",{""BLkcaH5DKF2SPu9BycnyYnZM97iFxAnFXk"":0.0101,""BGtn7VTXsAFwyMSUsEQiK5Av9hAuZ8CaU1"":0.0101,""B5bDhcZqcpBg1nqfRGM3PwhHBd6J851k7G"":0.0101}]}"
            // serialize json for the request
            byte[] byteArray = Encoding.UTF8.GetBytes(s);
            webRequest__1.ContentLength = byteArray.Length;

            using (Stream dataStream = webRequest__1.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }

            using (WebResponse webResponse = webRequest__1.GetResponse())
            {
                using (Stream str = webResponse.GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(str))
                    {
                        string res = sr.ReadToEnd();
                        return JsonConvert.DeserializeObject<JObject>(res);
                    }
                }
            }
        }

        public string InvokeMethodReturnString(string a_sMethod, params object[] a_params)
        {
            HttpWebRequest webRequest__1 = (HttpWebRequest)WebRequest.Create(Url);
            webRequest__1.Credentials = Credentials;

            webRequest__1.ContentType = "application/json-rpc";
            webRequest__1.Method = "POST";

            JObject joe = new JObject
            {
                ["jsonrpc"] = "1.0",
                ["id"] = "1",
                ["method"] = a_sMethod
            };

            if (a_params != null)
            {
                if (a_params.Length > 0)
                {
                    JArray props = new JArray();
                    foreach (object p in a_params)
                    {
                        props.Add(p);
                    }
                    joe.Add(new JProperty("params", props));
                }
            }

            string s = JsonConvert.SerializeObject(joe);
            //Dim t As String = "{""jsonrpc"":""1.0"",""id"":""1"",""method"":""sendmany"",""params"":[""R"",{""BLkcaH5DKF2SPu9BycnyYnZM97iFxAnFXk"":0.0101,""BGtn7VTXsAFwyMSUsEQiK5Av9hAuZ8CaU1"":0.0101,""B5bDhcZqcpBg1nqfRGM3PwhHBd6J851k7G"":0.0101}]}"
            // serialize json for the request
            byte[] byteArray = Encoding.UTF8.GetBytes(s);
            webRequest__1.ContentLength = byteArray.Length;
            try
            {
                using (Stream dataStream = webRequest__1.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }
            }
            catch
            {
            }

            using (WebResponse webResponse = webRequest__1.GetResponse())
            {
                using (Stream str = webResponse.GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(str))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }

          
        }

        public JObject InvokeSpecialQuoteAsParamMethod(string a_sMethod, string a_params)
        {
            HttpWebRequest webRequest__1 = (HttpWebRequest)WebRequest.Create(Url);
            webRequest__1.Credentials = Credentials;

            webRequest__1.ContentType = "application/json-rpc";
            webRequest__1.Method = "POST";

            JObject joe = new JObject
            {
                ["jsonrpc"] = "1.0",
                ["id"] = "1",
                ["method"] = a_sMethod
            };

            a_params = a_params.Replace(" ", "");

            joe.Add(new JProperty("params", a_params));

            string s = JsonConvert.SerializeObject(joe);
            s = s.Replace((char)34 + "\\", "");
            s = s.Replace("\\", "");
            // serialize json for the request

            // s = {"jsonrpc":"1.0","id":"1","method":"createrawtransaction","params":"[{"txid":"e226b8691e7b08a13efa9f79d0f3273c1343b10c0769a38df11d3c1ccd687f52","vout":0}]{"XopyTfnfFMLWtjed1JNBn2B72JRrJaqXLD":0.999}"}

            byte[] byteArray = Encoding.UTF8.GetBytes(s);
            webRequest__1.ContentLength = byteArray.Length;

            using (Stream dataStream = webRequest__1.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }

            using (WebResponse webResponse = webRequest__1.GetResponse())
            {
                using (Stream str = webResponse.GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(str))
                    {
                        return JsonConvert.DeserializeObject<JObject>(sr.ReadToEnd());
                    }
                }
            }
        }

        public int GetPeerCount()
        {
            return ((JArray)InvokeMethod("getpeerinfo")["result"]).Count;
        }

        public JArray GetPeerInfo()
        {
            return (JArray)InvokeMethod("getpeerinfo")["result"];
        }

        public JObject GetStakingInfo()
        {
            return (JObject)InvokeMethod("getstakinginfo")["result"];
        }

        public void BackupWallet(string a_destination)
        {
            InvokeMethod("backupwallet", a_destination);
        }

        public string GetAccount(string a_address)
        {
            return InvokeMethod("getaccount", a_address)["result"].ToString();
        }

        public string GetAccountAddress(string a_account)
        {
            return InvokeMethod("getaccountaddress", a_account)["result"].ToString();
        }

        public object GetAddressesByAccount(string a_account)
        {
            dynamic res = InvokeMethod("getaddressesbyaccount", a_account)["result"];
            return res;
        }

        public decimal GetBalance(string a_account = null, int a_minconf = 1)
        {
            if (a_account == null)
            {
                return Convert.ToDecimal(InvokeMethod("getbalance")["result"]);
            }
            return Convert.ToDecimal(InvokeMethod("getbalance", a_account, a_minconf)["result"]);
        }

        public string GetBlockByCount(int a_height)
        {
            return InvokeMethod("getblockbynumber", a_height)["result"].ToString();
        }

        public int GetBlockCount()
        {
            //object test = InvokeMethod("getblockcount")["result"].ToString();
            return Convert.ToInt32(InvokeMethod("getblockcount")["result"].ToString());
        }

        public object ListAccounts()
        {
            return InvokeMethod("listaccounts")["result"];
        }

        public int GetBlockNumber()
        {
            return Convert.ToInt32(InvokeMethod("getblocknumber")["result"]);
        }

        public int GetConnectionCount()
        {
            return Convert.ToInt32(InvokeMethod("getconnectioncount")["result"]);
        }

        public float GetDifficulty()
        {
            return Convert.ToSingle(InvokeMethod("getdifficulty")["result"]);
        }

        public bool GetGenerate()
        {
            return Convert.ToBoolean(InvokeMethod("getgenerate")["result"]);
        }

        public float GetHashesPerSec()
        {
            return Convert.ToSingle(InvokeMethod("gethashespersec")["result"]);
        }

        public JObject GetInfo()
        {
            return InvokeMethod("getinfo")["result"] as JObject;
        }

        public string GetNewAddress(string a_account)
        {
            return InvokeMethod("getnewaddress", a_account)["result"].ToString();
        }

        public float GetReceivedByAccount(string a_account, int a_minconf = 1)
        {
            return Convert.ToSingle(InvokeMethod("getreceivedbyaccount", a_account, a_minconf)["result"]);
        }

        public float GetReceivedByAddress(string a_address, int a_minconf = 1)
        {
            return Convert.ToSingle(InvokeMethod("getreceivedbyaddress", a_address, a_minconf)["result"]);
        }

        public int TxConfirmationCount(string a_txid)
        {
            dynamic res = InvokeMethodReturnString("gettransaction", a_txid);
            res = res.Remove(res.IndexOf("txid") - 1, 74);
            res = res.Remove(res.IndexOf("time") - 1, 18);
            dynamic jres = JObject.Parse(res);
            return JObject.Parse(res)["result"]["confirmations"];
            // Return TryCast(("result"), JObject)
        }

        public int TxBlockNumber(string a_txid)
        {
            dynamic res = InvokeMethodReturnString("gettransaction", a_txid);
            res = res.Remove(res.IndexOf("txid") - 1, 74);
            res = res.Remove(res.IndexOf("time") - 1, 18);
            dynamic jres = JObject.Parse(res);
            dynamic bres = InvokeMethod("getblock", jres["result"]["blockhash"])["result"]["height"];

            return Convert.ToInt32(bres);
            // Return TryCast(("result"), JObject)
        }

        public JObject GetWork()
        {
            return InvokeMethod("getwork")["result"] as JObject;
        }

        public bool GetWork(string a_data)
        {
            return Convert.ToBoolean(InvokeMethod("getwork", a_data)["result"]);
        }

        public string Help(string a_command = "")
        {
            return InvokeMethod("help", a_command)["result"].ToString();
        }

        public JObject ListAccounts(int a_minconf = 1)
        {
            return InvokeMethod("listaccounts", a_minconf)["result"] as JObject;
        }

        public JArray ListReceivedByAccount(int a_minconf = 1, bool a_includeEmpty = false)
        {
            return InvokeMethod("listreceivedbyaccount", a_minconf, a_includeEmpty)["result"] as JArray;
        }

        //Public Function ListReceivedByAddress(Optional a_minconf As Integer = 1, Optional a_includeEmpty As Boolean = False) As JArray
        //    Return TryCast(InvokeMethod("listreceivedbyaddress", a_minconf, a_includeEmpty)("result"), JArray)
        //End Function

        public JArray ListTransactions(string a_account, int a_count = 10)
        {
            return InvokeMethod("listtransactions", a_account, a_count)["result"] as JArray;
        }

        public bool Move(string a_fromAccount, string a_toAccount, float a_amount, int a_minconf = 1, string a_comment = "")
        {
            return Convert.ToBoolean(InvokeMethod("move", a_fromAccount, a_toAccount, a_amount, a_minconf, a_comment)["result"]);
        }

        public string SendFrom(string a_fromAccount, string a_toAddress, float a_amount, int a_minconf = 1, string a_comment = "", string a_commentTo = "")
        {
            return InvokeMethod("sendfrom", a_fromAccount, a_toAddress, a_amount, a_minconf, a_comment, a_commentTo)["result"].ToString();
        }

        public JObject SendToAddress(string a_address, decimal a_amount, string a_comment, string a_commentTo)
        {
            return InvokeMethod("sendtoaddress", a_address, a_amount, a_comment, a_commentTo);
        }

        public string SendMany(string account, string @params)
        {
            return InvokeSpecialQuoteAsParamMethod("sendmany", @params)["result"].ToString();
        }

        public string SignMessage(string a_address, string message)
        {
            return InvokeMethod("signmessage", a_address, message)["result"].ToString();
        }

        public string VerifyMessage(string a_address, string signature, string message)
        {
            return InvokeMethod("verifymessage", a_address, signature, message)["result"].ToString();
        }

        public string WalletPassphrase(string a_passphrase, int a_time, bool staking_only)
        {
            return InvokeMethod("walletpassphrase", a_passphrase, a_time, staking_only)["result"].ToString();
        }

        public void SetAccount(string a_address, string a_account)
        {
            InvokeMethod("setaccount", a_address, a_account);
        }

        public void SetGenerate(bool a_generate, int a_genproclimit = 1)
        {
            InvokeMethod("setgenerate", a_generate, a_genproclimit);
        }

        public void Stop()
        {
            InvokeMethod("stop");
        }

        public JObject ValidateAddress(string a_address)
        {
            return InvokeMethod("validateaddress", a_address)["result"] as JObject;
        }
    }
}