using LaserTag.Controllers;
using Microsoft.SPOT;

namespace LaserTag
{
    public class Program
    {
        public static void Main()
        {
            Debug.Print("Program started.");

            var _gunController = new GunController(0x01);
            var _commController = new CommController();

            while (true)
            {
                _gunController.Process();
                _commController.Process();
            }
        }
    }
}
