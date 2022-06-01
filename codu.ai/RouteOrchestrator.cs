using codu.ai.Enums;
using codu.ai.Model;
using System.Collections.Generic;
using System.Linq;

namespace codu.ai
{
    public class RouteOrchestrator
    {
        private Dictionary<RouteType, Route> routeWiseStations;
        public RouteOrchestrator()
        {
            routeWiseStations = new Dictionary<RouteType, Route>();
        }

        public void AddRouteWiseStations(RouteType routeType, Station station)
        {
            if (routeWiseStations.TryGetValue(routeType, out Route route))
            {
                if (route.Stations == null)
                {
                    route.Stations = new List<Station>();
                }
                route.Stations.Add(station);
            }
            else
            {
                routeWiseStations.Add(routeType, new Route { Stations = new List<Station>() { station } });
            }

        }

        public IEnumerable<Station> GetStaionByRouteTypeAndStationCode(RouteType routeType, IEnumerable<string> stationCodes)
        {
            if (routeWiseStations.TryGetValue(routeType, out Route route))
            {
                return (from routeStation in route.Stations
                        join stationcode in stationCodes
                        on routeStation.StationCode equals stationcode
                        select routeStation);
            }

            return null;
        }
    }
}
