using System.Threading;

namespace LaserTag.Server
{
    class Program
    {
        static async void MainAsync(string[] args)
        {
            var listener = new Listener();

            await listener.StartAsync();
        }

        static void Main(string[] args)
        {
            MainAsync(args);

            Thread.Sleep(Timeout.Infinite);
        }
    }
}
