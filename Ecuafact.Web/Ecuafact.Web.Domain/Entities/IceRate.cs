namespace Ecuafact.Web.Domain.Entities
{

    public class IceRate : CatalogBaseDto
    {
        public decimal? Rate { get; set; }

        public decimal? EspecificRate { get; set; }

        public string EspecificRateDescription { get; set; }

    }
}
