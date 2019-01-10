using System;
using System.Diagnostics;
using System.ServiceProcess;

using GoDogServer;
using GoDogMqttClient;

namespace GoDogService
{
    public partial class GoDogService : ServiceBase
    {
        public GoDogProcess goDogProcess;
        public ManagedSubscriber mqttSubscriber;

        string inputFile = @"rtsp://admin:Foxhound1!@192.168.1.64/Streaming/Channels/1";
        string outputFile = @"rtmp://104.248.182.51/live/2";

        public GoDogService()
        {
            InitializeComponent();

            this.goDogProcess = GoDogProcess.GetGoDogProcess(inputFile, outputFile);
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
            Logger.Log("Service started at " + DateTime.Now);
            this.goDogProcess.StartConversion();
        }

        protected override void OnPause()
        {
            this.goDogEventLog.WriteEntry("Service paused at " + DateTime.Now);
            Logger.Log("Service paused at " + DateTime.Now);
        }

        protected override void OnContinue()
        {
            this.goDogEventLog.WriteEntry("Service continued at " + DateTime.Now);
            Logger.Log("Service continued at " + DateTime.Now);
        }

        protected override void OnShutdown()
        {
            this.goDogProcess.StopConversion(true);
            Logger.Log("System shutdown at " + DateTime.Now);
            this.goDogEventLog.WriteEntry("System shutdown at " + DateTime.Now);
        }

        protected override void OnStop()
        {
            this.mqttSubscriber.DisconnectClient();
            this.goDogProcess.StopConversion(true);
            Logger.Log("Service stoppped at " + DateTime.Now);
            this.goDogEventLog.WriteEntry("Service stoppped at " + DateTime.Now);
        }
    }
}
