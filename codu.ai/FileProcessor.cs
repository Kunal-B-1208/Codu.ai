using codu.ai.Enums;
using codu.ai.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace codu.ai
{
    public class FileProcessor
    {
        private readonly RouteOrchestrator routeOrchestrator;
        private readonly TrainOrchestrator trainOrchestrator;
        public FileProcessor(RouteOrchestrator routeOrchestrator, TrainOrchestrator trainOrchestrator)
        {
            this.routeOrchestrator = routeOrchestrator;
            this.trainOrchestrator = trainOrchestrator;
        }
        public void LoadRoutesFromFile()
        {
            Console.WriteLine("Loading Master Routes for Train from Data Files......");
            var fileNames = new Dictionary<RouteType, string>()
            {
                { RouteType.Route_A, ServiceConstants.Train_A_Routes },
                { RouteType.Route_B, ServiceConstants.Train_B_Routes },
            };

            var folderPath = Environment.CurrentDirectory;//Directory.GetCurrentDirectory();

            foreach (var filename in fileNames)
            {
                var fileWithPath = Path.Combine(folderPath, "Data", filename.Value);

                foreach (var station in ReadRouteStationsFromFile(fileWithPath))
                {
                    routeOrchestrator.AddRouteWiseStations(filename.Key, station);
                }
            }

            Console.WriteLine("Routes Master loaded from Files......");
            Console.WriteLine();
        }

        private IEnumerable<Station> ReadRouteStationsFromFile(string fileName)
        {
            Station station = null;
            if (File.Exists(fileName))
            {
                foreach (var line in File.ReadLines(fileName))
                {
                    var data = line.Split('|');

                    if (data.Count() == 3)
                    {
                        station = new Station
                        {
                            StationName =  data[0],
                            StationCode = data[1],
                            Distance = int.Parse(data[2])
                        };

                        yield return station;
                    }
                }
            }
        }

        public void LoadTrainBogiesFromFileInput(string inputfileName)
        {
            Console.WriteLine("Loading Order of Bogies from Input file....");

            var fileName = Path.Combine(Directory.GetCurrentDirectory(), "Data", inputfileName);

            foreach (var trainBoggies in ReadTrainWiseBogieInputFromFile(fileName))
            {
                trainOrchestrator.AddTrainWiseBogies(trainBoggies.Key, trainBoggies.Value);
            }

            Console.WriteLine("Order of Bogies Loaded for Train-A and Train-B....");
            Console.WriteLine();

        }

        private Dictionary<TrainType,IEnumerable<Station>> ReadTrainWiseBogieInputFromFile(string fileName)
        {
            Dictionary<TrainType, IEnumerable<Station>> result = new Dictionary<TrainType, IEnumerable<Station>>();
            if (File.Exists(fileName))
            {
                foreach (var line in File.ReadLines(fileName))
                {
                    var data = line.Split(':');

                    if (data.Count() == 2)
                    {
                        var stationCodes = data[1].Split('|');
                        RouteType routeType = data[0] == "Train_A" ? 
                                              RouteType.Route_A : data[0] == "Train_B" ? 
                                              RouteType.Route_B : RouteType.NA;

                        TrainType trainType = data[0] == "Train_A" ?
                                              TrainType.Train_A : data[0] == "Train_B" ?
                                              TrainType.Train_B : TrainType.Train_AB;

                        if (stationCodes.Length > 0)
                        {
                            var getStations = routeOrchestrator.GetStaionByRouteTypeAndStationCode(routeType, stationCodes.ToList());
                            if (!result.ContainsKey(trainType))
                            {
                                result.Add(trainType, getStations);
                            }
                            else
                            {
                                result[trainType] = getStations;
                            }
                        }
                    }

                }
            }

            return result;
        }

    }
}
