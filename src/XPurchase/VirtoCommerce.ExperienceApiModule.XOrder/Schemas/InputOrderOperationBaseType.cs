using GraphQL.Types;
using VirtoCommerce.OrdersModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.XOrder.Schemas
{
    public abstract class InputOrderOperationBaseType : InputObjectGraphType<OrderOperation>
    {
        protected InputOrderOperationBaseType()
        {
            Field(x => x.Id, true);
            Field(x => x.ObjectType, true);
            Field(x => x.CancelReason, true);
            Field(x => x.CancelledDate, true);
            Field(x => x.IsCancelled);
            Field(x => x.OuterId, true);
            Field(x => x.Sum);
            Field(x => x.Currency);
            Field(x => x.Comment, true);
            Field(x => x.Status, true);
            Field(x => x.IsApproved);
            Field(x => x.Number);
            Field(x => x.ParentOperationId, true);
            Field(x => x.OperationType);
            //public IEnumerable<IOperation> ChildrenOperations);
            //public ICollection<DynamicObjectProperty> DynamicProperties);
            //public ICollection<OperationLog> OperationsLog);
        }
    }
}
