using Newtonsoft.Json.Linq;
using Votacao.Service.Interface;
using RestSharp;
using System;
using Votacao.Data;

namespace Votacao.Service
{
    public class LoginService : ILogin
    {
        #region Private Attributes

        private RestClient client = null;
        private readonly string ROLE_HANDLER = "Role.HANDLER";
        private readonly string ROLE_SYSTEM = "Role.SYSTEM";
        private readonly string ROLE_ADMIN = "Role.ADMIN";
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Constructor

        public LoginService(string host)
        {
            client = new RestClient(host);
        }

        #endregion

        #region Public Methods

        public DTO.Login Login(string user, string password)
        {
            DTO.Login login = new DTO.Login();

            try
            {
                log.Info(string.Format("Login {0}", user));

                var request = new RestRequest("auth", Method.POST);
                request.AddJsonBody(new { login = user, pass = password });

                IRestResponse response = client.Execute(request);
                JObject responseJson = JObject.Parse(response.Content);

                login.Status = GetStatusFromJSON(responseJson);
                login.Token = GetTokenFromJSON(responseJson);
                login.Election = GetElectionFromJSON(responseJson);
                login.NameUser = GetNameUserFromJSON(responseJson);

                if (ValidateAuthorization(GetRoleFromJSON(responseJson)))
                {
                    ParametersSingleton.Instance.Token = login.Token;
                    log.Info(string.Format("Login {0}", response.Content));
                }
                else
                {
                    log.Info(string.Format("Login acesso negado {0}", response.Content));
                    login = new DTO.Login() { Status = DTO.StatusTransaction.ACCESSDENIED };
                }
            }
            catch (Exception ex)
            {
                log.Error(string.Format("Login erro {0}", user), ex);
                login = new DTO.Login() { Status = DTO.StatusTransaction.FAIL };
            }

            return login;
        }

        #endregion

        #region Private Methods

        private bool ValidateAuthorization(string role)
        {
            return (role == ROLE_HANDLER
                || role == ROLE_ADMIN
                || role == ROLE_SYSTEM);
        }

        private string GetRoleFromJSON(JObject responseJson)
        {
            return responseJson.SelectToken("data.permissions")[0][1].Value<string>();
        }

        private string GetElectionFromJSON(JObject responseJson)
        {
            return responseJson.SelectToken("data.permissions")[0][0].Value<string>();
        }

        private string GetTokenFromJSON(JObject responseJson)
        {
            return responseJson.SelectToken("data.token").Value<string>();
        }

        private string GetNameUserFromJSON(JObject responseJson)
        {
            return responseJson.SelectToken("data.name").Value<string>();
        }

        private DTO.StatusTransaction GetStatusFromJSON(JObject responseJson)
        {
            return (DTO.StatusTransaction)Enum.Parse(typeof(DTO.StatusTransaction), responseJson.SelectToken("status").Value<string>(), true);
        }

        #endregion
    }
}