//using System.Collections.Generic;
//using GraphQL;
//using GraphQL.Types;
//using VirtoCommerce.XDigitalCatalog.Queries;

//namespace VirtoCommerce.Exp.ExtensionSamples.UseCases.TypeExtension.Queries
//{
//    public class SearchProductQueryExtended : SearchProductQuery
//    {
//        public string NewProperty { get; set; }

//        public override IEnumerable<QueryArgument> GetArguments()
//        {
//            foreach (var argument in base.GetArguments())
//            {
//                yield return argument;
//            }

//            yield return Argument<StringGraphType>(nameof(NewProperty));
//        }

//        public override void Map(IResolveFieldContext context)
//        {
//            base.Map(context);

//            NewProperty = context.GetArgument<string>(nameof(NewProperty));
//        }
//    }
//}

