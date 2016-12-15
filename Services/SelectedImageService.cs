using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Amba.ImagePowerTools.Extensions;
using Amba.ImagePowerTools.Models;
using ExifLib;
using Orchard;

namespace Amba.ImagePowerTools.Services
{
    public interface ISelectedImageService : IDependency
    {
        IEnumerable<SelectedImage> UpdateExifProperties(IEnumerable<SelectedImage> selectedImages);
    }

    public class SelectedImageService : ISelectedImageService
    {
        private readonly IMediaFileSystemService _mediaFileSystemService;

        public SelectedImageService(IMediaFileSystemService mediaFileSystemService)
        {
            _mediaFileSystemService = mediaFileSystemService;
        }

        public IEnumerable<SelectedImage> UpdateExifProperties(IEnumerable<SelectedImage> selectedImages)
        {
            if (selectedImages == null)
                return selectedImages;
            selectedImages.AsParallel().ForAll(selectedImage =>
            {
                try
                {
                    var serverPath = _mediaFileSystemService.GetServerPath(selectedImage.FilePath);
                    if (!File.Exists(serverPath))
                        return;
                    var fileInfo = new FileInfo(serverPath);
                    if (fileInfo.Extension.ToLower() == ".jpg")
                    {
                        var reader = new ExifReader(serverPath);
                        selectedImage.Latitude = reader.GetLat();
                        selectedImage.Longtitude = reader.GetLon();
                    }
                }
                catch (Exception e)
                {
                } 
            });
            return selectedImages;
        }
    }
}