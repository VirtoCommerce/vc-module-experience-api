using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VirtoCommerce.ExperienceApiModule.Extension.UseCases.Recommendations.Configuration
{
    public class Scenario
    {
        [Required]
        public string Name { get; set; }
        public string Filter { get; set; }
        [Required]
        public string ConnectionName { get; set; }
        public Connection Connection { get; set; }
    }
}
