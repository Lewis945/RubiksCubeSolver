using System;
using System.Windows.Forms;

namespace RubiksCube.Game
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
            //RubiksCube.OpenCV.Bootstrapper.Main();
        }
    }
}
