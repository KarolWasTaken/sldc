using PropertyHook;
using sldc.Converter.CovenantConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static sldc.Stores.DRPClientStore;

namespace sldc.Model
{
    internal class DS3Hook : BaseHook
    {
        private PHPointer BaseA;
        public DS3Hook() : base(5000, 5000, p => p.ProcessName == "DarkSoulsIII")
        {
            BaseA = RegisterRelativeAOB("48 8B 05 ? ? ? ? 48 85 C0 ? ? 48 8B 40 ? C3", 3, 7, 0);
        }

        public override int Death
        {
            get => BaseA.ReadInt32(0x98);
        }
        // maybe for drp
        public override int slLvl
        {
            get => CreateChildPointer(BaseA, 0x10).ReadInt32(0x70);
        }
        public override string Covenant
        {
            get => ByteToCovenantConverter.Convert(CreateChildPointer(BaseA, 0x10).ReadBytes(0x328, 4), ENVTokens.DS3_TOKEN);
        }
    }
}
