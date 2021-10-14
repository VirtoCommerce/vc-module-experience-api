using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas.ScalarTypes
{
    public class OptionalNullableDecimalGraphType : DecimalGraphType
    {
        public OptionalNullableDecimalGraphType()
        {
            Name = "OptionalNullableDecimal";
        }

        public override object ParseValue(object value)
        {
            var parsedValue = base.ParseValue(value);

            var result = new Optional<decimal?>(parsedValue);

            return result;
        }
    }
}
