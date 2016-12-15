using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace Amba.ImagePowerTools.Settings
{
    public class ImageMultiPickerFieldSettings
    {
        public string Hint { get; set; }
        public bool ShowInAdminList { get; set; }

        private string _customFields;

        public string CustomFields
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_customFields))
                    _customFields = "[{name:'descr', displayName:'', type:'textarea'}]";
                else if (_customFields == "{{customFields | json}}")
                {
                    _customFields = "[]";
                }
                return _customFields;
            }
            set { _customFields = value; }
        }

        public IEnumerable<CustomFieldDefinition> CustomFieldsList
        {
            get
            {
                if (CustomFields == "{{customFields | json}}")
                    CustomFields = "[]";
                return JsonConvert.DeserializeObject<List<CustomFieldDefinition>>(CustomFields);
            }
        }

        private int _previewWidth;

        [DisplayName("Width for image preview")]        
        public int PreviewWidth
        {
            get
            {
                if (_previewWidth < 1)
                    return 100;
                return _previewWidth;
            }
            set
            {
                _previewWidth = value;
            }
        }
    }
}