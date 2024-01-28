using sldc.Model;
using sldc.Stores;
using sldc.ViewModel;
using sldc.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sldc.Commands
{
    public class OpenStreamerWindowComand : CommandBase
    {
        private HookStore _hookStore;
        private StreamerWindowStore _streamerWindowStore;
        public OpenStreamerWindowComand(HookStore hookStore, StreamerWindowStore streamerWindowStore)
        {
            _hookStore = hookStore;
            _streamerWindowStore = streamerWindowStore;
        }
        public OpenStreamerWindowComand(StreamerWindowStore streamerWindowStore)
        {
            _streamerWindowStore = streamerWindowStore;
        }
        public override void Execute(object? parameter)
        {
            if(_streamerWindowStore.streamerWindow != null)
            {
                return;
            }
            StreamerWindow streamerWindow = new StreamerWindow()
            {
                DataContext = new StreamerWindowViewModel(_streamerWindowStore, _hookStore)
            };
            _streamerWindowStore.streamerWindow = streamerWindow;
            streamerWindow.Show();
        }
    }
}
