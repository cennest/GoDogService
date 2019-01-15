using System;
using System.Diagnostics;
using System.ServiceProcess;

using GoDogServer;
using GoDogMqttClient;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace GoDogService
{
    public partial class GoDogService : ServiceBase
    {
        protected Logger logger;

        public GoDogProcess goDogProcess;
        public ManagedSubscriber mqttSubscriber;
        private Task[] streamingTasks;
        private string[] inputUrls;
        private Dictionary<int, CancellationTokenSource> tokens;


        public GoDogService()
        {
            InitializeComponent();

            inputUrls[0] = "rtsp://admin:Foxhound1!@192.168.1.64/Streaming/Channels/1";
            inputUrls[1] = "rtsp://admin:Foxhound1!@192.168.1.64/Streaming/Channels/2";
            inputUrls[2] = "rtsp://admin:admin@192.168.1.120:554/live/main";
            inputUrls[3] = "rtsp://admin:admin@192.168.1.120:554/live/sub";

            streamingTasks = new Task[inputUrls.Length];
            logger = new Logger().GetLogger();

            for (int i = 0; i < inputUrls.Length; i++)
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
            this.goDogEventLog = new EventLog();
            this.mqttSubscriber = new ManagedSubscriber();

            if (!EventLog.SourceExists("GoDogSource"))
            {
                EventLog.CreateEventSource("GoDogSource", "GoDogLog");
            }

            this.goDogEventLog.Source = "GoDogSource";
            this.goDogEventLog.Log = "GoDogLog";
        }

        protected override void OnStart(string[] args)
        {
            this.goDogEventLog.WriteEntry("Service started at " + DateTime.Now);
            logger.Log("Service started at " + DateTime.Now);
            for (int i = 0; i < inputUrls.Length; i++)
            {
                streamingTasks[i].Start();
            }
        }

        protected override void OnPause()
        {
            this.goDogEventLog.WriteEntry("Service paused at " + DateTime.Now);
            logger.Log("Service paused at " + DateTime.Now);
        }

        protected override void OnContinue()
        {
            this.goDogEventLog.WriteEntry("Service continued at " + DateTime.Now);
            logger.Log("Service continued at " + DateTime.Now);
        }

        protected override void OnShutdown()
        {
            for (int i = 0; i < inputUrls.Length; i++)
            {
                tokens[i].Cancel();
            }
            logger.Log("System shutdown at " + DateTime.Now);
            this.goDogEventLog.WriteEntry("System shutdown at " + DateTime.Now);
        }

        protected override void OnStop()
        {
            for (int i = 0; i < tokens.Count; i++)
            {
                tokens[i].Cancel();
            }
            this.mqttSubscriber.DisconnectClient();
            logger.Log("Service stoppped at " + DateTime.Now);
            this.goDogEventLog.WriteEntry("Service stoppped at " + DateTime.Now);
        }
    }
}
