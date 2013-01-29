using System.Web.Mvc;
using Amba.ImagePowerTools.Models;
using Amba.ImagePowerTools.Services;
using Amba.ImagePowerTools.ViewModels.Admin;
using Orchard;
using Orchard.Data;
using Orchard.Environment;
using Orchard.UI.Admin;

namespace Amba.ImagePowerTools.Controllers
{
    [Admin]
    public class AdminController : Controller
    {
        private readonly IPowerToolsSettingsService _settingsService;

        public AdminController(IPowerToolsSettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public ActionResult Settings()
        {
            return View(new SettingsViewModel(_settingsService.Settings));
        }

        [HttpPost]
        public ActionResult Settings(SettingsViewModel viewModel)
        {
            if (!this.ModelState.IsValid)
                return View(viewModel);
            _settingsService.Settings.EnableFrontendResizeAction = viewModel.EnableFrontendResizeAction;
            _settingsService.Settings.MaxImageHeight = viewModel.MaxImageHeight;
            _settingsService.Settings.MaxImageWidth = viewModel.MaxImageWidth;
            _settingsService.SaveSettings();
            return RedirectToAction("Settings");
        }
    }
}