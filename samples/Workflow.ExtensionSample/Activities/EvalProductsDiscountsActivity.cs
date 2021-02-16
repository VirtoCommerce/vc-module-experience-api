using System;
using System.Threading;
using System.Threading.Tasks;
using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Extensions;
using Elsa.Results;
using Elsa.Services;
using Elsa.Services.Models;
using WorkflowExtension.Model;
using WorkflowExtension.Queries;

namespace WorkflowExtension.Activities
{
    [ActivityDefinition(Category = "VirtoCommerce", Description = "Evaluated discounts for given products")]
    public class EvalProductsDiscountsActivity : Activity
    {
        private readonly IWorkflowExpressionEvaluator _expressionEvaluator;
        public EvalProductsDiscountsActivity(IWorkflowExpressionEvaluator expressionEvaluator)
        {
            _expressionEvaluator = expressionEvaluator;
        }
        [ActivityProperty(Hint = "The request")]
        public WorkflowExpression<GeMyCoolProductsQuery> Request
        {
            get => GetState(() => new WorkflowExpression<GeMyCoolProductsQuery>(LiteralEvaluator.SyntaxName, ""));
            set => SetState(value);
        }
        [ActivityProperty(Hint = "The response")]
        public WorkflowExpression<GetMyCoolProductsResponse> Response
        {
            get => GetState(() => new WorkflowExpression<GetMyCoolProductsResponse>(LiteralEvaluator.SyntaxName, ""));
            set => SetState(value);
        }

        protected override async Task<ActivityExecutionResult> OnExecuteAsync(WorkflowExecutionContext context, CancellationToken cancellationToken)
        {
            var response = await _expressionEvaluator.EvaluateAsync(Response, context, cancellationToken);
            foreach (var product in response?.Products ?? Array.Empty<MyCoolProduct>())
            {
                product.Discount = 10.99m;
            }
            return Done();
        }
    }
}
