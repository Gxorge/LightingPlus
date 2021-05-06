using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightingPlus.UI
{
    internal class BoostSetView : BSMLResourceViewController
    {
        [UIComponent("setList")]
        public CustomListTableData customListTableData;

        public override string ResourceName
        {
            get
            {
                return "LightingPlus.UI.SetList.bsml";
            }
        }

        [UIAction("setSelect")]
        internal void SelectSet(TableView view, int row)
        {
            Plugin.Config.Update(Plugin.Config.BoostColours[row]);
        }

        [UIAction("#post-parse")]
        internal void SetupSetList()
        {
            customListTableData.data.Clear();
            foreach (BoostColour b in Plugin.Config.BoostColours)
            {
                customListTableData.data.Add(new CustomListTableData.CustomCellInfo(b.name, "soon", null));
            }
            customListTableData.tableView.ReloadData();
            int idx = Plugin.Config.BoostColours.IndexOf(Plugin.Boost);
            customListTableData.tableView.ScrollToCellWithIdx(idx, TableView.ScrollPositionType.Center, false);
            customListTableData.tableView.SelectCellWithIdx(idx, false);
        }

        protected override void DidDeactivate(bool removedFromHierarchy, bool screenSystemDisabling)
        {
            base.DidDeactivate(removedFromHierarchy, screenSystemDisabling);
        }
    }
}
