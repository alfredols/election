using System;

namespace Votacao.Service.Interface
{
    public interface IConnectBackend
    {
        bool TryConnection();

        DateTime GetDateTime();
    }
}
