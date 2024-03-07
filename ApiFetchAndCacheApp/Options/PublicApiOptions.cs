using System.ComponentModel.DataAnnotations;

namespace ApiFetchAndCacheApp.Options
{
    public class PublicApiOptions
    {
        public PublicApiOptions()
        {
        }

        public const string Section = "PublicApi";

        [Required(AllowEmptyStrings = false)]
        [Url]
        public string RandomGetUri { get; set; } = String.Empty;
    }
}
