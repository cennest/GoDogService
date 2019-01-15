using GoDogServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            GoDogProcess goDogProcess = GoDogProcess.GetGoDogProcess("rtsp://admin:Foxhound1!@192.168.1.64/Streaming/Channels/1", "rtmp://104.248.182.51/live/2");
            goDogProcess.StartConversion();
            Console.Read();
        }
    }
}
