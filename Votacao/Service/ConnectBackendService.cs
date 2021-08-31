using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using Votacao.Service.Interface;

namespace Votacao.Service
{
    public class ConnectBackendService : IConnectBackend
    {

        #region Private Attributes

        private RestClient client = null;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Constructor

        public ConnectBackendService(string host)
        {
            if (!string.IsNullOrEmpty(host))
                client = new RestClient(host);
        }

        #endregion

        #region Public Methods

        public bool TryConnection()
        {
            if (client != null)
            {
                try
                {
                    var request = new RestRequest(@"/", Method.GET);

                    IRestResponse response = client.Execute(request);

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        return true;
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }

            return false;
        }

        public DateTime GetDateTime()
        {
            if (client != null)
            {
                try
                {
                    var request = new RestRequest(@"/datetime", Method.GET);

                    IRestResponse response = client.Execute(request);

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        JObject responseJson = JObject.Parse(response.Content);
                        return GetDateTimeFromJSON(responseJson);
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }

            return DateTime.Now;
        }

        private DateTime GetDateTimeFromJSON(JObject responseJson)
        {
            return responseJson.SelectToken("data").Value<DateTime>();
        }

        #endregion

    }
}
