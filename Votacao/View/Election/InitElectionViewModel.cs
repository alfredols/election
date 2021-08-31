using MVVMC;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Votacao.Data;
using Votacao.Service;
using Votacao.Service.DTO;
using Votacao.Service.Interface;
using Votacao.SocketConn;
using Votacao.SocketConn.TCP;

namespace Votacao.View.Election
{
    public class InitElectionViewModel : MVVMCViewModel
    {

        #region Attributes

        private IFilteredVoter voterService;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private System.Timers.Timer timerGrid;

        #endregion


        #region Commands

        private ICommand newSearchCommand;
        public ICommand NewSearchCommand
        {
            get
            {
                if (newSearchCommand == null)
                    newSearchCommand = new DelegateCommand(() =>
                    {
                        HideResult();
                        ClearFields();
                    },
                    () =>
                    {
                        return NewSearchCommand != null;
                    });
                return newSearchCommand;
            }
        }

        private ICommand sendVoterCommand;
        public ICommand SendVoterCommand
        {
            get
            {
                if (sendVoterCommand == null)
                    sendVoterCommand = new DelegateCommand(() =>
                    {
                        SendVoter();
                    },
                    () =>
                    {
                        return SendVoterCommand != null;
                    });
                return sendVoterCommand;
            }
        }

        private ICommand searchVoterCommand;
        public ICommand SearchVoterCommand
        {
            get
            {
                if (searchVoterCommand == null)
                    searchVoterCommand = new DelegateCommand(() =>
                    {
                        bool sendVoter = ValidateScreen();
                        ShowResult(sendVoter);
                    },
                    () =>
                    {
                        return SearchVoterCommand != null;
                    });
                return searchVoterCommand;
            }
        }

        private ICommand cancelVotingCommand;
        public ICommand CancelVotingCommand
        {
            get
            {
                if (cancelVotingCommand == null)
                    cancelVotingCommand = new DelegateCommand<string>((string ip) =>
                    {
                        CancelVoting(ip);
                        HideResult();
                        ClearFields();
                    },
                    (string ip) =>
                    {
                        return CancelVotingCommand != null;
                    });
                return cancelVotingCommand;
            }
        }

        private ICommand openSettingsBallotBoxCommand;
        public ICommand OpenSettingsBallotBoxCommand
        {
            get
            {
                if (openSettingsBallotBoxCommand == null)
                    openSettingsBallotBoxCommand = new DelegateCommand<string>((string ip) =>
                    {
                        OpenBallotBoxScreen(ip);
                    },
                    (string ip) =>
                    {
                        return OpenSettingsBallotBoxCommand != null;
                    });
                return openSettingsBallotBoxCommand;
            }
        }

        #endregion

        #region Construtor

        public InitElectionViewModel()
        {
            Voter = new Voter();
            voterService = FactoryService.getFilteredVoter();
            HideResult();
            TryConnectToBallotBoxes(ParametersSingleton.Instance.BallotBoxes);
            CreateTimerUpdateBallotBoxes();
        }

        #endregion

        #region Properties

        private ObservableCollection<ShowClientModel> _itens = new ObservableCollection<ShowClientModel>();
        public ObservableCollection<ShowClientModel> Itens
        {
            get { return _itens; }
            set
            {
                _itens = value;
                OnPropertyChanged("Itens");
            }
        }

        private Voter voter;
        public Voter Voter
        {
            get { return voter; }
            set
            {
                voter = value;
                OnPropertyChanged("Voter");
            }
        }

        private string electorNumber;
        public string ElectorNumber
        {
            get { return electorNumber; }
            set
            {
                electorNumber = value;
                OnPropertyChanged("ElectorNumber");
            }
        }

        private string messageSearch;
        public string MessageSearch
        {
            get { return messageSearch; }
            set
            {
                messageSearch = value;
                OnPropertyChanged("MessageSearch");
            }
        }

        private Visibility visibilityResult;
        public Visibility VisibilityResult
        {
            get { return visibilityResult; }
            set
            {
                visibilityResult = value;
                OnPropertyChanged("VisibilityResult");
            }
        }

        private Visibility visibilityFilter;
        public Visibility VisibilityFilter
        {
            get { return visibilityFilter; }
            set
            {
                visibilityFilter = value;
                OnPropertyChanged("VisibilityFilter");
            }
        }

        private Visibility visibilitySendVoter;
        public Visibility VisibilitySendVoter
        {
            get { return visibilitySendVoter; }
            set
            {
                visibilitySendVoter = value;
                OnPropertyChanged("VisibilitySendVoter");
            }
        }

