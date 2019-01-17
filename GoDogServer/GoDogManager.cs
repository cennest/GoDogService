using System;
using System.Collections.Generic;
using GoDogCommon;

namespace GoDogServer
{
    public class GoDogManager
    {
        private Dictionary<long, CameraManager> _cameraMagers;
        private static GoDogManager _GoDogManager;

        protected Logger logger;

        public GoDogManager()
        {
            _cameraMagers = new Dictionary<long, CameraManager>();
            logger = Logger.GetLogger();
        }

        public static GoDogManager GetGoDogManager()
        {
            if (_GoDogManager == null)
            {
                _GoDogManager = new GoDogManager();
            }
            return _GoDogManager;
        }

        public void StartCamera(long cameraID)
        {
            CameraManager cameraManager = _cameraMagers[cameraID];
            if (cameraManager != null)
            {
                cameraManager.StartConversion();
            }
        }

        public void StopCamera(long cameraID)
        {
            CameraManager cameraManager = _cameraMagers[cameraID];
            if (cameraManager != null)
            {
                cameraManager.StopConversion(true);
            }
        }
    }
}
