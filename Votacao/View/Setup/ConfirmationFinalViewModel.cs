using MahApps.Metro.Controls;
using MVVMC;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Votacao.Data;
using Votacao.Service;
using Votacao.Service.DTO;
using Votacao.Service.Interface;

namespace Votacao.View.Setup
{
    public class ConfirmationFinalViewModel : MVVMCViewModel
    {
        #region Attributes

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();
        private Main menuScreen;

        #endregion

        #region Commands

        private ICommand rePairBallotBoxCommand;
        public ICommand RePairBallotBoxCommand
        {
            get
            {
                if (rePairBallotBoxCommand == null)
                    rePairBallotBoxCommand = new DelegateCommand(() =>
                    {
                        NavigateToRePairBallotBox();
                    },
                    () =>
                    {
                        return RePairBallotBoxCommand != null;
                    });
                return rePairBallotBoxCommand;
            }
        }

        #endregion

        #region Properties

        private string _region;
        public string Region
        {
            get { return _region; }
            set
            {
                _region = value;
                OnPropertyChanged("Region");
            }
        }

        private string _site;
        public string Site
        {
            get { return _site; }
            set
            {
                _site = value;
                OnPropertyChanged("Site");
            }
        }

        private string _nickname;
        public string Nickname
        {
            get { return _nickname; }
            set
            {
                _nickname = value;
                OnPropertyChanged("Nickname");
            }
        }

        private string _stationType;
        public string StationType
        {
            get { return _stationType; }
            set
            {
                _stationType = value;
                OnPropertyChanged("StationType");
            }
        }

        private string _section;
        public string Section
        {
            get { return _section; }
            set
            {
                _section = value;
                OnPropertyChanged("Section");
            }
        }

        private Visibility loading;
        public Visibility Loading
        {
            get { return loading; }
            set
            {
                loading = value;
                OnPropertyChanged("Loading");
            }
        }

        private Visibility visibilityRePairBallotBox;
        public Visibility VisibilityRePairBallotBox
        {
            get { return visibilityRePairBallotBox; }
            set
            {
                visibilityRePairBallotBox = value;
                OnPropertyChanged("VisibilityRePairBallotBox");
            }
        }

        private Visibility visibilityUpdateConfiguration;
        public Visibility VisibilityUpdateConfiguration
        {
            get { return visibilityUpdateConfiguration; }
            set
            {
                visibilityUpdateConfiguration = value;
                OnPropertyChanged("VisibilityUpdateConfiguration");
            }
        }

        private Visibility configuredStation;
        public Visibility ConfiguredStation
        {
            get { return configuredStation; }
            set
            {
                configuredStation = value;
                OnPropertyChanged("ConfiguredStation");
            }
        }

        private Visibility searchStation;
        public Visibility SearchStation
        {
            get { return searchStation; }
            set
            {
                searchStation = value;
                OnPropertyChanged("SearchStation");
            }
        }

        private List<Station> _itens;
        public List<Station> Itens
        {
            get { return _itens; ; }
            set
            {
                _itens = value;
                OnPropertyChanged("Itens");
            }
        }

        #endregion

        #region Constructor

        public ConfirmationFinalViewModel()
        {
            RestoreConfiguration();
            RestorePairingStationConfiguration();

            if (StationHasNotConfigured())
            {
                ShowScreenLoading();

                if (IsSearchStation())
                {
                    ShowScreenSearchStation();
                }
                else
                {
                    SetTimerLoadFilteredVoters();
                }

                SetStationConfigured();

                if (IsVotingStationOfficer())
                {
                    SetUniqueIdentifierVotingStationOfficer();
                    SetIdBallotBox();
                }

                ReloadMainMenu();
                NavigateToNextScreen();
            }
            else
            {
                if (IsSearchStation())
                {
                    ShowScreenSearchStation();
                }
                else
                {
                    ShowScreenConfiguredStation();
                }

                if (ElectionFinalized())
                    ShowScreenFinalizedStation();

            }
        }

        #endregion

        #region Public Methods

        public void ResetConfiguration()
        {
            ParametersSingleton.Instance.BallotBoxes = null;
            ParametersSingleton.Instance.TypeStation = null;
            ParametersSingleton.Instance.StatusStation = null;
            ParametersSingleton.Instance.VotingStationOfficer = null;
            ParametersSingleton.Instance.Region = 0;
            ParametersSingleton.Instance.Section = 0;
            ParametersSingleton.Instance.Site = 0;
            ParametersSingleton.Instance.IPStation = null;
            ParametersSingleton.Instance.NicknameStation = null;
            ParametersSingleton.Instance.IdVotingStation = null;

            IFilteredVoter voter = FactoryService.getFilteredVoter();
            voter.Clear();

            log.Info("Station configuration reset");
        }

        public bool AskConfirmation()
        {
            DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Deseja refazer a configuração da estação? Os dados atuais serão perdidos.", "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                return true;
            }

            return false;
        }

        #endregion

        #region Private Methods

        private void ShowScreenFinalizedStation()
        {
            VisibilityUpdateConfiguration = Visibility.Collapsed;
            VisibilityRePairBallotBox = Visibility.Collapsed;
        }

        private bool ElectionFinalized()
        {
            return ParametersSingleton.Instance.StatusStation == StatusStation.FINALIZED;
        }

