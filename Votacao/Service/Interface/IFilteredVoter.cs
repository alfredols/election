using Votacao.Service.DTO;

namespace Votacao.Service.Interface
{
    public interface IFilteredVoter
    {
        void Clear();

        void Populate(int sectionId);

        Voter Get(string identifier, int sectionId);

        Voter Get(string identifier);

        void Voted(int voterId);

        bool IsEmpty();
    }
}
