using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Votacao.Service.Interface
{
    public interface IQRCode
    {
        byte[] GetQrCodeToByte(string value);

    }
}
