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
        public virtual string SelectedId { get; set; }

        [UseConverter(typeof(ListConverter<BoostColour>))]
        public virtual List<BoostColour> BoostColours { get; set; } = new List<BoostColour>();

        public virtual void Change() { }
    }
}
