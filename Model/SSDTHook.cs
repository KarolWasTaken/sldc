using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using PropertyHook;

namespace sldc.Model
{
    // doesnt work. testing.
    internal class SSDTHook : PHook
    {
        private PHPointer pPlayer;
        public SSDTHook() : base(5000, 5000, p => p.ProcessName == "sekiro")
        {
            pPlayer = RegisterAbsoluteAOB("E8 ? ? ? ? 48 ? ? 48 8B CB 48 ? ? FF");
        }

        // abstract variable for death count
        public int CurrentHP
        {
            get => pPlayer.ReadInt32(0x0);
        }
        public string Covenant { get; }
        // check for deaths flag
        private bool _checkForDeaths;
        public bool CheckForDeaths
        {
            get { return _checkForDeaths; }
            set
            {
                _checkForDeaths = value;
                if (_checkForDeaths == true)
                {
                    //OnConnectToGame();
                }

            }
        }
        //public virtual void OnConnectToGame()
        //{
        //    // start the death updater
        //    Task deathUpdaterTask = Task.Run(() => DeathUpdater());
        //}

        // updater that invokes event to execute subs when deaths change.
        //internal async Task DeathUpdater()
        //{
        //    int oldDeath = 0;
        //    int newDeath;
        //    while (CheckForDeaths)
        //    {
        //        newDeath = Death;
        //        if (newDeath != oldDeath && newDeath > oldDeath)
        //        {
        //            DeathCountChanged?.Invoke(Death);
        //            oldDeath = newDeath;
        //        }
        //        // delay to not throttle cpu
        //        await Task.Delay(150);
        //    }
        //}
    }
}
