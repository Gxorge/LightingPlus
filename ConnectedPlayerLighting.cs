using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightingPlus
{
    public class ConnectedPlayerLighting
    {
        public TubeBloomPrePassLight laserLeft { get; set; }
        public TubeBloomPrePassLight laserRight { get; set; }
        public TubeBloomPrePassLight laserFront { get; set; }

        public TubeBloomPrePassLight downLaserLeft { get; set; }
        public TubeBloomPrePassLight downLaserRight { get; set; }
        public TubeBloomPrePassLight downLaserConnector { get; set; }

        public TubeBloomPrePassLight extendedLaserLeft { get; set; }
        public TubeBloomPrePassLight extendedLaserRight { get; set; }

        public List<TubeBloomPrePassLight> allLasers { get; set; }

        public void AddAllToList()
        {
            allLasers = new List<TubeBloomPrePassLight>() { laserLeft, laserRight, laserFront, downLaserLeft, downLaserRight, downLaserConnector, extendedLaserLeft, extendedLaserRight };
            Plugin.Log.Info("AddAllToList()");
        }
    }
}
