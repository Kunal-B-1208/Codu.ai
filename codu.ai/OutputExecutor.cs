using codu.ai.Enums;
using System.Collections.Generic;

namespace codu.ai
{
    public class OutputExecutor
    {
        private readonly RouteOrchestrator routeOrchestrator;
        private readonly TrainOrchestrator trainOrchestrator;
        public OutputExecutor(RouteOrchestrator routeOrchestrator, TrainOrchestrator trainOrchestrator)
        {
            this.routeOrchestrator = routeOrchestrator;
            this.trainOrchestrator = trainOrchestrator;
        }

        public Dictionary<TrainType, IEnumerable<string>> GetTrainWithBoogies()
        {
            Dictionary<TrainType, IEnumerable<string>> trainBoogies= new Dictionary<TrainType, IEnumerable<string>>();
            trainBoogies.Add(TrainType.Train_A,trainOrchestrator.GetTrainBogiesByTrainType(TrainType.Train_A));
            trainBoogies.Add(TrainType.Train_B, trainOrchestrator.GetTrainBogiesByTrainType(TrainType.Train_B));
            return trainBoogies;
        }

        public Dictionary<TrainType, IEnumerable<string>> GetTrainBoggiesAtMergerStationArrival()
        {
            Dictionary<TrainType, IEnumerable<string>> trainBoogies = new Dictionary<TrainType, IEnumerable<string>>();
            trainBoogies.Add(TrainType.Train_A, trainOrchestrator.GetTrainBogiesOrderAfterMergerStationArrival(TrainType.Train_A));
            trainBoogies.Add(TrainType.Train_B, trainOrchestrator.GetTrainBogiesOrderAfterMergerStationArrival(TrainType.Train_B));
            return trainBoogies;
        }

        public IEnumerable<string> GetTrainABDepartureBogies(out bool IsJourneyEnds)
        {
            return trainOrchestrator.GetTrainABDepartureBogiesAtMergerStation(out IsJourneyEnds);
        }


    }
}
