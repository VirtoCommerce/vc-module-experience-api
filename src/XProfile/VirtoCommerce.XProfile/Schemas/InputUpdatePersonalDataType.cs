using GraphQL.Types;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    public class InputUpdatePersonalDataType : InputObjectGraphType
    {
        public InputUpdatePersonalDataType()
        {
            Field<NonNullGraphType<InputPersonalDataType>>("PersonalData");
        }
    }
}
