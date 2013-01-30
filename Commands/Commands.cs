using Amba.ImagePowerTools.Services;
using Orchard.Commands;

namespace Amba.ImagePowerTools.Commands
{
    public class Go2SeeCommands : DefaultOrchardCommandHandler
    {
        private readonly IPowerToolsSettingsService _settingsService;
   
        public Go2SeeCommands(
            IPowerToolsSettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        [CommandHelp("Deletes cache for deleted images")]
        [CommandName("amba.ipt deleteold")]
        public void CacheDeleteOld() 
        {
            var resizeService = new ImageResizerService(_settingsService);
            resizeService.DeleteOldCache();
        }

        [CommandHelp("Deletes all files from cache")]
        [CommandName("amba.ipt clearcache")]
        public void ClearCache()
        {
            var resizeService = new ImageResizerService(_settingsService);
            resizeService.ClearCache();
        }
    }
}