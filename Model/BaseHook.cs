using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyHook;

namespace sldc.Model
{
    public abstract class BaseHook : PHook
    {
        public event Action<string> CovenantChanged;
        public event Action<int> DeathCountChanged;
        public BaseHook(int refreshInterval, int minLifetime, Func<Process, bool> processSelector) : base(refreshInterval, minLifetime, processSelector)
        {
            // this wont be used anyways. need here to fill PHook requirements.
        }

        // abstract variable for death count
        public abstract int Death { get; }
        public abstract int slLvl { get; }
        public abstract string Covenant { get; }
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
                    OnConnectToGame();
                }

            }
        }
        public virtual void OnConnectToGame()
        {
            // start the death updater
            Task deathUpdaterTask = Task.Run(() => DeathUpdater());
            Task covenantUpdaterTask = Task.Run(() => CovenantUpdater());
        }

        // updater that invokes event to execute subs when deaths change.
        internal async Task DeathUpdater()
        {
            int oldDeath = 0;
            int newDeath;
            while (CheckForDeaths)
            {
                newDeath = Death;
                if (newDeath != oldDeath)
                {
                    if(newDeath < oldDeath)
                    {
                        bool waitComplete = false;

                        // Start a Task that checks every second for a new non-zero value or if Death becomes zero
                        var waitTask = Task.Run(async () =>
                        {
                            for (int i = 0; i < 30; i++)
                            {
                                // Wait for a second before checking again
                                await Task.Delay(1000);
                                newDeath = Death;

                                // If we receive a non-zero value, we stop waiting
                                if (newDeath != 0)
                                {
                                    waitComplete = true;
                                    break;
                                }
                            }
                        });
                        await waitTask;

                        // If we received a non-zero value, update and invoke the change
                        if (waitComplete && newDeath != 0)
                        {
                            DeathCountChanged?.Invoke(Death);
                            oldDeath = newDeath;
                        }
                        else if (!waitComplete || newDeath == 0)
                        {
                            // If 30 seconds passed and we received zero, still invoke the change
                            DeathCountChanged?.Invoke(Death);
                            oldDeath = newDeath;
                        }
                    }
                    else
                    {
                        DeathCountChanged?.Invoke(Death);
                        oldDeath = newDeath;
                    }
                }
                // delay to not throttle cpu
                await Task.Delay(150);
            }
        }
        internal async Task CovenantUpdater()
        {
            string newCov = "";
            string oldCov = "";
            while (CheckForDeaths)
            {
                if (Covenant != null)
                {
                    newCov = Covenant;
                    if (newCov != oldCov)
                    {
                        CovenantChanged?.Invoke(Covenant);
                        oldCov = newCov;
                    }
                }
                // delay to not throttle cpu
                await Task.Delay(150);
            }

        }
        //protected virtual void OnDeathCountChanged()
        //{
        //    DeathCountChanged?.Invoke();
        //}
    }
}
