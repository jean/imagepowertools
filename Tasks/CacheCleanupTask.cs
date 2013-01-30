using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Amba.ImagePowerTools.Services;
using Orchard.Logging;
using Orchard.Tasks;

namespace Amba.ImagePowerTools.Tasks
{
    public class CacheCleanupTask : IBackgroundTask
    {
        private static bool _wasStarted = false;
        private static readonly object _syncRoot = new object();
        private static DateTime _lastRun = new DateTime(2010, 10, 10);

        private const int CleanPeriodInMinutes = 30;
        private IPowerToolsSettingsService _settingsService;

        public ILogger Logger { get; set; }

        public CacheCleanupTask(IPowerToolsSettingsService settingsService)
        {
            Logger = NullLogger.Instance;
            _settingsService = settingsService;
        }

        public void Sweep()
        {
            if (_wasStarted)
                return; 
            lock (_syncRoot)
            {
                if (_lastRun > DateTime.Now.AddMinutes(CleanPeriodInMinutes * -1))
                {
                    return;
                }
                _wasStarted = true;
                try
                {
                    var resizeService = new ImageResizerService(_settingsService);
                    resizeService.DeleteOldCache();
                    _lastRun = DateTime.Now;
                    _settingsService.Settings.DeleteOldLastJobRun = _lastRun;
                    _settingsService.SaveSettings();
                }
                catch(Exception e)
                {
                     Logger.Error(e, "Amba.ImagePowerTools Cache cleanup task failed");   
                }
                finally
                {
                    _wasStarted = false;
                }
            }
        }
    }
}