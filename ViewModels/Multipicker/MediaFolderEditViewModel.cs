using System.Collections.Generic;
using System.Web;
using Amba.ImagePowerTools.ViewModels.Admin;
using Amba.ImagePowerTools.Extensions;

namespace Amba.ImagePowerTools.ViewModels.Multipicker
{
    public class MediaFolderEditViewModel
    {
        public string SearchFilter { get; set; }
        public string FolderName { get; set; }
        public string MediaPath { get; set; }
        public IEnumerable<FolderViewModel> Folders { get; set; }
        public IEnumerable<FileViewModel> Files { get; set; }
        public string PublicPath { get; set; }
        public IEnumerable<BreadcrumbViewModel> BreadCrumbs { get; set; }
        public string Scope { get; set; }
        public bool IsFolderNotExists { get; set; }

        public MediaFolderEditViewModel()
        {
            Folders = new List<FolderViewModel>();
            Files = new List<FileViewModel>();
        }

        public string GetPickerUrl(string folderMediaPath)
        {
            var result = string.Format(
                "/Amba.ImagePowerTools/Multipicker/Index".ToAbsoluteUrl() +  "?scope={0}&mediaPath={1}",
                Scope,
                HttpUtility.HtmlEncode(folderMediaPath));
            return result;
        }

        
    }
}