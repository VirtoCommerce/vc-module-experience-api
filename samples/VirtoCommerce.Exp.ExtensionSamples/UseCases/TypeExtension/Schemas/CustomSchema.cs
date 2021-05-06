using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Builders;
using GraphQL.Resolvers;
using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.Core.Infrastructure;

namespace VirtoCommerce.Exp.ExtensionSamples
{
    /// <example>
    /// This is an example JSON request for a mutation
    /// {
    ///   "query": "mutation ($inventory:InputUpdateInventoryType!){ saveInventory(inventory: $inventory) { productId inStockQuantity } }",
    ///   "variables": {
    ///     "inventory": {
    ///       "productId": "my-cool-product"
    ///       "inStockQuantity": 22
    ///     }
    ///   }
    /// }
    /// </example>
    public class CustomSchema : ISchemaBuilder
    {
        public void Build(ISchema schema)
        {
            var inventoryQueryField = new FieldType
            {
                Name = "inventory",
                Arguments = new QueryArguments(new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id", Description = "id of the inventory" }),
                Type = typeof(InventoryType),
                Resolver = new AsyncFieldResolver<Inventory>(context =>
                {
                    return Task.FromResult(new Inventory { ProductId = "1", FulfillmentCenterId = "center1" });
                })
            };

            schema.Query.AddField(inventoryQueryField);

            var saveInventoryField = FieldBuilder.Create<Inventory, Inventory>(typeof(InventoryType))
                                                  .Name("saveInventory")
                                                  .Argument<NonNullGraphType<InputUpdateInventoryType>>("inventory")
                                                  .Resolve(context =>
            {
                var inventory = context.GetArgument<Inventory>("inventory");
                // Insert mutation logic here
                return inventory;
            }).FieldType;

            schema.Mutation.AddField(saveInventoryField);
        }
    }
}
