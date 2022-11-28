// See https://aka.ms/new-console-template for more information
using DigitalTwin.Middleware.DataInput;
using DigitalTwin.Middleware.DataInput.Models;
using DigitalTwin.Middleware.DataInput.Services;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Linq.Expressions;
using System.Net.Http.Json;
using System.Security.Authentication.ExtendedProtection;
using System.Text.Json.Serialization;

Console.WriteLine("SimToCare Simulator Data Input");

var serviceCollection = new ServiceCollection();

serviceCollection.AddTransient<RabbitMqService>();
serviceCollection.AddTransient<UserPointCalculator>();

var serviceProvider = serviceCollection.BuildServiceProvider();

var simtoCareJsonFile = "simtocare-recording.json";

var fileExists = File.Exists(simtoCareJsonFile);

if (!fileExists)
    throw new FileNotFoundException($"You need to manually add the {simtoCareJsonFile} file");

var recording = DataReader.Read(simtoCareJsonFile);

Console.WriteLine($"Simulation start time: {recording.Started}");

var rabbitMqService = serviceProvider.GetRequiredService<RabbitMqService>();

var dataInput = recording.Data.Select(dataInput => new UserBehaviourInput(
    dataInput.Pos[0],
    dataInput.Pos[1],
    dataInput.Pos[2]
    )
).FirstOrDefault();

if (dataInput is null)
    throw new ArgumentNullException(nameof(dataInput));

rabbitMqService.PublishAndWaitForConsumerEvent(dataInput);