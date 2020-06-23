namespace VirtoCommerce.ExperienceApiModule.XProfile.Schemas
{
    /*
    public class OrganizationMutation : ISchemaBuilder
    {
        public void Build(ISchema schema)
        {

            var saveInventoryField = FieldBuilder.Create<Inventory, Inventory>(typeof(InventoryType))
                                                      .Name("updateOrganization")
                                                      .Argument<NonNullGraphType<InputUpdateInventoryType>>("inventory")
                                                      .Resolve(context =>
                                                      {
                                                          var inventory = context.GetArgument<Inventory>("inventory");
                                                          //TODO: Insert mutation logic here
                                                          return inventory;
                                                      }).FieldType;
            schema.Mutation.AddField(saveInventoryField);
        }
    }
    */
}
