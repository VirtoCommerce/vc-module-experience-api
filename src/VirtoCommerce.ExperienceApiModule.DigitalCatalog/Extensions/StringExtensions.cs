namespace VirtoCommerce.XDigitalCatalog.Extensions
{
    public static class StringExtensions
    {
        public static int TryParse(this string input, int defaultValue)
        {
            var result = defaultValue;

            if (!string.IsNullOrEmpty(input) && int.TryParse(input, out var parseResult))
            {
                result = parseResult;
            }

            return result;
        }
    }
}
