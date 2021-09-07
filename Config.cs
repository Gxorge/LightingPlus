using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace LightingPlus
{
    internal class Config
    {

        public virtual bool BoostColoursEnabled { get; set; } = true;
        public virtual bool MultiPlayerLightingEnabled { get; set; } = true;
        public virtual bool StaticLightsColoursEnabled { get; set; } = true;

        public virtual StaticLights StaticLightsColours { get; set; } = new StaticLights(140f, 140f, 140f);

        public virtual string SelectedBoostId { get; set; } = "Default";

        [UseConverter(typeof(ListConverter<BoostColour>))]
        public virtual List<BoostColour> BoostColours { get; set; } = new List<BoostColour>() { new BoostColour("Default", 48f, 152f, 225f, 136f, 22f, 225f) };

        public virtual void Update(BoostColour colour)
        {
            SelectedBoostId = colour.name;
            Plugin.Boost = colour;
            Plugin.Log.Info("Updated boost set to " + colour.name);
        }
    }
}
