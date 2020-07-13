using GraphQL.Types;

namespace VirtoCommerce.XDigitalCatalog.Schemas
{
    public class FacetRangeType : ObjectGraphType<FacetRange>
    {
        public FacetRangeType()
        {
            Field(d => d.Count, nullable: true).Description("Amount of products for which the values in a field fall into the specified range");
            Field(d => d.From, nullable: true).Description("The range’s lower endpoint in number format, 0 represents infinity");
            Field(d => d.IncludeFrom).Description("The flag indicates that From exclusive");
            Field(d => d.FromStr, nullable: true).Description("The range’s lower endpoint in string format, empty string represents infinity");
            Field(d => d.Max, nullable: true).Description("Maximum value among all values contained within the range");
            Field(d => d.Min, nullable: true).Description("Minimum value among all values contained within the range");
            Field(d => d.To, nullable: true).Description("The range’s upper endpoint in number format, 0 represents infinity");
            Field(d => d.IncludeTo).Description("The flag indicates that To exclusive");
            Field(d => d.ToStr, nullable: true).Description("The range’s upper endpoint in string format, empty string represents infinity");
            Field(d => d.Total, nullable: true).Description("Sum of all values contained in the range");
        }
    }
}
