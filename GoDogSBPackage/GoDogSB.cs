using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;

namespace GoDogSBPackage
{
    public class GoDogSB
    {
        protected static GoDogSB GoDog_SB;
        private GoDogLogger logger;
        private Thread bgWorker;
        private Process process;

        public bool IsForcedStopped { get; set; }

        public GoDogSB()
        {
            logger = new GoDogLogger();
        }

        public static GoDogSB GetGoDogSB()
        {
            if (GoDog_SB == null)
            {
                GoDog_SB = new GoDogSB();
            }

            return GoDog_SB;
        }

        public void StartConversion()
        {
            IsForcedStopped = false;
            bgWorker = new Thread(new ThreadStart(StartVideoConversion));
            bgWorker.Start();
        }

        public void StopConversion(bool forcedStop = false)
        {
            try
            {
                IsForcedStopped = forcedStop;
                if (!process.HasExited)
                {
                    process.Kill();
                }
                bgWorker.Abort();
            }
            catch (Exception e)
            {
                this.Log(e.Message);
            }
        }

        public void Log(string message)
        {
            logger.Log(message);
        }

        private void StartVideoConversion()
        {
            string inputFile = @"rtsp://admin:Foxhound1!@192.168.1.64/Streaming/Channels/1";
            string outputFile = @"rtmp://104.248.182.51/live/2";
            string filArgs = string.Format("-stimeout 5000 -rtsp_transport tcp -i \"{0}\" -c:v copy -c:a aac -b:a 128k -ar 44100 -f flv \"{1}\"", inputFile, outputFile);

            process = new Process();

            try
            {
                process.StartInfo.FileName = AppDomain.CurrentDomain.BaseDirectory + "\\ffmpeg\\ffmpeg.exe";
                process.StartInfo.Arguments = filArgs;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;

                process.Start();

                this.Log("Conversion process started.");

                while (!process.StandardError.EndOfStream)
                {
                    this.Log(process.StandardError.ReadLine());
                }

                this.Log($"Responding: {process.Responding.ToString()}");
            }
            catch (Exception ex)
            {
                this.Log($"Exception: {ex.ToString()}");
            }
            finally
            {
                process.WaitForExit();
                process.Close();

                if (!IsForcedStopped)
                {
                    this.Log("Restarting conversion process.");
                    StopConversion();
                    StartConversion();
                }

                this.Log("Conversion process exited.");
            }
        }
    }
}
