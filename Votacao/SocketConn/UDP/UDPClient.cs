using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Votacao.Data;

namespace Votacao.SocketConn.UDP
{
    public class UDPClient
    {
        #region Attributes

        private const int listenPort = 23000;
        private UdpClient client;
        private IPAddress ip;
        private IPEndPoint ipEndpoint;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Constructor

        public UDPClient(string localIP)
        {
            client = new UdpClient();
            Network localNT = new Network();
            ip = IPAddress.Parse(localIP);
            IPAddress subnet = localNT.GetSubnetMask(ip);
            IPAddress broadcast = localNT.GetBroadcastAddress(ip, subnet);
            client.EnableBroadcast = true;
            ipEndpoint = new IPEndPoint(broadcast, listenPort);

            log.Info(string.Format("UDP client broadcast created port {0}", listenPort));
        }

        #endregion

        #region Public Methods

        public void SendBroadcast(string message)
        {
            try
            {
                Packet objChatPacket = new Packet();

                SetOriginPacket(objChatPacket);
                
                objChatPacket.Message = message;
                objChatPacket.PacketType = PacketType.DISCOVERY;

                string strJSONDiscovery = JsonConvert.SerializeObject(objChatPacket);

                log.Info(string.Format("UDP client broadcast message {0}", strJSONDiscovery));

                var dataBytes = Encoding.UTF8.GetBytes(strJSONDiscovery);
                client.Send(dataBytes, dataBytes.Length, ipEndpoint);
            }
            catch (Exception ex) 
            { 
                log.Error("UDP client error", ex);
            }
        }

        public void Close() 
        {
            client.Close();

            log.Info("UDP client broadcast closed");
        }

        #endregion

        #region Private Methods

        private void SetOriginPacket(Packet objChatPacket)
        {
            objChatPacket.NicknameStation = ParametersSingleton.Instance.NicknameStation;
            objChatPacket.IPFrom = ip.ToString();
            objChatPacket.HostName = System.Environment.MachineName;
        }

        #endregion
    }
}
