using sldc.Model;
using sldc.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sldc.Converter
{
    public class HookToENVTokenConverter
    {
        public static DRPClientStore.ENVTokens Convert(BaseHook hook)
        {
            switch (hook)
            {
                case DS2Hook:
                    return DRPClientStore.ENVTokens.DS2_TOKEN;
                case DS3Hook:
                    return DRPClientStore.ENVTokens.DS3_TOKEN;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
