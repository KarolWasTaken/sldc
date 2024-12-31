using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static sldc.Stores.DRPClientStore;

namespace sldc.Converter.CovenantConverters
{
    public class CovenantNameToSmallImageKeyConverter
    {
        public static string Convert(string covenantName)
        {
            if (covenantName == "Princess's Guard")
                return "princess_guard";
            else
                return covenantName.Replace(" ", "_").ToLower();
        }
    }
}
