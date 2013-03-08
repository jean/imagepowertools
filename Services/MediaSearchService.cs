using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using Orchard;
using Orchard.Environment.Configuration;
using Orchard.Media.Models;
using Orchard.Media.Services;

namespace Amba.ImagePowerTools.Services
{
    public interface IMediaSearchService : IDependency
    {
        IEnumerable<MediaFile> FindFiles(string mediaPath, string pattern);
    }

    public class MediaSearchService : IMediaSearchService
    {
        private readonly IMediaService _mediaService;
        private readonly ShellSettings _shellSettings;
        private readonly string _mediaServerPath;

        public MediaSearchService(IMediaService mediaService, ShellSettings shellSettings)
        {
            _mediaService = mediaService;
            _shellSettings = shellSettings;

            var mediaPath = HostingEnvironment.IsHosted
                                ? HostingEnvironment.MapPath("~/Media/") ?? ""
                                : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Media");

            _mediaServerPath = Path.Combine(mediaPath, _shellSettings.Name);
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