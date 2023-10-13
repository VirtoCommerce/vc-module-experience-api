using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.ExperienceApiModule.Core.Schemas
{
    public class AuditableEntityType<T>: EntityType<T>
        where T: AuditableEntity
    {
        public AuditableEntityType()
        {
            Field(x => x.CreatedBy, nullable: false).Description("The ID of user who created the object.");
            Field(x => x.CreatedDate, nullable: false).Description("The date when the object was created.");
            Field(x => x.ModifiedBy, nullable: true).Description("The Id of user who last modified the object.");
            Field(x => x.ModifiedDate, nullable: true).Description("The date when the object was last modified.");
        }   
    }
}
