namespace Votacao.Service.DTO
{
    public class Vote
    {
        public int Id { get; set; }
        public int? CandidateId { get; set; }
        public string VoterIdentifier { get; set; }
    }
}


