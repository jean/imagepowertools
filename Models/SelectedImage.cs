using Newtonsoft.Json;

namespace Amba.ImagePowerTools.Models
{
    public class SelectedImage
    {
        [JsonProperty(PropertyName = "file")]
        public string FilePath { get; set; }
        [JsonProperty(PropertyName = "descr")]
        public string Description { get; set; }
    }
}