using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace LaserTag.Server
{
    public class Listener
    {
        private TcpListener _tcpListener;

        public Listener()
        {
            _tcpListener = new TcpListener(IPAddress.Any, 1337);
        }


        public async Task StartAsync()
        {
            _tcpListener.Start();

            while (true)
            {
                var client = await _tcpListener.AcceptTcpClientAsync();

                EchoAsync(client);
            }
        }


        private async Task EchoAsync(TcpClient client)
        {
            using (var sr = new StreamReader(client.GetStream()))
            using (var sw = new StreamWriter(client.GetStream()))
            {
                while (true)
                {
                    var timeout = Task.Delay(10000);
                    var line = sr.ReadLineAsync();

                    var completed = await Task.WhenAny(line, timeout);

                    if (completed == timeout)
                        break;

                    await sw.WriteLineAsync($"You said: {line.Result}");
                    await sw.FlushAsync();
                }
            }

            client.Close();
            client.Dispose();
        }
    }
}
