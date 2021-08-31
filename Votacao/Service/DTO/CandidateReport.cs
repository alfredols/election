using System;

namespace Votacao.Service.DTO
{
    [Serializable]
    public class CandidateReport

    {
        public int CandidateId { get; set; }

        public string CandidateName { get; set; }

        public int SumVote { get; set; }

    }
}
