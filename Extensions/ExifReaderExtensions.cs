using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ExifLib;

namespace Amba.ImagePowerTools.Extensions
{
    public static class ExifReaderExtension
    {
        public static double? GetLat(this ExifReader exifReader)
        {
            double[] lat;
            string latRef;

            if (
                exifReader.GetTagValue(ExifTags.GPSLatitude, out lat) &&
                exifReader.GetTagValue(ExifTags.GPSLatitudeRef, out latRef))
            {
                var result = ExifCoordinatsToDecimal(lat[0], lat[1], lat[2], latRef);

                var deg = Math.Truncate(result);
                var rest = (result - deg);
                var min = Math.Truncate(rest * 60);
                rest = rest * 60 - min;
                var sec = rest * 60;
                return result;
            }
            return null;
        }

        public static double? GetLon(this ExifReader exifReader)
        {
            double[] lon;

            string lonRef;

            if (
                exifReader.GetTagValue(ExifTags.GPSLongitude, out lon) &&
                exifReader.GetTagValue(ExifTags.GPSLongitudeRef, out lonRef))
            {
                return ExifCoordinatsToDecimal(lon[0], lon[1], lon[2], lonRef);
            }
            return null;
        }

        private static double ExifCoordinatsToDecimal(double deg, double min, double sec, string hem)
        {
            double d = deg + (min / 60) + (sec / 3600);
            return (hem == "S" || hem == "W") ? d *= -1 : d;
        }
    }
}