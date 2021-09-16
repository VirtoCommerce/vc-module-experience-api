using System.Text;

namespace VirtoCommerce.ExperienceApiModule.XProfile.Extensions
{
    public static class UrlExtensions
    {
        public static string TrimLastSlash(this string url)
        {
            var result = url.EndsWith("/") ? url[..^1] : url;

            return result;
        }

        /// <summary>
        /// Normalize values like "/reset/" and "reset"
        /// to "/reset"
        /// </summary>
        /// <param name="urlSuffix"></param>
        /// <returns></returns>
        public static string NormalizeUrlSuffix(this string urlSuffix)
        {
            var result = new StringBuilder(urlSuffix);

            if (!string.IsNullOrEmpty(urlSuffix))
            {
                if (!urlSuffix.StartsWith("/"))
                {
                    result.Insert(0, "/");
                }

                if (urlSuffix.EndsWith("/"))
                {
                    result.Remove(result.Length - 1, 1);
                }
            }

            return result.ToString();
        }
    }
}
