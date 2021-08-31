using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Votacao.Service.DTO;

namespace Votacao.Service.Interface
{
    public interface IVote
    {
        List<Vote> ListVote();

        void Save(int? votedId, string nicknameStation);
    }
}
