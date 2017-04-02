using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using YeelightControl.Services.Interfaces;

namespace YeelightControl.Services
{
    public class YeelightMessenger : IYeelightMessenger, IDisposable
    {
        private UdpClient _client;
        private IPAddress _multicastAddress;
        private IPEndPoint _endPoint;

        public YeelightMessenger()
        {
            _multicastAddress = IPAddress.Parse("239.255.255.250");
            _endPoint = new IPEndPoint(_multicastAddress, 1982);
            _client = new UdpClient();
            _client.ExclusiveAddressUse = false;
        }

        public IEnumerable<string> SearchNetwork(int timeOut)
        {
            const string searchStr = "M-SEARCH * HTTP/1.1\r\n" +
                                     "HOST: 239.255.255.250:1982\r\n" +
                                     "MAN: \"ssdp:discover\"\r\n" +
                                     "ST: wifi_bulb";
            var bytes = Encoding.ASCII.GetBytes(searchStr);
            _client.Send(bytes, bytes.Length, _endPoint);
            var stopwatch = Stopwatch.StartNew();
            var sender = new IPEndPoint(IPAddress.Any, 0);
            List<string> answers = new List<string>();
            _client.Client.ReceiveTimeout = timeOut;
            while (stopwatch.ElapsedMilliseconds < timeOut)
            {
                try
                {
                    var data = _client.Receive(ref sender);
                    var str = Encoding.ASCII.GetString(data);
                    answers.Add(str);
                }
                catch (SocketException e) when (e.ErrorCode == 10060)
                {
                    // ignore timeout
                }
            }
            stopwatch.Stop();
            return answers;
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}