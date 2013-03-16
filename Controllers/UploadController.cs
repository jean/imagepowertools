using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Amba.ImagePowerTools.Services;
using Orchard.Themes;
using Amba.ImagePowerTools.Extensions;

namespace Amba.ImagePowerTools.Controllers
{
    [Themed(false)]
    public class UploadController : Controller
    {

        private readonly IMediaFileSystemService _mediaFileSystemService;

        public UploadController(IMediaFileSystemService mediaFileSystemService)
        {
            _mediaFileSystemService = mediaFileSystemService;
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file, string folder = "")
        {
            if (file == null || file.ContentLength == 0)
                return Content("No file in request");

            if (!folder.ToLower().StartsWith(_mediaFileSystemService.GetMediaFolderBase().ToLower()))
            {
                return Content("Forbidden");
            }
            var isSavingSuccess = 
                Task.Factory.StartNew(() => _mediaFileSystemService.SaveFile(file, folder)).Result;
            
            if (!isSavingSuccess)
            {
                return Content("Fail");
            }
            // redirect back to the index action to show the form once again
            return Content("Ok");
        }
    }
}