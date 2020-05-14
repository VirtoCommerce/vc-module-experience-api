using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RecommendationsGatewayModule.Core.Requests;

namespace RecommendationsGatewayModule.Core.Configuration
{
    public class Connection
    {
        [Required]
        public string Name { get; set; }
        public string Method { get; set; } = "GET";
        public string ContentType { get; set; } = "application/json";
        [Required, Url]
        public string Url { get; set; }
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
        [Required]
        public string RequestContentTemplate { get; set; }
        public Type ResponseType { get; set; } = typeof(GetRecommendationsResponse);
        [Required]
        public string ResponseJsonPath { get; set; }
    }
}
