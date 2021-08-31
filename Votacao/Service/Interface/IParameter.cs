using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace Votacao.Service.Interface
{
    public interface IParameter
    {   
        void Add(string key, string value);
        
        void Update(string key, string value);

        string Get(string key);

        bool Exists(string key);
    }
  
}
