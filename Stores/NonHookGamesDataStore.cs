using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sldc.Stores
{
    public class NonHookGamesDataStore
    {
        public Dictionary<string, int>? Bloodborne {  get; set; }
        public Dictionary<string, int>? DemonSoulsPS3 { get; set; }
        public Dictionary<string, int>? DemonSoulsPS5 { get; set; }
    }
}
