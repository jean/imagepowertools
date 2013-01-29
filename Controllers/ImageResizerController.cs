using System;
using System.Linq;
using System.Web.Mvc;
using Amba.ImagePowerTools.Services;
using ImageResizer.Configuration;
using Orchard.Themes;
using Orchard.UI.Admin;

namespace Amba.ImagePowerTools.Controllers
{
    [HandleError]
    [Themed(false)]
    public class ImageResizerController : Controller
    {
        private readonly IImageResizerService _imageResizerService;
        private readonly IPowerToolsSettingsService _settingsService; 

        public ImageResizerController(
            IImageResizerService imageResizerService,
            IPowerToolsSettingsService settingsService)
        {
            _imageResizerService = imageResizerService;
            _settingsService = settingsService;
        }

        private string ModuleContentFolder = @"/modules/Amba.ImagePowerTools/content/";

        public ActionResult Test()
        {
            return Content(Config.Current.GetDiagnosticsPage());
        }

        public ActionResult ResizedImage(
            string url, 
            string defaultImage = "/modules/Amba.ImagePowerTools/content/image_not_found.jpg")
        {
            if (!_settingsService.Settings.EnableFrontendResizeAction && !User.Identity.IsAuthenticated)
                return HttpNotFound();
            if (string.IsNullOrWhiteSpace(url))
                return HttpNotFound();

            string ext = _imageResizerService.GetCleanFileExtension(url);
            if (!_imageResizerService.SupportedFileExtensions().Contains(ext))
            {
                var alternativeUrl = ModuleContentFolder + ext + ".png";
                if (System.IO.File.Exists(Server.MapPath(alternativeUrl)))
                {
                    url = alternativeUrl;
                }
                else
                {
                    throw new ArgumentException("Invalid file extension! suported file extensions are: " +
                                                string.Join(",", _imageResizerService.SupportedFileExtensions()));
                }
            }

            var retValImageUrl = _imageResizerService.ResizeImage(url, Request.Url.Query);
            if (string.IsNullOrWhiteSpace(retValImageUrl))
            {
                retValImageUrl = _imageResizerService.ResizeImage(defaultImage, Request.Url.Query);  
            }
            if (string.IsNullOrWhiteSpace(retValImageUrl))
            {
                return HttpNotFound();
            }
            return Redirect(retValImageUrl);
        }
    }
}