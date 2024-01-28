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
        public static DRPClientStore.ENVTokens Convert(string HookedGame)
        {
            switch (HookedGame)
            {
                case "DS2":
                    return DRPClientStore.ENVTokens.DS2_TOKEN;
                case "DS3":
                    return DRPClientStore.ENVTokens.DS3_TOKEN;
                case "BL":
                    return DRPClientStore.ENVTokens.BL_TOKEN;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
