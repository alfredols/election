namespace Votacao.Service.DTO
{
    public class Section
    {
        public int Id { set; get; }

        public int Number { get; set; }

        public int SiteId { get; set; }

        public string Name
        {
            get
            {
                return string.Format("Seção {0}", Number);
            }
        }
    }
}
