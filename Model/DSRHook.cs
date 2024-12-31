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
    internal class DSRHook : BaseHook
    {
        private PHPointer BaseB;
        public DSRHook() : base(5000, 5000, p => p.ProcessName == "DarkSoulsRemastered")
        {
            BaseB = RegisterRelativeAOB("48 8B 05 ? ? ? ? 45 33 ED 48 8B F1 48 85 C0", 3, 7, 0);
        }

        public override int Death
        {
            get => BaseB.ReadInt32(0x98);
        }
        // maybe for drp
        public override int slLvl
        {
            get => CreateChildPointer(BaseB, 0x10).ReadInt32(0x70);
        }
        public override string Covenant
        {
            get => ByteToCovenantConverter.Convert(CreateChildPointer(BaseB, 0x10).ReadBytes(0x113, 1), ENVTokens.DS1_TOKEN);
        }
    }
}
