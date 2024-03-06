using Azure.Data.Tables;
using Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ApiFetchAndCacheApp.Options
{
    public class PayloadStorageOptions 
    {
        public PayloadStorageOptions()
        {
        }

        public static string Section = "PayloadStorage";

        [Required(AllowEmptyStrings = false)]
        [MinLength(4)]
        public string Container { get; set; } = String.Empty;

        [Required(AllowEmptyStrings = false)]
        public string StorageConnectionString { get; set; } = String.Empty;

    }
}
