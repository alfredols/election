using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votacao.Service.DTO
{
    public class Site
    {
        public int Id { set; get; }

        public string Name { set; get; }

        public string Address { set; get; }

        public int RegionId { set; get; }

    }
}
