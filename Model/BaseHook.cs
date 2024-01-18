using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyHook;

namespace sldc.Model
{
    public abstract class BaseHook : PHook
    {
        public BaseHook(int refreshInterval, int minLifetime, Func<Process, bool> processSelector) : base(refreshInterval, minLifetime, processSelector)
        {
            // this wont be used anyways. need here to fill PHook requirements.
        }

        // abstract variable for death count
        public abstract int Death { get; }
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
                    // start the death updater
                    Task deathUpdaterTask = Task.Run(() => DeathUpdater());
                }

            }
        }
        // updater that invokes event to execute subs when deaths change.
        private async Task DeathUpdater()
        {
            int oldDeath = 0;
            int newDeath;
            while (CheckForDeaths)
            {
                newDeath = Death;
                if (newDeath != oldDeath && newDeath > oldDeath)
                {
                    OnDeathCountChanged();
                    oldDeath = newDeath;
                }
                // delay to not throttle cpu
                await Task.Delay(150);
            }
        }
        public event Action DeathCountChanged;
        protected virtual void OnDeathCountChanged()
        {
            DeathCountChanged?.Invoke();
        }
    }
}
