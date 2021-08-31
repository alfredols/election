using System.Collections.Generic;
using Votacao.Service.DTO;

namespace Votacao.Service.Interface
{
    public interface ICandidate
    {
        List<Candidate> ListCandidate(int regionId);
        Candidate GetValidByNumber(int number, int regionId);
        int GetMaxSizeCandidateNumber();
    }
}


