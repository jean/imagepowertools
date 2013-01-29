using Amba.ImagePowerTools.Models;
using Orchard.Data.Migration;

namespace Amba.ImagePowerTools
{
    public class Migrations : DataMigrationImpl
    {
        public int Create()
        {
            SchemaBuilder.CreateTable("ImagePowerToolsSettingsRecord",
                table => table
                    .Column<int>("Id", c => c.PrimaryKey().Identity())
                    .Column<int>("MaxCacheSizeMB")
                    .Column<int>("MaxCacheAgeDays")
                    .Column<bool>("EnableFrontendResizeAction")
                    .Column<int>("MaxImageWidth")
                    .Column<int>("MaxImageHeight")
                );
            return 1;
        }
    }
}