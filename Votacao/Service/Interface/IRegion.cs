using System.Collections.Generic;
using Votacao.Service.DTO;

namespace Votacao.Service.Interface
{
    public interface IRegion
    {
        List<Region> List();
        Region Get(int regionId);
    }
}
