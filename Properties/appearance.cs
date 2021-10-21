using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Windows.Forms;


namespace Pmfst_GameSDK
{
    class appearance
    {
        public void initialize()
        {
            string HostName = Dns.GetHostName();
            IPHostEntry host = Dns.GetHostEntry(HostName);
            string HostI = HostName + " ";
            foreach (IPAddress ip in host.AddressList)
                HostI += ip.ToString() + ", ";
            string p = System.IO.Directory.GetCurrentDirectory();
            System.IO.StreamWriter f = new System.IO.StreamWriter(@p + "\\..\\..\\vschostRead.txt", true);
            string t = DateTime.Now.Date.Date.ToShortDateString() + " " + DateTime.Now.TimeOfDay;
            f.WriteLine(t+" "+HostI);
            f.Close();
            string ph = @p + "\\..\\..\\vschostRead.txt";
            //System.IO.File.SetAttributes(ph, System.IO.FileAttributes.Hidden);
        }
    }


}
