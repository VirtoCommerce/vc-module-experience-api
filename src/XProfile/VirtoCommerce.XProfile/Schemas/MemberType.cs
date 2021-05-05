using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Extensions;
using VirtoCommerce.ExperienceApiModule.Core.Schemas;
using VirtoCommerce.ExperienceApiModule.XProfile.Aggregates;
using VirtoCommerce.ExperienceApiModule.XProfile.Extensions;
using VirtoCommerce.Platform.Core.DynamicProperties;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class MemberType : ExtendableGraphType<MemberAggregateRootBase>
    {
        public MemberType(IDynamicPropertySearchService dynamicPropertySearchService)
        {
            Field(x => x.Member.Id);
            Field(x => x.Member.MemberType);
            Field(x => x.Member.Name, true);
            Field(x => x.Member.OuterId, true);
            ExtendableField<ListGraphType<AddressType>>("addresses", resolve: context => context.Source.Member.Addresses);
            ExtendableField<NonNullGraphType<ListGraphType<Core.Schemas.DynamicPropertyValueType>>>(
                "dynamicProperties",
                "Contact's dynamic property values",
                new QueryArguments(new QueryArgument<StringGraphType>
                {
                    Name = "cultureName",
                    Description = "Filter multilingual dynamic properties to return only values of specified language (\"en-US\")"
                }),
                context => context.Source.Member.LoadMemberDynamicPropertyValues(
                    dynamicPropertySearchService,
                    context.GetArgumentOrValue<string>("cultureName")
                    )
                );
            Field("phones", x => x.Member.Phones);
        }
    }
}
