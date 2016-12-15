using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Amba.ImagePowerTools.Services;
using Amba.ImagePowerTools.ViewModels.Admin;
using Amba.ImagePowerTools.ViewModels.Multipicker;
using Orchard;
using Orchard.Environment.Configuration;
using Orchard.Themes;
using Orchard.UI.Admin;

namespace Amba.ImagePowerTools.Controllers
{
    [Themed(false)]
    [Admin]
    public class MultipickerController : Controller
    {
        private readonly IMediaFileSystemService _mediaFileSystemService;
        private readonly ShellSettings _shellSettings;

        public IOrchardServices Services { get; set; }

        public MultipickerController(
            IMediaFileSystemService mediaFileSystemService,
            IOrchardServices services, ShellSettings shellSettings)
        {
            Services = services;
            _shellSettings = shellSettings;
            _mediaFileSystemService = mediaFileSystemService;
        }

        const string MediaPathCookieKey = "AmbaImageMuliPicker.MediaPath";
        private string LastSavedMediaPath
        {
            get
            {
                string mediaPath;
                var httpCookie = Request.Cookies[MediaPathCookieKey];
                if (httpCookie != null)
                {
                    mediaPath = httpCookie.Value;
                    if (!_mediaFileSystemService.IsFolderExists(mediaPath))
                    {
                        mediaPath = string.Empty;
                    }
                }
                else
                {
                    mediaPath = string.Empty;
                }
                return mediaPath;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    Response.Cookies.Add(new HttpCookie(MediaPathCookieKey, value) { Expires = DateTime.Now.AddMonths(1) });
                }
            }
        }

        public ActionResult Search(string mediaPath, string scope, string search = "")
        {
            mediaPath = mediaPath.Trim();
            if (!_mediaFileSystemService.IsFolderExists(mediaPath))
            {
                var emptyModel = new MediaFolderEditViewModel
                    {
                        IsFolderNotExists = true
                    };
                return View("Index", emptyModel);
            }

            var searchFilter = ("*" + search + "*").Replace("**", "*");
            var files = _mediaFileSystemService.FindFiles(mediaPath, searchFilter);

            var model = new MediaFolderEditViewModel
            {
                Files = files,
                MediaPath = mediaPath,
                Scope = scope,
                BreadCrumbs = CreateBreadCrumbs(mediaPath),
                SearchFilter = search
            };
            return View("Index", model);
        }

        public ActionResult Index(string mediaPath, string scope)
        {
            if (mediaPath == ":last")
            {
                mediaPath = LastSavedMediaPath;
            }
            if (string.IsNullOrWhiteSpace(mediaPath))
                mediaPath = string.Empty;
            mediaPath = mediaPath.Trim();

            if (!_mediaFileSystemService.IsFolderExists(mediaPath))
            {
                var emptyModel = new MediaFolderEditViewModel
                    {
                        IsFolderNotExists = true
                    };
                return View(emptyModel);
            }

            LastSavedMediaPath = mediaPath;

            var mediaFolders = _mediaFileSystemService.GetMediaFolders(mediaPath);
            var mediaFiles = _mediaFileSystemService.GetMediaFiles(mediaPath);
            var model = new MediaFolderEditViewModel
                {
                    Files = mediaFiles, 
                    Folders = mediaFolders, 
                    MediaPath = mediaPath,
                    Scope = scope,
                    BreadCrumbs = CreateBreadCrumbs(mediaPath)
                };
            return View(model);
        }

        private IEnumerable<BreadcrumbViewModel> CreateBreadCrumbs(string mediaPath)
        {
            var paths = mediaPath.Split('/', '\\');
            var result = new List<BreadcrumbViewModel>();
            for (int i = 0; i < paths.Count(); i++)
            {
                string path = "";
                for (int j = 0; j <= i; j++)
                {
                    path += (j != 0 ? "/" : "") + paths[j];
                }
                if (paths[i] == "Media" || paths[i] == _shellSettings.Name || string.IsNullOrWhiteSpace(paths[i]))
                    continue;
                result.Add(new BreadcrumbViewModel {FolderName = paths[i], MediaPath = path});
            }
            return result;
        }
    }
}