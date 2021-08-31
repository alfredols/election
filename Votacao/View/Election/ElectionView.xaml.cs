using Newtonsoft.Json;
using System;
using System.Windows.Controls;
using Votacao.Data;
using Votacao.Service;
using Votacao.Service.DTO;
using Votacao.Service.Interface;
using Votacao.SocketConn;
using Votacao.SocketConn.TCP;
using Votacao.SocketConn.UDP;

namespace Votacao.View.Election
{
    public partial class ElectionView : UserControl
    {

        #region Attributes

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private TCPServer server;
        private UDPServer udpServer;

        #endregion

        #region Constructor

        public ElectionView()
        {
            InitializeComponent();

            if (IsVotingStation())
            {
                CreateTCPServer();
                CreateUDPServer();
            }

            StopServersWhenUnloadedScreen();
        }

        #endregion

        #region Private Events
        private void UdpServer_MessageReceived(Packet message)
        {
            SetVoterVotedOtherVotingStation(message);
        }

        private Packet Server_MessageReceived(Packet packet)
        {
            if (packet.PacketType == PacketType.VOTE)
            {
                Service.DTO.Vote vote = JsonConvert.DeserializeObject<Service.DTO.Vote>(packet.Message);

                SaveVote(packet, vote);

                SetVoterVoted(vote);

                BroadcastVoterOthersVotingStation(vote);
            }

            Packet packetResult = CreateResponseVote();

            return packetResult;
        }

        private void ElectionView_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (server != null)
                server.StopListening();

            if (udpServer != null)
                udpServer.StopListening();
        }

        #endregion

        #region Private Methods

        private void StopServersWhenUnloadedScreen()
        {
            this.Unloaded += ElectionView_Unloaded;
        }

        private void CreateUDPServer()
        {
            udpServer = new UDPServer();
            udpServer.MessageReceived += UdpServer_MessageReceived;
            udpServer.StartListening();

            log.Info("Voting Station listening broadcast UDP");

        }

        private void CreateTCPServer()
        {
            int port = 23002;
            server = new TCPServer(port);
            server.MessageReceived += Server_MessageReceived;
            server.StartListening();

            log.Info(string.Format("Voting Station listening vote TCP port {0}", port));
        }

        private static bool IsVotingStation()
        {
            return ParametersSingleton.Instance.TypeStation == Service.DTO.TypeStation.VOTING_OFFICER;
        }

        private void SetVoterVotedOtherVotingStation(Packet message)
        {
            IFilteredVoter voterService = FactoryService.getFilteredVoter();
            Voter voter = voterService.Get(RemovePunctuations(message.Message));

            if (voter != null)
            {
                voterService.Voted(voter.Id);
                log.Info(string.Format("Voter voted other station voterId {0} station {1} nicknameStation {2}", message.Message, message.HostName, message.NicknameStation));
            }
            else
            {
                log.Info(string.Format("Voter voted other station not found voterId {0} station {1} nicknameStation {2}", message.Message, message.HostName, message.NicknameStation));
            }
        }

        private void BroadcastVoterOthersVotingStation(Service.DTO.Vote vote)
        {
            UDPClient cliente = new UDPClient(ParametersSingleton.Instance.IPStation);
            IFilteredVoter voterService = FactoryService.getFilteredVoter();

            Voter voter = voterService.Get(RemovePunctuations(vote.VoterIdentifier));

            if (voter != null)
            {
                cliente.SendBroadcast(voter.Identifier);
                log.Info(string.Format("Broadcast voter id {0}", voter.Identifier));
            }
        }

        private void SetVoterVoted(Service.DTO.Vote vote)
        {
            IFilteredVoter voterService = FactoryService.getFilteredVoter();
            Voter voter = voterService.Get(RemovePunctuations(vote.VoterIdentifier));

            if (voter != null)
            {
                voterService.Voted(voter.Id);
                log.Info(string.Format("Voter voted id {0}", voter.Id));
            }
            else
            { 
                log.Info(string.Format("Voter not found id {0}", vote.VoterIdentifier));
            }
        }

        private void SaveVote(Packet packet, Service.DTO.Vote vote)
        {
            IVote voteService = FactoryService.getVote();
            voteService.Save(vote.CandidateId, packet.NicknameStation);

            log.Info(string.Format("Vote saved CandidateId {0} nicknameStation {1}", vote.CandidateId, packet.NicknameStation));
        }

        private Packet CreateResponseVote()
        {
            Packet packetResult = new Packet();
            packetResult.PacketType = PacketType.VOTE_SAVED;
            return packetResult;
        }

        private string RemovePunctuations(string electorNumber)
        {
            return electorNumber.Replace(".", string.Empty);
        }

        #endregion

    }
}