namespace Amba.ImagePowerTools.ViewModels
{
    public class FileViewModel
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsImage { get; set; }
        public string Extension { get; set; }

        //ex: woof.jpg
        public string FileName { get; set; }

        //ex: /Media/Default/foo/bar/
        public string SiteFolder { get; set; }

        //ex: /Media/Default/foo/bar/woof.jpg
        public string SitePath { get; set; }

        //ex: c:/orchard/src/Orchard.Web/Media/Default/foo/bar/woof.jpg
        public string ServerPath { get; set; }

        public long Size { get; set; }

        public string DisplayPath { get; set; }

        public double? Lon { get; set; }
        public double? Lat { get; set; }
    }
}