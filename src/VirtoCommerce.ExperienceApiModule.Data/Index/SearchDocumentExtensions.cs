using Newtonsoft.Json.Linq;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.SearchModule.Core.Model;

namespace VirtoCommerce.ExperienceApiModule.Data.Index
{
    public static class SearchDocumentExtensions
    {
        public static T Materialize<T>(this SearchDocument document, string objFieldName = "__object") where T : class
        {
            T result = null;

            if (document.ContainsKey(objFieldName))
            {
                var obj = document[objFieldName];
                var objType = AbstractTypeFactory<T>.TryCreateInstance().GetType();
                result = obj as T;
                if (result == null && obj is JObject jobj)
                {
                    result = jobj.ToObject(objType) as T;
                }
            }
            return result;
        }
    }
}
