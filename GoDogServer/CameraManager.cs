using GoDogCommon;
using GoDogServer.Models;
using System;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace GoDogServer
{
    class CameraManager
    {
        protected Logger logger;
        protected Process process;
        protected bool isNetworkAvailable = true;
        private Camera Camera;

        public bool IsForcedStopped { get; set; }

        public CameraManager(Camera camera)
        {
            this.Camera = camera;
            this.logger = Logger.GetLogger();
            NetworkChange.NetworkAvailabilityChanged += NetworkChange_NetworkAvailabilityChanged;
        }

        public void StartConversion()
        {
            IsForcedStopped = false;
            StartConversionProcess();
        }

        public void StopConversion(bool forcedStop = false)
        {
            try
            {
                IsForcedStopped = forcedStop;
                if (process != null && !process.HasExited)
                {
                    process.CancelErrorRead();
                    process.CancelOutputRead();
                    process.Kill();
                }
            }
            catch (Exception e)
            {
                logger.Log(e.Message);
            }
        }

        private void StartConversionProcess()
        {
            //string cameraURL = string.Format("rtsp://{0}:{1}@{2}/axis-media/media.amp", this.Camera.Username, this.Camera.Password, this.Camera.IPAddress);
            string cameraURL = this.Camera.CameraURL;
            if (!string.IsNullOrWhiteSpace(cameraURL) && !string.IsNullOrWhiteSpace(this.Camera.StreamingURL))
            {
                string filArgs = string.Format("-stimeout 5000 -rtsp_transport tcp -re -i \"{0}\" -c:v copy -c:a aac -b:a 128k -ar 44100 -f flv \"{1}\"", cameraURL, this.Camera.StreamingURL);
                try
                {
                    process = new Process();

                    process.StartInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + "\\ffmpeg\\ffmpeg.exe";
                    process.StartInfo.Arguments = filArgs;

                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                    process.EnableRaisingEvents = true;
                    process.Exited += Process_Exited;
                    process.OutputDataReceived += Process_OutputDataReceived;
                    process.ErrorDataReceived += Process_ErrorDataReceived;

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    logger.Log($"Camera {this.Camera.ID} FFMPEG process started.");
                }
                catch (Exception ex)
                {
                    logger.Log($"Exception: {ex.ToString()}");
                }
            }
            else
            {
                throw new Exception($"Unable to start FFMPEG process of Camera {this.Camera.ID} due to unavailability of Input/Output streams.");
            }
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            logger.Log(e.Data);
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            logger.Log(e.Data);
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            process.CancelErrorRead();
            process.CancelOutputRead();
            process.Close();

            if (isNetworkAvailable)
            {
                if (!IsForcedStopped)
                {
                    logger.Log($"Camera {this.Camera.ID} Restarting FFMPEG process.");
                    StartConversion();
                }

                logger.Log($"Camera {this.Camera.ID} FFMPEG process exited.");
            }
            else
            {
                logger.Log($"Camera {this.Camera.ID} FFMPEG process stopped due to no internet connectivity.");
            }
        }

        private void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            if (isNetworkAvailable != e.IsAvailable)
            {
                isNetworkAvailable = e.IsAvailable;

                System.Threading.Tasks.Task.Delay(5000).Wait();

                if (isNetworkAvailable && process.HasExited)
                {
                    logger.Log($"Restarting Camera {this.Camera.ID} FFMPEG process on network availability.");
                    StartConversion();
                }
            }
        }
    }
}
