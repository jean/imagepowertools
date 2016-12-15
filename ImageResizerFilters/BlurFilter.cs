using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using AForge.Imaging.Filters;
using ImageResizer.Configuration;
using ImageResizer.Plugins;
using ImageResizer.Resizing;
using ImageResizer.Util;

namespace Amba.ImagePowerTools.ImageResizerFilters
{
    public class BlurFilter : BuilderExtension, IPlugin, IQuerystringPlugin
    {
        public IPlugin Install(Config c)
        {
            c.Plugins.add_plugin(this);
            return this;
        }

        public bool Uninstall(Config c)
        {
            c.Plugins.remove_plugin(this);
            return true;
        }

        public static IEnumerable<string> SupportedKeys = new[] { "ipt.blur" };

        public IEnumerable<string> GetSupportedQuerystringKeys()
        {
            return SupportedKeys;
        }

        protected override RequestedAction PostRenderImage(ImageState s)
        {
            if (s.destBitmap == null) 
                return RequestedAction.None;
            if (!s.settings.WasOneSpecified(GetSupportedQuerystringKeys().ToArray()))
                return RequestedAction.None;;
            int blurSize; 
            if (!int.TryParse(s.settings["ipt.blur"], out blurSize))
            {
                return RequestedAction.None;
            }

            new GaussianBlur(1.4, blurSize).ApplyInPlace(s.destBitmap);

            return RequestedAction.None;
        }
    }
}