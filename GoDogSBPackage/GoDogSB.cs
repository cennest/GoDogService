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

        public string InputURL { get; set; }
        public string OutputURL { get; set; }

        public bool IsForcedStopped { get; set; }

        public GoDogSB(string inputURL, string outputURL)
        {
            this.InputURL = inputURL;
            this.OutputURL = outputURL;
            logger = new GoDogLogger();
        }

        public static GoDogSB GetGoDogSB()
        {
            if (GoDog_SB == null)
            {
                GoDog_SB = new GoDogSB(string.Empty, string.Empty);
            }
            return GoDog_SB;
        }

        public static GoDogSB GetGoDogSB(string inputURL, string outputURL)
        {
            if (GoDog_SB == null)
            {
                GoDog_SB = new GoDogSB(inputURL, outputURL);
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
            if (string.IsNullOrWhiteSpace(this.InputURL) && string.IsNullOrWhiteSpace(this.OutputURL))
            {
                try
                {
                    string filArgs = string.Format("-stimeout 5000 -rtsp_transport tcp -i \"{0}\" -c:v copy -c:a aac -b:a 128k -ar 44100 -f flv \"{1}\"", this.InputURL, this.OutputURL);

                    process = new Process();
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
                }
                catch (Exception ex)
                {
                    this.Log($"Exception: {ex.ToString()}");
                }
                finally
                {
                    if (process != null)
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
            else
            {
                throw new Exception("Input/Output streams not found. PLease provoide Input/Output streams.");
            }
        }
    }
}
