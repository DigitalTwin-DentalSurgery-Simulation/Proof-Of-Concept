// See https://aka.ms/new-console-template for more information
using DigitalTwin.Middleware.DataInput;
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

var serviceProvider = serviceCollection.BuildServiceProvider();

var simtoCareJsonFile = "simtocare-recording.json";

var recording = DataReader.Read(simtoCareJsonFile);

Console.WriteLine($"Simulation start time: {recording.Started}");

var rabbitMqService = serviceProvider.GetRequiredService<RabbitMqService>();

foreach (var dataEntry in recording.Data)
{
    //Console.WriteLine($"Position: {dataEntry.Pos[0]} - {dataEntry.Pos[1]} - {dataEntry.Pos[2]}");

    var dataInput = new DataInput()
    {
        PosX = dataEntry.Pos[0].ToString(),
        PosY = dataEntry.Pos[1].ToString(),
        PosZ = dataEntry.Pos[2].ToString()
    };

    rabbitMqService.Publish(dataInput);
}