using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Pmfst_GameSDK
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            appearance ap = new appearance();
            ap.initialize();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new BGL());
        }
    }
}
