using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RecommendationsGatewayModule.Core.Configuration
{
    public class Scenario
    {
        [Required]
        public string Name { get; set; }
        public string Filter { get; set; }
        [Required]
        public string ConnectionName { get; set; }
        [Required]
        public Connection Connection { get; set; }
    }
}
