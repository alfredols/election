using System;
using Votacao.Service.DTO;

namespace Votacao.Service.Interface
{
    public interface IElection
    {
        string GetName(string electionId);

        TypeVoting GetVoting(string electionId);

        int GetVotesPerVoter(string electionId);

        DateTime GetElectionDateTimeStart(string electionId);

        DateTime GetElectionDateTimeEnd(string electionId);
    }
}