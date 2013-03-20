﻿using System.Linq;
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
        public static IHtmlString ToAbsoluteUrl(this UrlHelper url, string relativeUrl)
        {
            return new HtmlString(relativeUrl.ToAbsoluteUrl());
        }


        public static HtmlString ResizedImage(
            this HtmlHelper helper, 
            //image path to reseize
            string path, 
            int width = 0, 
            int height = 0,
            string defaultImage = "/modules/Amba.ImagePowerTools/content/image_not_found.jpg",
            string settings = "",
            bool renderImgSizeAttributes = true,            
            object htmlAttributes = null,
            bool isAbosuleImageUrl = false)
        {
            path = ResizedImageUrl(helper, path, width, height, defaultImage: defaultImage, settings: settings, isAbosuleImageUrl: isAbosuleImageUrl).ToString();
            if (string.IsNullOrWhiteSpace(path))
            {
                return new HtmlString(string.Empty);
            }
            var sb = new StringBuilder();
            if (renderImgSizeAttributes)
            {
                if (width > 0)
                    sb.AppendFormat("width:{0}px; ", width);
                if (height > 0)
                    sb.AppendFormat("height:{0}px; ", height);
            }
            var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

            var attrBuilder = new StringBuilder();
            attributes.IfNotNull(x => x.ToList().ForEach(
                i => attrBuilder.AppendFormat(@" {0}=""{1}""", i.Key, i.Value.ToString().Replace("\"", "&quot;")
            )));
            return new HtmlString(string.Format(@"<img src=""{0}"" style=""{1}"" {2}/>", path, sb, attrBuilder));
        }        

        public static IHtmlString ResizedImageUrl(
            this HtmlHelper helper, string path, int width = 0, int height = 0,
            string settings = "",
            string defaultImage = "/modules/Amba.ImagePowerTools/content/image_not_found.jpg",
            bool isAbosuleImageUrl = false)
        {
            var workContext = helper.ViewContext.RequestContext.GetWorkContext();
            //var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);
            if (string.IsNullOrWhiteSpace(path))
            {
                return new HtmlString(string.Empty);
            }
            if (isAbosuleImageUrl)
            {
                path = path.TrimStart('/').Substring("/".ToAbsoluteUrl().TrimStart('/').Length);
            }
            //path = urlHelper.Content(path);
            var resizeService = workContext.Resolve<IImageResizerService>();
            path = resizeService.ResizeImage(path, width, height, settings: settings);
            if (string.IsNullOrWhiteSpace(path) && !string.IsNullOrWhiteSpace(defaultImage))
            {
                path = resizeService.ResizeImage(defaultImage, width, height, settings: settings);
            }
            return new HtmlString(path.ToAbsoluteUrl());
        }
    }
}