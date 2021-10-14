using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas.ScalarTypes
{
    public class OptionalDecimalGraphType : DecimalGraphType
    {
        public OptionalDecimalGraphType()
        {
            Name = "OptionalDecimal";
        }

        public override object ParseValue(object value)
        {
            var parsedValue = base.ParseValue(value);

            var result = new Optional<decimal>(parsedValue);

            return result;
        }
    }
}
