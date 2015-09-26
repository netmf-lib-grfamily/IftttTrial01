using System;
using Microsoft.SPOT;

namespace IftttTrial01
{
    public class Program
    {
        public static void Main()
        {
            var device = new MakerDevice();
            device.Run();
        }
    }
}
