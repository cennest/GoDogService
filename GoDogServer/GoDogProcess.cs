﻿using System;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace GoDogServer
{
    public class GoDogProcess
    {
        private static GoDogProcess _GoDogProcess;

        protected Logger logger;
        protected Process process;
        protected bool isNetworkAvailable = true;

        public string InputURL { get; set; }
        public string OutputURL { get; set; }

        public bool IsForcedStopped { get; set; }

        public GoDogProcess()
        {
            logger = new Logger().GetLogger();
            NetworkChange.NetworkAvailabilityChanged += NetworkChange_NetworkAvailabilityChanged;
        }

        public GoDogProcess(string inputURL, string outputURL)
        {
            this.InputURL = inputURL;
            this.OutputURL = outputURL;
            logger = new Logger().GetLogger();

            NetworkChange.NetworkAvailabilityChanged += NetworkChange_NetworkAvailabilityChanged;
        }

        public static GoDogProcess GetGoDogProcess()
        {
            if (_GoDogProcess == null)
            {
                _GoDogProcess = new GoDogProcess();
            }
            return _GoDogProcess;
        }

        public static GoDogProcess GetGoDogProcess(string inputURL, string outputURL)
        {
            if (_GoDogProcess == null)
            {
                _GoDogProcess = new GoDogProcess(inputURL, outputURL);
            }
            return _GoDogProcess;
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
            if (!string.IsNullOrWhiteSpace(this.InputURL) && !string.IsNullOrWhiteSpace(this.OutputURL))
            {
                string filArgs = string.Format("-stimeout 5000 -rtsp_transport tcp -re -i \"{0}\" -c:v copy -c:a aac -b:a 128k -ar 44100 -f flv \"{1}\"", this.InputURL, this.OutputURL);
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

                    logger.Log("FFMPEG process started.");
                }
                catch (Exception ex)
                {
                    logger.Log($"Exception: {ex.ToString()}");
                }
            }
            else
            {
                throw new Exception("Unable to start FFMPEG process due to unavailability of Input/Output streams.");
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
                    logger.Log("Restarting FFMPEG process.");
                    StartConversion();
                }

                logger.Log("FFMPEG process exited.");
            }
            else
            {
                logger.Log("FFMPEG process stopped due to no internet connectivity.");
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
                    logger.Log("Restarting FFMPEG process on network availability.");
                    StartConversion();
                }
            }
        }
    }
}
