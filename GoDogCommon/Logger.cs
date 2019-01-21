using System;
using System.IO;
using System.Linq;

namespace GoDogCommon
{
    public sealed class Logger
    {
        const int FILE_SIZE = 10;
        const int LINES_TO_SKIP = 1000;

        private static Logger logger;
        private static readonly object locker = new object();

        public Logger()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// A helper method to create singleton instance of Logger class
        /// </summary>
        /// <returns></returns>
        public static Logger GetLogger()
        {
            lock (locker)
            {
                if (logger == null)
                {
                    logger = new Logger();
                }
                return logger;
            }
        }

        /// <summary>
        /// Creates the physical log file. 
        /// </summary>
        private void CreateLogFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                File.Create(fileName).Close();
            }
        }

        private long GetMaxFileSize()
        {
            return FILE_SIZE * 1024 * 1024;
        }

        public void Log(string message)
        {
            try
            {
                string filename = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\GoDog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";

                CreateLogFile(filename);

                long length = new FileInfo(filename).Length;
                if (length > GetMaxFileSize())
                {
                    File.WriteAllLines(filename, File.ReadAllLines(filename)
                        .Where((line, index) => index >= LINES_TO_SKIP));
                }

                using (StreamWriter sw = File.AppendText(filename))
                {
                    sw.WriteLine(message);
                }
            }
            catch
            {

            }
        }
    }
}
