using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votacao.Service.DTO
{
    public class IdentificationTerminal
    {
        public string SiteId { get; set; }

        public string RegionId { get; set; }

        public string SectionId { get; set; }

        public string NicknameStation { get; set; }
    }
}
