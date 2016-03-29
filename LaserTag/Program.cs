using LaserTag.Gun.Controllers;
using Microsoft.SPOT;

namespace LaserTag.Gun
{
    public class Program
    {
        public static void Main()
        {
            Debug.Print("Program started.");

            var ioController = new IOController(0x01);
            var btController = new BluetoothController(ioController);

            var gameController = new GameController(btController, ioController);
            btController.GameController = gameController;
            ioController.GameController = gameController;

            gameController.Respawn();

            Debug.Print("Init complete, looping...");

            while (true)
            {
                ioController.Process();
                btController.Process();
            }
        }
    }
}
