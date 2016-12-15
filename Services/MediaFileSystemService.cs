using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using Amba.ImagePowerTools.Models;
using Amba.ImagePowerTools.ViewModels;
using ExifLib;
using Orchard;
using Orchard.Environment.Configuration;
using Orchard.Logging;
using Amba.ImagePowerTools.Extensions;
using FileViewModel = Amba.ImagePowerTools.ViewModels.FileViewModel;

namespace Amba.ImagePowerTools.Services
{
    public interface IMediaFileSystemService : IDependency
    {
        IEnumerable<FileViewModel> FindFiles(string mediaPath, string pattern);
        
        string GetServerPath(string path);
        bool SaveFile(HttpPostedFileBase file, string folder);
        string GetContentItemUploadFolder(int id, string fieldName);
        void DeleteNotUsedFiles(string folder, IEnumerable<SelectedImage> usedImages);

        IEnumerable<FolderViewModel> GetMediaFolders(string siteFolderPath);

        IEnumerable<FileViewModel> GetMediaFiles(string siteFolderPath);

        bool IsFolderExists(string siteFolder);

    }

    public class MediaFileSystemService : IMediaFileSystemService
    {
        private readonly IOrchardServices _services;

        private IImageResizerService _imageResizerService;
        private IImageResizerService ResizerService
        {
            get
            {
                if (_imageResizerService == null)
                    _imageResizerService = _services.WorkContext.Resolve<IImageResizerService>();
                return _imageResizerService;
            }
        }

        private ISwfService _swfService;
        private ISwfService SwfService
        {
            get
            {
                if (_swfService == null)
                    _swfService = _services.WorkContext.Resolve<ISwfService>();
                return _swfService;
            }
        }

        //ex: /Media/Default
        private readonly string _mediaFolderRoot;
        //ex: c:\orchard\src\orchard.web\Media\Default
        private readonly string _mediaServerPath;
        //ex :\orchard\src\orchard.web\ 
        private readonly string _siteRootPath;
        

        private readonly ShellSettings _shellSettings;
        
        public ILogger Logger { get; set; }

        public MediaFileSystemService(ShellSettings shellSettings, IOrchardServices services)
        {
            _shellSettings = shellSettings;
            _services = services;
            var mediaPath = GetServerPath("Media");
            _mediaServerPath = Path.Combine(mediaPath, _shellSettings.Name);
            _mediaFolderRoot = "/Media/" + _shellSettings.Name;
            _siteRootPath = GetServerPath("/");
            Logger = NullLogger.Instance;
        }

        public void DeleteNotUsedFiles(string folder, IEnumerable<SelectedImage> usedImages)
        {
            var files = GetFolderFiles(folder);
            foreach (var filePath in files)
            {
                try
                {
                    if (!usedImages.Any(x => x.FilePath == filePath))
                    {
                        var serverPath = GetServerPath(filePath);
                        File.Delete(serverPath);
                    }
                }
                catch(Exception e)
                {
                    Logger.Error("DeleteNotUsedFiles: cannot process file " + filePath, e);
                }
            }
        }

        private IEnumerable<string> GetFolderFiles(string uploadFolder)
        {
            var serverPath = GetServerPath(uploadFolder);
            if (!Directory.Exists(serverPath))
                return new List<string>();
            var mediaServerLength = GetServerPath("").Length;
            return Directory.GetFiles(serverPath)
                .Select(x => x.Substring(mediaServerLength - 1).Replace('\\', '/'))
                .ToList();
        }

        public string GetServerPath(string path)
        {
            path = path.RegexRemove(@"^~/").TrimStart('/');
            return HostingEnvironment.IsHosted
                                ? HostingEnvironment.MapPath("~/" + path) ?? ""
                                : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
        }