        private void NavigateToRePairBallotBox()
        {
            DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Tem certeza que deseja reparear urnas?", "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                INavigationService svc = NavigationServiceProvider.GetNavigationServiceInstance();
                svc.GetController("Wizard").Navigate("PairVotingStationOfficer", true);
            }
        }

        private void RestorePairingStationConfiguration()
        {
            var list = new List<Station>();

            if (ParametersSingleton.Instance.BallotBoxes != null)
            {
                foreach (var ballotbox in ParametersSingleton.Instance.BallotBoxes)
                {
                    list.Add(new Station() { IP = ballotbox.IP, HostName = ballotbox.HostName, NicknameStation = ballotbox.NicknameStation });
                }
            }

            if (ParametersSingleton.Instance.VotingStationOfficer != null)
            {
                list.Add(new Station() { IP = ParametersSingleton.Instance.VotingStationOfficer.IP, HostName = ParametersSingleton.Instance.VotingStationOfficer.HostName, NicknameStation = ParametersSingleton.Instance.VotingStationOfficer.NicknameStation });
            }

            Itens = list;
        }

        private void RestoreConfiguration()
        {
            if (ParametersSingleton.Instance.Region != 0)
                Region = FactoryService.getRegion().Get(Convert.ToInt32(ParametersSingleton.Instance.Region)).Name;

            if (ParametersSingleton.Instance.Site != 0)
                Site = FactoryService.getSite().Get(Convert.ToInt32(ParametersSingleton.Instance.Site)).Name;

            if (ParametersSingleton.Instance.Section != 0)
                Section = FactoryService.getSection().Get(Convert.ToInt32(ParametersSingleton.Instance.Section)).Name;

            Nickname = ParametersSingleton.Instance.NicknameStation;

            if (ParametersSingleton.Instance.TypeStation == TypeStation.VOTING_OFFICER)
            {
                StationType = "Terminal de mesário";
            }
            else if (ParametersSingleton.Instance.TypeStation == TypeStation.BALLOT_BOX)
            {
                StationType = "Urna (TVME)";
            }
            else if (ParametersSingleton.Instance.TypeStation == TypeStation.VOTER_SEARCH)
            {
                StationType = "Triagem (Consulta de eleitor)";
            }

            log.Info(string.Format("Confirmation Screen Region {0} Site {1} Section {2}", Region, Site, Section));
        }

        private void ReloadMainMenu()
        {
            if (menuScreen == null)
                menuScreen = GetMainScreenReference();

            menuScreen.SetStateMachine();
        }

        private static Main GetMainScreenReference()
        {
            INavigationService svc = NavigationServiceProvider.GetNavigationServiceInstance();
            MVVMC.Region region = (MVVMC.Region)svc.GetType().GetMethod("FindRegionByID", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(svc, new object[] { "Wizard" });
            FrameworkElement parent = (FrameworkElement)region.Parent;

            while (parent.GetType() != typeof(Main))
            {
                parent = (FrameworkElement)parent.Parent;
            }

            Main menuScreen = (Main)parent;
            return menuScreen;
        }

        private void NavigateToNextScreen()
        {
            if (menuScreen == null)
                menuScreen = GetMainScreenReference();

            menuScreen.NavigateToScreen();
        }

        private void SetIdBallotBox()
        {
            if (menuScreen == null)
                menuScreen = GetMainScreenReference();

            menuScreen.SetIdBallotBox();
        }

        private static void SetStationConfigured()
        {
            ParametersSingleton.Instance.StatusStation = Service.DTO.StatusStation.CONFIGURED;

            log.Info("Station configured");
        }

        private void FilterVoters()
        {
            IFilteredVoter voter = FactoryService.getFilteredVoter();
            voter.Populate(Convert.ToInt32(ParametersSingleton.Instance.Section));

            ShowScreenConfiguredStation();
            log.Info("Voters filtered by section");
        }

        private void SetTimerLoadFilteredVoters()
        {
            myTimer.Tick += new EventHandler(TimerEventProcessor);
            myTimer.Interval = 1000;
            myTimer.Start();
        }

        private void ShowScreenLoading()
        {
            Loading = Visibility.Visible;
            ConfiguredStation = Visibility.Collapsed;
            SearchStation = Visibility.Collapsed;
        }

        private void ShowScreenConfiguredStation()
        {
            Loading = Visibility.Collapsed;
            ConfiguredStation = Visibility.Visible;
            SearchStation = Visibility.Collapsed;
            VisibilityUpdateConfiguration = Visibility.Visible;

            if (ParametersSingleton.Instance.TypeStation == TypeStation.VOTING_OFFICER)
                VisibilityRePairBallotBox = Visibility.Visible;
            else
                VisibilityRePairBallotBox = Visibility.Collapsed;
        }

        private void ShowScreenSearchStation()
        {
            Loading = Visibility.Collapsed;
            ConfiguredStation = Visibility.Collapsed;
            SearchStation = Visibility.Visible;
            VisibilityUpdateConfiguration = Visibility.Visible;
        }

        private void TimerEventProcessor(Object myObject,
                                   EventArgs myEventArgs)
        {
            myTimer.Stop();
            Task.Run(() => FilterVoters());
        }

        private bool IsSearchStation()
        {
            return ParametersSingleton.Instance.TypeStation == TypeStation.VOTER_SEARCH;
        }

        private bool StationHasNotConfigured()
        {
            return !ParametersSingleton.Instance.StatusStation.HasValue;
        }

        private void SetUniqueIdentifierVotingStationOfficer()
        {
            ParametersSingleton.Instance.IdVotingStation = RandomIDUE.Generate(7);
            log.Info(string.Format("Setting IDUE {0}", ParametersSingleton.Instance.IdVotingStation));
        }

        private bool IsVotingStationOfficer()
        {
            return ParametersSingleton.Instance.TypeStation == Service.DTO.TypeStation.VOTING_OFFICER;
        }

        #endregion
    }
}