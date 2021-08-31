using Votacao.Service.DTO;

namespace Votacao.Service.Interface
{
    public interface IVoter
    {
        Voter Get(string identifier);
    }
}
