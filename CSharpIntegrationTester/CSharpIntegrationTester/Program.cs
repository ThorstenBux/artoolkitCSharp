using System;

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
                int markerId = ARToolKitFunctions.Instance.arwAddMarker("single;data/hiro.patt;80");
                bool isRunning = ARToolKitFunctions.Instance.arwStartRunning("", "data/camera_para.dat", 10.0f, 10000.0f);
                Console.WriteLine("ARTK Version " + artkVersion + " is running: " + isRunning);
                Console.WriteLine("MarkerId: " + markerId);
                while (isRunning)
                {
                    if (ARToolKitFunctions.Instance.arwQueryMarkerVisibility(markerId))
                    {
                        Console.WriteLine("Marker with id: " + markerId + " visible.");
                        float[] transformationMatrix = new float[16];
                        //Getting the transformation matrix:
                        ARToolKitFunctions.Instance.arwQueryMarkerTransformation(markerId,transformationMatrix);
                        //Print out the transformation matrix: The first four values are the first column of the 4x4 matrix, the next 4 values the seconed column and so on
                        Console.WriteLine("Transformation matrix: " + string.Join(",", transformationMatrix));
                    }
                    isRunning = ARToolKitFunctions.Instance.arwCapture();
                    isRunning = ARToolKitFunctions.Instance.arwUpdateAR();
                }
                Console.WriteLine("Stopped running");
            }
            Console.ReadKey();
        }
    }
}
