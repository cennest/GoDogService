using System;
using System.IO;

namespace GoDogServer
{
    public class Logger
    {
        private object locker = new object();
        private static Logger _Logger;

        public Logger()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
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

        /// <summary>
        /// A helper method to create singleton instance of Logger class
        /// </summary>
        /// <returns></returns>
        public Logger GetLogger()
        {
            if (_Logger == null)
            {
                lock (locker)
                {
                    if (_Logger == null)
                    {
                        _Logger = new Logger();
                    }
                }
            }
            return _Logger;
        }

        public void Log(string message)
        {
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\GoDog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + "-"+ DateTime.Now.Hour + ".txt";
            CreateLogFile(filepath);

            using (StreamWriter sw = File.AppendText(filepath))
            {
                sw.WriteLine(message);
            }
        }
    }
}
