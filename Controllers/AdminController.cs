using System.Web.Mvc;
using Amba.ImagePowerTools.Services;
using Amba.ImagePowerTools.ViewModels.Admin;
using Orchard.UI.Admin;

namespace Amba.ImagePowerTools.Controllers
{
    [Admin]
    public class AdminController : Controller
    {
        private readonly IPowerToolsSettingsService _settingsService;
        private readonly IImageResizerService _resizerService;

        public AdminController(IPowerToolsSettingsService settingsService, IImageResizerService resizerService)
        {
            _settingsService = settingsService;
            _resizerService = resizerService;
        }

        public ActionResult Settings()
        {
            var viewModel = new SettingsViewModel(_settingsService.Settings);
            return View(viewModel);
        }       

        [HttpPost]
        public ActionResult Settings(SettingsViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);
            _settingsService.Settings.EnableFrontendResizeAction = viewModel.EnableFrontendResizeAction;
            _settingsService.Settings.MaxImageHeight = viewModel.MaxImageHeight;
            _settingsService.Settings.MaxImageWidth = viewModel.MaxImageWidth;
            _settingsService.Settings.EnableContentItemFolderCleanup = viewModel.EnableContentItemFolderCleanup;
            _settingsService.SaveSettings();
            return RedirectToAction("Settings");
        }

        public ActionResult Cache()
        {
            var viewModel = new CacheStatisticsViewModel(_settingsService.Settings);
            _resizerService.CacheStatistics(out viewModel.FilesCount, out viewModel.TotalSize);
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult ClearCache()
        {
            _resizerService.ClearCache();
            return RedirectToAction("Cache");
        }
    }
}