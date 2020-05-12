using System;
using System.Collections.Generic;
using System.Text;

namespace VirtoCommerce.ExperienceApiModule.Extension.UseCases.Recommendations
{
    /// <summary>
    /// Various reasons for a protocol endpoint error
    /// </summary>
    public enum ResponseErrorType
    {
        /// <summary>
        /// none
        /// </summary>
        None,

        /// <summary>
        /// application related - valid response, but some application level error.
        /// </summary>
        Application,

        /// <summary>
        /// HTTP error - e.g. 404.
        /// </summary>
        Http,

        /// <summary>
        /// An exception occurred - exception while connecting to the endpoint, e.g. TLS problems.
        /// </summary>
        Exception,
    }
}
