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
        private Field field;
        private static GoDogManager goDogManager;
        private Dictionary<long, CameraManager> cameraManagers;

        private static readonly object locker = new object();

        public GoDogManager()
        {
            //string biosId = GoDogCommon.Configuration.BiosID();
            cameraManagers = new Dictionary<long, CameraManager>();
            logger = Logger.GetLogger();
            field = DataHelper.GetDataHelper().GetField();
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
            if (field != null)
            {
                foreach (Camera camera in field.NUC.Cameras)
                {
                    this.logger.Log($"Init Camera {camera.ID}");
                    cameraManagers[camera.ID] = new CameraManager(camera);
                }
            }
        }

        public void StartAllCameras()
        {
            this.logger.Log($"Starting FFMPEG process.");
            foreach (var manager in cameraManagers)
            {
                try
                {
                    CameraManager cameraManager = (CameraManager)manager.Value;
                    cameraManager.StartConversion();
                    this.logger.Log($"Started Camera {manager.Key} FFMPEG process.");
                }
                catch (Exception)
                {

                }
            }
        }

        public void StopAllCameras()
        {
            this.logger.Log($"Stopping FFMPEG process.");
            foreach (var manager in cameraManagers)
            {
                try
                {
                    CameraManager cameraManager = (CameraManager)manager.Value;
                    cameraManager.StopConversion(true);
                    this.logger.Log($"Stopped Camera {manager.Key} FFMPEG process");
                }
                catch (Exception)
                {

                }
            }
        }

        public void StartCamera(long cameraID)
        {
            this.logger.Log($"Starting Camera {cameraID} FFMPEG process.");
            CameraManager cameraManager = cameraManagers[cameraID];
            if (cameraManager != null)
            {
                cameraManager.StartConversion();
            }
        }

        public void StopCamera(long cameraID)
        {
            this.logger.Log($"Stopping Camera {cameraID} FFMPEG process.");
            CameraManager cameraManager = cameraManagers[cameraID];
            if (cameraManager != null)
            {
                cameraManager.StopConversion(true);
            }
        }
    }
}