        private Visibility visibilityPanelVoter;
        public Visibility VisibilityPanelVoter
        {
            get { return visibilityPanelVoter; }
            set
            {
                visibilityPanelVoter = value;
                OnPropertyChanged("VisibilityPanelVoter");
            }
        }

        #endregion

        #region Private Methods

        private void CancelVoting(string ip)
        {
            MessageBoxResult result = MessageBox.Show("Tem certeza que deseja cancelar a tela de votação do eleitor?", "Confirmação", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes) 
            {
                Packet packetReceived = SendTCPRequestBallotBoxCloseVote(ip);
                TryConnectToBallotBoxes(ParametersSingleton.Instance.BallotBoxes);
            }
        }

        private void OpenBallotBoxScreen(string ip)
        {
            MessageBoxResult result = MessageBox.Show("Tem certeza que deseja abrir a tela de configuração na urna?", "Confirmação", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Packet packetReceived = SendTCPRequestBallotBoxOpenSettings(ip);
            }
        }

        private void SendVoter()
        {
            var itens = GetBallotBoxSeleted();

            if (HasBallotBoxSelected(itens))
            {
                BallotBox ballotBox = CreateBallotBoxModel(itens);

                Packet packetReceived = SendTCPRequestBallotBoxOpenVote(ballotBox, this.Voter);

                string msgSendElector = GetMessageFromPacketBallotBox(packetReceived);

                log.Info(string.Format("Ballot box message returned: {0}", msgSendElector));

                MessageBox.Show(msgSendElector);

                HideResult();
                ClearFields();
            }
            else
            {
                MessageBox.Show("É preciso selecionar um terminal");
                log.Info("Ballot box not selected");
            }
        }

        private List<ShowClientModel> GetBallotBoxSeleted()
        {
            return this.Itens.Where(x => x.IsChecked).ToList();
        }

        private bool HasBallotBoxSelected(List<ShowClientModel> itens)
        {
            return itens != null && itens.Any();
        }

        private BallotBox CreateBallotBoxModel(List<ShowClientModel> itens)
        {
            return new BallotBox
            {
                IP = itens[0].IP,
                SendElector = true
            };
        }

        private string GetMessageFromPacketBallotBox(Packet packetReceived)
        {
            return packetReceived == null || packetReceived.PacketType == PacketType.DISCONNECTED
                                ? "Atenção: O Eleitor não pode ser enviado!"
                                : "Eleitor enviado com sucesso";
        }

        private Packet SendTCPRequestBallotBoxOpenSettings(string ip)
        {
            TCPClient client = new TCPClient();
            var packetReceived = client.SendMessage(new Packet() { IP = ip, PacketType = PacketType.OPEN_SETTINGS, Message = string.Empty }, 23001);
            return packetReceived;
        }

        private Packet SendTCPRequestBallotBoxCloseVote(string ip)
        {
            TCPClient client = new TCPClient();
            var packetReceived = client.SendMessage(new Packet() { IP = ip, PacketType = PacketType.CANCEL_VOTE, Message = string.Empty }, 23001);
            return packetReceived;
        }

        private Packet SendTCPRequestBallotBoxOpenVote(BallotBox ballotBox, Voter voter)
        {
            TCPClient client = new TCPClient();
            string message = JsonConvert.SerializeObject(voter);
            var packetReceived = client.SendMessage(new Packet() { IP = ballotBox.IP, PacketType = PacketType.OPEN_VOTE, Message = message }, 23001);
            log.Info(string.Format("Try to open ballot box for vote IP: {0} Message: {1}", ballotBox.IP, message));
            return packetReceived;
        }

        private void CreateTimerUpdateBallotBoxes()
        {
            timerGrid = new System.Timers.Timer(2000);
            timerGrid.Elapsed += delegate
            {
                TryConnectToBallotBoxes(ParametersSingleton.Instance.BallotBoxes);
            };
            timerGrid.Enabled = true;
        }

        private bool ValidateScreen()
        {
            if (!ValidateElectorNumber())
            {
                ShowErrorElectorNumber();
                log.Info(string.Format("Number elector is wrong {0}", ElectorNumber));
                return false;
            }
            else
            {
                Voter = voterService.Get(RemovePunctuations(ElectorNumber), Convert.ToInt32(ParametersSingleton.Instance.Section));

                if (Voter == null)
                {
                    ShowErrorElectorNotFound();
                    log.Info(string.Format("Elector not found {0}", ElectorNumber));
                    return false;
                }
                else if (ElectorHasVoted())
                {
                    ShowErrorElectorHasVoted();
                    log.Info(string.Format("Elector has voted {0}", ElectorNumber));
                    return false;
                }
                else 
                {
                    ShowElectorOK();
                    return true;
                }
            }
        }

