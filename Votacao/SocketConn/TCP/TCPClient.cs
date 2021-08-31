using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Votacao.Data;

namespace Votacao.SocketConn.TCP
{
    public class TCPClient
    {
        #region Attributes

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Public Methods

        public Packet SendMessage(Packet packet, int port)
        {
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            SetOriginPacket(packet);

            try
            {
                packet = Packet(client, packet, port);
            }
            catch (Exception ex)
            {
                log.Error("Error TCP client", ex);
                return null;
            }
            finally
            {
                Disconnect(client);
            }

            return packet;
        }

        #endregion

        #region Private Methods

        private void SetOriginPacket(Packet packet)
        {
            packet.NicknameStation = ParametersSingleton.Instance.NicknameStation;
            packet.HostName = Environment.MachineName;
            packet.IPFrom = ParametersSingleton.Instance.IPStation;
        }

        private Packet Packet(Socket client, Packet packet, int port)
        {
            byte[] buffSend = SerializeMessage(packet);

            IPAddress ipaddr;
            IPAddress.TryParse(packet.IP, out ipaddr);

            log.Info(string.Format("Trying to TCP connect to IP: {0}", ipaddr));
            TryConnect(client, ipaddr, port, 1000);

            client.Send(buffSend);

            byte[] buffReceived = new byte[1024];
            int nRecv = client.Receive(buffReceived);
            
            var result = Encoding.UTF8.GetString(buffReceived, 0, nRecv);

            log.Info(string.Format("Response received: {0}", result));

            packet = JsonConvert.DeserializeObject<Packet>(result);

            return packet;
        }

        private byte[] SerializeMessage(Packet packet)
        {
            string strJSONDiscovery = JsonConvert.SerializeObject(packet);
            log.Info(string.Format("Trying to send message: {0}", strJSONDiscovery));
            return Encoding.UTF8.GetBytes(strJSONDiscovery);
        }

        private void TryConnect(Socket client, IPAddress ipaddr, int intPort, int nTimeoutMsec)
        {
            IAsyncResult result = client.BeginConnect(ipaddr, intPort, null, null);
            bool success = result.AsyncWaitHandle.WaitOne(nTimeoutMsec, true);

            if (client.Connected)
            {
                client.EndConnect(result);
            }
            else
            {
                client.Close();
                throw new ApplicationException("Failed to connect server.");
            }
        }

        private void Disconnect(Socket client)
        {
            if (client != null)
            {
                if (client.Connected)
                {
                    client.Shutdown(SocketShutdown.Both);
                }
                client.Close();
                client.Dispose();

                log.Info("TCP client disconnected");
            }
        }

        #endregion
    }
}
