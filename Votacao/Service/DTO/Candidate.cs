namespace Votacao.Service.DTO
{
    public class Candidate
    {     
        public int Id { get; set; }

        public string Name { get; set; }

        public string Nickname { get; set; }

        public byte[] Picture { get; set; }

        public int RegionId { get; set; }

        public int? Number { get; set; }
    }
}
