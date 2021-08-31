namespace Votacao.Service.DTO
{
    public class Voter
    {
        public int Id { get; set; }
        public string Identifier { get; set; }
        public string Name { get; set; }
        public bool? HasVoted { get; set; }
        public int? SectionId { get; set; }
    }
}
