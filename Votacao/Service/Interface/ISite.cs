using System.Collections.Generic;
using Votacao.Service.DTO;

namespace Votacao.Service.Interface
{
    public interface ISite
    {
        List<Site> ListByRegion(int regionId);

        Site Get(int regionId);
    }
}
