using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using Orchard;
using Orchard.Environment.Configuration;
using Orchard.Media.Models;
using Orchard.Media.Services;
using Amba.ImagePowerTools.Extensions;

namespace Amba.ImagePowerTools.Services
{
    public interface IMediaFileSystemService : IDependency
    {
        IEnumerable<MediaFile> FindFiles(string mediaPath, string pattern);

        string GetMediaFolderBase();

        bool SaveFile(HttpPostedFileBase file, string folder);
    }

    public class MediaFileSystemService : IMediaFileSystemService
    {
        private readonly IMediaService _mediaService;
        private readonly ShellSettings _shellSettings;
        private readonly string _mediaServerPath;

        public MediaFileSystemService(IMediaService mediaService, ShellSettings shellSettings)
        {
            _mediaService = mediaService;
            _shellSettings = shellSettings;

            var mediaPath = HostingEnvironment.IsHosted
                                ? HostingEnvironment.MapPath("~/Media/") ?? ""
                                : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Media");

            _mediaServerPath = Path.Combine(mediaPath, _shellSettings.Name);
        }

        private string MapPath(string path)
        {
            path = path.RegexRemove(@"^~/").TrimStart('/');
            return HostingEnvironment.IsHosted
                                ? HostingEnvironment.MapPath("~/" + path) ?? ""
                                : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
        }

        public string GetMediaFolderBase()
        {
            return "/Media/" + _shellSettings.Name;
        }

        public bool SaveFile(HttpPostedFileBase file, string folder)
        {
            try
            {
                var fileName = Path.GetFileName(file.FileName);
                var serverFolder = MapPath(folder);
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

        public IEnumerable<MediaFile> FindFiles(string mediaPath, string pattern)
        {
            var searchFolder = Path.Combine(_mediaServerPath, mediaPath);
            var files = Directory.GetFiles(searchFolder, pattern, SearchOption.AllDirectories)
                .Take(100)
                .AsParallel()
                .Select(x => CreateMediaFile(x))
                .ToList();
            return files;
        }

        

        private MediaFile CreateMediaFile(string serverFilePath)
        {
            var fileInfo = new FileInfo(serverFilePath);
            var relativePath = serverFilePath.Substring(_mediaServerPath.Length).Replace("\\", "/").Trim('/', '\\');
            relativePath = relativePath.Substring(0, relativePath.Length - fileInfo.Name.Length);
            var mediaFile = new MediaFile
            {
                Name = fileInfo.Name,
                Type = fileInfo.Extension,
                FolderName = relativePath,
                Size = fileInfo.Length,
                MediaPath = _mediaService.GetMediaPublicUrl(relativePath, fileInfo.Name)
            };
            return mediaFile;
        }
    }
}