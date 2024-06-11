using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PipelineNet.Middleware;
using VirtoCommerce.CustomerModule.Core.Model;
using VirtoCommerce.CustomerModule.Core.Services;
using VirtoCommerce.ExperienceApiModule.Core.Models;
using VirtoCommerce.ExperienceApiModule.XDigitalCatalog.Index;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Core.Modularity;

namespace VirtoCommerce.XDigitalCatalog.Middlewares
{
    public class EvalSearchRequestUserGroupsMiddleware : IAsyncMiddleware<IndexSearchRequestBuilder>
    {
        protected readonly IMemberResolver _memberResolver;
        protected readonly IMemberService _memberService;
        protected readonly IModuleCatalog _moduleCatalog;

        public EvalSearchRequestUserGroupsMiddleware(IMemberResolver memberResolver, IMemberService memberService, IModuleCatalog moduleCatalog)
        {
            _memberResolver = memberResolver;
            _memberService = memberService;
            _moduleCatalog = moduleCatalog;
        }

        public virtual async Task Run(IndexSearchRequestBuilder parameter, Func<IndexSearchRequestBuilder, Task> next)
        {
            /// Please note that this solution is temporary. In the upcoming release, we are actively working on resolving this issue by introducing optional dependencies.
            /// With optional dependencies, the XAPI will seamlessly integrate with the Catalog Personalization Module if it is installed, and gracefully handle scenarios where the module is not present.
            /// This approach will provide a more robust and flexible solution, enabling smoother interactions between the XAPI and the Catalog Personalization Module.
            if (IsCatalogPersonalizationModuleInstalled())
            {
                var userGroups = new List<string> { "__any" };

                if (!string.IsNullOrEmpty(parameter?.UserId) && !AnonymousUser.UserName.EqualsInvariant(parameter.UserId))
                {
                    var member = await _memberResolver.ResolveMemberByIdAsync(parameter.UserId);

                    if (member is Contact contact)
                    {
                        userGroups.AddRange(await GetUserGroupsInheritedAsync(contact));
                    }
                }

                var userGroupsValue = string.Join(',', userGroups);
                parameter?.AddTerms(new[] { $"user_groups:{userGroupsValue}" });
            }

            await next(parameter);
        }


        /// <summary>
        /// Checks if the Catalog Personalization Module is installed.
        /// </summary>
        /// <returns></returns>
        private bool IsCatalogPersonalizationModuleInstalled()
        {
            return _moduleCatalog.Modules.Any(m => m.ModuleName == "VirtoCommerce.CatalogPersonalization");
        }

        private async Task<IList<string>> GetUserGroupsInheritedAsync(Contact contact)
        {
            var userGroups = new List<string>();

            if (!contact.Groups.IsNullOrEmpty())
            {
                userGroups.AddRange(contact.Groups);
            }

            if (!contact.Organizations.IsNullOrEmpty())
            {
                var organizations = await _memberService.GetByIdsAsync(contact.Organizations.ToArray(), MemberResponseGroup.WithGroups.ToString());
                userGroups.AddRange(organizations.OfType<Organization>().SelectMany(x => x.Groups));
            }

            return userGroups;
        }
    }
}
