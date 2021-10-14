using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas.ScalarTypes
{
    public class OptionalStringGraphType : StringGraphType
    {
        public OptionalStringGraphType()
        {
            Name = "OptionalString";
        }

        public override object ParseValue(object value)
        {
            var parsedValue = base.ParseValue(value);

            var result = new Optional<string>(parsedValue);

            return result;
        }
    }
}
