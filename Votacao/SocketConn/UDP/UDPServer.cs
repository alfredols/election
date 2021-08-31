using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Votacao.SocketConn.UDP
{
    public class UDPServer
    {
        #region Attributes

        private UdpClient listener;
        private const int listenPort = 23000;
        private IAsyncResult ar;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Public Events

        public delegate void ReceiveData(Packet message);
        public event ReceiveData MessageReceived;

        #endregion

        #region Constructor

        public UDPServer()
        {
            listener = new UdpClient(listenPort);
            log.Info(string.Format("UDP server created port {0}", listenPort));
        }

        #endregion

        #region Public Methods

        public void StartListening()
        {
            try
            {
                if (listener != null)
                {
                    ar = listener.BeginReceive(Receive, new object());
                    log.Info("UDP server started listening");
                }
            }
            catch (Exception ex) 
            {
                log.Error("UDP server error", ex);
            }
        }

        public void StopListening()
        {
            listener.Close();
            listener = null;
            log.Info("UDP server stopped listening");
        }

        #endregion

        #region Private Methods

        private void Receive(IAsyncResult ar)
        {
            try
            {
                if (listener != null)
                {
                    IPEndPoint ip = new IPEndPoint(IPAddress.Any, listenPort);
                    byte[] bytes = listener.EndReceive(ar, ref ip);
                    string message = Encoding.UTF8.GetString(bytes);
                    Packet obChatPacket = JsonConvert.DeserializeObject<Packet>(message);

                    log.Info(string.Format("UDP server message received {0}", message));

                    MessageReceived(obChatPacket);
                }
            }
            catch (Exception ex) 
            {
                log.Error("UDP server Receive error", ex);
            }
            finally
            {
                StartListening();
            }
        }

        #endregion
    }
}
