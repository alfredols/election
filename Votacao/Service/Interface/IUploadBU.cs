using Votacao.Service.DTO;

namespace Votacao.Service.Interface
{
    public interface IUploadBU
    {

        StatusTransaction UploadZeresima(string bu);
        StatusTransaction Upload(string bu);

    }
}
