using MVVMC;
using Votacao.Data;

namespace Votacao.View.Election
{
    public class ElectionController : Controller
    {
        #region Attributes

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Public Methods

        public override void Initial()
        {
            if (ParametersSingleton.Instance.TypeStation == Service.DTO.TypeStation.VOTING_OFFICER)
            {
                InitElection();
            }
            else
            {
                InitBallotBoxCandidate();
            }
        }

        public void InitElection()
        {
            ExecuteNavigation();
            log.Info("Navigate to InitElection");
        }

        public void InitBallotBoxCandidate()
        {
            ExecuteNavigation();
            log.Info("Navigate to InitBallotBoxCandidate");
        }

        #endregion

    }
}
