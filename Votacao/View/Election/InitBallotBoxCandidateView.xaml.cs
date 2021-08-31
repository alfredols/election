using MVVMC;
using Newtonsoft.Json;
using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Votacao.Data;
using Votacao.Service.DTO;
using Votacao.SocketConn;
using Votacao.SocketConn.TCP;

namespace Votacao.View.Election
{
    public partial class InitBallotBoxCandidateView : UserControl
    {

        #region Attributes

        private TCPServer server;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private int countVotes;
        private double fiveMinutes = 5 * 60 * 1000;

        #endregion

        #region Constructor

        public InitBallotBoxCandidateView()
        {
            InitializeComponent();

            CreateTCPServer();

            UnloadFormEvent();

            SetPastingHandlerTextBoxes();

            HideShowMenu(true);
        }

        #endregion

        #region Properties

        private InitBallotBoxCandidateViewModel ViewModel
        {
            get;
            set;
        }

        #endregion

        #region Events

        private void InitBallotBoxView_Unloaded(object sender, RoutedEventArgs e)
        {
            server.StopListening();
        }

        private Packet Server_MessageReceived(Packet packet)
        {
            GetViewModel();

            Packet packetResult = new Packet();

            if (IsMessageCheckBallotBox(packet))
            {
                ReturnStatusBallotBox(packetResult);
            }

            if (IsMessageToOpenForVote(packet))
            {
                OpenBallotBoxForVote(packet);
            }

            if (IsMessageElectionFinished(packet))
            {
                ShowElectionFinished(packet);
            }

            if (IsMessageToCancelVote(packet))
            {
                CancelBallotBoxVote(packet);
            }

            if (IsMessageToOpenSettings(packet))
            {
                OpenBallotBoxSettings(packet);
            }

            return packetResult;
        }

        private void tb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = AcceptOnlyNumbers(e.Text);
        }

