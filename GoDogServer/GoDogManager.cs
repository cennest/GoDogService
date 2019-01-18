using System;
using System.Collections.Generic;

using GoDogCommon;
using GoDogServer.Helpers;
using GoDogServer.Models;

namespace GoDogServer
{
    public sealed class GoDogManager
    {
        private Logger logger;
        private Facility facility;
        private static GoDogManager goDogManager;
        private Dictionary<long, CameraManager> cameraManagers;

        private static readonly object locker = new object();

        GoDogManager()
        {
            cameraManagers = new Dictionary<long, CameraManager>();
            logger = Logger.GetLogger();
            facility = DataHelper.GetDataHelper().GetFacility();
            InitCameraManager();
        }

        /// <summary>
        /// A helper method to create singleton instance of Logger class
        /// </summary>
        /// <returns></returns>
        public static GoDogManager GetGoDogManager()
        {
            lock (locker)
            {
                if (goDogManager == null)
                {
                    goDogManager = new GoDogManager();
                }
                return goDogManager;
            }
        }

        private void InitCameraManager()
        {
            if(facility != null)
            {
                foreach (Field field in facility.Fields)
                {
                    if (field.NUC != null)
                    {
                        foreach (Camera camera in field.NUC.Cameras)
                        {
                            cameraManagers[camera.ID] = new CameraManager(camera);
                        }
                    }
                }
            }
        }

        public void StopAllCameras()
        {
            foreach(var manager in cameraManagers)
            {
                try
                {
                    CameraManager cameraManager = (CameraManager)manager.Value;
                    cameraManager.StopConversion(true);
                }
                catch(Exception)
                {

                }
            }
        }

        public void StartAllCameras()
        {
            foreach (var manager in cameraManagers)
            {
                try
                {
                    CameraManager cameraManager = (CameraManager)manager.Value;
                    cameraManager.StartConversion();
                }
                catch (Exception)
                {

                }
            }
        }

        public void StartCamera(long cameraID)
        {
            CameraManager cameraManager = cameraManagers[cameraID];
            if (cameraManager != null)
            {
                cameraManager.StartConversion();
            }
        }

        public void StopCamera(long cameraID)
        {
            CameraManager cameraManager = cameraManagers[cameraID];
            if (cameraManager != null)
            {
                cameraManager.StopConversion(true);
            }
        }
    }
}
