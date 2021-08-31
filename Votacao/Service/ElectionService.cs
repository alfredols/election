using Newtonsoft.Json.Linq;
using Votacao.Service.Interface;
using RestSharp;
using System;
using Votacao.Service.DTO;

namespace Votacao.Service
{
    public class ElectionService : IElection
    {
        #region Private Attributes

        private RestClient client = null;
        private string token;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Constructor

        public ElectionService(string host, string token)
        {
            client = new RestClient(host);
            this.token = token;
        }

        #endregion

        #region Public Methods

        public string GetName(string electionId)
        {
            try
            {
                log.Info(string.Format("election electionId {0}", electionId));

                var request = new RestRequest(string.Format(@"election/{0}", electionId), Method.GET);

                request.AddHeader("Authorization", string.Format("jwt {0}", token));

                IRestResponse response = client.Execute(request);
                JObject responseJson = JObject.Parse(response.Content);

                log.Info(string.Format("election {0}", response.Content));

                return responseJson.SelectToken("data.name").Value<string>();
            }
            catch (Exception ex)
            {
                log.Error(string.Format("election electionId {0}", electionId), ex);
            }

            return string.Empty;
        }

        public TypeVoting GetVoting(string electionId)
        {
            try
            {
                log.Info(string.Format("election electionId {0}", electionId));

                var request = new RestRequest(string.Format(@"election/{0}", electionId), Method.GET);

                request.AddHeader("Authorization", string.Format("jwt {0}", token));

                IRestResponse response = client.Execute(request);
                JObject responseJson = JObject.Parse(response.Content);

                log.Info(string.Format("election {0}", response.Content));
                
                TypeVotingStringConverter converter = new TypeVotingStringConverter();

                return converter.ConvertStringToTypeVoting(responseJson.SelectToken("data.voting").Value<string>());
            }
            catch (Exception ex)
            {
                log.Error(string.Format("election electionId {0}", electionId), ex);
            }

            return TypeVoting.CANDIDATE;
        }

        public int GetVotesPerVoter(string electionId)
        {
            try
            {
                log.Info(string.Format("election electionId {0}", electionId));

                var request = new RestRequest(string.Format(@"election/{0}", electionId), Method.GET);

                request.AddHeader("Authorization", string.Format("jwt {0}", token));

                IRestResponse response = client.Execute(request);
                JObject responseJson = JObject.Parse(response.Content);

                log.Info(string.Format("election {0}", response.Content));

                return responseJson.SelectToken("data.votesPerVoter").Value<int>();
            }
            catch (Exception ex)
            {
                log.Error(string.Format("election electionId {0}", electionId), ex);
            }

            return 1;
        }

        public DateTime GetElectionDateTimeStart(string electionId)
        {
            try
            {
                log.Info(string.Format("election electionId {0}", electionId));

                var request = new RestRequest(string.Format(@"election/{0}", electionId), Method.GET);

                request.AddHeader("Authorization", string.Format("jwt {0}", token));

                IRestResponse response = client.Execute(request);
                JObject responseJson = JObject.Parse(response.Content);

                log.Info(string.Format("election {0}", response.Content));

                int date = responseJson.SelectToken("data.electionDateStart").Value<int>();

                return DateTimeOffset.FromUnixTimeSeconds(date).DateTime.ToLocalTime();
            }
            catch (Exception ex)
            {
                log.Error(string.Format("election electionId {0}", electionId), ex);
            }

            return System.DateTime.Now;
        }

        public DateTime GetElectionDateTimeEnd(string electionId)
        {
            try
            {
                log.Info(string.Format("election electionId {0}", electionId));

                var request = new RestRequest(string.Format(@"election/{0}", electionId), Method.GET);

                request.AddHeader("Authorization", string.Format("jwt {0}", token));

                IRestResponse response = client.Execute(request);
                JObject responseJson = JObject.Parse(response.Content);

                log.Info(string.Format("election {0}", response.Content));

                int date = responseJson.SelectToken("data.electionDateEnd").Value<int>();

                return DateTimeOffset.FromUnixTimeSeconds(date).DateTime.ToLocalTime();
            }
            catch (Exception ex)
            {
                log.Error(string.Format("election electionId {0}", electionId), ex);
            }

            return System.DateTime.Now;
        }

        #endregion
    }
}
