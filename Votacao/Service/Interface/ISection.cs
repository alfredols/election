using System.Collections.Generic;
using Votacao.Service.DTO;

namespace Votacao.Service.Interface
{
    public interface ISection
    {
        List<Section> ListBySite(int siteId);

        Section Get(int siteId);
    }
}
