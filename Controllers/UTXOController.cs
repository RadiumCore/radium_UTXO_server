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
    public class StakeController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<string> Get()
        {

            return "Stake API";
        }

        // GET api/values/5
        [HttpGet("{address}/{blocks}")]
        public ActionResult<string> Get(string address, int blocks)
        {
            IEnumerable<stake> result = UtxoServer.stakes.Where(i => i.address == address && i.block > (UtxoServer.SyncHeight - blocks) );
            JArray response = new JArray();
            foreach (stake _stake in result)
            {
                response.Add(_stake.ToJson());
            }

            return response.ToString();
        }

       

    }
}
