using System;
using System.Collections.Generic;
using System.Text;

namespace RecommendationsGatewayModule.Core.Configuration
{
    public class RecommendationOptions
    {
        public string DefaultScenario { get; set; }
        public IList<Scenario> Scenarios { get; set; } = new List<Scenario>();
        public IList<Connection> Connections { get; set; } = new List<Connection>();
    }
}
