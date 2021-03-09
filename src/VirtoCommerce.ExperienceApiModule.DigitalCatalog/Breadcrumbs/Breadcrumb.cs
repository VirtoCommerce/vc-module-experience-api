namespace VirtoCommerce.XDigitalCatalog.Breadcrumbs
{
    public abstract class Breadcrumb
    {
        protected Breadcrumb(string type)
        {
            TypeName = type;
        }
        public string TypeName { get; private set; }
        public virtual string Title { get; set; }
        public virtual string SeoPath { get; set; }
    }
}
