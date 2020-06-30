namespace VirtoCommerce.XPurchase.Schemas
{
    public class InputAddOrUpdateCartShipmentType : InputCartBaseType
    {
        public InputAddOrUpdateCartShipmentType()
        {
            Field<InputShipmentType>("shipment");
        }
    }
}
