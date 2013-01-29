using Amba.ImagePowerTools.Services;
using Orchard.Commands;
using Orchard.Environment.Extensions;

namespace Amba.ImagePowerTools.Commands
{

    public class Go2SeeCommands : DefaultOrchardCommandHandler
    {
        private readonly IExtensionManager _extensionManager;
        private readonly IPowerToolsSettingsService _settingsService;
   

        public Go2SeeCommands(
            IExtensionManager extensionManager,
            IPowerToolsSettingsService settingsService)
        {
            _extensionManager = extensionManager;
            _settingsService = settingsService;
        }

        [CommandHelp("")]
        [CommandName("amba.imp cleanup")]
        public void CacheCleanup() 
        {
            var resizeService = new ImageResizerService(_settingsService);
            resizeService.DeleteOldCache();
        }

     
    }
}