using GoDogServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TaskConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {

            Task[] streamingTasks;
            string[] inputUrls = new string[4];

            Dictionary<int, CancellationTokenSource> tokens = new Dictionary<int, CancellationTokenSource>();

            inputUrls[0] = "rtsp://admin:Foxhound1!@192.168.1.64/Streaming/Channels/1";
            inputUrls[1] = "rtsp://admin:Foxhound1!@192.168.1.64/Streaming/Channels/2";
            inputUrls[2] = "rtsp://admin:admin@192.168.1.120:554/live/main";
            inputUrls[3] = "rtsp://admin:admin@192.168.1.120:554/live/sub";
            streamingTasks = new Task[inputUrls.Length];

            Action<object> action = (data) =>
            {
                Console.WriteLine("rtmp://104.248.182.51/live/" + (Convert.ToInt32(data) + 2));
                GoDogProcess goDogProcess = new GoDogProcess(inputUrls[Convert.ToInt32(data)], "rtmp://104.248.182.51/live/" + (Convert.ToInt32(data) + 2));
                goDogProcess.StartConversion();
                //while (true)
                //{
                //    if (data.token.IsCancellationRequested)
                //    {
                //        goDogProcess.StopConversion();
                //    }
                //}
            };

            for (int i = 0; i < inputUrls.Length; i++)
            {
                //CancellationTokenSource tokenSource = new CancellationTokenSource();
                //CancellationToken token = tokenSource.Token;
                //tokens.Add(i, tokenSource);

                streamingTasks[i] = new Task(action, i);
            }

            for (int i = 0; i < streamingTasks.Length; i++)
            {
                streamingTasks[i].Start();
            }

            Console.Read();
        }
    }
}
