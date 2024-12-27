using sldc.Model;
using sldc.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static sldc.Model.GameDeathDataSerialiser;

namespace sldc.ViewModel
{
    public class SelectCaptureDeviceViewModel : ViewModelBase
    {

        public NonHookGameViewModelBase NonHookGameViewModelBase;
        public RelayCommand CloseDialogue { get; private set; }
        public SelectCaptureDeviceViewModel(NonHookGameViewModelBase nonHookGameViewModelBase)
        {
            // set up fields
            NonHookGameViewModelBase = nonHookGameViewModelBase;

            // set up commands
            CloseDialogue = new RelayCommand(CloseDialogueCommand);
        }
        public void SelectCaptureDevice(string deviceMoniker)
        {
            CaptureScreen cs = new CaptureScreen(NonHookGameViewModelBase, NonHookGameViewModelBase.DiscordRpcClientStore, ref NonHookGameViewModelBase.ImageSimilarityNotifier);
            if (deviceMoniker == "PS Remote")
            {
                cs.CaptureRemotePlay(NonHookToGameToken(NonHookGameViewModelBase.Game));
            }
            else
            {
                cs.CaptureCaptureCard(deviceMoniker);
            }
            CloseDialogueCommand();
        }
        private static string NonHookToGameToken(NON_HOOKABLE_GAME game)
        {
            switch (game) 
            { 
                case NON_HOOKABLE_GAME.BLOODBORNE:
                    return "BL";
                case NON_HOOKABLE_GAME.DEMON_SOULS_PS3:
                    throw new NotImplementedException();
                case NON_HOOKABLE_GAME.DEMON_SOULS_PS5:
                    throw new NotImplementedException();
                default:
                    throw new Exception();
            }
        }
        public void CloseDialogueCommand(object parameter = null)
        {
            NonHookGameViewModelBase.CurrentSubView = null;
        }
    }
}
