using sldc.Model;
using sldc.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

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
        // font properties
        public FontFamily FontFamily { get; }
        public FontWeight FontWeight { get; }
        public FontStyle FontStyle { get; }
        public FontStretch FontStretch { get; }
        public string FontColour { get; }
        public StreamerWindowViewModel(StreamerWindowStore streamerWindowStore, HookStore hookStore = null)
        {
            if(hookStore.HookedGame != null)
            { 
                _hook = hookStore.hook;
                _deathCount = _hook.Death;
                _hook.DeathCountChanged += OnDeathCountChanged;
            }
            Settings userSettings = Helper.ReturnSettings();
            FontFamily = new FontFamily(userSettings.StreamerWindowFontFamily ?? "Plus Jakarta Sans");
            FontWeight = userSettings.StreamerWindowFontWeight != null
            ? (FontWeight)new FontWeightConverter().ConvertFromInvariantString(userSettings.StreamerWindowFontWeight)
            : FontWeights.Normal;
            FontStyle = userSettings.StreamerWindowFontStyle != null
            ? (FontStyle)new FontStyleConverter().ConvertFromInvariantString(userSettings.StreamerWindowFontStyle)
            : FontStyles.Normal;
            FontStretch = userSettings.StreamerWindowFontStretch != null
            ? (FontStretch)new FontStretchConverter().ConvertFromInvariantString(userSettings.StreamerWindowFontStretch)
            : FontStretches.Normal;
            FontColour = userSettings.FontColour;

            StreamerWindowStore = streamerWindowStore;
        }
        private void OnDeathCountChanged(int death)
        {
            _deathCount = death;
            OnPropertyChanged(nameof(DeathCountText));
        }
    }
}
