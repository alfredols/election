using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Votacao.SocketConn.TCP
{
    public class TCPServer
    {
        #region Attributes

        private Socket listener;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Public Event

        public delegate Packet ReceiveData(Packet packet);
        public event ReceiveData MessageReceived;

        #endregion

        #region Constructor

        public TCPServer(int port)
        {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, port);
            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(localEndPoint);

            log.Info(string.Format("TCP server created port {0}", port));
        }

        #endregion

        #region Public Methods

        public void StopListening()
        {
            if (listener != null)
            {
                if (listener.Connected)
                    listener.Shutdown(SocketShutdown.Both);
                listener.Close();
                listener = null;
                log.Info("TCP server stopped listening");
            }
        }

        public void StartListening()
        {
            try
            {
                if (listener != null)
                {
                    listener.Listen(5);

                    listener.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        listener);

                    log.Info("TCP server started listening");
                }
            }
            catch (Exception ex)
            {
                log.Error("TCP server error", ex);
            }
        }

        #endregion

        #region Private Events

        private void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                if (listener != null)
                {
                    Socket listener = (Socket)ar.AsyncState;
                    Socket handler = listener.EndAccept(ar);
                    StateObject state = new StateObject();
                    state.workSocket = handler;
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);

                    log.Info("TCP server request received");
                }
            }
            catch (Exception ex)
            {
                log.Error("TCP server AcceptCallback error", ex);
            }
        }

        private void ReadCallback(IAsyncResult ar)
        {
            try
            {
                String content = String.Empty;
                StateObject state = (StateObject)ar.AsyncState;
                Socket handler = state.workSocket;
                int bytesRead = handler.EndReceive(ar);
                if (bytesRead > 0)
                {
                    state.sb.Append(Encoding.UTF8.GetString(state.buffer, 0, bytesRead));
                    content = state.sb.ToString();

                    log.Info(string.Format("TCP server request message {0}", content));

                    Packet packet = JsonConvert.DeserializeObject<Packet>(content);

                    packet = MessageReceived(packet);

                    var textToSend = JsonConvert.SerializeObject(packet);

                    Send(handler, textToSend);
                }
            }
            catch (Exception ex)
            {
                log.Error("TCP server ReadCallback error", ex);
            }
        }

        private void Send(Socket handler, String data)
        {
            try
            {
                log.Info(string.Format("TCP server reply message {0}", data));

                byte[] byteData = Encoding.UTF8.GetBytes(data);
                handler.BeginSend(byteData, 0, byteData.Length, 0, new AsyncCallback(SendCallback), handler);
            }
            catch (Exception ex)
            {
                log.Error("TCP server Send error", ex);
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket handler = (Socket)ar.AsyncState;
                int bytesSent = handler.EndSend(ar);

                log.Info("TCP server reply sent");

                StartListening();
            }
            catch (Exception ex)
            {
                log.Error("TCP server SendCallback error", ex);
            }
        }

        #endregion

        #region Destructor

        ~TCPServer()
        {
            StopListening();
        }

        #endregion

    }

    #region StateObject

    public class StateObject
    {
        public Socket workSocket = null;
        public const int BufferSize = 1024;
        public byte[] buffer = new byte[BufferSize];
        public StringBuilder sb = new StringBuilder();
    }

    #endregion
}