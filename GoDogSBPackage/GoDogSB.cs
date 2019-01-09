using System;
using System.Diagnostics;

namespace GoDogSBPackage
{
    public class GoDogSB
    {
        protected static GoDogSB GoDog_SB;
        private GoDogLogger logger;
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
            StartVideoConversion();
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
                this.Log(e.Message);
            }
        }

        public void Log(string message)
        {
            logger.Log(message);
        }

        private void StartVideoConversion()
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

                    this.Log("Conversion process started.");
                }
                catch (Exception ex)
                {
                    this.Log($"Exception: {ex.ToString()}");
                }
            }
            else
            {
                throw new Exception("Input/Output streams not found. PLease provoide Input/Output streams.");
            }
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            this.Log(e.Data);
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            this.Log(e.Data);
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            process.CancelErrorRead();
            process.CancelOutputRead();
            process.Close();

            if (!IsForcedStopped)
            {
                this.Log("Restarting conversion process.");
                StartConversion();
            }

            this.Log("Conversion process exited.");
        }
    }
}