        private void tb_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }

            if (e.Key == Key.Enter)
            {
                ValidateCandidateNumber();
            }

            if (e.Key == Key.Subtract)
            {
                ClearScreen();
                SetFocusFirstBox();
            }

            if (e.Key == Key.Multiply)
            {
                SetWhiteVote();
            }
        }

        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            e.CancelCommand();
        }

        private void tb_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            string text = ((TextBox)sender).Text;

            if (text.Length == 1)
            {
                string name = ((TextBox)sender).Name;

                if (name == "tbFirst")
                {
                    if (tbSecond.Visibility == Visibility.Visible)
                    {
                        tbSecond.Focus();
                    }
                    else
                    {
                        LoadCandidate();
                    }
                }

                if (name == "tbSecond")
                {
                    if (tbThird.Visibility == Visibility.Visible)
                    {
                        tbThird.Focus();
                    }
                    else
                    {
                        LoadCandidate();
                    }
                }

                if (name == "tbThird")
                {
                    if (tbFourth.Visibility == Visibility.Visible)
                    {
                        tbFourth.Focus();
                    }
                    else
                    {
                        LoadCandidate();
                    }
                }

                if (name == "tbFourth")
                {
                    LoadCandidate();
                }
            }

            if (e.Key == Key.Back)
            {
                string name = ((TextBox)sender).Name;

                if (name == "tbSecond")
                    tbFirst.Focus();

                if (name == "tbThird")
                    tbSecond.Focus();

                if (name == "tbFourth")
                    tbThird.Focus();
            }
        }

        #endregion

        #region Private Methods

        private void HideShowMenu(bool hide)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                INavigationService svc = NavigationServiceProvider.GetNavigationServiceInstance();
                MVVMC.Region region = (MVVMC.Region)svc.GetType().GetMethod("FindRegionByID", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(svc, new object[] { "Election" });
                FrameworkElement parent = (FrameworkElement)region.Parent;

                while (parent.GetType() != typeof(Main))
                {
                    parent = (FrameworkElement)parent.Parent;
                }

                Main menuScreen = (Main)parent;

                if (hide)
                    menuScreen.HideMenu();
                else
                    menuScreen.ShowMenu();
            }));
        }

        private void CreateTCPServer()
        {
            server = new TCPServer(23001);
            server.MessageReceived += Server_MessageReceived;
            server.StartListening();
        }

        private void SetPastingHandlerTextBoxes()
        {
            DataObject.AddPastingHandler(tbFirst, OnPaste);
            DataObject.AddPastingHandler(tbSecond, OnPaste);
            DataObject.AddPastingHandler(tbThird, OnPaste);
            DataObject.AddPastingHandler(tbFourth, OnPaste);
        }

        private void UnloadFormEvent()
        {
            this.Unloaded += InitBallotBoxView_Unloaded;
        }

        private void OpenBallotBoxForVote(Packet packet)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                ViewModel.Voter = JsonConvert.DeserializeObject<Voter>(packet.Message);
                log.Info(string.Format("Opened for vote id {0}", ViewModel.Voter.Id));
                ViewModel.Loading = Visibility.Collapsed;
                ViewModel.Voting = Visibility.Visible;
                HideShowMenu(true);
                ClearScreen();
                SetFocusFirstBox();
                CreateCloseVoteScreen();
            }));
        }

        private void CreateCloseVoteScreen()
        {
            var timer = new System.Timers.Timer(fiveMinutes) { AutoReset = false };
            timer.Elapsed += delegate
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    ClearScreen();
                    ViewModel.ShowScreenLoading();
                }));
            };
            timer.Enabled = true;
        }

        private void CancelBallotBoxVote(Packet packet)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                ClearScreen();
                ViewModel.ShowScreenLoading();
            }));
        }

        private void ShowElectionFinished(Packet packet)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                HideShowMenu(true);
                ViewModel.ShowElectionFinishedScreen();
                ViewModel.ChangeStatusFinished();
            }));
        }

        private void ShowBallotBoxLoading(Packet packet)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                HideShowMenu(true);
                ViewModel.ShowScreenLoading();
                ViewModel.ChangeStatusInitialized();
            }));
        }

        private void OpenBallotBoxSettings(Packet packet)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                HideShowMenu(false);
                CreateTimerCloseMenu();
            }));
        }

        private void CreateTimerCloseMenu()
        {
            var timer = new System.Timers.Timer(fiveMinutes) { AutoReset = false };
            timer.Elapsed += delegate
            {
                HideShowMenu(true);
            };
            timer.Enabled = true;
        }

        private bool AcceptOnlyNumbers(string text)
        {
            return Regex.IsMatch(text, "[^0-9]+");
        }

        private void ValidateCandidateNumber()
        {
            if (WhiteVote()
                || NullVote()
                || (GetCandidateNumber().HasValue
                    && GetCandidateNumber().Value.ToString().Length == ViewModel.MaxCandidateNumber))
            {
                if (ViewModel.SaveVote(GetCandidateNumber()))
                {
                    ViewModel.PlayConfirmationSound();
                    SetCountOfVotes();

                    if (ReachNumberOfVoterPerVoter())
                    {
                        ViewModel.ShowScreenAcknowledge();
                    }
                }
                else
                {
                    ViewModel.ShowAutoClosingMessageBoxError("Erro tente novamente, por favor.", "Erro");
                }

                ClearScreen();
            }
        }

        private bool ReachNumberOfVoterPerVoter()
        {
            return countVotes == ParametersSingleton.Instance.VotesPerVoter;
        }

        private void SetFocusFirstBox()
        {
            tbFirst.Focus();
        }

        private void SetCountOfVotes()
        {
            if (countVotes < ParametersSingleton.Instance.VotesPerVoter)
                countVotes++;
            else
                countVotes = 1;
        }

        private bool NullVote()
        {
            string vote = tbFirst.Text + tbSecond.Text + tbThird.Text + tbFourth.Text;
            return ViewModel.Nickname == "Voto Nulo"
                && string.IsNullOrEmpty(vote);
        }

        private bool WhiteVote()
        {
            string vote = tbFirst.Text + tbSecond.Text + tbThird.Text + tbFourth.Text;
            return ViewModel.Nickname == "Voto Branco"
                && string.IsNullOrEmpty(vote);
        }

        private void LoadCandidate()
        {
            int? cand = GetCandidateNumber();

            if (cand.HasValue)
                ViewModel.LoadCandidate(cand.Value);
        }

        private int? GetCandidateNumber()
        {
            if (NullVote())
                return null;

            if (WhiteVote())
                return 0;

            string vote = tbFirst.Text + tbSecond.Text + tbThird.Text + tbFourth.Text;
            return Convert.ToInt32(string.IsNullOrEmpty(vote) ? "0" : vote);
        }

        private bool IsMessageToOpenForVote(Packet packet)
        {
            return packet.PacketType == PacketType.OPEN_VOTE;
        }

        private bool IsMessageElectionFinished(Packet packet)
        {
            return packet.PacketType == PacketType.ELECTION_FINISHED;
        }

        private bool IsMessageToCancelVote(Packet packet)
        {
            return packet.PacketType == PacketType.CANCEL_VOTE;
        }

        private bool IsMessageToOpenSettings(Packet packet)
        {
            return packet.PacketType == PacketType.OPEN_SETTINGS;
        }

        private void ReturnStatusBallotBox(Packet packetResult)
        {
            if (ViewModel.Loading == Visibility.Visible)
            {
                packetResult.PacketType = PacketType.AVAILABLE;
            }
            else
            {
                packetResult.PacketType = PacketType.UNAVAILABLE;

                if(ViewModel != null
                    && ViewModel.Voter != null)
                    packetResult.Message = ViewModel.Voter.Name;
            }
        }

        private bool IsMessageCheckBallotBox(Packet packet)
        {
            return packet.PacketType == PacketType.CONFIRMATION;
        }

        private void SetWhiteVote()
        {
            ClearScreen();
            ViewModel.Nickname = "Voto Branco";
        }

        private void ClearScreen(bool focus = true)
        {
            ViewModel.ClearFields();
            tbFirst.Text = string.Empty;
            tbSecond.Text = string.Empty;
            tbThird.Text = string.Empty;
            tbFourth.Text = string.Empty;
            tbFirst.Focus();
        }

        private void FocusMenuElectionItem()
        {
            INavigationService svc = NavigationServiceProvider.GetNavigationServiceInstance();
            MVVMC.Region region = (MVVMC.Region)svc.GetType().GetMethod("FindRegionByID", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(svc, new object[] { "Election" });
            FrameworkElement parent = (FrameworkElement)region.Parent;

            while (parent.GetType() != typeof(Main))
            {
                parent = (FrameworkElement)parent.Parent;
            }

            Main menuScreen = (Main)parent;

            menuScreen.FocusMenu();
        }

        private void GetViewModel()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                INavigationService svc = NavigationServiceProvider.GetNavigationServiceInstance();
                ViewModel = (InitBallotBoxCandidateViewModel)svc.GetCurrentViewModelByControllerID("Election");
            }));
        }

        #endregion

    }
}
