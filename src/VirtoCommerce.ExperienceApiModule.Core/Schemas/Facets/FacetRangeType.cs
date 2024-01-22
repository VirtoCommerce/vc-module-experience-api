using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Models.Facets;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas.Facets
{
    public class FacetRangeType : ObjectGraphType<FacetRange>
    {
        public FacetRangeType()
        {
            Field(d => d.Count, nullable: false).Description("Amount of products for which the values in a field fall into the specified range");
            Field(d => d.From, nullable: false).Description("The range’s lower endpoint in number format, 0 represents infinity");
            Field(d => d.IncludeFrom, nullable: false).Description("The flag indicates that From exclusive");
            Field(d => d.FromStr, nullable: true).Description("The range’s lower endpoint in string format, empty string represents infinity");
            Field(d => d.Max, nullable: false).Description("Maximum value among all values contained within the range");
            Field(d => d.Min, nullable: false).Description("Minimum value among all values contained within the range");
            Field(d => d.To, nullable: false).Description("The range’s upper endpoint in number format, 0 represents infinity");
            Field(d => d.IncludeTo, nullable: false).Description("The flag indicates that To exclusive");
            Field(d => d.ToStr, nullable: true).Description("The range’s upper endpoint in string format, empty string represents infinity");
            Field(d => d.Total, nullable: false).Description("Sum of all values contained in the range");
            Field(d => d.Label, nullable: false).Description("Localization label");
            Field(d => d.IsSelected, nullable: false).Description("is selected state");
        }
    }
}
