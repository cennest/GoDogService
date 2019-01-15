using GoDogServer;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            Task[] streamingTasks;
            string[] inputUrls=new string[4];
            Dictionary<int, CancellationTokenSource> tokens=new Dictionary<int, CancellationTokenSource>();

            inputUrls[0] = "rtsp://admin:Foxhound1!@192.168.1.64/Streaming/Channels/1";
            inputUrls[1] = "rtsp://admin:Foxhound1!@192.168.1.64/Streaming/Channels/2";
            inputUrls[2] = "rtsp://admin:admin@192.168.1.120:554/live/main";
            inputUrls[3] = "rtsp://admin:admin@192.168.1.120:554/live/sub";
            streamingTasks = new Task[inputUrls.Length];


            for (int i = 0; i < inputUrls.Length-1; i++)
            {
                CancellationTokenSource tokenSource = new CancellationTokenSource();
                CancellationToken token = tokenSource.Token;
                tokens.Add(i, tokenSource);
                streamingTasks[i] = new Task(() =>
                {
                    GoDogProcess goDogProcess = new GoDogProcess(inputUrls[i], "rtmp://104.248.182.51/live/" + i + 2);
                    goDogProcess.StartConversion();
                    while (true)
                    {
                        if (token.IsCancellationRequested)
                        {
                            goDogProcess.StopConversion();
                        }
                    }
                }, token);
            }

            for (int i = 0; i < inputUrls.Length-1; i++)
            {
                streamingTasks[i].Start();
            }

            Console.Read();
        }
    }
}
