namespace WorkflowExtension.Model
{
    public class MyCoolProduct
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal? Price { get; set; }
        public decimal? Discount { get; set; }
        public int? InStockQty { get; set; }

        public virtual void MergefromOther(MyCoolProduct other)
        {
            Id = other.Id ?? Id;
            Name = other.Name ?? Name;
            Price = other.Price ?? Price;
            Discount = other.Discount ?? Discount;
            InStockQty = other.InStockQty ?? InStockQty;
        }
    }
}
