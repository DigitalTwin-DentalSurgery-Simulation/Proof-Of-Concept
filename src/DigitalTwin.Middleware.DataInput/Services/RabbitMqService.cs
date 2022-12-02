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

        public Task PublishAndWaitForConsumerEvent(UserBehaviourInput initialUserInput)
        {
            var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672, UserName = "guest", Password = "guest" };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    Count += 1;

                    var body = ea.Body.ToArray();

                    var content  = Encoding.UTF8.GetString(body);

                    var hapticOutput = JsonConvert.DeserializeObject<HapticOutput>(content);


                    if (hapticOutput is null)
                        throw new ArgumentNullException(nameof(hapticOutput));

                    if (hapticOutput.OutputUserPosZToMiddleware is 0.00F)
                    {
                        Console.WriteLine("Time is null");
                    }
                    else
                    {
                        Console.WriteLine("Time is not null");

                        var calculateNextPoint = userPointCalculator.CalculateNextStep(hapticOutput, Count);

                        if (calculateNextPoint is null)
                            throw new ArgumentNullException("The procedure seems to be finished");

                        var properties = channel.CreateBasicProperties();
                        properties.Persistent = true;

                        var visualizationInput = new VisualizationInput()
                        {
                            UserPosX = hapticOutput.OutputUserPosXToMiddleware,
                            UserPosY = hapticOutput.OutputUserPosYToMiddleware,
                            UserPosZ = hapticOutput.OutputUserPosZToMiddleware,
                            OpPosX = hapticOutput.OutputOpPosXToMiddleware,
                            OpPosY = hapticOutput.OutputOpPosYToMiddleware,
                            OpPosZ = hapticOutput.OutputOpPosZToMiddleware
                        };

                        var visualizationJson = JsonConvert.SerializeObject(visualizationInput);
                        var visualizationBody = Encoding.UTF8.GetBytes(visualizationJson);

                        var inputJson = JsonConvert.SerializeObject(calculateNextPoint);
                        var inputBody = Encoding.UTF8.GetBytes(inputJson);

                        Thread.Sleep(100);

                        channel.BasicPublish(exchange: "dt",
                                                routingKey: "visualization",
                                                basicProperties: properties,
                                                body: visualizationBody);

                        channel.BasicPublish(exchange: "dt",
                                                routingKey: "input",
                                                basicProperties: properties,
                                                body: inputBody);

                        Console.WriteLine($"Published message number: {Count}");
                    }
                };


                var json = JsonConvert.SerializeObject(initialUserInput);

                var body = Encoding.UTF8.GetBytes(json);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                channel.BasicPublish(exchange: "dt",
                                        routingKey: "input",
                                        basicProperties: properties,
                                        body: body);
                Console.WriteLine($"Published message number: {Count}");

                channel.BasicConsume(queue: "fromsim",
                     autoAck: false,
                     consumer: consumer);

                Console.WriteLine("Ready to receive events...");
                Console.ReadLine();
            }

            return Task.CompletedTask;
        }

    }
}
