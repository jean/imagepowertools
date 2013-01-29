using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using Amba.ImagePowerTools.Extensions;
using Amba.ImagePowerTools.Services;
using Orchard;

namespace Amba.ImagePowerTools.HtmlHelpers
{
    public static class HtmlImageResizeHelper
    {
        public static HtmlString ResizedImage(
            this HtmlHelper helper, string url, 
            int width = 0, int height = 0,
            string defaultImage = "/modules/Amba.ImagePowerTools/content/image_not_found.jpg",
            object htmlAttributes = null)
        {
            url = ResizedImageUrl(helper, url, width, height);
            if (string.IsNullOrWhiteSpace(url) && !string.IsNullOrWhiteSpace(defaultImage))
            {
                url = ResizedImageUrl(helper, defaultImage, width, height);
            }
            var sb = new StringBuilder();
            if (width > 0)
                sb.AppendFormat("width:{0}px;", width);
            if (height > 0)
                sb.AppendFormat("height:{0}px;", height);

            var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

            var attrBuilder = new StringBuilder();
            attributes.IfNotNull(x => x.ToList().ForEach(
                i => attrBuilder.AppendFormat(@" {0}=""{1}""", i.Key, i.Value.ToString().Replace("\"", "&quot;")
            )));
            
            return new HtmlString(string.Format(@"<img src=""{0}"" style=""{1}"" {2}/>", url, sb, attrBuilder));
        }

        public static string ResizedImageUrl(this HtmlHelper helper, string url, int width = 0, int height = 0)
        {
            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);
            if (string.IsNullOrWhiteSpace(url))
            {
                return string.Empty;
            }
            url = urlHelper.Content(url);
            var resizeService = new ImageResizerService(null);
            url = resizeService.ResizeImage(url, width, height);
            return url;
        }

        public static string ResizedImageUrl(this HtmlHelper helper, string url, string settings)
        {
            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);
            if (string.IsNullOrWhiteSpace(url))
            {
                return string.Empty;
            }
            url = urlHelper.Content(url);
            var resizeService = new ImageResizerService(null);
            url = resizeService.ResizeImage(url, settings);
            return url;
        }
    }
}