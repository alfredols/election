using MVVMC;
using Newtonsoft.Json;
using System;
using System.Windows;
using Votacao.Data;
using Votacao.Service.DTO;
using Votacao.SocketConn;
using Votacao.SocketConn.TCP;
using Votacao.SocketConn.UDP;

namespace Votacao.View.Setup
{
    public class PairBallotBoxViewModel : MVVMCViewModel
    {
        #region Attributes

        private UDPClient client;
        private TCPServer server;
        private System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();
        private IdentificationTerminal terminal;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Constructor

        public PairBallotBoxViewModel()
        {
            RestoreIdentificationTerminal();
            ConfigureClientUDPBroadcast();
            CreateTimerBroadcast();
            ConfigureTCPServer();
        }

        #endregion

        #region Public Methods

        public void StopNetworkActivity() 
        {
            myTimer.Stop();
            server.StopListening();
            client.Close();
        }

        #endregion

        #region Events

        private Packet Server_MessageReceived(Packet packet)
        {
            StopBroadcast();

            SaveVotingStationAddress(packet);

            NavigateToNextScreen();

            Packet packetResult = CreateResponse();

            return packetResult;
        }

        private void TimerEventProcessor(Object myObject,
                                           EventArgs myEventArgs)
        {
            client.SendBroadcast(JsonConvert.SerializeObject(terminal));
            log.Info("Sending broadcasting");
        }

        #endregion

        #region Private Methods

        private void RestoreIdentificationTerminal()
        {
            terminal = new IdentificationTerminal();
            terminal.RegionId = ParametersSingleton.Instance.Region.ToString();
            terminal.SiteId = ParametersSingleton.Instance.Site.ToString();
            terminal.SectionId = ParametersSingleton.Instance.Section.ToString();
            terminal.NicknameStation = ParametersSingleton.Instance.NicknameStation;
        }

        private Packet CreateResponse()
        {
            Packet packetResult = new Packet();
            packetResult.PacketType = PacketType.CONFIRMATION;
            return packetResult;
        }

        private void NavigateToNextScreen()
        {
            Application.Current.Dispatcher.Invoke(new Action(() => { this.GetController().Navigate("Next", null); }));
        }

        private void StopBroadcast()
        {
            client.Close();
            myTimer.Stop();
        }

        private void ConfigureClientUDPBroadcast()
        {
            client = new UDPClient(ParametersSingleton.Instance.IPStation);
        }

        private void ConfigureTCPServer()
        {
            server = new TCPServer(23001);
            server.MessageReceived += Server_MessageReceived;
            server.StartListening();
        }

        private void CreateTimerBroadcast()
        {
            myTimer.Tick += new EventHandler(TimerEventProcessor);
            myTimer.Interval = 2000;
            myTimer.Start();
        }

        private static void SaveVotingStationAddress(Packet packet)
        {
            ParametersSingleton.Instance.VotingStationOfficer = new VotingStationOfficer() { IP = packet.IPFrom, NicknameStation = packet.NicknameStation, HostName = packet.HostName };
            log.Info(string.Format("Voting Station Officer saved IP: {0} Nickname: {1}", packet.IPFrom, packet.NicknameStation));
        }

        #endregion
    }
}
