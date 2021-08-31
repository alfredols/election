using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Votacao.SocketConn
{
    public class Network
    {
        public IPAddress GetBroadcastAddress(IPAddress address, IPAddress subnetMask)
        {
            byte[] ipAdressBytes = address.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

            if (ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            byte[] broadcastAddress = new byte[ipAdressBytes.Length];
            for (int i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (byte)(ipAdressBytes[i] | (subnetMaskBytes[i] ^ 255));
            }
            return new IPAddress(broadcastAddress);
        }

        public IPAddress GetLocalIPAddress()
        {
            NetworkInterface[] devs = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface inf in devs)
            {
                if (inf.NetworkInterfaceType == NetworkInterfaceType.Ethernet
                    && !(inf.Name.IndexOf("vEthernet") > -1))
                {
                    foreach (UnicastIPAddressInformation address in inf.GetIPProperties().UnicastAddresses)
                    {
                        if (address.Address.AddressFamily == AddressFamily.InterNetwork)
                            return address.Address;
                    }
                }
            }
            return null;
        }

        public List<IPAddress> ListLocalIPAddress()
        {
            List<IPAddress> list = new List<IPAddress>();
            NetworkInterface[] devs = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface inf in devs)
            {
                if (inf.NetworkInterfaceType == NetworkInterfaceType.Ethernet 
                    || inf.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                {
                    if (inf.OperationalStatus == OperationalStatus.Up)
                    {
                        foreach (UnicastIPAddressInformation address in inf.GetIPProperties().UnicastAddresses)
                        {
                            if (address.Address.AddressFamily == AddressFamily.InterNetwork)
                                list.Add(address.Address);
                        }
                    }
                }
            }

            return list;
        }

        public IPAddress GetSubnetMask(IPAddress address)
        {
            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (UnicastIPAddressInformation unicastIPAddressInformation in adapter.GetIPProperties().UnicastAddresses)
                {
                    if (unicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        if (address.Equals(unicastIPAddressInformation.Address))
                        {
                            return unicastIPAddressInformation.IPv4Mask;
                        }
                    }
                }
            }

            throw new ArgumentException(string.Format("Can't find subnetmask for IP address '{0}'", address));
        }

    }
}
