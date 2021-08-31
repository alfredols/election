using System.Collections.Generic;
using Votacao.Service.DTO;

namespace Votacao.Service.Interface
{
    public interface IReport
    {
        void Save(Report report);

        List<Report> List();
    }
}
