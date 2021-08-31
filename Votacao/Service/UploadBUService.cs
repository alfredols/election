using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using Votacao.Data;
using Votacao.Service.DTO;
using Votacao.Service.Interface;

namespace Votacao.Service
{
    public class UploadBUService : IUploadBU
    {
        #region Private Attributes

        private RestClient client = null;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Constructor

        public UploadBUService(string host)
        {
            client = new RestClient(host);
        }

        #endregion

        #region Public Methods

        public StatusTransaction Upload(string bu)
        {
            try
            {
                log.Info(string.Format("Sending BU {0}", bu));

                var request = new RestRequest(string.Format(@"uploadbu/{0}", ParametersSingleton.Instance.Election), Method.POST);

                request.AddJsonBody(new { data = bu });

                request.AddHeader("Authorization", string.Format("jwt {0}", ParametersSingleton.Instance.Token));

                IRestResponse response = client.Execute(request);

                JObject responseJson = JObject.Parse(response.Content);

                log.Info(string.Format("Response {0}", responseJson));

                return GetStatusFromJSON(responseJson);
            }
            catch (Exception ex)
            {
                log.Error("election electionId", ex);
            }

            return StatusTransaction.FAIL;
        }

        public StatusTransaction UploadZeresima(string bu)
        {
            try
            {
                log.Info(string.Format("Sending zeresima {0}", bu));

                var request = new RestRequest(string.Format(@"ballotbox/{0}", ParametersSingleton.Instance.Election), Method.POST);

                request.AddJsonBody(new { data = bu });

                request.AddHeader("Authorization", string.Format("jwt {0}", ParametersSingleton.Instance.Token));

                IRestResponse response = client.Execute(request);

                JObject responseJson = JObject.Parse(response.Content);

                log.Info(string.Format("Response {0}", responseJson));

                return GetStatusFromJSON(responseJson);
            }
            catch (Exception ex)
            {
                log.Error("election electionId", ex);
            }

            return StatusTransaction.FAIL;
        }

        #endregion

        #region Private Methods

        private StatusTransaction GetStatusFromJSON(JObject responseJson)
        {
            return (StatusTransaction)Enum.Parse(typeof(StatusTransaction), responseJson.SelectToken("status").Value<string>(), true);
        }

        #endregion

    }
}
