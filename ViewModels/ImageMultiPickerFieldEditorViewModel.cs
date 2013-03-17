using Amba.ImagePowerTools.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Amba.ImagePowerTools.Settings;

namespace Amba.ImagePowerTools.ViewModels
{
    public class ImageMultiPickerFieldEditorViewModel
    {
        public string Data { get; set; }
        public ImageMultiPickerField Field { get; set; }
        public string FieldFolderName { get; set; }
        public ImageMultiPickerFieldSettings Settings { get; set; }
    }
}