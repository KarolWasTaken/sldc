using sldc.Model;
using sldc.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sldc.ViewModel
{
    public class StreamerWindowViewModel : ViewModelBase
    {
        private BaseHook _hook;
        public StreamerWindowStore StreamerWindowStore;

        private int _deathCount;
		public string DeathCountText
		{
			get { return $"Deaths: {_deathCount}"; }
		}

		public StreamerWindowViewModel(HookStore hookStore, StreamerWindowStore streamerWindowStore)
        {
            _hook = hookStore.hook;
            _deathCount = _hook.Death;
            _hook.DeathCountChanged += OnDeathCountChanged;
            StreamerWindowStore = streamerWindowStore;
        }
        private void OnDeathCountChanged()
        {
            _deathCount = _hook.Death;
            OnPropertyChanged(nameof(DeathCountText));
        }
    }
}
