using codu.ai.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace codu.ai
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("*************************************************");
            Console.WriteLine("Starting Codu.ai Test");

            var serviceProvider = new ServiceCollection()
            .AddSingleton<TrainOrchestrator>()
            .AddSingleton<RouteOrchestrator>()
            .AddSingleton<FileProcessor>()
            .AddSingleton<OutputExecutor>()
            .BuildServiceProvider();

            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional:false, reloadOnChange:true).Build();

            var fileName = builder.GetSection("InputFileName").Value;

            var fileProcessor = serviceProvider.GetService<FileProcessor>();
            fileProcessor.LoadRoutesFromFile();
            fileProcessor.LoadTrainBogiesFromFileInput(fileName);

            Console.WriteLine("--------------------------------------------------------");
            Console.WriteLine("Showing Train wise Order of Bogies at Source Station");
            var outputExecutor = serviceProvider.GetService<OutputExecutor>();
            var trainTypeBogies = outputExecutor.GetTrainWithBoogies();

            if (trainTypeBogies != null && trainTypeBogies.Values != null)
            {
                foreach (var trainBogies in trainTypeBogies)
                {
                    var booggies = string.Join('|', trainBogies.Value);
                    Console.WriteLine($"{Enum.GetName(typeof(TrainType), trainBogies.Key)} :- { booggies }");
                }
            }
            else
            {
                Console.WriteLine("No Data found for Train Bogies");
                Console.ReadLine();
            }

            Console.WriteLine("*************************************************");

            Console.WriteLine("---- Output -----");
            Console.WriteLine("---- Trains Arrival At HYDRABAD ----");

            var trainTypesBogiesAtMergeStationArrival = outputExecutor.GetTrainBoggiesAtMergerStationArrival();

            foreach (var trainBogies in trainTypesBogiesAtMergeStationArrival)
            {
                var booggies = string.Join('|', trainBogies.Value);
                Console.WriteLine($"ARRIVAL:- {Enum.GetName(typeof(TrainType), trainBogies.Key)} :- { booggies }");
            }

            var trainABDepartureBogies = outputExecutor.GetTrainABDepartureBogies(out bool _isJourneyEnds);

            if (_isJourneyEnds)
            {
                Console.WriteLine("-----  JOURNEY_ENDED -------");
            }
            else
            {
                var booggies = string.Join('|', trainABDepartureBogies);
                Console.WriteLine($"DEPARTURE :- {Enum.GetName(typeof(TrainType), TrainType.Train_AB)} :- { booggies }");
            }

            Console.ReadLine();

        }
    }
}
