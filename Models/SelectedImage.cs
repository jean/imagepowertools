using System.Collections.Generic;
using System.Dynamic;
using Newtonsoft.Json;

namespace Amba.ImagePowerTools.Models
{
    public class SelectedImage : DynamicObject
    {
        [JsonProperty(PropertyName = "file")]
        public string FilePath { get; set; }
        [JsonProperty(PropertyName = "descr")]
        public string Description { get; set; }

        private Dictionary<string, object> _properties = new Dictionary<string, object>();

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
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
    }
}