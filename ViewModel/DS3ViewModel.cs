using sldc.Commands;
using sldc.Model;
using sldc.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using static sldc.Stores.DRPClientStore;

namespace sldc.ViewModel
{
    class DS3ViewModel : GameHookBaseViewModel
    {
		private string _covenant;
		public string Covenant
		{
			get
			{
				return $"Covenant: {_covenant}";
			}
			set
			{
				_covenant = value;
				OnPropertyChanged(nameof(Covenant));
			}
		}
		public DS3ViewModel(StreamerWindowStore streamerWindowStore, DRPClientStore discordRpcClientStore, HookStore hookStore, ENVTokens envToken)
            : base(streamerWindowStore, discordRpcClientStore, hookStore, envToken)
        {
            ConnectedToGame += OnConnectedToGame;
        }

        private void OnConnectedToGame()
        {
            DS3Hook ds3h = (DS3Hook)hook;
            ds3h.CovenantChanged += OnCovenantChanged;
        }

        private void OnCovenantChanged(string covenant)
        {
            Covenant = covenant;
        }
    }
}
