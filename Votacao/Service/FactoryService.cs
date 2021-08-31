using Microsoft.Reporting.WinForms;
using Votacao.Data;
using Votacao.Service.Interface;

namespace Votacao.Service
{
    public class FactoryService
    {

        #region Public Methods

        public static ILogin getLoginService(bool isConnected) 
        {
            if (isConnected)
                return new LoginService(ParametersSingleton.Instance.Host);
            else
                return new LoginDAO();
        }

        public static IConnectBackend getConnectService()
        {
            return new ConnectBackendService(ParametersSingleton.Instance.Host);
        }

        public static IUploadBU getUploadBU()
        {
            return new UploadBUService(ParametersSingleton.Instance.Host);
        }

        public static IElection getElectionService(bool isConnected)
        {
            if (isConnected)
                return new ElectionService(ParametersSingleton.Instance.Host, ParametersSingleton.Instance.Token);
            else
                return new ElectionDAO();
        }

        public static IRegion getRegion()
        {
            return new RegionDAO();
        }

        public static ISite getSite()
        {
            return new SiteDAO();
        }

        public static ISection getSection()
        {
            return new SectionDAO();
        }

        public static IParameter getParameter()
        {
            return new ParameterDAO(); 
        }

        public static IVoter getVoter()
        {
            return new VoterDAO();
        }

        public static IFilteredVoter getFilteredVoter()
        {
            return new FilteredVoterDAO();
        }

        public static IVote getVote()
        {
            return new VoteDAO();
        }

        public static ICandidate getCandidate()
        {
            return new CandidateDAO();
        }

        public static IQRCode getQRCode()
        {
            return new QrCodeService();
        }

        public static IReport getReport()
        {
            return new ReportDAO();
        }

        #endregion

    }
}
