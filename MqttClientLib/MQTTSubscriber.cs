using System;
using System.Text;
using System.Threading.Tasks;

using MQTTnet;
using MQTTnet.Client;
using GoDogSBPackage;

namespace MqttClientLib
{
    public class MQTTSubscriber
    {
        private string BROKER_URL = "broker.hivemq.com";
        private IMqttClient mqttClient;
        private MqttFactory factory;
        private IMqttClientOptions options;

        private GoDogSB goDogSB;
        private string topic;

        public MQTTSubscriber(string topic)
        {
            this.topic = topic;
            goDogSB = GoDogSB.GetGoDogSB();
            factory = new MqttFactory();
            mqttClient = factory.CreateMqttClient();
            options = new MqttClientOptionsBuilder()
                    .WithWebSocketServer(BROKER_URL + ":8000/mqtt")
                    .Build();

            mqttClient.ConnectAsync(options);
            mqttClient.Connected += async (s, e) => await OnConnectedAsync(s, e);
            mqttClient.Disconnected += (s, e) => OnDisconnect(s, e);
            mqttClient.ApplicationMessageReceived += (s, e) => OnMessageReceived(s, e);
        }

        public void DisconnectClient()
        {
            if (mqttClient.IsConnected)
            {
                mqttClient.DisconnectAsync().Wait();
            }
        }

        private async Task OnConnectedAsync(object s, MqttClientConnectedEventArgs e)
        {
            this.goDogSB.Log("### Connected to MQTT server ###");
            // Subscribe to a topic
            await mqttClient.SubscribeAsync(new TopicFilterBuilder().WithTopic(this.topic).Build());
        }

        private void OnMessageReceived(object s, MqttApplicationMessageReceivedEventArgs e)
        {
            string message = Encoding.UTF8.GetString(e.ApplicationMessage.Payload).ToLower();
            this.goDogSB.Log($"MQTT message received , {message}");

            if (message == "start")
            {
                this.goDogSB.StartConversion();
            }
            else if (message == "stop")
            {
                this.goDogSB.StopConversion(true);
            }
        }

        private void OnDisconnect(object s, MqttClientDisconnectedEventArgs e)
        {
            this.goDogSB.Log("### Disconnected From MQTT server ###");
        }
    }
}
