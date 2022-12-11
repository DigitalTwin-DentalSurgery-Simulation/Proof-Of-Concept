using DigitalTwin.Middleware.DataInput.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTwin.Middleware.DataInput.Services
{
    public class RabbitMqService
    {
        private static int Count = 0;
        private static int Discarded = 1;
        private readonly UserPointCalculator userPointCalculator;
        private static UserBehaviourInput? lastInput;
        private static int MaxStepSizeReceived = -1;
        private static DateTime LastReceivedInput = DateTime.UtcNow;

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
      

                    var body = ea.Body.ToArray();

                    var content  = Encoding.UTF8.GetString(body);

                    var hapticOutput = JsonConvert.DeserializeObject<HapticOutput>(content);


                    if (hapticOutput is null)
                        throw new ArgumentNullException(nameof(hapticOutput));

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    if(hapticOutput.OutputUserPosXToMiddleware == 5.0F)
                    {
                        Console.WriteLine($"We got default value {hapticOutput.OutputUserPosXToMiddleware}");

                        lastInput.Time = DateTime.UtcNow.ToString("yyyy-MM-ddTHH\\:mm\\:ss\\.ffffzzzz", CultureInfo.InvariantCulture);

                        var json = JsonConvert.SerializeObject(lastInput);

                        var body2 = Encoding.UTF8.GetBytes(json);

                        var properties2 = channel.CreateBasicProperties();
                        properties.Persistent = true;

                        //Thread.Sleep(140);

                        channel.BasicPublish(exchange: "dt",
                            routingKey: "input",
                            basicProperties: properties2,
                            body: body2);

                        return;
                    }

                    if (hapticOutput.OutputUserPosXToMiddleware == 0.0F)
                    {
                        Console.WriteLine(content);
                        Console.WriteLine($"Discard number: {Discarded}");

                        Discarded += 1;

                        if(content == "{\"internal_status\":\"ready\", \"internal_message\":\"waiting for input data for simulation\"}")
                        {
                            lastInput.Time = DateTime.UtcNow.ToString("yyyy-MM-ddTHH\\:mm\\:ss\\.ffffzzzz", CultureInfo.InvariantCulture);

                            var json = JsonConvert.SerializeObject(lastInput);

                            var body2 = Encoding.UTF8.GetBytes(json);

                            var properties2 = channel.CreateBasicProperties();
                            properties.Persistent = true;

                            //Thread.Sleep(140);

                            channel.BasicPublish(exchange: "dt",
                                routingKey: "input",
                                basicProperties: properties2,
                                body: body2);
                        }

                        return;
                    }
                        

                    var calculateNextPoint = userPointCalculator.CalculateNextStep(hapticOutput);

                    Count += 1;


                    lastInput = calculateNextPoint;

                    if (calculateNextPoint is null)
                        throw new ArgumentNullException("The procedure seems to be finished");

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

                    //Thread.Sleep(140);



                    channel.BasicPublish(exchange: "dt",
                                            routingKey: "input",
                                            basicProperties: properties,
                                            body: inputBody);

                    if(MaxStepSizeReceived < hapticOutput.StepSize)
                    {
                        LastReceivedInput = DateTime.UtcNow;

                        MaxStepSizeReceived = hapticOutput.StepSize;

                        channel.BasicPublish(exchange: "dt",
                            routingKey: "visualization",
                            basicProperties: properties,
                            body: visualizationBody);

                        Console.WriteLine($"Visualization Queue Updated: {MaxStepSizeReceived}");

                    }

                    Console.WriteLine($"Published message number: {Count}");

                    /*if(hapticOutput.OutputOpPosXToMiddleware == default)
                    {
                        Console.WriteLine("Time is null");

                        // This is wrong - should not be initial again
                        var json = JsonConvert.SerializeObject(initialUserInput);

                        var body2 = Encoding.UTF8.GetBytes(json);

                        var properties = channel.CreateBasicProperties();
                        properties.Persistent = true;

                        lastInput = initialUserInput;

                        Thread.Sleep(45);

                        channel.BasicPublish(exchange: "dt",
                            routingKey: "input",
                            basicProperties: properties,
                            body: body2);
                    }*/
                    /*else if (hapticOutput.OutputUserPosXToMiddleware == default(float))
                    {
                        Console.WriteLine($"Value that triggers resend: {hapticOutput.OutputUserPosXToMiddleware}");
                        Console.WriteLine("Resending....");

                        // This is wrong - should not be initial again
                        var json = JsonConvert.SerializeObject(initialUserInput);

                        var body2 = Encoding.UTF8.GetBytes(json);

                        var properties = channel.CreateBasicProperties();
                        properties.Persistent = true;

                        Thread.Sleep(45);

                        if (lastInput is null)
                            throw new ArgumentNullException(nameof(lastInput));

                        lastInput.Time = DateTime.UtcNow.ToString("yyyy-MM-ddTHH\\:mm\\:ss\\.ffffzzzz", CultureInfo.InvariantCulture);

                        var inputJson = JsonConvert.SerializeObject(lastInput);
                        var inputBody = Encoding.UTF8.GetBytes(inputJson);

                        channel.BasicPublish(exchange: "dt",
                            routingKey: "input",
                            basicProperties: properties,
                            body: inputBody);
                    }*/
                };



                initialUserInput.Time = DateTime.UtcNow.ToString("yyyy-MM-ddTHH\\:mm\\:ss\\.ffffzzzz", CultureInfo.InvariantCulture);

                lastInput = initialUserInput;

                channel.BasicConsume(queue: "fromsim",
                     autoAck: true,
                     consumer: consumer);
                
                for (int i = 0; i < 1; i++)
                {
                    initialUserInput.Time = DateTime.UtcNow.ToString("yyyy-MM-ddTHH\\:mm\\:ss\\.ffffzzzz", CultureInfo.InvariantCulture);

                    var json = JsonConvert.SerializeObject(initialUserInput);

                    var body = Encoding.UTF8.GetBytes(json);

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    channel.BasicPublish(exchange: "dt",
                        routingKey: "input",
                        basicProperties: properties,
                        body: body);

                    Console.WriteLine($"Published message number: {Count}");

                    Count += 1;

                    Thread.Sleep(100);
                }
                

                Console.WriteLine("Ready to receive events...");
                Console.ReadLine();
            }

            return Task.CompletedTask;
        }

    
        public Task SendNewPoint(ref IModel channel)
        {
            while (true)
            {

            }
        }
    
    }
}
