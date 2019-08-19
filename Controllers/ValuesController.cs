using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace radium_UTXO_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UTXOController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<string> Get()
        {
            JObject response = new JObject();
            response.Add("synced", UtxoServer.Synced);
            response.Add("progress", UtxoServer.sync_percent);
            return response.ToString();
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(string id)
        {
            IEnumerable<utxo> result = UtxoServer.utxos.Where(i => i.address == id);
            JArray response = new JArray();
            foreach (utxo unspent in result)
            {
                response.Add(unspent.ToJson());
            }

            return response.ToString();
        }


        
    }
}
