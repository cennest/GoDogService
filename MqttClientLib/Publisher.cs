using MQTTnet;
using MQTTnet.Client;
using System;
using System.Text;
using System.Threading.Tasks;

namespace MqttClientLib
{
    class Publisher
    {
        public static string BROKER_URL = "broker.hivemq.com";
        public static IMqttClient mqttClient;
        public static MqttFactory factory;
        public static IMqttClientOptions options;

        public Publisher()
        {
            factory = new MqttFactory();
            mqttClient = factory.CreateMqttClient();
            options = new MqttClientOptionsBuilder()
                    .WithWebSocketServer(BROKER_URL + ":8000/mqtt")
                    .Build();

            mqttClient.ConnectAsync(options);
            mqttClient.Connected += async (s, e) => await OnConnectedAsync(s, e);
            mqttClient.Disconnected += (s, e) => OnDisconnect(s, e);
        }

        private async Task OnConnectedAsync(object s, MqttClientConnectedEventArgs e)
        {
            Console.WriteLine("### CONNECTED WITH SERVER ###");

            // Subscribe to a topic
            await mqttClient.SubscribeAsync(new TopicFilterBuilder().WithTopic("/#").Build());

            Console.WriteLine("### SUBSCRIBED ###");
        }

        private async void OnDisconnect(object s, MqttClientDisconnectedEventArgs e)
        {
            Console.WriteLine("### DISCONNECTED FROM SERVER ###");
            await Task.Delay(TimeSpan.FromSeconds(5));

            try
            {
                await mqttClient.ConnectAsync(options);
            }
            catch
            {
                Console.WriteLine("### RECONNECTING FAILED ###");
            }
        }
    }
}