        private void ShowElectorOK()
        {
            MessageSearch = "Pendente votação";
            VisibilityPanelVoter = Visibility.Visible;
        }

        private void ShowErrorElectorHasVoted()
        {
            MessageSearch = "Eleitor já votou";
            VisibilityPanelVoter = Visibility.Visible;
        }

        private void ShowErrorElectorNotFound()
        {
            MessageSearch = "Eleitor não consta para votação nessa seção";
            VisibilityPanelVoter = Visibility.Visible;
        }

        private void ShowErrorElectorNumber()
        {
            MessageSearch = "Título de eleitor inválido";
            VisibilityPanelVoter = Visibility.Collapsed;
        }

        private void ClearFields()
        {
            ElectorNumber = string.Empty;
            Voter = null;
        }

        private void TryConnectToBallotBoxes(List<BallotBox> ballots)
        {
            if (HasBallotBoxes(ballots))
            {
                foreach (var model in ballots)
                {
                    log.Info(string.Format("Trying to TCP connect to ballot box IP: {0} Nickname: {1} Host: {2}", model.IP, model.NicknameStation, model.HostName));

                    Packet packetReceived = SendTCPRequestBallotBoxStatus(model);

                    ShowClientModel view = CreateShowClientModel(model);

                    GetStatusBallotBox(packetReceived, view);

                    AddToGrid(view);
                }
            }
        }

        private void GetStatusBallotBox(Packet packetReceived, ShowClientModel view)
        {
            if (packetReceived == null)
            {
                BallotBoxUnreachable(view);
            }
            else
            {
                switch (packetReceived.PacketType)
                {
                    case PacketType.AVAILABLE:
                        BallotBoxAvailable(view);
                        break;
                    case PacketType.UNAVAILABLE:
                        BallotBoxUnavailable(view, packetReceived.Message);
                        break;
                }
            }
        }

        private void BallotBoxUnavailable(ShowClientModel view, string voter)
        {
            view.StatusUrn = "Indisponível";
            view.ColorIcon = "Gray";
            view.NameIcon = "Computer";
            view.IsEnabled = false;
            view.IsChecked = false;
            view.IsOpenSettingsEnabled = false;
            view.IsCancelVoteEnabled = true;
            view.VoterName = voter;
            log.Info(string.Format("Unable to connect to ballot box IP: {0} Nickname: {1} Host: {2}", view.IP, view.NicknameStation, view.HostName));
        }

        private void BallotBoxAvailable(ShowClientModel view)
        {
            view.StatusUrn = "Disponível";
            view.NameIcon = "Computer";
            view.ColorIcon = "#226096";

            if (VisibilitySendVoter == System.Windows.Visibility.Visible)
            {
                view.IsOpenSettingsEnabled = false;
                view.IsEnabled = true;
            }
            else
            {
                view.IsOpenSettingsEnabled = true;
                view.IsChecked = false;
                view.IsEnabled = false;
            }

            view.IsCancelVoteEnabled = false;

            log.Info(string.Format("Ballot box available IP: {0} Nickname: {1} Host: {2}", view.IP, view.NicknameStation, view.HostName));
        }

        private void BallotBoxUnreachable(ShowClientModel view)
        {
            view.StatusUrn = "Não foi possível conectar";
            view.NameIcon = "Error";
            view.ColorIcon = "Red";
            view.IsChecked = false;
            view.IsEnabled = false;
            view.IsOpenSettingsEnabled = false;
            view.IsCancelVoteEnabled = false;
            log.Info(string.Format("Ballot box unavailable IP: {0} Nickname: {1} Host: {2}", view.IP, view.NicknameStation, view.HostName));
        }

