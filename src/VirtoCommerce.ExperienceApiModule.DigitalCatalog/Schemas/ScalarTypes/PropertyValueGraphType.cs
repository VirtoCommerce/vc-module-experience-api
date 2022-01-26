using GraphQL.Language.AST;
using VirtoCommerce.ExperienceApiModule.Core.Schemas.ScalarTypes;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.XDigitalCatalog.Schemas.ScalarTypes
{
    public class PropertyValueGraphType: AnyValueGraphType
    {
        public override object ParseLiteral(IValue value)
        {
            if (value is StringValue stringValue)
            {
                return GeoPoint.TryParse(stringValue.Value) ?? base.ParseLiteral(value);
            }
            return base.ParseLiteral(value);
        }

        public override object ParseValue(object value)
        {
            return value is GeoPoint ? value.ToString() : base.ParseValue(value);
        }
    }
}
