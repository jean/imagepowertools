using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Web;
using ImageResizer.Configuration;
using ImageResizer.Plugins;
using ImageResizer.Resizing;

namespace Amba.ImagePowerTools.ImageResizerFilters
{
    public class GrayscaleFilter : BuilderExtension, IPlugin, IQuerystringPlugin
    {
        public static string FilterKey = "tograyscale";

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

        public IEnumerable<string> GetSupportedQuerystringKeys()
        {
            return new [] { FilterKey };
        }

        protected override RequestedAction PostCreateImageAttributes(ImageState s)
        {
            if (s.copyAttibutes == null) 
                return RequestedAction.None;

            if (!s.settings.WasOneSpecified(GetSupportedQuerystringKeys().ToArray())) 
                return RequestedAction.None;

            float grayscaleParam;
            if (!float.TryParse(s.settings[FilterKey], out grayscaleParam))
            {
                grayscaleParam = 0.5f;
            }

            s.copyAttibutes.SetColorMatrix(new ColorMatrix(Grayscale(grayscaleParam)));
            return RequestedAction.None;
        }

        static float[][] Grayscale(float x = 0.3f)
        {
            return (new[]
                {
                    new[] {x, x, x, 0.0f, 0.0f},
                    new[] {x, x, x, 0.0f, 0.0f},
                    new[] {x, x, x, 0.0f, 0.0f},
                    new[] {0.0f, 0.0f, 0.0f, 1.0f, 0.0f},
                    new[] {0.0f, 0.0f, 0.0f, 0.0f, 1.0f}
                });

        }
    }
}