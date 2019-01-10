using System;
using System.Text;
using System.Threading.Tasks;

using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;

using GoDogServer;

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
            logger = new Logger().GetLogger();

            var options = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                    .WithClientOptions(new MqttClientOptionsBuilder()
                    .WithClientId(MQTT_CLIENTID)
                    .WithTcpServer(BROKER_URL, 1883)
                    .Build())
                .Build();

            managedClient = new MqttFactory().CreateManagedMqttClient();
            managedClient.StartAsync(options);
            managedClient.Connected += ManagedClient_Connected;
        }

        private async void ManagedClient_Connected(object sender, MqttClientConnectedEventArgs e)
        {
            logger.Log("*** Connected with MQTT server ***");

            await managedClient.SubscribeAsync(new TopicFilterBuilder()
                .WithTopic(MQTT_TOPIC).Build());

            managedClient.ApplicationMessageReceived += ManagedClient_ApplicationMessageReceived;
        }

        private void ManagedClient_ApplicationMessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            logger.Log($"MQTT Message: {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
        }

        public void DisconnectClient()
        {
            managedClient.StopAsync().Wait();
        }
    }
}