        public bool SaveFile(HttpPostedFileBase file, string folder)
        {
            try
            {
                var fileName = Path.GetFileName(file.FileName);
                var serverFolder = GetServerPath(folder);
                if (!Directory.Exists(serverFolder))
                {
                    Directory.CreateDirectory(serverFolder);
                }
                var path = Path.Combine(serverFolder, fileName);
                file.SaveAs(path);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public string GetContentItemUploadFolder(int id, string fieldName)
        {
            return _mediaFolderRoot + Consts.ContentItemUploadFolderPrefix + id + "_" + fieldName;
        }

        public IEnumerable<FolderViewModel> GetMediaFolders(string siteFolderPath)
        {
            var folderPath = GetFolderServerPath(siteFolderPath);
            var directoryInfo = new DirectoryInfo(folderPath);
            return 
                directoryInfo.GetDirectories()
                .Select(x => CreateFolderViewModel(x))
                .OrderBy(x => x.Name)
                .ToList();
        }

        private string GetFolderServerPath(string siteFolderPath)
        {
            if (string.IsNullOrWhiteSpace(siteFolderPath))
            {
                siteFolderPath = _mediaFolderRoot;
            }
            var serverPath = GetServerPath(siteFolderPath);
            if (!Directory.Exists(serverPath))
            {
                serverPath = _mediaServerPath;
            }
            return serverPath;
        }

        private FolderViewModel CreateFolderViewModel(DirectoryInfo folderInfo)
        {
            var result = new FolderViewModel();
            result.Name = folderInfo.Name;
            result.SitePath = GetSitePath(folderInfo.FullName);
            return result;
        }

        public IEnumerable<FileViewModel> GetMediaFiles(string siteFolderPath)
        {
            var searchFolder = GetFolderServerPath(siteFolderPath);
            var files = Directory.GetFiles(searchFolder, "*.*", SearchOption.TopDirectoryOnly)
                         .Select(x => CreateFileViewModel(x))
                         .OrderBy(x => x.FileName)
                         .ToList();
            files.AsParallel()
                 .WithDegreeOfParallelism(Environment.ProcessorCount*2)
                 .ForEach(x => ExtendFileViewModel(x));

            return files;
        }

        public bool IsFolderExists(string siteFolder)
        {
            if (string.IsNullOrWhiteSpace(siteFolder))
                return true;
            var serverPath = GetServerPath(siteFolder);
            return Directory.Exists(serverPath);
        }

        public IEnumerable<FileViewModel> FindFiles(string mediaPath, string pattern)
        {
            var searchFolder = GetFolderServerPath(mediaPath);
            var files = Directory.GetFiles(searchFolder, pattern, SearchOption.AllDirectories)
                .Take(100)
                .Select(x => CreateFileViewModel(x))
                .OrderBy(x => x.FileName)
                .ToList();
            files.AsParallel()
                 .WithDegreeOfParallelism(Environment.ProcessorCount*2)
                 .ForEach(x => ExtendFileViewModel(x));
            return files;
        }

        private string GetSitePath(string serverFilePath)
        {
            var path = serverFilePath.Substring(_siteRootPath.Length - 1);
            return "/" + path.Replace('\\', '/').Trim('/');
        }

        private string GetSiteFolder(string sitePath, string fileName)
        {
            return sitePath.Substring(0, sitePath.Length - fileName.Length);
        }

        private string GetDisplayPath(string sitePath)
        {
            return sitePath.Substring(_mediaFolderRoot.Length);
        }

        private FileViewModel CreateFileViewModel(string serverFilePath)
        {
            var fileInfo = new FileInfo(serverFilePath);
            var result = new FileViewModel
            {
                FileName = fileInfo.Name,
                Extension = fileInfo.Extension.Trim('.').ToLower(),
                Size = fileInfo.Length,
                ServerPath = serverFilePath,
                SitePath = GetSitePath(serverFilePath),
            };
            result.SiteFolder = GetSiteFolder(result.SitePath, fileInfo.Name);
            result.DisplayPath = GetDisplayPath(result.SitePath);
            return result;
        }

        private FileViewModel ExtendFileViewModel(FileViewModel file)
        {
            file.IsImage = ResizerService.IsImageExtension(file.Extension);
            try
            {
                SetFileSizes(file);
                SetLonLat(file);
            }
            catch
            {
            }
            return file;
        }

        private void SetLonLat(FileViewModel file)
        {
            var fileInfo = new FileInfo(file.ServerPath);
            if (fileInfo.Extension.ToLower() == ".jpg")
            {
                var exifReader = new ExifReader(file.ServerPath);
                file.Lat = exifReader.GetLat();
                file.Lon = exifReader.GetLon();
            }
        }

        private void SetFileSizes(FileViewModel file)
        {
            if (file.IsImage)
            {
                var size = ImageHeader.GetDimensions(file.ServerPath);
                file.Width = size.Width;
                file.Height = size.Height;
            }
            else if (file.Extension == "swf")
            {
                int width, height;
                SwfService.GetSwfFileDimensions(file.ServerPath, out width, out height);
                file.Width = width;
                file.Height = height;
            }
        } 

        /*
        [HttpPost]
        public JsonResult CreateFolder(string path, string folderName)
        {
            if (!Services.Authorizer.Authorize(Permissions.ManageMedia))
            {
                return Json(new {Success = false, Message = T("Couldn't create media folder").ToString()});
            }

            try
            {
                _mediaService.CreateFolder(HttpUtility.UrlDecode(path), folderName);
                return Json(new {Success = true, Message = ""});
            }
            catch (Exception exception)
            {
                return
                    Json(new {Success = false, Message = T("Creating Folder failed: {0}", exception.Message).ToString()});
            }
        }
         */ 
    }
}