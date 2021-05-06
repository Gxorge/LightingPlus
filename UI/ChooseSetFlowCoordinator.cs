using BeatSaberMarkupLanguage;
using HMUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightingPlus.UI
{
    internal class ChooseSetFlowCoordinator : FlowCoordinator
    {
        private BoostSetView _boostSetView;

        public void Awake()
        {
            if (_boostSetView == null)
            {
                _boostSetView = BeatSaberUI.CreateViewController<BoostSetView>();
            }
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            try
            {
                if (firstActivation)
                {
                    base.SetTitle("Boost Colour Sets", ViewController.AnimationType.In);
                    base.showBackButton = true;
                    base.ProvideInitialViewControllers(_boostSetView, null, null, null, null);
                }
            }
            catch (Exception e)
            {
                Plugin.Log.Error("FlowCoordinator error: " + e.Message);
            }
        }

        protected override void BackButtonWasPressed(ViewController topViewController)
        {
            MainFlowCoordinator main = BeatSaberUI.MainFlowCoordinator;
            main.DismissFlowCoordinator(this, null, ViewController.AnimationDirection.Horizontal, false);
        }
    }
}