        private void AddToGrid(ShowClientModel view)
        {
            if (App.Current != null)
            {
                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    var itemList = Itens.FirstOrDefault(i => i.IP == view.IP);

                    if (itemList != null)
                    {
                        if (VisibilitySendVoter == System.Windows.Visibility.Visible)
                        {
                            view.IsChecked = itemList.IsChecked;
                        }
                        else 
                        {
                            itemList.IsChecked = view.IsChecked;
                        }

                        itemList.ColorIcon = view.ColorIcon;
                        itemList.IsCancelVoteEnabled = view.IsCancelVoteEnabled;
                        itemList.IsEnabled = view.IsEnabled;
                        itemList.IsOpenSettingsEnabled = view.IsOpenSettingsEnabled;
                        itemList.NameIcon = view.NameIcon;
                        itemList.StatusUrn = view.StatusUrn;
                        itemList.VoterName = view.VoterName;
                    }
                    else 
                    {
                        Itens.Add(view);
                    }
                });
            }
        }

        private bool HasBallotBoxes(List<BallotBox> ballots)
        {
            return ballots != null;
        }

        private ShowClientModel CreateShowClientModel(BallotBox model)
        {
            return new ShowClientModel {
                IP = model.IP,
                NicknameStation = model.NicknameStation,
                HostName = model.HostName
            };
        }

        private Packet SendTCPRequestBallotBoxStatus(BallotBox model)
        {
            TCPClient client = new TCPClient();
            var packetReceived = client.SendMessage(new Packet() { IP = model.IP, PacketType = PacketType.CONFIRMATION }, 23001);
            return packetReceived;
        }

        private string RemovePunctuations(string electorNumber)
        {
            return electorNumber.Replace(".", string.Empty);
        }

        private bool ValidateElectorNumber()
        {
            if (!string.IsNullOrEmpty(ElectorNumber))
            {
                if (ElectorNumber.Length < 14)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        private void HideResult()
        {
            VisibilityResult = Visibility.Collapsed;
            VisibilityFilter = Visibility.Visible;
            VisibilitySendVoter = Visibility.Collapsed;

            foreach (var item in Itens)
            {
                item.IsEnabled = false;
            }
        }

        private bool ElectorHasVoted()
        {
            return Voter.HasVoted.HasValue
                                    && Voter.HasVoted.Value;
        }

        private void ShowResult(bool sendVoter)
        {
            VisibilityResult = Visibility.Visible;
            VisibilityFilter = Visibility.Collapsed;

            if (sendVoter)
            {
                VisibilitySendVoter = Visibility.Visible;

                foreach (var item in Itens)
                {
                    item.IsEnabled = true;
                }
            }
        }

        #endregion

    }

    #region ShowClientModel

    public class ShowClientModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string ip;
        public string IP 
        { 
            get
            {
                return ip;
            } 
            set 
            {
                ip = value;
                OnPropertyChanged("IP");
            } 
        }

        private string hostName;
        public string HostName
        {
            get
            {
                return hostName;
            }
            set
            {
                hostName = value;
                OnPropertyChanged("HostName");
            }
        }

        private string nicknameStation;
        public string NicknameStation 
        {
            get
            {
                return nicknameStation;
            }
            set
            {
                nicknameStation = value;
                OnPropertyChanged("NicknameStation");
            }
        }

        private string statusUrn;
        public string StatusUrn 
        {
            get
            {
                return statusUrn;
            }
            set
            {
                statusUrn = value;
                OnPropertyChanged("StatusUrn");
            }
        }

        private string nameIcon;
        public string NameIcon 
        {
            get
            {
                return nameIcon;
            }
            set
            {
                nameIcon = value;
                OnPropertyChanged("NameIcon");
            }
        }

        private string voterName;
        public string VoterName
        {
            get
            {
                return voterName;
            }
            set
            {
                voterName = value;
                OnPropertyChanged("VoterName");
            }
        }

        private string colorIcon;
        public string ColorIcon 
        {
            get
            {
                return colorIcon;
            }
            set
            {
                colorIcon = value;
                OnPropertyChanged("ColorIcon");
            }
        }

        private bool isEnabled;
        public bool IsEnabled 
        {
            get
            {
                return isEnabled;
            }
            set
            {
                isEnabled = value;
                OnPropertyChanged("IsEnabled");
            }
        }

        private bool isChecked;
        public bool IsChecked 
        {
            get
            {
                return isChecked;
            }
            set
            {
                isChecked = value;
                OnPropertyChanged("IsChecked");
            }
        }

        private bool isCancelVoteEnabled;
        public bool IsCancelVoteEnabled
        {
            get
            {
                return isCancelVoteEnabled;
            }
            set
            {
                isCancelVoteEnabled = value;
                OnPropertyChanged("IsCancelVoteEnabled");
            }
        }

        private bool isOpenSettingsEnabled;
        public bool IsOpenSettingsEnabled
        {
            get
            {
                return isOpenSettingsEnabled;
            }
            set
            {
                isOpenSettingsEnabled = value;
                OnPropertyChanged("IsOpenSettingsEnabled");
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    #endregion
}
