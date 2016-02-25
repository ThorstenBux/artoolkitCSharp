using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            // Initialize ARToolKitComponent wrapper
            Console.WriteLine("Hello AR");

            if (ARToolKitFunctions.Instance.arwInitialiseAR())
            {
                string artkVersion = ARToolKitFunctions.Instance.arwGetARToolKitVersion();
                Console.WriteLine(artkVersion);
            }

            Console.ReadKey();

        }
    }
}
