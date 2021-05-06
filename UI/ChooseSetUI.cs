using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LightingPlus.UI
{
    public class ChooseSetUI : MonoBehaviour
    {
        public static ChooseSetUI _instance;
        internal ChooseSetFlowCoordinator _setListFlow;
        internal BoostSetView _viewController;

        internal static void OnLoad()
        {
            if (_instance == null)
            {
                new GameObject("ChooseSetUI").AddComponent<ChooseSetUI>();
            }
        }

        private void Awake()
        {
            _instance = this;
            UnityEngine.Object.DontDestroyOnLoad(this);
            this.CreateMenuButton();
        }

        private void CreateMenuButton()
        {
            PersistentSingleton<MenuButtons>.instance.RegisterButton(new MenuButton("Lighting+", "Change your boost colour set here!", new Action(this.ShowSetListFlow), true));
        }

        internal void ShowSetListFlow()
        {
            if (_setListFlow == null)
            {
                _setListFlow = BeatSaberUI.CreateFlowCoordinator<ChooseSetFlowCoordinator>();
            }
            BeatSaberUI.MainFlowCoordinator.PresentFlowCoordinatorOrAskForTutorial(_setListFlow);
        }

    }
}
