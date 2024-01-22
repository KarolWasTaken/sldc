using PropertyHook;
using sldc.Converter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        //public string Covenant
        //{
        //    get => CreateChildPointer(BaseA, 0x10).ReadString(0x328, Encoding.Unicode, 4);
        //}
        public byte[] Covenant
        {
            get => CreateChildPointer(BaseA, 0x10).ReadBytes(0x328, 4);
        }


       



        public event Action<string> CovenantChanged;
        internal async override Task DeathUpdater()
        {
            string newCov = "";
            string oldCov = "";
            while (CheckForDeaths)
            {
                var matchingEntry = ByteToCovenantConverter.GetMatchingEntry(Covenant);
                if (Covenant != null && !matchingEntry.Equals(default(KeyValuePair<byte[], string>)))
                {
                    newCov = matchingEntry.Value;
                    if (newCov != oldCov)
                    {
                        CovenantChanged?.Invoke(matchingEntry.Value);
                        oldCov = newCov;
                    }
                }
                // delay to not throttle cpu
                await Task.Delay(150);
                await base.DeathUpdater();
            }

        }
        

    }
}
