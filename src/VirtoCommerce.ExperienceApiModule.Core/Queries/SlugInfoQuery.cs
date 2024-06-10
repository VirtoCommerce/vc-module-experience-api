using System;
using System.Collections.Generic;
using GraphQL;
using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.BaseQueries;

namespace VirtoCommerce.ExperienceApiModule.Core.Queries
{
    public class SlugInfoQuery : Query<SlugInfoResponse>
    {
        private string _permalink;

        [Obsolete("Use Permalink property", DiagnosticId = "VC0008", UrlFormat = "https://docs.virtocommerce.org/products/products-virto3-versions/")]
        public string Slug { get; set; }

#pragma warning disable VC0008
        public string Permalink { get => Slug ?? _permalink; set => _permalink = value; }
#pragma warning restore VC0008
        public string StoreId { get; set; }
        public string UserId { get; set; }
        public string CultureName { get; set; }

        public override IEnumerable<QueryArgument> GetArguments()
        {
#pragma warning disable VC0008
            yield return Argument<StringGraphType>(nameof(Slug));
#pragma warning restore VC0008
            yield return Argument<StringGraphType>(nameof(Permalink));
            yield return Argument<StringGraphType>(nameof(StoreId));
            yield return Argument<StringGraphType>(nameof(UserId));
            yield return Argument<StringGraphType>(nameof(CultureName));
        }

        public override void Map(IResolveFieldContext context)
        {
#pragma warning disable VC0008
            Slug = context.GetArgument<string>(nameof(Slug));
#pragma warning restore VC0008
            Permalink = context.GetArgument<string>(nameof(Permalink));
            StoreId = context.GetArgument<string>(nameof(StoreId));
            UserId = context.GetArgument<string>(nameof(UserId));
            CultureName = context.GetArgument<string>(nameof(CultureName));
        }
    }
}
