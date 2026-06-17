using System;
using System.Windows.Forms;

namespace ConwaysGameOfLife
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Enables modern visual styles and smooth grid rendering styles
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // If your compiler complains about the line below, you can uncomment it 
            // (Required for some modern .NET high-DPI scaling setups)
            // ApplicationConfiguration.Initialize();

            // Boots up your pure-code MainForm layout
            Application.Run(new MainForm());
        }
    }
}