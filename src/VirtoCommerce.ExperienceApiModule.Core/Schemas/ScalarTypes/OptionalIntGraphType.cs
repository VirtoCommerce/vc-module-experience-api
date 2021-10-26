using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas.ScalarTypes
{
    public sealed class OptionalIntGraphType : IntGraphType
    {
        public OptionalIntGraphType()
        {
            Name = "OptionalInt";
        }

        public override object ParseValue(object value)
        {
            var parsedValue = base.ParseValue(value);

            var result = new Optional<int>(parsedValue);

            return result;
        }
    }
}
