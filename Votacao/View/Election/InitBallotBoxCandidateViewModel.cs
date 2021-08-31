using MVVMC;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Imaging;
using Votacao.Data;
using Votacao.Service;
using Votacao.Service.DTO;
using Votacao.Service.Interface;
using Votacao.SocketConn;
using Votacao.SocketConn.TCP;

namespace Votacao.View.Election
{
    public class InitBallotBoxCandidateViewModel : MVVMCViewModel
    {

        #region Attributes

        private ICandidate candidateService = null;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Properties

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

        private string type;
        public string Type
        {
            get { return type; }
            set
            {
                type = value;
                OnPropertyChanged("Type");
            }
        }

        private string nickname;
        public string Nickname
        {
            get { return nickname; }
            set
            {
                nickname = value;
                OnPropertyChanged("Nickname");
            }
        }

        private string electionName;
        public string ElectionName
        {
            get { return electionName; }
            set
            {
                electionName = value;
                OnPropertyChanged("ElectionName");
            }
        }

        private BitmapImage imageCandidate;
        public BitmapImage ImageCandidate
        {
            get { return imageCandidate; }
            set
            {
                imageCandidate = value;
                OnPropertyChanged("ImageCandidate");
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

        private Visibility voting;
        public Visibility Voting
        {
            get { return voting; }
            set
            {
                voting = value;
                OnPropertyChanged("Voting");
            }
        }

        private Visibility acknowledgment;
        public Visibility Acknowledgment
        {
            get { return acknowledgment; }
            set
            {
                acknowledgment = value;
                OnPropertyChanged("Acknowledgment");
            }
        }

        private Visibility finished;
        public Visibility Finished
        {
            get { return finished; }
            set
            {
                finished = value;
                OnPropertyChanged("Finished");
            }
        }

        private Visibility visibilitySecondField;
        public Visibility VisibilitySecondField
        {
            get { return visibilitySecondField; }
            set
            {
                visibilitySecondField = value;
                OnPropertyChanged("VisibilitySecondField");
            }
        }

        private Visibility visibilityThirdField;
        public Visibility VisibilityThirdField
        {
            get { return visibilityThirdField; }
            set
            {
                visibilityThirdField = value;
                OnPropertyChanged("VisibilityThirdField");
            }
        }

        private Visibility visibilityFourthField;
        public Visibility VisibilityFourthField
        {
            get { return visibilityFourthField; }
            set
            {
                visibilityFourthField = value;
                OnPropertyChanged("VisibilityFourthField");
            }
        }

        public int MaxCandidateNumber { get; set; }

        #endregion

        #region Constructor

        public InitBallotBoxCandidateViewModel()
        {
            if (ElectionFinished())
            {
                ShowElectionFinishedScreen();
            }
            else
            {
                GetMaxDigitCandidateNumber();
                SetControlsMaxDigit(MaxCandidateNumber);
                ShowScreenLoading();
                SetTypeVoting();
                SetElectionName();
            }
        }

        #endregion

        #region Public Methods

        public void ClearFields()
        {
            Nickname = string.Empty;
            ImageCandidate = null;
        }

        public bool SaveVote(int? candidateNumber)
        {
            int? candidateId = GetCandidateId(candidateNumber);

            SaveVoteDatabase(candidateId);

            string message = CreateVoteMessage(candidateId);

            Packet packetReceived = SendVoteToVotingStation(message);

            return IsVoteReceived(packetReceived);
        }

        public void LoadCandidate(int number)
        {
            Candidate candidate = candidateService.GetValidByNumber(number, Convert.ToInt32(ParametersSingleton.Instance.Region));

            log.Info(string.Format("Loaded candidate number {0}", number));

            if (IsValid(candidate))
            {
                Nickname = candidate.Nickname;
                ImageCandidate = ConvertByteToImage(candidate.Picture);
            }
            else
            {
                Nickname = "Voto Nulo";
                ImageCandidate = null;
            }
        }

        public void ShowScreenAcknowledge()
        {
            Loading = Visibility.Collapsed;
            Voting = Visibility.Collapsed;
            Acknowledgment = Visibility.Visible;

            var timer = new System.Timers.Timer(5000) { AutoReset = false };
            timer.Elapsed += delegate
            {
                ShowScreenLoading();
            };
            timer.Enabled = true;
        }

        public void PlayConfirmationSound()
        {
            var sri = Application.GetResourceStream(new Uri("pack://application:,,,/view/sound/confirmation.wav"));

            if ((sri != null))
            {
                using (var s = sri.Stream)
                {
                    System.Media.SoundPlayer player = new System.Media.SoundPlayer(s);
                    player.Load();
                    player.Play();
                }
            }
        }

        public void ShowAutoClosingMessageBoxError(string message, string caption)
        {
            var timer = new System.Timers.Timer(5000) { AutoReset = false };
            timer.Elapsed += delegate
            {
                IntPtr hWnd = FindWindowByCaption(IntPtr.Zero, caption);
                if (hWnd.ToInt32() != 0) PostMessage(hWnd, WM_CLOSE, 0, 0);
            };
            timer.Enabled = true;
            MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void ShowScreenLoading()
        {
            Loading = Visibility.Visible;
            Voting = Visibility.Collapsed;
            Acknowledgment = Visibility.Collapsed;
            Finished = Visibility.Collapsed;
        }

        public void ShowElectionFinishedScreen()
        {
            Loading = Visibility.Collapsed;
            Voting = Visibility.Collapsed;
            Acknowledgment = Visibility.Collapsed;
            Finished = Visibility.Visible;
        }

        public void ChangeStatusFinished()
        {
            ParametersSingleton.Instance.StatusStation = StatusStation.FINALIZED;
        }

        public void ChangeStatusInitialized()
        {
            if (ParametersSingleton.Instance.StatusStation == StatusStation.CONFIGURED)
                ParametersSingleton.Instance.StatusStation = StatusStation.INITIALIZED;
        }

        #endregion

        #region Private Methods

        private bool ElectionFinished()
        {
            return ParametersSingleton.Instance.StatusStation == StatusStation.FINALIZED;
        }

        private void SetElectionName()
        {
            ElectionName = ParametersSingleton.Instance.ElectionName;
        }

        private void GetMaxDigitCandidateNumber()
        {
            candidateService = FactoryService.getCandidate();
            MaxCandidateNumber = candidateService.GetMaxSizeCandidateNumber();
        }

        private void SetControlsMaxDigit(int maxCandidateNumber)
        {
            if (maxCandidateNumber == 1)
            {
                VisibilitySecondField = Visibility.Collapsed;
                VisibilityThirdField = Visibility.Collapsed;
                VisibilityFourthField = Visibility.Collapsed;
            }

            if (maxCandidateNumber == 2)
            {
                VisibilityThirdField = Visibility.Collapsed;
                VisibilityFourthField = Visibility.Collapsed;
            }

            if (maxCandidateNumber == 3)
            {
                VisibilityFourthField = Visibility.Collapsed;
            }
        }

        private bool IsVoteReceived(Packet packetReceived)
        {
            return packetReceived != null
                && packetReceived.PacketType == PacketType.VOTE_SAVED;
        }

        private Packet SendVoteToVotingStation(string message)
        {
            TCPClient client = new TCPClient();
            var packetReceived = client.SendMessage(new Packet() { IP = ParametersSingleton.Instance.VotingStationOfficer.IP, PacketType = PacketType.VOTE, NicknameStation = ParametersSingleton.Instance.NicknameStation, IPFrom = ParametersSingleton.Instance.IPStation, Message = message }, 23002);
            log.Info(string.Format("Send TCP to votation station message {0}", message));
            return packetReceived;
        }

        private string CreateVoteMessage(int? candidateId)
        {
            Service.DTO.Vote vote = new Service.DTO.Vote();
            vote.CandidateId = candidateId;
            vote.VoterIdentifier = this.Voter.Identifier;
            string message = JsonConvert.SerializeObject(vote);
            return message;
        }

        private void SaveVoteDatabase(int? candidateId)
        {
            IVote voteService = FactoryService.getVote();
            voteService.Save(candidateId, ParametersSingleton.Instance.NicknameStation);
            log.Info(string.Format("Vote saved id {0}", candidateId));
        }

        private int? GetCandidateId(int? candidateNumber)
        {
            int? candidateId = null;

            if (candidateNumber.HasValue)
            {
                ICandidate candidateService = FactoryService.getCandidate();
                Candidate cand = candidateService.GetValidByNumber(candidateNumber.Value, Convert.ToInt32(ParametersSingleton.Instance.Region));
                if (cand == null)
                    candidateId = 0;
                else
                    candidateId = cand.Id;
            }

            return candidateId;
        }

        private bool IsValid(Candidate candidate)
        {
            return candidate != null;
        }

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        [DllImport("user32.Dll")]
        static extern int PostMessage(IntPtr hWnd, UInt32 msg, int wParam, int lParam);

        private const UInt32 WM_CLOSE = 0x0010;

        private BitmapImage ConvertByteToImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;
            var image = new BitmapImage();
            using (var mem = new MemoryStream(imageData))
            {
                mem.Position = 0;
                image.BeginInit();
                image.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = null;
                image.StreamSource = mem;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }

        private void SetTypeVoting()
        {
            if (ParametersSingleton.Instance.TypeVoting == TypeVoting.CANDIDATE)
                Type = "CNADIDATO";
            else
                Type = "CHAPA";
        }

        #endregion

    }
}
