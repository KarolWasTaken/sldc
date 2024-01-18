﻿using sldc.Model;
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
        private BaseHook _hook;
        private StreamerWindowStore _streamerWindowStore;
        public OpenStreamerWindowComand(BaseHook hook, StreamerWindowStore streamerWindowStore)
        {
            _hook = hook;
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
                DataContext = new StreamerWindowViewModel(_hook, _streamerWindowStore)
            };
            _streamerWindowStore.streamerWindow = streamerWindow;
            streamerWindow.Show();
        }
    }
}
