using PropertyHook;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace sldc.Model
{
    class DS2Hook : BaseHook
    {
 

        private PHPointer BaseA;
        public DS2Hook() : base(5000, 5000, p => p.ProcessName == "DarkSoulsII")
        {
            BaseA = RegisterRelativeAOB("48 8B 05 ? ? ? ? 48 8B 58 38 48 85 DB 74 ? F6", 3, 7, 0);
        }

        public override int Death
        {
            get => CreateChildPointer(BaseA, 0xD0, 0x490).ReadInt32(0x1A4);
        }
        public int CurrentHP
        {
            get => CreateChildPointer(BaseA, 0xD0).ReadInt32(0x168);
        }
        // maybe for drp
        public override int slLvl
        {
            get => CreateChildPointer(BaseA, 0xD0, 0x490).ReadInt32(0xD0);
        }

    }
}
