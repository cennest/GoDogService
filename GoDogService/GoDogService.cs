using System;
using System.Diagnostics;
using System.ServiceProcess;

using GoDogServer;
using GoDogMqttClient;

namespace GoDogService
{
    public partial class GoDogService : ServiceBase
    {
        protected Logger logger;

        public GoDogProcess goDogProcess;
        public ManagedSubscriber mqttSubscriber;

        public GoDogService()
        {
            InitializeComponent();

            logger = new Logger().GetLogger();
            this.goDogProcess = GoDogProcess.GetGoDogProcess(Configuration.InputStreamURL, Configuration.OutputStreamURL);
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

            this.goDogProcess.InputURL = Configuration.InputStreamURL;
            this.goDogProcess.OutputURL = Configuration.OutputStreamURL;

            this.goDogProcess.StartConversion();
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
            this.goDogProcess.StopConversion(true);
            logger.Log("System shutdown at " + DateTime.Now);
            this.goDogEventLog.WriteEntry("System shutdown at " + DateTime.Now);
        }

        protected override void OnStop()
        {
            this.mqttSubscriber.DisconnectClient();
            this.goDogProcess.StopConversion(true);
            logger.Log("Service stoppped at " + DateTime.Now);
            this.goDogEventLog.WriteEntry("Service stoppped at " + DateTime.Now);
        }
    }
}
