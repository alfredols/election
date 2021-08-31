namespace Votacao.Service.Interface
{
    public interface ILogin
    {
        DTO.Login Login(string user, string password);
    }
}