using codu.ai.Enums;
using codu.ai.Model;
using System.Collections.Generic;
using System.Linq;

namespace codu.ai
{
    public class TrainOrchestrator
    {
        private Dictionary<TrainType, Train> trainWiseBogies;
        public TrainOrchestrator()
        {
            trainWiseBogies = new Dictionary<TrainType, Train>();
        }

        public void AddTrainWiseBogies(TrainType trainType, IEnumerable<Station> stations)
        {
            if (!trainWiseBogies.ContainsKey(trainType))
            {
                var train = new Train();
                train.TrainType = trainType;

                train.TrainRouteStations = new SortedDictionary<int, Dictionary<string, IEnumerable<Station>>>();
                Dictionary<string, IEnumerable<Station>> stationCodeWiseStations = null;

                foreach (var station in stations)
                {
                    if (train.TrainRouteStations.TryGetValue(station.Distance, out stationCodeWiseStations))
                    {
                        if (stationCodeWiseStations.TryGetValue(station.StationCode, out IEnumerable<Station> codewiseStations))
                        {
                            var codewiseStationsList = codewiseStations.ToList();
                            codewiseStationsList.Add(station);
                            stationCodeWiseStations[station.StationCode] = codewiseStationsList;
                        }
                    }
                    else
                    {
                        stationCodeWiseStations = new Dictionary<string, IEnumerable<Station>>();
                        stationCodeWiseStations.Add(station.StationCode, new List<Station> { station });
                        train.TrainRouteStations.Add(station.Distance, stationCodeWiseStations);
                    }

                }

                trainWiseBogies.Add(trainType, train);
            }
        }

        public IEnumerable<string> GetTrainBogiesByTrainType(TrainType trainType)
        {

            if (trainWiseBogies.TryGetValue(trainType, out Train train))
            {

                return GetTrainBogiesByTrainType(train.TrainRouteStations);
            }

            return null;
        }

        public IEnumerable<string> GetTrainBogiesOrderAfterMergerStationArrival(TrainType trainType)
        {
            var IsKeyPresent = trainWiseBogies.TryGetValue(trainType, out Train train);
            if (IsKeyPresent)
            {
                var keys = GetKeysToBeRemoved(trainType, train);

                foreach (var key in keys)
                {
                    train.TrainRouteStations.Remove(key);
                }

                return GetTrainBogiesByTrainType(train.TrainRouteStations);

            }

            return null;
        }

        public IEnumerable<string> GetTrainABDepartureBogiesAtMergerStation(out bool IsJourneyEnd)
        {
            List<Station> bogiesStations = new List<Station>();
            IEnumerable<Station> bogies = null;
            IsJourneyEnd = false;
            foreach (var train in trainWiseBogies)
            {
                if (train.Value.TrainType == TrainType.Train_A)
                {
                    bogies = GetStationsByTrain(train.Value, ServiceConstants.MergeStationDistanceRoute_A);

                }
                else if (train.Value.TrainType == TrainType.Train_B)
                {
                    bogies = GetStationsByTrain(train.Value, ServiceConstants.MergeStationDistanceRoute_B);
                }

                if (bogies != null)
                {
                    bogiesStations.AddRange(bogies);
                }
            }

            if (bogiesStations.Count() == 0)
            {
                IsJourneyEnd = true;
            }

            return bogiesStations.OrderByDescending(x => x.Distance).Select(x => x.StationCode).Prepend("Engine").Prepend("Engine").ToList();

        }

        private IEnumerable<Station> GetStationsByTrain(Train train, int distanceCalculator)
        {
            return train.TrainRouteStations.Values.SelectMany(stationCode => 
                                                              stationCode.Values
                                                                         .SelectMany(station =>
                                                                                     station.Select(x =>
                                                                                                     {
                                                                                                        x.Distance -= distanceCalculator;
                                                                                                        return x;
                                                                                                     })
                                                                                     )
                                                              );

        }

        private IEnumerable<string> GetTrainBogiesByTrainType(SortedDictionary<int, Dictionary<string, IEnumerable<Station>>> dictionaryValues)
        {
            return dictionaryValues.Reverse()
                                   .SelectMany(x =>
                                               x.Value
                                                .Values
                                                .SelectMany(y =>
                                                            y.Select(a =>
                                                                     a.StationCode))
                                                                     )
                                               .Prepend("Engine").ToList();
        }

        private IEnumerable<int> GetKeysToBeRemoved(TrainType trainType, Train train)
        {
            if (trainType == TrainType.Train_A)
            {
                return train.TrainRouteStations.Where(x => x.Key <= ServiceConstants.MergeStationDistanceRoute_A).Select(x => x.Key).ToList();
            }
            else if (trainType == TrainType.Train_B)
            {
                return train.TrainRouteStations.Where(x => x.Key <= ServiceConstants.MergeStationDistanceRoute_B).Select(x => x.Key).ToList();
            }

            return null;
        }

    }
}
