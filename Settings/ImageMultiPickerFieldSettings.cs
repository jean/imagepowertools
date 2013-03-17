using System.ComponentModel;

namespace Amba.ImagePowerTools.Settings
{
    public class ImageMultiPickerFieldSettings
    {
        public string Hint { get; set; }
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