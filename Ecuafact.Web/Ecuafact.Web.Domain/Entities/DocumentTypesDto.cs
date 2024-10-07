namespace Ecuafact.Web.Domain.Entities
{
    public class DocumentTypesDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SriCode { get; set; }
        public class Input
        {
            public string TokenId { get; set; }
        }
    }
}
