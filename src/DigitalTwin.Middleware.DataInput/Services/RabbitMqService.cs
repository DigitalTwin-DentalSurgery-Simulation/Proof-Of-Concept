using DigitalTwin.Middleware.DataInput.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections;
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
        private readonly UserPointCalculator userPointCalculator;

        public RabbitMqService(UserPointCalculator userPointCalculator)
        {
            this.userPointCalculator = userPointCalculator;
        }

        public Task Publish(UserBehaviourInput dataInput)
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

        public Task PublishAll(IEnumerable<UserBehaviourInput> dataInputs)
        {
            var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672, UserName = "guest", Password = "guest" };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                foreach (var dataInput in dataInputs)
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

                    Count++;

                    Thread.Sleep(50);
                }
            }
            
            return Task.CompletedTask;
        }

        public Task PublishAndWaitForConsumerEvent(UserBehaviourInput initialUserInput)
        {
            var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672, UserName = "guest", Password = "guest" };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();

                    var content  = Encoding.UTF8.GetString(body);

                    var hapticOutput = JsonConvert.DeserializeObject<HapticOutput>(content);

                    if (hapticOutput is null)
                        throw new ArgumentNullException(nameof(hapticOutput));

                    var calculateNextPoint  = UserPointCalculator.CalculateNextStep(hapticOutput, Count);

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    channel.BasicPublish(exchange: "dt",
                                            routingKey: "input",
                                            basicProperties: properties,
                                            body: body);
                    Console.WriteLine($"Published message number: {Count}");

                    Count++;
                };
                channel.BasicConsume(queue: "haptic-output",
                                     autoAck: true,
                                     consumer: consumer);


                var json = JsonConvert.SerializeObject(initialUserInput);

                var body = Encoding.UTF8.GetBytes(json);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                channel.BasicPublish(exchange: "dt",
                                        routingKey: "input",
                                        basicProperties: properties,
                                        body: body);
                Console.WriteLine($"Published message number: {Count}");

                Count++;

                Console.WriteLine("Ready to receive another event...");
                Console.ReadLine();
            }

            return Task.CompletedTask;
        }

    }
}
