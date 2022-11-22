using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTwin.Middleware.DataInput.Services
{
    public class RabbitMqService
    {
        private static int Count = 0;
        public RabbitMqService()
        {
        }

        public Task Publish(DataInput dataInput)
        {
            var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672, UserName = "guest", Password = "guest" };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {

                var json = JsonConvert.SerializeObject(dataInput);

                var body = Encoding.UTF8.GetBytes(json);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                channel.BasicPublish(exchange: "dt",
                                     routingKey: "input",
                                     basicProperties: properties,
                                     body: body);
                Console.WriteLine($"Published message number: {Count}");


            }

            Count++;

            return Task.CompletedTask;
        }

    }
}
