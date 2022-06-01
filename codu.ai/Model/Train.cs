using codu.ai.Enums;
using System.Collections.Generic;

namespace codu.ai.Model
{
    public class Train
    {
        public TrainType TrainType { get; set; }
        public SortedDictionary<int,Dictionary<string,IEnumerable<Station>>> TrainRouteStations { get; set; }
    }
}
