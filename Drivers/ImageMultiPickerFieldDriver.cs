using Amba.ImagePowerTools.Fields;
using Amba.ImagePowerTools.Models;
using Amba.ImagePowerTools.ViewModels;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Amba.ImagePowerTools.Drivers
{
    public class ImageMultiPickerFieldDriver : FieldDriverBase<ImageMultiPickerField> 
    {
        private IOrchardServices _services { get; set; }

        public ImageMultiPickerFieldDriver(IOrchardServices services)
        {
            _services = services;
        }

        protected override DriverResult Display(ContentPart part, ImageMultiPickerField field, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Fields_ImageMultiPicker", GetDifferentiator(field, part),
                () =>
                {
                    return shapeHelper.Fields_ImageMultiPicker(Name: field.Name, Field: field);
                });
        }

        protected override DriverResult Editor(ContentPart part, ImageMultiPickerField field, dynamic shapeHelper)
        {
            return ContentShape("Fields_ImageMultiPicker_Edit", GetDifferentiator(field, part),
                () =>
                {
                    var viewModel = new ImageMultiPickerFieldEditorViewModel
                    {
                        Field = field,
                        Data = string.IsNullOrWhiteSpace(field.Data) ? "[]" : field.Data
                    };

                    return shapeHelper.EditorTemplate(TemplateName: "Fields_ImageMultiPicker_Edit", Model: viewModel, Prefix: GetPrefix(field, part));
                });
        }

        protected override DriverResult Editor(ContentPart part, ImageMultiPickerField field, IUpdateModel updater, dynamic shapeHelper)
        {
            var viewModel = new ImageMultiPickerFieldEditorViewModel();

            if (updater.TryUpdateModel(viewModel, GetPrefix(field, part), null, null))
            {
                field.Data = viewModel.Data;
            }
            return Editor(part, field, shapeHelper);
        }
        
    }
}