using System;
using System.Text;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;

using GoDogCommon;

namespace GoDogMqttClient
{
    public class ManagedSubscriber
    {
        protected const string BROKER_URL = "broker.hivemq.com";
        protected const string MQTT_CLIENTID = "godog-mqtt";
        protected const string MQTT_TOPIC = "godog/service";

        protected Logger logger;
        protected IManagedMqttClient managedClient;

        public ManagedSubscriber()
        {
            this.logger = Logger.GetLogger();

            ManagedMqttClientOptions options = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                    .WithClientOptions(new MqttClientOptionsBuilder()
                    .WithClientId(MQTT_CLIENTID)
                    .WithTcpServer(BROKER_URL, 1883)
                    .Build())
                .Build();

            this.managedClient = new MqttFactory().CreateManagedMqttClient();
            this.managedClient.StartAsync(options);
            this.managedClient.Connected += ManagedClient_Connected;
        }

        private async void ManagedClient_Connected(object sender, MqttClientConnectedEventArgs e)
        {
            this.logger.Log("*** Connected with MQTT server ***");

            await this.managedClient.SubscribeAsync(new TopicFilterBuilder()
                .WithTopic(MQTT_TOPIC).Build());

            this.managedClient.ApplicationMessageReceived += ManagedClient_ApplicationMessageReceived;
        }

        private void ManagedClient_ApplicationMessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            this.logger.Log($"MQTT message: {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
        }

        public void DisconnectClient()
        {
            this.managedClient.StopAsync().Wait();
        }
    }
}
