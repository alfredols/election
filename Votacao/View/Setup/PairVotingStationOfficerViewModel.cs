using MVVMC;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Votacao.Data;
using Votacao.Service.DTO;
using Votacao.SocketConn;
using Votacao.SocketConn.TCP;
using Votacao.SocketConn.UDP;

namespace Votacao.View.Setup
{
    public class PairVotingStationOfficerViewModel : MVVMCViewModel
    {
        #region Attributes

        private UDPServer server;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Commands

        private ICommand removeItemCommand;
        public ICommand RemoveItemCommand
        {
            get
            {
                if (removeItemCommand == null)
                    removeItemCommand = new DelegateCommand<string>((string ip) =>
                    {
                        RemoveItensBallotBox(ip);
                    },
                    (string ip) =>
                    {
                        return RemoveItemCommand != null;
                    });
                return removeItemCommand;
            }
        }

        #endregion

        #region Properties

        private ObservableCollection<BallotBox> _itens = new ObservableCollection<BallotBox>();
        public ObservableCollection<BallotBox> Itens
        {
            get { return _itens; }
            set
            {
                _itens = value;
                OnPropertyChanged("Itens");
            }
        }

        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                OnPropertyChanged("IsActive");
            }
        }

        private Visibility _visibilityGrid;
        public Visibility VisibilityGrid
        {
            get { return _visibilityGrid; }
            set
            {
                _visibilityGrid = value;
                OnPropertyChanged("VisibilityGrid");
            }
        }

        private Visibility _visibilityBackButton;
        public Visibility VisibilityBackButton
        {
            get { return _visibilityBackButton; }
            set
            {
                _visibilityBackButton = value;
                OnPropertyChanged("VisibilityBackButton");
            }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                OnPropertyChanged("ErrorMessage");
            }
        }

        #endregion

        #region Constructor

        public PairVotingStationOfficerViewModel()
        {
            ConfigureUDPServer();
            SetScreenLoading();
        }

        #endregion

        #region Public Methods

        public void StopUDPServer()
        {
            server.StopListening();
        }

        public bool ValidateScreen()
        {
            if (NoBallotBoxSelected())
            {
                ShowMessageErrorSelection();
                return false;
            }
            else
            {
                if (ThereIsBallotBoxNew())
                {
                    var ballotBoxesReceived = TryToConnectBallotBoxes();

                    if (ConnectedSuccessfully(ballotBoxesReceived))
                    {
                        RestoreBallotBoxAlreadyPaired(ballotBoxesReceived);
                        SaveConfiguration(ballotBoxesReceived);
                    }
                    else
                    {
                        return false;
                    }
                }
                else 
                {
                    return true;
                }
            }

            return true;
        }

        public override void Initialize()
        {
            IsRePairBallotBoxScreen();
        }

        #endregion

        #region Events

        private void Server_MessageReceived(Packet message)
        {
            var item = new Votacao.Service.DTO.BallotBox();
            item.IP = message.IPFrom;
            item.HostName = message.HostName;
            item.DateConnection = DateTime.Now.xToDateTimeString();

            IdentificationTerminal terminal = JsonConvert.DeserializeObject<IdentificationTerminal>(message.Message);
            item.NicknameStation = terminal.NicknameStation;

            if (FilterRegionSiteSection(terminal))
            {
                AddBallotBoxStoredToGrid();
                AddBallotBoxToGrid(item);
            }
        }

        #endregion

        #region Private Methods

        private bool ThereIsBallotBoxNew()
        {
            return Itens.Any(x => x.IsConnected && !x.IsPaired);
        }

        private void IsRePairBallotBoxScreen()
        {
            bool rePair = false;
            if (this.NavigationParameter != null)
                rePair = (bool)this.NavigationParameter;

            if (rePair)
            {
                AddBallotBoxStoredToGrid();
                VisibilityBackButton = Visibility.Collapsed;
                IsActive = false;
                VisibilityGrid = Visibility.Visible;
            }
            else
            {
                VisibilityBackButton = Visibility.Visible;
            }
        }

        private void RestoreBallotBoxAlreadyPaired(List<BallotBox> ballotBoxesReceived)
        {
            if (ParametersSingleton.Instance.BallotBoxes != null &&
                ParametersSingleton.Instance.BallotBoxes.Any(x => x.IsConnected && x.IsPaired))
            {
                List<BallotBox> itensStoredPaired = ParametersSingleton.Instance.BallotBoxes.Where(x => x.IsConnected && x.IsPaired).ToList();

                foreach (var bb in itensStoredPaired)
                {
                    if (!ballotBoxesReceived.Any(x => x.IP == bb.IP))
                        ballotBoxesReceived.Add(bb);
                }
            }
        }

        private void RemoveItensBallotBox(string ip)
        {
            DialogResult dialogResult = System.Windows.Forms.MessageBox.Show("Tem certeza que deseja remover esta urna?", "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (dialogResult == DialogResult.Yes)
            {
                Itens.Remove(Itens.First(x => x.IP == ip));
                SaveConfiguration(Itens.ToList());
            }
        }

        private void AddBallotBoxToGrid(BallotBox item)
        {
            log.Info(string.Format("Ballot box received: {0}", item.IP));

            App.Current.Dispatcher.Invoke((Action)delegate
            {
                if (!Itens.Any(x => x.IP == item.IP))
                {
                    IsActive = false;
                    VisibilityGrid = Visibility.Visible;
                    Itens.Add(item);
                }
            });
        }

        private void AddBallotBoxStoredToGrid()
        {
            if (ParametersSingleton.Instance.BallotBoxes != null)
            {
                foreach (var ballotbox in ParametersSingleton.Instance.BallotBoxes)
                {
                    App.Current.Dispatcher.Invoke((Action)delegate
                    {
                        if (!Itens.Any(x => x.IP == ballotbox.IP))
                        {
                            Itens.Add(ballotbox);
                        }
                    });
                }
            }
        }

        private bool FilterRegionSiteSection(IdentificationTerminal terminal)
        {
            return terminal.RegionId == ParametersSingleton.Instance.Region.ToString() &&
                           terminal.SiteId == ParametersSingleton.Instance.Site.ToString() &&
                           terminal.SectionId == ParametersSingleton.Instance.Section.ToString();
        }

        private bool ConnectedSuccessfully(List<BallotBox> ballotBoxesReceived)
        {
            return ballotBoxesReceived != null
                && ballotBoxesReceived.Any()
                && !ballotBoxesReceived.Any(x => x.IsPaired == false);
        }

        private List<BallotBox> TryToConnectBallotBoxes()
        {
            List<BallotBox> ballotBoxesReceived = new List<BallotBox>();
            TCPClient client = new TCPClient();

            bool failedConnection = false;

            foreach (var item in Itens.Where(x => x.IsConnected && !x.IsPaired))
            {
                var packetReceived = client.SendMessage(new Packet() { IP = item.IP, PacketType = PacketType.DISCOVERY }, 23001);

                if (packetReceived != null
                    && packetReceived.PacketType == PacketType.CONFIRMATION)
                {
                    item.IsPaired = true;
                    ballotBoxesReceived.Add(item);
                    log.Info(string.Format("Voting Station connected with ballot box: {0}", item.IP));
                }
                else
                {
                    failedConnection = true;
                    item.IsPaired = false;
                    ballotBoxesReceived.Add(item);
                    log.Error(string.Format("Error trying to connect to ballot box: {0}", item.IP));
                }
            }

            if (failedConnection)
            {
                ShowMessageErrorConnection();
            }

            return ballotBoxesReceived;
        }

        private void ShowMessageErrorConnection()
        {
            ErrorMessage = "Erro ao conectar com o terminal";
        }

        private void ConfigureUDPServer()
        {
            server = new UDPServer();
            server.MessageReceived += Server_MessageReceived;
            server.StartListening();
        }

        private void SaveConfiguration(List<BallotBox> ballotBoxes)
        {
            ParametersSingleton.Instance.BallotBoxes = ballotBoxes;
        }

        private void SetScreenLoading()
        {
            IsActive = true;
            VisibilityGrid = Visibility.Hidden;
            ErrorMessage = "";
        }

        private bool NoBallotBoxSelected()
        {
            return !this.Itens.Where(x => x.IsConnected).Any();
        }

        private void ShowMessageErrorSelection()
        {
            ErrorMessage = "Por favor, selecione algum terminal";
        }

        #endregion
    }
}
