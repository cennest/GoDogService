using System;
using System.Diagnostics;
using System.ServiceProcess;

using GoDogSBPackage;
using MqttClientLib;

namespace GoDogService
{
    public partial class GoDogService : ServiceBase
    {
        public GoDogSB goDogSB;
        public MQTTSubscriber mqttSubscriber;
        public GoDogService()
        {
            InitializeComponent();

            this.goDogSB = GoDogSB.GetGoDogSB();
            this.goDogEventLog = new EventLog();
            this.mqttSubscriber = new MQTTSubscriber("godog/service");

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
            this.goDogSB.Log("Service started at " + DateTime.Now);
            this.goDogSB.StartConversion();
        }

        protected override void OnPause()
        {
            this.goDogEventLog.WriteEntry("Service paused at " + DateTime.Now);
            this.goDogSB.Log("Service paused at " + DateTime.Now);
        }

        protected override void OnContinue()
        {
            this.goDogEventLog.WriteEntry("Service continued at " + DateTime.Now);
            this.goDogSB.Log("Service continued at " + DateTime.Now);
        }

        protected override void OnShutdown()
        {
            this.goDogEventLog.WriteEntry("System shutdown at " + DateTime.Now);
            this.goDogSB.Log("System shutdown at " + DateTime.Now);
            this.goDogSB.StopConversion(true);
        }

        protected override void OnStop()
        {
            this.mqttSubscriber.DisconnectClient();
            this.goDogSB.StopConversion(true);

            this.goDogEventLog.WriteEntry("Service stoppped at " + DateTime.Now);
            this.goDogSB.Log("Service stoppped at " + DateTime.Now);
        }
    }
}
