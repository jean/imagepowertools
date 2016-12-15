using Newtonsoft.Json;

namespace Amba.ImagePowerTools.Settings
{
    public class CustomFieldDefinition
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "displayName")]
        public string DisplayName { get; set; }
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
    }
}