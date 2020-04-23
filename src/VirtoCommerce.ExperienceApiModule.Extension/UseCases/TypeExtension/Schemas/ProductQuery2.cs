using System;
using System.Collections.Generic;
using System.Text;
using GraphQL.Types;
using VirtoCommerce.ExperienceApiModule.GraphQLEx;

namespace VirtoCommerce.ExperienceApiModule.Extension.GraphQL.Schemas
{
    public class ProductQuery2 : ISchemaBuilder
    {
    
        public void Build(ISchema schema)
        {
            //Add new arguments
            //var productsField = schema.Query.GetField("products");
            //productsField.Arguments.Add(new QueryArgument(typeof(StringGraphType)) { Name = "currency" });

        }

     
    }
}
