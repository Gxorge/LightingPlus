using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightingPlus
{
    public class StaticLights
    {
        public float r { get; set; }
        public float g { get; set; }
        public float b { get; set; }

        public StaticLights(float r, float b, float g)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }

        public StaticLights() { }
    }
}
