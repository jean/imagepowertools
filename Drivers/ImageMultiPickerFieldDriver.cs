using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Amba.ImagePowerTools.Fields;
using Amba.ImagePowerTools.Models;
using Amba.ImagePowerTools.Services;
using Amba.ImagePowerTools.Settings;
using Amba.ImagePowerTools.ViewModels;
using ExifLib;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Amba.ImagePowerTools.Extensions;


namespace Amba.ImagePowerTools.Drivers
{
    public class ImageMultiPickerFieldDriver : FieldDriverBase<ImageMultiPickerField>
    {
        private readonly IMediaFileSystemService _mediaFileSystemService;
        private readonly IPowerToolsSettingsService _settingsService;
        private readonly ISelectedImageService _selectedImageService;

        public ImageMultiPickerFieldDriver(IMediaFileSystemService mediaFileSystemService, IPowerToolsSettingsService settingsService, ISelectedImageService selectedImageService)
        {
            _mediaFileSystemService = mediaFileSystemService;
            _settingsService = settingsService;
            _selectedImageService = selectedImageService;
        }

        protected override DriverResult Display(ContentPart part, ImageMultiPickerField field, string displayType, dynamic shapeHelper)
        {
            ImageMultiPickerFieldSettings settings = null;
            if (displayType == "SummaryAdmin")
            {
                settings = field.PartFieldDefinition.Settings.GetModel<ImageMultiPickerFieldSettings>();
                if (!settings.ShowInAdminList)
                    return new DriverResult();
                return ContentShape("Fields_ImageMultiPicker_SummaryAdmin", GetDifferentiator(field, part),
                () => shapeHelper.Fields_ImageMultiPicker_SummaryAdmin(
                    Settings: settings,
                    Name: field.Name,
                    Field: field));
            }
            settings = field.PartFieldDefinition.Settings.GetModel<ImageMultiPickerFieldSettings>();
            return ContentShape("Fields_ImageMultiPicker", GetDifferentiator(field, part),
                () => shapeHelper.Fields_ImageMultiPicker(
                    Settings:settings,
                    Name: field.Name, 
                    Field: field));
        }

        protected override DriverResult Editor(ContentPart part, ImageMultiPickerField field, dynamic shapeHelper)
        {
            return ContentShape("Fields_ImageMultiPicker_Edit", GetDifferentiator(field, part),
                () =>
                {
                    var viewModel = new ImageMultiPickerFieldEditorViewModel
                    {
                        Field = field,
                        Data = IsDataInvalid(field.Data) ? "[]" : field.Data,
                        FieldFolderName = _mediaFileSystemService.GetContentItemUploadFolder(part.Id, field.Name),
                        Settings = field.PartFieldDefinition.Settings.GetModel<ImageMultiPickerFieldSettings>()
                    };

                    return shapeHelper.EditorTemplate(TemplateName: "Fields_ImageMultiPicker_Edit", Model: viewModel, Prefix: GetPrefix(field, part));
                });
        }

        private static bool IsDataInvalid(string data)
        {
            return string.IsNullOrWhiteSpace(data) || data.RegexRemove(@"\s") == "{{data|json}}";
        }

        protected override DriverResult Editor(ContentPart part, ImageMultiPickerField field, IUpdateModel updater, dynamic shapeHelper)
        {
            var viewModel = new ImageMultiPickerFieldEditorViewModel();
            
            if (updater.TryUpdateModel(viewModel, GetPrefix(field, part), null, null))
            {                
                field.Data = IsDataInvalid(viewModel.Data) ? "[]" : viewModel.Data;
                
                var images = field.Images;
                if (images != null && _settingsService.Settings.EnableContentItemFolderCleanup)
                {
                    var uploadFolder = _mediaFileSystemService.GetContentItemUploadFolder(part.Id, field.Name);
                    Task.Factory.StartNew(() => _mediaFileSystemService.DeleteNotUsedFiles(uploadFolder, images));
                }
                field.Images = _selectedImageService.UpdateExifProperties(field.Images.ToList());
            }
            return Editor(part, field, shapeHelper);
        }

        protected override void Importing(ContentPart part, ImageMultiPickerField field, ImportContentContext context)
        {
            var imageElement = context.Data.Element(field.FieldDefinition.Name + "." + field.Name);
            if (imageElement == null)
            {
                return;
            }

            var dataElement = imageElement.Element("Data");
            if (dataElement != null)
            {
                field.Data = dataElement.Value;
            }
        }

        protected override void Exporting(ContentPart part, ImageMultiPickerField field, ExportContentContext context)
        {
            var imageElement = context.Element(field.FieldDefinition.Name + "." + field.Name);
            imageElement.Add(new XElement("Data", new XCData(field.Data)));
        }

    }
}