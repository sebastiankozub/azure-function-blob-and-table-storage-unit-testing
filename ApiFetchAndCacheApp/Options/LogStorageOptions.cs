using System.ComponentModel.DataAnnotations;

namespace ApiFetchAndCacheApp.Options
{
    public class LogStorageOptions
    {
        public LogStorageOptions()
        {
        }

        public static string Section = "LogStorage";

        [Required(AllowEmptyStrings = false)]
        [MinLength(4)]
        public string Table { get; set; } = String.Empty;

        [Required(AllowEmptyStrings = false)]
        [MinLength(4)]
        public string PartitionKey { get; set; } = String.Empty;

        [Required(AllowEmptyStrings = false)]
        public string StorageConnectionString { get; set; } = String.Empty;
    }
}
