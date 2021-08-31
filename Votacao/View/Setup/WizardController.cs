using MVVMC;
using Votacao.Data;

namespace Votacao.View.Setup
{
    public class WizardController : Controller
    {
        #region Attributes

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Public Methods

        public override void Initial()
        {
            if (StationNotConfigured())
            {
                DefineTypeStation();
            }
            else
            {
                ConfirmationFinal();
            }
        }

        public void Next()
        {
            if (GetCurrentViewModel() is DefineTypeStationViewModel defineTypeStationViewModel)
            {
                if (defineTypeStationViewModel.ValidateScreen())
                {
                    if (defineTypeStationViewModel.IsSearchVoterStation())
                    {
                        ConfirmationFinal();
                    }
                    else
                    {
                        ChooseIPAddress();
                        ChooseIPAddressViewModel chooseIPAddressViewModel = GetCurrentViewModel() as ChooseIPAddressViewModel;
                        if (chooseIPAddressViewModel.NextScreen())
                        {
                            DefineRegionSiteSection();
                        }
                    }
                }
            }
            else if (GetCurrentViewModel() is ChooseIPAddressViewModel chooseIPAddressViewModel)
            {
                if (chooseIPAddressViewModel.ValidateScreen())
                {
                    DefineRegionSiteSection();
                }
            }
            else if (GetCurrentViewModel() is DefineRegionSiteSectionViewModel defineRegionSiteSectionViewModel)
            {
                if (defineRegionSiteSectionViewModel.ValidateScreen())
                {
                    if (ParametersSingleton.Instance.TypeStation == Service.DTO.TypeStation.VOTING_OFFICER)
                        PairVotingStationOfficer(false);
                    else
                        PairBallotBox();
                }
            }
            else if (GetCurrentViewModel() is PairVotingStationOfficerViewModel pairVotingStationOfficerViewModel)
            {
                if (pairVotingStationOfficerViewModel.ValidateScreen())
                {
                    pairVotingStationOfficerViewModel.StopUDPServer();
                    ConfirmationFinal();
                }
            }
            else if (GetCurrentViewModel() is PairBallotBoxViewModel pairBallotBoxViewModel)
            {
                pairBallotBoxViewModel.StopNetworkActivity();
                ConfirmationFinal();
            }
            else if (GetCurrentViewModel() is ConfirmationFinalViewModel confirmationFinalViewModel)
            {
                if (confirmationFinalViewModel.AskConfirmation())
                {
                    confirmationFinalViewModel.ResetConfiguration();
                    ClearHistory();
                    DefineTypeStation();
                }
            }
        }

        public void Back()
        {
            if (GetCurrentViewModel() is ChooseIPAddressViewModel chooseIPAddressViewModel)
            {
                DefineTypeStation();
            }
            else if (GetCurrentViewModel() is DefineRegionSiteSectionViewModel defineRegionSiteSectionViewModel)
            {
                ChooseIPAddress();
                ChooseIPAddressViewModel chooseIPAddressVM = GetCurrentViewModel() as ChooseIPAddressViewModel;
                if (chooseIPAddressVM.NextScreen())
                {
                    DefineTypeStation();
                }
            }
            else if (GetCurrentViewModel() is PairVotingStationOfficerViewModel pairVotingStationOfficerViewModel)
            {
                pairVotingStationOfficerViewModel.ErrorMessage = "";
                pairVotingStationOfficerViewModel.StopUDPServer();
                DefineRegionSiteSection();
            }
            else if (GetCurrentViewModel() is PairBallotBoxViewModel pairBallotBoxViewModel)
            {
                pairBallotBoxViewModel.StopNetworkActivity();
                DefineRegionSiteSection();
            }
            else if (GetCurrentViewModel() is ConfirmationFinalViewModel confirmationFinalViewModel)
            {
                if (ParametersSingleton.Instance.TypeStation == Service.DTO.TypeStation.VOTING_OFFICER)
                    PairVotingStationOfficer(false);
                else
                    PairBallotBox();
            }
        }

        public void PairVotingStationOfficer(bool rePair)
        {
            ExecuteNavigation(rePair, null);
            log.Info("Navigate to PairVotingStationOfficer");
        }

        #endregion

        #region Private Methods

        private void DefineTypeStation()
        {
            ExecuteNavigation();
            log.Info("Navigate to DefineTypeStation");
        }

        private void ChooseIPAddress()
        {
            ExecuteNavigation();
            log.Info("Navigate to ChooseIPAddress");
        }

        private void DefineRegionSiteSection()
        {
            ExecuteNavigation();
            log.Info("Navigate to DefineRegionSiteSection");
        }

        private void PairBallotBox()
        {
            ExecuteNavigation();
            log.Info("Navigate to PairBallotBox");
        }

        private void ConfirmationFinal()
        {
            ExecuteNavigation();
            log.Info("Navigate to ConfirmationFinal");
        }

        private static bool StationNotConfigured()
        {
            return ParametersSingleton.Instance.StatusStation == null;
        }

        #endregion
    }
}
