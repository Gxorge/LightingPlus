using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightingPlus
{
    public class BoostColour
    {
        public string name { get; set; }
        public float r0 { get; set; }
        public float g0 { get; set; }
        public float b0 { get; set; }
        public float r1 { get; set; }
        public float g1 { get; set; }
        public float b1 { get; set; }

        public BoostColour(string name, float r0, float b0, float g0, float r1, float b1, float g1)
        {
            this.name = name;
            this.r0 = r0;
            this.g0 = g0;
            this.b0 = b0;

            this.r1 = r1;
            this.g1 = g1;
            this.b1 = b1;
        }

        public BoostColour() { }
    }
}
