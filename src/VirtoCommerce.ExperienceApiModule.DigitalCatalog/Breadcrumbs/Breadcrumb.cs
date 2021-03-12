namespace VirtoCommerce.XDigitalCatalog.Breadcrumbs
{
    public class Breadcrumb
    {
        public Breadcrumb(string type)
        {
            TypeName = type;
        }
        public string ItemId { get; set; }
        public string TypeName { get; private set; }
        public virtual string Title { get; set; }
        public virtual string SeoPath { get; set; }
    }
}
