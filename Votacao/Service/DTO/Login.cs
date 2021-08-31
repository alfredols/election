namespace Votacao.Service.DTO
{
    public class Login
    {
        #region Properties

        public StatusTransaction Status
        {
            get;
            set;
        }

        public string NameUser
        {
            get;
            set;
        }

        public string Token
        {
            get;
            set;
        }

        public string Election
        {
            get;
            set;
        }

        #endregion
    }
}
