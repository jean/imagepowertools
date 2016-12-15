using System.Collections.Generic;
using System.Dynamic;
using Newtonsoft.Json;
using System.Linq;

namespace Amba.ImagePowerTools.Models
{
    public class SelectedImage : DynamicObject
    {
        [JsonProperty(PropertyName = "file")]
        public string FilePath { get; set; }

        [JsonProperty(PropertyName = "iptx_lon")]
        public double? Longtitude { get; set; }

        [JsonProperty(PropertyName = "iptx_lat")]
        public double? Latitude { get; set; }

        [JsonProperty(PropertyName = "descr")]
        public string Description { get; set; }

        private Dictionary<string, object> _properties = new Dictionary<string, object>();

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (!GetDynamicMemberNames().Contains(binder.Name))
            {
                result = string.Empty;
                return true;
            }
            return _properties.TryGetValue(binder.Name, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _properties[binder.Name] = value;
            return true;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _properties.Keys;
        }

        public dynamic AsDynamic()
        {
            return (dynamic) this;
        }

        public string this[string name]
        {
            get
            {
                if (name == "file")
                    return FilePath;
                    
                if (name == "descr")
                    return Description;
                if (!_properties.ContainsKey(name))
                    return string.Empty;
                return (string)_properties[name];
            }
            set
            {
                if (name == "file")
                {
                    FilePath = value;
                    return;
                }                    
                if (name == "descr")
                {
                    Description = value;
                    return;
                }
                    
                _properties[name] = value;
            }
        }
    }
}