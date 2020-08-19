using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.CustomerModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.Core
{
    //TODO: Move into ContactModule
    public interface IMemberResolver
    {
        Task<Member> ResolveMemberByIdAsync(string userId);
    }
}
