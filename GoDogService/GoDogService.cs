using System;
using System.Diagnostics;
using System.ServiceProcess;

using GoDogNUCServer;
using GoDogMqttClient;

namespace GoDogService
{
    public partial class GoDogService : ServiceBase
    {
        public GoDogManager goDogManager;
        public ManagedSubscriber mqttSubscriber;

        public GoDogService()
        {
            InitializeComponent();

            this.goDogManager = GoDogManager.GetGoDogManager();
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
            this.goDogManager.StartAllCameras();
            this.goDogEventLog.WriteEntry("Service started at " + DateTime.Now);
        }

        protected override void OnPause()
        {
            this.goDogEventLog.WriteEntry("Service paused at " + DateTime.Now);
        }

        protected override void OnContinue()
        {
            this.goDogEventLog.WriteEntry("Service continued at " + DateTime.Now);
        }

        protected override void OnStop()
        {
            this.goDogManager.StopAllCameras();
            this.mqttSubscriber.DisconnectClient();
            this.goDogEventLog.WriteEntry("Service stoppped at " + DateTime.Now);
        }
    }
}
