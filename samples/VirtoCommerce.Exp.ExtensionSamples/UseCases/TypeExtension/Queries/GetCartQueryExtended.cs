using System.Collections.Generic;
using GraphQL;
using GraphQL.Types;
using VirtoCommerce.XPurchase.Queries;

namespace VirtoCommerce.Exp.ExtensionSamples.UseCases.TypeExtension.Queries
{
    public class GetCartQueryExtended : GetCartQuery
    {
        //define custom properties here
        public string ContractId { get; set; }

        public override IEnumerable<QueryArgument> GetArguments()
        {
            foreach (var argument in base.GetArguments())
            {
                yield return argument;
            }

            yield return Argument<StringGraphType>(nameof(ContractId));
        }

        public override void Map(IResolveFieldContext context)
        {
            base.Map(context);

            ContractId = context.GetArgument<string>(nameof(ContractId));
        }
    }
}
