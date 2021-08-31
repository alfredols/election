using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Votacao.SocketConn;
using System.Net;
using Votacao.SocketConn.UDP;
using Votacao.SocketConn.TCP;

namespace VotacaoTest
{
    [TestClass]
    public class NetworkTest
    {
        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        [TestMethod]
        public void ListIPsTest()
        {
            Network nt = new Network();

            List<IPAddress> ips = nt.ListLocalIPAddress();

            Assert.IsTrue(ips.Count > 1);
        }

        [TestMethod]
        public void UDPBroadcastTest()
        {
            Network nt = new Network();

            UDPServer server = new UDPServer();

            server.MessageReceived += Server_MessageReceived;

            server.StartListening();

            UDPClient client = new UDPClient(nt.GetLocalIPAddress().ToString());

            client.SendBroadcast("teste");

            server.StopListening();
        }

        private void Server_MessageReceived(Packet message)
        {
            TestContext.WriteLine(string.Format("Mensagem recebida: {0}", message.Message));
        }

        [TestMethod]
        public void TCPTest()
        {
            TCPServer server = new TCPServer(23001);
            Network nt = new Network();

            server.MessageReceived += Server_MessageReceived1;

            server.StartListening();

            TCPClient client = new TCPClient();
            Packet req = new Packet();
            req.IP = nt.GetLocalIPAddress().ToString();
            req.Message = "teste TCP";
            Packet resp = client.SendMessage(req, 23001);

            resp = client.SendMessage(resp, 23001);

            TestContext.WriteLine(string.Format("Retorno: {0}", resp.Message));
        }

        private Packet Server_MessageReceived1(Packet packet)
        {
            TestContext.WriteLine(string.Format("Mensagem recebida: {0}", packet.Message));
            packet.Message += " 123";
            return packet;
        }
    }
}
