using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace VirtoCommerce.ExperienceApiModule.Core.Pipelines
{
    public interface IGenericPipelineLauncher
    {
        Task Execute<TParameter>(TParameter parameter) where TParameter : class;
    }
}
